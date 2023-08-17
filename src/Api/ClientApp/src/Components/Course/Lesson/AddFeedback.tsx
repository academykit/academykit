import React, { useState } from 'react';
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
import { useForm, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { LessonType } from '@utils/enums';
import {
  useCreateLesson,
  useUpdateLesson,
} from '@utils/services/courseService';
import { ILessonFeedback } from '@utils/services/types';
import { useParams, Link, useNavigate } from 'react-router-dom';
import errorType from '@utils/services/axiosError';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import RichTextEditor from '@components/Ui/RichTextEditor/Index';

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t('feedback_name_required') as string),
  });
};

const AddFeedback = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setIsEditing,
}: {
  setAddState: (s: string) => void;
  item?: ILessonFeedback;
  isEditing?: boolean;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const { id: slug } = useParams();
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(
    // item?.courseId || "",
    // item?.id,
    slug as string
  );
  const { t } = useTranslation();
  const navigate = useNavigate();

  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );

  const form = useForm({
    initialValues: {
      name: item?.name ?? '',
      description: item?.description ?? '',
      isMandatory: item?.isMandatory ?? false,
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const submitForm = async (values: { name: string; description: string }) => {
    try {
      const assignmentData = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.Feedback,
        ...values,
        isMandatory,
      };
      if (!isEditing) {
        const response = await lesson.mutateAsync(
          assignmentData as ILessonFeedback
        );
        form.reset();
        navigate(`${response.data.id}/feedback`);
      } else {
        await updateLesson.mutateAsync({
          ...assignmentData,
          lessonIdentity: item?.id,
        } as ILessonFeedback);
        setIsEditing(false);
      }
      showNotification({
        title: t('success'),
        message: `${t('feedback')} ${isEditing ? t('edited') : t('added')} ${t(
          'successfully'
        )}`,
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
          <Grid align={'center'} justify="space-around">
            <Grid.Col span={12} lg={8}>
              <CustomTextFieldWithAutoFocus
                withAsterisk
                label={t('feedback_title')}
                placeholder={t('feedback_title') as string}
                {...form.getInputProps('name')}
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
          <Box my={20}>
            <Text size={'sm'}>{t('feedback_description')}</Text>
            <RichTextEditor
              placeholder={t('feedback_description') as string}
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
            {isEditing && (
              <Button component={Link} to={`${item?.id}/feedback`}>
                {t('add_more_feedback')}
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </React.Fragment>
  );
};

export default AddFeedback;
