import {
  Button,
  Grid,
  Group,
  Text,
  Paper,
  Switch,
  Tooltip,
  Box,
} from '@mantine/core';
import { createFormContext, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { LessonType } from '@utils/enums';
import {
  useCreateLesson,
  useUpdateLesson,
} from '@utils/services/courseService';
import { ILessonAssignment, ILessonFile } from '@utils/services/types';
import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import errorType from '@utils/services/axiosError';
import * as Yup from 'yup';
import FileUploadLesson from '@components/Ui/FileUploadLesson';
import { useTranslation } from 'react-i18next';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import RichTextEditor from '@components/Ui/RichTextEditor/Index';

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t('file_name_required') as string),
    documentUrl: Yup.string().required(t('file_required') as string),
  });
};

const [FormProvider, useFormContext, useForm] = createFormContext();

const AddDocument = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setAddLessonClick,
  setIsEditing,
}: {
  setAddState: (s: string) => void;
  item?: ILessonAssignment;
  setAddLessonClick: (b: boolean) => void;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
  isEditing?: boolean;
  sectionId: string;
}) => {
  const { id: slug } = useParams();
  const lesson = useCreateLesson(slug as string);
  const { t } = useTranslation();

  const updateLesson = useUpdateLesson(
    // item?.courseId || "",
    // item?.id,
    slug as string
  );

  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );

  const [, setOpened] = useState(false);
  const [, setLessonId] = useState('');

  const form = useForm({
    initialValues: {
      name: item?.name ?? '',
      description: item?.description ?? '',
      documentUrl: item?.documentUrl ?? '',
      isMandatory: item?.isMandatory ?? false,
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const submitForm = async (values: any) => {
    try {
      const fileData = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.Document,
        ...values,
        isMandatory,
      };
      if (!isEditing) {
        const response: any = await lesson.mutateAsync(fileData as ILessonFile);
        setLessonId(response?.data?.id);
        form.reset();
        setOpened(true);
      } else {
        await updateLesson.mutateAsync({
          ...fileData,
          lessonIdentity: item?.id,
        } as ILessonFile);
        setIsEditing(false);
      }
      showNotification({
        title: t('success'),
        message: `${t('file')} ${isEditing ? t('edited') : t('added')} ${t(
          'successfully'
        )}`,
      });
      setAddLessonClick(true);
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
    <FormProvider form={form}>
      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Grid align={'center'} justify="space-around">
            <Grid.Col span={12} lg={8}>
              <CustomTextFieldWithAutoFocus
                withAsterisk
                label={t('file_title')}
                placeholder={t('file_name') as string}
                {...form.getInputProps('name')}
                styles={{ error: { position: 'absolute' } }}
              />
            </Grid.Col>
            <Tooltip multiline label={t('mandatory_tooltip')} width={220}>
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
          </Grid>
          <Text size={'sm'} mt={10}>
            {t('file')} <span className="global-astrick"> *</span>
          </Text>
          <FileUploadLesson
            currentFile={item?.documentUrl}
            formContext={useFormContext}
          />
          <Box my={form.errors['documentUrl'] ? 20 : 10}>
            <Text size={'sm'}>{t('file_description')}</Text>
            <RichTextEditor
              placeholder={t('file_description') as string}
              {...form.getInputProps('description')}
            />
          </Box>
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
        </Paper>
      </form>
    </FormProvider>
  );
};

export default AddDocument;
