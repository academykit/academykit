import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import {
  Box,
  Button,
  Checkbox,
  Container,
  Flex,
  Group,
  Paper,
  Radio,
  Select,
  Text,
  TextInput,
  UnstyledButton,
} from '@mantine/core';
import { createFormContext, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { QuestionType } from '@utils/enums';
import {
  IAssignmentQuestion,
  ICreateAssignment,
  useAddAssignmentQuestion,
  useEditAssignmentQuestion,
} from '@utils/services/assignmentService';
import errorType from '@utils/services/axiosError';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t('question_title_required') as string),
    type: Yup.string()
      .required(t('question_type_required') as string)
      .nullable(),

    answers: Yup.array()
      .when(['type'], {
        is: QuestionType.MultipleChoice.toString(),
        then: Yup.array()
          .min(2, t('more_option_required') as string)
          .test(
            t('test'),
            t('one_option_selected_on_multiple_choice') as string,
            function (value: any) {
              const a = value?.filter((x: any) => x.isCorrect).length > 0;
              return a;
            }
          )
          .of(
            Yup.object().shape({
              option: Yup.string()
                .trim()
                .required(t('option_required') as string)
                .test(
                  t('test_option_format'),
                  t('option_required') as string,
                  function (value: any) {
                    // Check if the value is "<p></p>"
                    return value !== '<p></p>';
                  }
                ),
            })
          ),
      })
      .when(['type'], {
        is: QuestionType.SingleChoice.toString(),
        then: Yup.array()
          .min(2, t('more_option_required') as string)
          .test(
            t('test'),
            t('one_option_selected_on_single_choice') as string,
            function (value: any) {
              const length: number =
                value && value.filter((e: any) => e.isCorrect).length;
              return length === 1;
            }
          )
          .of(
            Yup.object().shape({
              option: Yup.string()
                .trim()
                .required(t('option_required') as string)
                .test(
                  t('test_option_format_single'),
                  t('option_required') as string,
                  function (value: any) {
                    // Check if the value is "<p></p>"
                    return value !== '<p></p>';
                  }
                ),
            })
          ),
      }),
  });
};

const [FormProvider, useFormContext, useForm] =
  createFormContext<ICreateAssignment>();
const EditAssignment = ({
  lessonId,
  onCancel,
  search,
  assignmentQuestion,
}: {
  lessonId: string;
  onCancel: () => void;
  search: string;
  assignmentQuestion?: IAssignmentQuestion;
}) => {
  const form = useForm({
    initialValues: {
      lessonId: lessonId,
      name: assignmentQuestion ? assignmentQuestion.name : '',
      description: assignmentQuestion ? assignmentQuestion.description : '',
      hints: assignmentQuestion ? assignmentQuestion?.hints || '' : '',
      type: assignmentQuestion ? assignmentQuestion.type.toString() : '',
      answers: assignmentQuestion?.assignmentQuestionOptions?.map((x) => ({
        option: x.option,
        isCorrect: x.isCorrect ?? false,
        isSelected: x.isSelected,
      })) ?? [{ option: '', isCorrect: false, isSelected: false }],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const { t } = useTranslation();

  const getQuestionType = () => {
    return Object.entries(QuestionType)
      .splice(0, Object.entries(QuestionType).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
  };

  const data = useMemo(() => getQuestionType(), []);

  const addAssignmentQuestion = useAddAssignmentQuestion(lessonId, search);
  const editAssignment = useEditAssignmentQuestion(lessonId, search);

  const onChangeRadioType = (index: number) => {
    form?.values.answers &&
      form?.values.answers.forEach((x, i) => {
        if (i === index) {
          return form.setFieldValue(`answers.${index}.isCorrect`, true);
        }
        return form.setFieldValue(`answers.${i}.isCorrect`, false);
      });
  };

  const onSubmit = async (data: ICreateAssignment) => {
    try {
      if (assignmentQuestion) {
        const dat = { ...data, lessonId: lessonId };
        await editAssignment.mutateAsync({
          data: dat,
          assignmentId: assignmentQuestion.id,
        });
        showNotification({
          message: t('edit_assignment_question_success'),
        });
      } else {
        await addAssignmentQuestion.mutateAsync({ data });
        showNotification({
          message: t('add_assignment_question_success'),
        });
      }
      form.reset();
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
              size={'lg'}
              withAsterisk
              label={t('title_question')}
              placeholder={t('enter_question_title') as string}
              {...form.getInputProps('name')}
            ></TextInput>
            <Box mt={20}>
              <Text size={'lg'}>{t('description')}</Text>
              <RichTextEditor
                placeholder={t('question_description') as string}
                formContext={useFormContext}
                label="description"
              />
            </Box>

            <Box mt={20}>
              <Text size={'lg'}>{t('hint')}</Text>
              <RichTextEditor
                placeholder={t('question_hint') as string}
                formContext={useFormContext}
                label="hints"
              />
            </Box>
            <Select
              mt={20}
              placeholder={t('select_question_type') as string}
              withAsterisk
              size={'lg'}
              label={t('question_type')}
              {...form.getInputProps('type')}
              data={data}
              onClick={() => {
                assignmentQuestion &&
                  form.setFieldValue('answers', [
                    { option: '', isCorrect: false, isSelected: false },
                  ]);
              }}
            ></Select>
            {(form.values.type === QuestionType.MultipleChoice.toString() ||
              form.values.type === QuestionType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>{t('options')}</Text>
                {form.values.answers &&
                  form.values.answers.map((_x, i) => (
                    <div
                      key={i}
                      style={{ marginBottom: '30px', marginTop: '10px' }}
                    >
                      <Flex gap={'md'} align="center">
                        {QuestionType.MultipleChoice.toString() ===
                        form.values.type ? (
                          <Checkbox
                            checked={
                              form?.values?.answers &&
                              form?.values?.answers[i].isCorrect
                            }
                            mr={10}
                            {...form.getInputProps(`answers.${i}.isCorrect`)}
                            name=""
                          ></Checkbox>
                        ) : (
                          <Radio
                            mr={10}
                            onChange={() => onChangeRadioType(i)}
                            checked={
                              form?.values?.answers &&
                              form?.values?.answers[i].isCorrect
                            }
                            // {...form.getInputProps(`answers.${i}.isCorrect`)}
                          ></Radio>
                        )}
                        <div style={{ width: '80%' }}>
                          <RichTextEditor
                            // style={{width:100}}
                            placeholder={t('option_placeholder') as string}
                            label={`answers.${i}.option`}
                            formContext={useFormContext}
                          ></RichTextEditor>
                        </div>
                        <UnstyledButton
                          ml={10}
                          onClick={() => {
                            form.insertListItem(
                              'answers',
                              {
                                option: '',
                                isCorrect: false,
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
                              ml={10}
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
                  addAssignmentQuestion.isLoading || editAssignment.isLoading
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

export default EditAssignment;
