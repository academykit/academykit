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
import { IPhysicalTraining } from '@utils/services/types';
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
    physicalStartDate: Yup.string()
      .required(t('start_date_required') as string)
      .typeError(t('start_date_required') as string),
    physicalStartTime: Yup.string()
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
  item?: IPhysicalTraining;
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
      physicalStartDate: new Date(),
      physicalStartTime: moment(new Date()).format('HH:mm'), // adding current time as default
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
      const startDateTime = moment(data.startDate + 'Z')
        .local()
        .toDate();

      form.setValues({
        name: data?.name ?? '',
        physicalStartDate: startDateTime,
        physicalStartTime: moment(startDateTime).format('HH:mm'),
        description: data?.description ?? '',
      });
    }
  }, [lessonDetails.isSuccess]);

  useEffect(() => {
    const { physicalStartTime, physicalStartDate } = form.values;

    if (physicalStartTime && physicalStartDate) {
      const date = getDateTime(physicalStartDate, physicalStartTime);

      setDateTime(() => date.utcDateTime);
    }
  }, [form.values]);

  const handleSubmit = async (values: any) => {
    const time = moment(values?.physicalStartTime, 'HH:mm').format('HH:mm');
    const date = moment(values?.physicalStartDate, 'MM/DD/YYYY').format(
      'MM/DD/YYYY'
    );

    try {
      const startDate = isEditing
        ? moment(date + ' ' + time, 'MM/DD/YYYY HH:mm').toDate()
        : new Date(dateTime).toISOString();

      if (isEditing) {
        await updateLesson.mutateAsync({
          startDate: startDate,
          name: values.name,
          courseId: slug,
          type: LessonType.Physical,
          lessonIdentity: item?.id,
          sectionIdentity: sectionId,
          description: values.description,
        } as IPhysicalTraining);
        setIsEditing(false);
      } else {
        await lesson.mutateAsync({
          startDate: startDate,
          name: values.name,
          courseId: slug,
          type: LessonType.Physical,
          sectionIdentity: sectionId,
          description: values.description,
        } as IPhysicalTraining);
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
          {...form.getInputProps('physicalStartDate')}
        />
        <TimeInput
          label={t('start_time')}
          withAsterisk
          {...form.getInputProps('physicalStartTime')}
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
