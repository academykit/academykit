import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import {
  Box,
  Button,
  Container,
  Flex,
  Group,
  Paper,
  Select,
  Text,
  TextInput,
  UnstyledButton,
} from '@mantine/core';
import { createFormContext, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { FeedbackType } from '@utils/enums';
import errorType from '@utils/services/axiosError';
import {
  ICreateFeedback,
  IFeedbackQuestions,
  useAddFeedbackQuestion,
  useEditFeedbackQuestion,
} from '@utils/services/feedbackService';
import * as Yup from 'yup';

import useFormErrorHooks from '@hooks/useFormErrorHooks';
import { useTranslation } from 'react-i18next';
const fieldSize = 'md';

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t('feedback_title_required') as string),
    type: Yup.string()
      .required(t('feedback_type_required') as string)
      .nullable(),

    answers: Yup.array()
      .when(['type'], {
        is: FeedbackType.MultipleChoice.toString(),
        then: Yup.array()
          .min(1, t('more_option_required') as string)
          .test(
            t('test'),
            t('more_option_required') as string,
            function (value: any) {
              const a = value.length > 1;
              return a;
            }
          )
          .of(
            Yup.object().shape({
              option: Yup.string()
                .trim()
                .required(t('option_required') as string),
            })
          ),
      })
      .when(['type'], {
        is: FeedbackType.SingleChoice.toString(),
        then: Yup.array()
          .test(
            t('test'),
            t('more_option_required') as string,
            function (value: any) {
              const length: number = value && value.length;
              return length > 1;
            }
          )
          .of(
            Yup.object().shape({
              option: Yup.string()
                .trim()
                .required(t('option_required') as string),
            })
          ),
      }),
  });
};

const [FormProvider, useFormContext, useForm] =
  createFormContext<ICreateFeedback>();
const EditFeedback = ({
  lessonId,
  onCancel,
  search,
  feedbackQuestion,
}: {
  lessonId: string;
  onCancel: () => void;
  search: string;
  feedbackQuestion?: IFeedbackQuestions;
}) => {
  const form = useForm({
    initialValues: {
      lessonId: lessonId,
      name: feedbackQuestion ? feedbackQuestion.name : '',
      type: feedbackQuestion ? feedbackQuestion.type.toString() : '',
      answers: feedbackQuestion?.feedbackQuestionOptions?.map((x) => ({
        option: x.option,
        isSelected: x.isSelected,
      })) ?? [{ option: '', isSelected: false }],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const { t } = useTranslation();
  const addFeedbackQuestions = useAddFeedbackQuestion(lessonId, search);
  const editFeedbackQuestion = useEditFeedbackQuestion(lessonId, search);

  const getQuestionType = () => {
    return Object.entries(FeedbackType)
      .splice(0, Object.entries(FeedbackType).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
  };

  const onSubmit = async (data: ICreateFeedback) => {
    try {
      if (feedbackQuestion) {
        await editFeedbackQuestion.mutateAsync({
          data,
          feedbackId: feedbackQuestion.id,
        });
        showNotification({
          title: t('successful'),
          message: t('edit_feedback_question_success'),
        });
      } else {
        await addFeedbackQuestions.mutateAsync({ data });
        showNotification({
          title: t('successful'),
          message: t('add_feedback_question_success'),
        });
        form.reset();
      }
      onCancel();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  return (
    <Container fluid>
      <FormProvider form={form}>
        <form onSubmit={form.onSubmit(onSubmit)}>
          <Paper p={20} withBorder mt={20}>
            <TextInput
              autoComplete="off"
              size={fieldSize}
              withAsterisk
              label={t('title_feeback')}
              placeholder={t('enter_feedback') as string}
              {...form.getInputProps('name')}
            />

            <Select
              mt={20}
              placeholder={t('enter_feedback_type') as string}
              size={fieldSize}
              label={t('feedback_type')}
              {...form.getInputProps('type')}
              data={getQuestionType()}
              onClick={() => {
                feedbackQuestion &&
                  form.setFieldValue('answers', [
                    { option: '', isSelected: false },
                  ]);
              }}
              withAsterisk
            ></Select>
            {(form.values.type === FeedbackType.MultipleChoice.toString() ||
              form.values.type === FeedbackType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>{t('options')}</Text>
                {form.values.answers &&
                  form.values.answers.map((x, i) => (
                    <div
                      key={i}
                      style={{ marginBottom: '30px', width: '100%' }}
                    >
                      <Flex>
                        <div style={{ width: '90%' }}>
                          <RichTextEditor
                            placeholder={t('option_placeholder') as string}
                            label={`answers.${i}.option`}
                            formContext={useFormContext}
                          ></RichTextEditor>
                        </div>
                        <UnstyledButton
                          mx={10}
                          onClick={() => {
                            form.insertListItem(
                              'answers',
                              {
                                option: '',
                              },
                              i + 1
                            );
                          }}
                        >
                          <IconPlus color="green" />
                        </UnstyledButton>
                        {form.values.answers &&
                          form.values.answers.length > 1 && (
                            <UnstyledButton
                              onClick={() => {
                                form.removeListItem('answers', i);
                              }}
                            >
                              <IconTrash color="red" />
                            </UnstyledButton>
                          )}
                      </Flex>
                    </div>
                  ))}
                {typeof form.errors[`answers`] === 'string' && (
                  <span style={{ color: 'red' }}>{form.errors[`answers`]}</span>
                )}
              </Box>
            )}
            <Group mt={20}>
              <Button
                size="sm"
                type="submit"
                loading={
                  addFeedbackQuestions.isLoading
                  // || editAssignment.isLoading
                }
              >
                {t('save')}
              </Button>
              <Button
                size="sm"
                type="reset"
                onClick={onCancel}
                variant="outline"
              >
                {t('cancel')}
              </Button>
            </Group>
          </Paper>
        </form>
      </FormProvider>
    </Container>
  );
};

export default EditFeedback;
