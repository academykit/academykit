import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import LessonVideoUpload from '@components/Ui/LessonVideoUpload';
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
import { createFormContext, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { LessonType } from '@utils/enums';
import errorType from '@utils/services/axiosError';
import {
  ILessons,
  useCreateLesson,
  useUpdateLesson,
} from '@utils/services/courseService';
import { ILessonLecture, ILessonRecording } from '@utils/services/types';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';
import * as Yup from 'yup';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t('video_name_required') as string),
    videoUrl: Yup.string().required(t('video_required') as string),
  });
};
type IProps = {
  setAddState: (s: string) => void;
  item?: ILessons;
  setAddLessonClick: (b: boolean) => void;
  isEditing?: boolean;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
};

interface IFormValues {
  videoUrl: string;
  name: string;
  description: string;
  isMandatory?: boolean;
}

const [FormProvider, useFormContext, useForm] =
  createFormContext<IFormValues>();

const AddLecture = ({
  setAddState,
  item,
  setAddLessonClick,
  isEditing,
  sectionId,
  setIsEditing,
}: IProps) => {
  const { id: slug } = useParams();
  const { t } = useTranslation();
  const [videoUrl] = React.useState<string>(item?.videoUrl ?? '');
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(slug as string);
  const isRecordedVideo = item?.type === LessonType.RecordedVideo;
  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );

  const form = useForm({
    initialValues: {
      videoUrl: item?.videoUrl ?? '',
      name: item?.name ?? '',
      description: item?.description ?? '',
      isMandatory: item?.isMandatory,
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const handleSubmit = async (values: any) => {
    const data = isRecordedVideo
      ? ({
          courseId: slug as string,
          sectionIdentity: sectionId,
          lessonIdentity: item?.id,
          type: LessonType.RecordedVideo,
          name: values?.name,
          videoUrl,
        } as ILessonRecording)
      : ({
          courseId: slug as string,
          sectionIdentity: sectionId,
          lessonIdentity: item?.id,
          type: LessonType.Video,
          ...values,
        } as ILessonLecture);
    try {
      if (isEditing) {
        await updateLesson.mutateAsync(data);
        setIsEditing(false);
      } else {
        await lesson.mutateAsync({
          courseId: slug as string,
          sectionIdentity: sectionId,
          type: LessonType.Video,
          ...values,
        } as ILessonLecture);
      }
      showNotification({
        title: t('successful'),
        message: isEditing
          ? t('lesson_edit_successful')
          : t('lesson_add_successful'),
      });
      setAddLessonClick(true);
    } catch (error: any) {
      const err = errorType(error);
      showNotification({
        color: 'red',
        title: t('error'),
        message: err,
      });
    }
  };
  return (
    <FormProvider form={form}>
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Paper withBorder p="md">
          <Grid align="center" justify={'space-around'}>
            <Grid.Col span={{ base: 12, lg: 8 }}>
              <CustomTextFieldWithAutoFocus
                style={{ width: '100%' }}
                label={isRecordedVideo ? t('recording_name') : t('video_name')}
                placeholder={
                  isRecordedVideo
                    ? (t('recording_name') as string)
                    : (t('video_name') as string)
                }
                withAsterisk
                {...form.getInputProps('name')}
                styles={{ error: { position: 'absolute' } }}
              />
            </Grid.Col>
            <Tooltip multiline label={t('mandatory_tooltip')} w={220}>
              <Grid.Col span={4}>
                {!isRecordedVideo && (
                  <Switch
                    label={t('is_mandatory')}
                    {...form.getInputProps('isMandatory')}
                    checked={isMandatory}
                    onChange={() => {
                      setIsMandatory(() => !isMandatory);
                      form.setFieldValue('isMandatory', !isMandatory);
                    }}
                  />
                )}
              </Grid.Col>
            </Tooltip>
          </Grid>
          <Text size={'md'} mt={10}>
            {isRecordedVideo ? t('recordings') : t('video')}

            <span className="global-astrick"> *</span>
          </Text>
          <LessonVideoUpload
            formContext={useFormContext}
            currentVideo={videoUrl}
            marginy={1}
          />
          <Box my={form.errors['videoUrl'] ? 20 : 10}>
            <Text size={'sm'}>
              {isRecordedVideo
                ? t('recording_description')
                : t('video_description')}
            </Text>
            <RichTextEditor
              placeholder={
                isRecordedVideo
                  ? (t('recording_description') as string)
                  : (t('video_description') as string)
              }
              formContext={useFormContext}
            />
          </Box>

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
                  setAddState('');
                }}
                variant="outline"
              >
                {t('close')}
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </FormProvider>
  );
};

export default AddLecture;
