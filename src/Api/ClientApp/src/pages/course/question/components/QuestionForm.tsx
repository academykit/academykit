import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import {
    Box,
    Button,
    Card,
    Checkbox,
    Flex,
    Group,
    Loader,
    Radio,
    Select,
    Text,
    UnstyledButton,
} from '@mantine/core';
import { createFormContext, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import TagMultiSelectCreatable from '@pages/course/component/TagMultiSelectCreatable';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { QuestionType } from '@utils/enums';
import queryStringGenerator from '@utils/queryStringGenerator';
import errorType from '@utils/services/axiosError';
import { useAddPool, usePools } from '@utils/services/poolService';
import {
    IAddQuestionType,
    useAddQuestion,
} from '@utils/services/questionService';
import { ITag, useAddTag, useTags } from '@utils/services/tagService';
import { Dispatch, SetStateAction, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { CreatablePoolSelect } from './CreatablePoolSelect';

const [FormProvider, useFormContext, useForm] =
  createFormContext<IAddQuestionType>();

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string()
      .trim()
      .required(t('question_title_required') as string),
    type: Yup.string()
      .required(t('question_type_required') as string)
      .nullable(),
    questionPoolId: Yup.string()
      .trim()
      .required(t('question_pool_required') as string),
    answers: Yup.array()
      .when(['type'], {
        is: QuestionType.MultipleChoice.toString(),
        then: Yup.array()
          .min(1, t('option_more_than_one') as string)
          .test(
            'test',
            t('multiple_choice_option_atleast') as string,
            function (value: any) {
              const a = value?.filter((x: any) => x.isCorrect).length > 0;
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
        is: QuestionType.SingleChoice.toString(),
        then: Yup.array()
          .test(
            t('test'),
            t('single_choice_option_atleast') as string,
            function (value: any) {
              const length: number =
                value && value.filter((e: any) => e?.isCorrect).length;
              return length === 1;
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

const QuestionForm = ({
  closeModal,
  setTransferListData,
}: {
  closeModal: () => void;
  setTransferListData: Dispatch<SetStateAction<any[]>>;
}) => {
  const { t } = useTranslation();
  const [isReset, setIsReset] = useState(false);
  const [searchParams] = useState('');
  const [tagsLists, setTagsLists] = useState<ITag[]>([]);
  const createPool = useAddPool('');
  const questionPools = usePools(queryStringGenerator({ size: 10000 }));
  const { mutateAsync, data: addTagData, isSuccess } = useAddTag();
  const tags = useTags(
    queryStringGenerator({
      search: searchParams,
      size: 10000,
    })
  );

  const questionPoolDropdown = questionPools.data?.items.map((question) => {
    return {
      value: question.id,
      label: question.name,
    };
  });

  const form = useForm({
    initialValues: {
      name: '',
      description: '',
      hints: '',
      tags: [],
      type: '1',
      answers: [{ option: '', isCorrect: false }],
      questionPoolId: '',
    },
    validate: yupResolver(schema()),
    validateInputOnChange: true,
  });
  useFormErrorHooks(form);
  const addQuestion = useAddQuestion(form.values.questionPoolId, '');

  const onSubmit = async (data: IAddQuestionType) => {
    try {
      const response = await addQuestion.mutateAsync({
        poolId: data.questionPoolId,
        data,
      });
      const tags = form.values.tags;
      const questionPreference = form.values.type;
      form.reset();

      // setting user's previous choices
      form.setFieldValue('tags', tags);
      form.setFieldValue('type', questionPreference);

      showNotification({
        title: t('successful'),
        message: t('question_created_success'),
      });

      setTransferListData((prev) => [
        prev[0],
        [
          ...prev[1],
          {
            description: response.data.description,
            label: response.data.name,
            value: response.data.questionPoolQuestionId,
          },
        ],
      ]);

      if (!isReset) {
        closeModal();
      }
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  useEffect(() => {
    // refetch after creation of new pool
    questionPools.refetch();
  }, [createPool.isSuccess]);

  useEffect(() => {
    if (tags.isSuccess && tags.isFetched) {
      setTagsLists(tags.data.items.map((x) => x));
    }
  }, [tags.isSuccess]);

  useEffect(() => {
    if (isSuccess) {
      setTagsLists([...tagsLists, addTagData.data]);
      form.setFieldValue('tags', [...form.values.tags, addTagData?.data?.id]);
    }
  }, [isSuccess]);

  useEffect(() => {
    if (form.values.type && !addQuestion.isSuccess) {
      form.values.answers.forEach((_x, i) => {
        return form.setFieldValue(`answers.${i}.isCorrect`, false);
      });
    }
  }, [form.values.type]);

  const onChangeRadioType = (index: number) => {
    form.values.answers.forEach((_x, i) => {
      if (i === index) {
        return form.setFieldValue(`answers.${index}.isCorrect`, true);
      }
      return form.setFieldValue(`answers.${i}.isCorrect`, false);
    });
  };

  const getQuestionType = () => {
    return [
      {
        value: QuestionType.MultipleChoice.toString(),
        label: t(`MultipleChoice`),
      },
      {
        value: QuestionType.SingleChoice.toString(),
        label: t(`SingleChoice`),
      },
    ];
  };

  return (
    <>
      <FormProvider form={form}>
        <Card mt={20}>
          <form onSubmit={form.onSubmit(onSubmit)}>
            <CustomTextFieldWithAutoFocus
              size={'lg'}
              withAsterisk
              label={t('title_question')}
              placeholder={t('enter_question_title') as string}
              {...form.getInputProps('name')}
            />
            <Box mt={20}>
              <Text size={'lg'}>{t('description')}</Text>
              <RichTextEditor
                label={t('description') as string}
                placeholder={t('question_description') as string}
                formContext={useFormContext}
              />
            </Box>
            {tags.isSuccess ? (
              <TagMultiSelectCreatable
                data={tagsLists ?? []}
                mutateAsync={mutateAsync}
                form={form}
                size="lg"
              />
            ) : (
              <Loader />
            )}

            <Box mt={20}>
              <Text size={'lg'}>{t('hint')}</Text>
              <RichTextEditor
                label={t('hints') as string}
                placeholder={t('question_hint') as string}
                formContext={useFormContext}
              />
            </Box>

            <CreatablePoolSelect
              data={questionPoolDropdown ?? []}
              form={form}
              api={createPool}
              size="lg"
            />

            <Select
              withAsterisk
              mt={20}
              placeholder={t('select_question_type') as string}
              size={'lg'}
              label={t('question_type')}
              {...form.getInputProps('type')}
              data={getQuestionType()}
            ></Select>
            {(form.values.type === QuestionType.MultipleChoice.toString() ||
              form.values.type === QuestionType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>{t('options')}</Text>
                {form.values.answers.map((_x, i) => (
                  <Flex align={'center'} gap={'md'} key={i} mb={30}>
                    {QuestionType.MultipleChoice.toString() ===
                    form.values.type ? (
                      <Checkbox
                        checked={form.values.answers[i].isCorrect}
                        {...form.getInputProps(`answers.${i}.isCorrect`)}
                        name=""
                      ></Checkbox>
                    ) : (
                      <Radio
                        onChange={() => onChangeRadioType(i)}
                        checked={form.values.answers[i].isCorrect}
                      ></Radio>
                    )}
                    <div style={{ width: '80%' }}>
                      <RichTextEditor
                        label={`answers.${i}.option`}
                        placeholder={t('option_placeholder') as string}
                        formContext={useFormContext}
                      ></RichTextEditor>
                    </div>
                    <UnstyledButton
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
                    {form.values.answers.length > 1 && (
                      <UnstyledButton
                        onClick={() => {
                          form.removeListItem('answers', i);
                        }}
                      >
                        <IconTrash color="red" />
                      </UnstyledButton>
                    )}
                  </Flex>
                ))}
                {typeof form.errors[`answers`] === 'string' && (
                  <span style={{ color: 'red' }}>{form.errors[`answers`]}</span>
                )}
              </Box>
            )}
            <Group mt={20}>
              <Button
                type="submit"
                loading={addQuestion.isLoading}
                onClick={() => setIsReset(false)}
              >
                {t('save')}
              </Button>
              {/* <Button
                type="submit"
                loading={addQuestion.isLoading}
                onClick={() => setIsReset(true)}
              >
                {t('save_more')}
              </Button> */}
              <Button
                type="button"
                variant="outline"
                loading={addQuestion.isLoading}
                onClick={() => closeModal()}
              >
                {t('cancel')}
              </Button>
            </Group>
          </form>
        </Card>
      </FormProvider>
    </>
  );
};

export default QuestionForm;
