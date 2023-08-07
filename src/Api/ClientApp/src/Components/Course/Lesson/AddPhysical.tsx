import React, { useEffect, useState } from 'react';
import { Button, Grid, Group, Textarea } from '@mantine/core';
import { DatePickerInput, TimeInput } from '@mantine/dates';
import { useForm, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { LessonType } from '@utils/enums';
import errorType from '@utils/services/axiosError';
import {
  useCreateLesson,
  useGetCourseLesson,
  useUpdateLesson,
} from '@utils/services/courseService';
import { ILessonMeeting } from '@utils/services/types';
import moment from 'moment';
import { useParams } from 'react-router-dom';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import { getDateTime } from '@utils/getDateTime';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t('physical_name_required') as string),
    meetingStartDate: Yup.string()
      .required(t('start_date_required') as string)
      .typeError(t('start_date_required') as string),
    meetingStartTime: Yup.string()
      .required(t('start_time_required') as string)
      .typeError(t('start_time_required') as string),
  });
};

const AddPhysical = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setAddLessonClick,
  setIsEditing,
}: {
  setAddState: React.Dispatch<React.SetStateAction<string>>;
  item?: ILessonMeeting;
  isEditing?: boolean;
  sectionId?: string;
  setAddLessonClick: React.Dispatch<React.SetStateAction<boolean>>;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const { id: slug } = useParams();
  const lesson = useCreateLesson(slug as string);
  const [dateTime, setDateTime] = useState<string>('');
  const lessonDetails = useGetCourseLesson(
    item?.courseId || '',
    item?.id,
    isEditing
  );
  const { t } = useTranslation();

  const form = useForm({
    initialValues: {
      name: '',
      meetingStartDate: new Date(),
      meetingStartTime: moment(new Date()).format('HH:mm'), // adding current time as default
      description: '',
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const updateLesson = useUpdateLesson(
    slug as string,
    item?.courseId,
    item?.id
  );

  useEffect(() => {
    if (lessonDetails.isSuccess && isEditing) {
      const data = lessonDetails.data;
      const startDateTime = moment(data?.meeting?.startDate + 'Z')
        .local()
        .toDate();

      form.setValues({
        name: data?.name ?? '',
        meetingStartDate: startDateTime,
        // meetingStartTime: startDateTime.toTimeString(),
        meetingStartTime: moment(startDateTime).format('HH:mm'),
        description: data?.description ?? '',
      });
    }
  }, [lessonDetails.isSuccess]);

  useEffect(() => {
    const { meetingStartTime, meetingStartDate } = form.values;

    if (meetingStartTime && meetingStartDate) {
      const date = getDateTime(meetingStartDate, meetingStartTime);

      setDateTime(() => date.utcDateTime);
    }
  }, [form.values]);

  const handleSubmit = async (values: any) => {
    // const time = new Date(values?.meetingStartTime).toLocaleTimeString();
    // const date = new Date(values?.meetingStartDate).toLocaleDateString();
    const time = moment(values?.meetingStartTime, 'HH:mm').format('HH:mm');
    const date = moment(values?.meetingStartDate, 'MM/DD/YYYY').format(
      'MM/DD/YYYY'
    );

    const meeting = {
      ...values,
      meetingStartDate: isEditing
        ? // ? new Date(date + " " + time)
          moment(date + ' ' + time, 'MM/DD/YYYY HH:mm').toDate()
        : new Date(dateTime).toISOString(),
    };

    delete meeting.meetingStartTime;

    try {
      if (isEditing) {
        await updateLesson.mutateAsync({
          meeting,
          name: values.name,
          courseId: slug,
          type: LessonType.LiveClass,
          lessonIdentity: item?.id,
          sectionIdentity: sectionId,
          isMandatory: values.isMandatory,
          description: values.description,
        } as ILessonMeeting);
        setIsEditing(false);
      } else {
        await lesson.mutateAsync({
          meeting,
          name: values.name,
          courseId: slug,
          type: LessonType.LiveClass,
          sectionIdentity: sectionId,
          isMandatory: values.isMandatory,
          description: values.description,
        } as ILessonMeeting);
      }
      showNotification({
        message: `${t('capital_lesson')} ${
          isEditing ? t('edited') : t('added')
        } ${t('successfully')}`,
        title: t('success'),
      });
      setAddLessonClick(true);
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: 'red',
        title: t('error'),
      });
    }
  };

  return (
    <form onSubmit={form.onSubmit(handleSubmit)}>
      <Grid align="center">
        <Grid.Col span={12} lg={6}>
          <CustomTextFieldWithAutoFocus
            label={t('physical_name')}
            placeholder={t('physical_name') as string}
            {...form.getInputProps('name')}
            withAsterisk
          />
        </Grid.Col>
      </Grid>
      <Group grow>
        <DatePickerInput
          valueFormat="MMM DD, YYYY"
          placeholder={t('pick_date') as string}
          label={t('start_date')}
          withAsterisk
          {...form.getInputProps('meetingStartDate')}
        />
        <TimeInput
          label={t('start_time')}
          withAsterisk
          {...form.getInputProps('meetingStartTime')}
        />
      </Group>

      <Textarea
        label={t('description')}
        placeholder={t('physical_name_description') as string}
        {...form.getInputProps('description')}
      />
      <Group position="left" mt="md">
        <Button
          type="submit"
          loading={lesson.isLoading || updateLesson.isLoading}
        >
          {t('submit')}
        </Button>
        {!isEditing && (
          <Button
            onClick={() => {
              setAddState('');
            }}
            variant="outline"
          >
            {t('close')}
          </Button>
        )}
      </Group>
    </form>
  );
};

export default AddPhysical;
