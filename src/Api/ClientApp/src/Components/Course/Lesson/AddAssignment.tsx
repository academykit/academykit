import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import {
  Box,
  Button,
  Grid,
  Group,
  Paper,
  Switch,
  Text,
  Tooltip,
} from '@mantine/core';
import { DatePickerInput, TimeInput } from '@mantine/dates';
import { useForm, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { IconCalendar } from '@tabler/icons-react';
import { LessonType } from '@utils/enums';
import { getDateTime } from '@utils/getDateTime';
import errorType from '@utils/services/axiosError';
import {
  useCreateLesson,
  useUpdateLesson,
} from '@utils/services/courseService';
import { ILessonAssignment } from '@utils/services/types';
import moment from 'moment';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';
import * as Yup from 'yup';

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t('assignment_title_required') as string),
    startTime: Yup.string()
      .required(t('start_time_required') as string)
      .typeError(t('start_time_required') as string),
    eventStartDate: Yup.date()
      .required(t('start_date_required') as string)
      .typeError(t('start_date_required') as string),
    eventEndDate: Yup.date()
      .required(t('end_date_required') as string)
      .typeError(t('end_date_required') as string),
    endTime: Yup.string()
      .required(t('end_time_required') as string)
      .typeError(t('end_time_required') as string),
  });
};

interface SubmitType {
  name: string;
  description: string;
  isMandatory?: boolean;
  eventStartDate?: Date;
  eventEndDate?: Date;
  startTime?: string;
  endTime?: string;
}

const AddAssignment = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setIsEditing,
}: {
  setAddState: () => void;
  item?: ILessonAssignment;
  isEditing?: boolean;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const { id: slug } = useParams();
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(slug as string);
  const { t } = useTranslation();
  const navigate = useNavigate();

  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );

  const startDateTime = item?.startDate
    ? moment(item?.startDate + 'Z')
        .local()
        .toDate()
    : new Date();

  const endDateTime = item?.endDate
    ? moment(item?.endDate + 'Z')
        .local()
        .toDate()
    : new Date();

  const form = useForm({
    initialValues: {
      name: item?.name ?? '',
      description: item?.description ?? '',
      isMandatory: item?.isMandatory ?? false,
      eventStartDate: startDateTime ?? new Date(),
      eventEndDate: endDateTime ?? new Date(),
      endTime: moment(endDateTime).format('HH:mm') ?? moment().format('HH:mm'),
      startTime:
        moment(startDateTime).format('HH:mm') ?? moment().format('HH:mm'),
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const submitForm = async (values: SubmitType) => {
    const val = { ...values };
    delete val.eventEndDate;
    delete val.endTime;
    delete val.eventStartDate;
    delete val.startTime;

    const startDate =
      values?.eventStartDate &&
      getDateTime(values?.eventStartDate, values?.startTime?.toString() ?? '');
    const endDate =
      values?.eventEndDate &&
      getDateTime(values?.eventEndDate, values?.endTime?.toString() ?? '');
    try {
      const assignmentData = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.Assignment,
        ...val,
        startDate: startDate && startDate.utcDateTime,
        endDate: endDate && endDate.utcDateTime,
        isMandatory,
      };

      if (!isEditing) {
        const response = await lesson.mutateAsync(
          assignmentData as ILessonAssignment
        );
        form.reset();
        navigate(`${response.data.id}/assignment/add`);
      } else {
        await updateLesson.mutateAsync({
          ...assignmentData,
          lessonIdentity: item?.id,
        } as ILessonAssignment);
        setIsEditing(false);
      }
      showNotification({
        title: t('success'),
        message: `${t('assignment')} ${
          isEditing ? t('edited') : t('added')
        } ${t('successfully')}`,
      });
    } catch (error: any) {
      const err = errorType(error);
      showNotification({
        title: t('error'),
        message: err,
        color: 'red',
      });
    }
  };
  return (
    <React.Fragment>
      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Grid align={'center'}>
            <Grid.Col span={6}>
              <CustomTextFieldWithAutoFocus
                label={t('assignment_title')}
                placeholder={t('assignment_title') as string}
                withAsterisk
                {...form.getInputProps('name')}
                styles={{ error: { position: 'absolute' } }}
              />
            </Grid.Col>
            <Tooltip multiline label={t('mandatory_tooltip')} w={220}>
              <Grid.Col span={4}>
                <Switch
                  label={t('is_mandatory')}
                  {...form.getInputProps('isMandatory')}
                  checked={isMandatory}
                  onChange={() => {
                    setIsMandatory(() => !isMandatory);
                    form.setFieldValue('isMandatory', !isMandatory);
                  }}
                />
              </Grid.Col>
            </Tooltip>
            <Grid.Col span={6}>
              <DatePickerInput
                w={'100%'}
                valueFormat="MMM DD, YYYY"
                placeholder={t('pick_start_date') as string}
                label={t('start_date')}
                leftSection={<IconCalendar size={16} />}
                minDate={moment(new Date()).toDate()}
                withAsterisk
                {...form.getInputProps('eventStartDate')}
                styles={{ error: { position: 'absolute' } }}
              />
            </Grid.Col>
            <Grid.Col span={6}>
              <TimeInput
                label={t('start_time')}
                withAsterisk
                {...form.getInputProps('startTime')}
                styles={{ error: { position: 'absolute' } }}
              />
            </Grid.Col>

            <Grid.Col span={6}>
              <DatePickerInput
                w={'100%'}
                valueFormat="MMM DD, YYYY"
                placeholder={t('pick_end_date') as string}
                label={t('end_date')}
                minDate={form.values.eventStartDate}
                leftSection={<IconCalendar size={16} />}
                withAsterisk
                {...form.getInputProps('eventEndDate')}
                styles={{ error: { position: 'absolute' } }}
              />
            </Grid.Col>
            <Grid.Col span={6}>
              <TimeInput
                label={t('end_time')}
                withAsterisk
                {...form.getInputProps('endTime')}
                styles={{ error: { position: 'absolute' } }}
              />
            </Grid.Col>
            <Grid.Col>
              <Box my={10}>
                <Text size={'sm'}>{t('assignment_description')}</Text>
                <RichTextEditor
                  placeholder={t('assignment_description') as string}
                  {...form.getInputProps('description')}
                />
              </Box>
            </Grid.Col>
          </Grid>
          <Group mt="md">
            <Button
              type="submit"
              loading={lesson.isLoading || updateLesson.isLoading}
            >
              {t('submit')}
            </Button>
            {!isEditing && (
              <Button
                onClick={() => {
                  setAddState();
                }}
                variant="outline"
              >
                {t('close')}
              </Button>
            )}
            {isEditing && (
              <Button component={Link} to={`${item?.id}/assignment/add`}>
                {t('add_more_questions')}
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </React.Fragment>
  );
};

export default AddAssignment;
