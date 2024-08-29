import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
import RichTextEditor from "@components/Ui/RichTextEditor/Index";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Box,
  Button,
  Card,
  Checkbox,
  Container,
  Flex,
  Group,
  Loader,
  Radio,
  Select,
  Text,
  UnstyledButton,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import TagMultiSelectCreatable from "@pages/course/component/TagMultiSelectCreatable";
import { IconPlus, IconTrash } from "@tabler/icons-react";
import { QuestionType } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import errorType from "@utils/services/axiosError";
import { useOnePool, usePools } from "@utils/services/poolService";
import {
  IAddQuestionType,
  useAddQuestion,
} from "@utils/services/questionService";
import { ITag, useAddTag, useTags } from "@utils/services/tagService";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import * as Yup from "yup";

const [FormProvider, useFormContext, useForm] =
  createFormContext<IAddQuestionType>();

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string()
      .trim()
      .required(t("question_title_required") as string),
    type: Yup.string()
      .required(t("question_type_required") as string)
      .nullable(),
    questionPoolId: Yup.string()
      .trim()
      .required(t("question_pool_required") as string),
    answers: Yup.array()
      .when(["type"], {
        is: QuestionType.MultipleChoice.toString(),
        then: (schema) =>
          schema
            .min(1, t("option_more_than_one") as string)
            .test(
              "test",
              t("multiple_choice_option_at_least_one") as string,
              function (value: any) {
                const a = value?.filter((x: any) => x.isCorrect).length > 0;
                return a;
              }
            )
            .of(
              Yup.object().shape({
                option: Yup.string()
                  .trim()
                  .required(t("option_required") as string),
              })
            ),
      })
      .when(["type"], {
        is: QuestionType.SingleChoice.toString(),
        then: (schema) =>
          schema
            .test(
              t("test"),
              t("single_choice_option_at_least_one") as string,
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
                  .required(t("option_required") as string),
              })
            ),
      }),
  });
};

const Create = () => {
  const params = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation();
  const questionPools = usePools(queryStringGenerator({ size: 10000 }));
  const currentPool = useOnePool(params.id as string);

  const questionPoolDropdown = questionPools.data?.items.map((question) => {
    return {
      value: question.id,
      label: question.name,
    };
  });

  const form = useForm({
    initialValues: {
      name: "",
      description: "",
      hints: "",
      tags: [],
      type: "1",
      answers: [{ option: "", isCorrect: false }],
      questionPoolId: currentPool.data?.id as string,
    },
    validate: yupResolver(schema()),
    validateInputOnChange: true,
  });
  useFormErrorHooks(form);

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

  const { id } = useParams();
  const addQuestion = useAddQuestion(id as string, "");
  const { mutateAsync, data: addTagData, isSuccess } = useAddTag();
  const [isReset, setIsReset] = useState(false);

  const onSubmit = async (data: IAddQuestionType) => {
    try {
      await addQuestion.mutateAsync({ poolId: id as string, data });
      const tags = form.values.tags;
      const questionPreference = form.values.type;
      form.reset();

      if (!isReset) {
        navigate(-1);
      }

      // setting user's previous choices
      form.setFieldValue("tags", tags);
      form.setFieldValue("type", questionPreference);

      showNotification({
        title: t("successful"),
        message: t("question_created_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  const [searchParams] = useState("");
  // const [tagsList, setTagsList] = useState<{ value: string; label: string }[]>(
  //   []
  // );
  const [tagsLists, setTagsLists] = useState<ITag[]>([]);
  const tags = useTags(
    queryStringGenerator({
      search: searchParams,
      size: 10000,
    })
  );

  useEffect(() => {
    if (tags.isSuccess && tags.isFetched) {
      // setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
      setTagsLists(tags.data.items.map((x) => x));
    }
  }, [tags.isSuccess]);

  useEffect(() => {
    if (isSuccess) {
      // setTagsList([
      //   ...tagsList,
      //   { label: addTagData.data.name, value: addTagData.data.id },
      // ]);
      setTagsLists([...tagsLists, addTagData.data]);
      form.setFieldValue("tags", [...form.values.tags, addTagData?.data?.id]);
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

  const cancelCreation = () => {
    navigate(-1);
  };

  return (
    <Container fluid>
      <FormProvider form={form}>
        <Card mt={20}>
          <form onSubmit={form.onSubmit(onSubmit)}>
            <CustomTextFieldWithAutoFocus
              size={"lg"}
              withAsterisk
              label={t("title_question")}
              placeholder={t("enter_question_title") as string}
              {...form.getInputProps("name")}
            />
            <Box mt={20}>
              <Text size={"lg"}>{t("description")}</Text>
              <RichTextEditor
                label={t("description") as string}
                placeholder={t("question_description") as string}
                formContext={useFormContext}
              />
            </Box>
            {tags.isSuccess ? (
              // <MultiSelect
              //   mt={15}
              //   searchable
              //   style={{ maxWidth: '500px' }}
              //   data={tagsList}
              //   {...form.getInputProps('tags')}
              //   size={'lg'}
              //   label={t('tags')}
              //   placeholder={t('select_tags') as string}
              // />
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
              <Text size={"lg"}>{t("hint")}</Text>
              <RichTextEditor
                label={t("hints") as string}
                placeholder={t("question_hint") as string}
                formContext={useFormContext}
              />
            </Box>

            <Select
              withAsterisk
              allowDeselect={false}
              readOnly
              mt={20}
              placeholder={t("select_pool") as string}
              size={"lg"}
              label={t("question_pool")}
              data={questionPoolDropdown ?? []}
              {...form.getInputProps("questionPoolId")}
            />

            <Select
              withAsterisk
              mt={20}
              placeholder={t("select_question_type") as string}
              size={"lg"}
              label={t("question_type")}
              {...form.getInputProps("type")}
              data={getQuestionType()}
            ></Select>
            {(form.values.type === QuestionType.MultipleChoice.toString() ||
              form.values.type === QuestionType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>{t("options")}</Text>
                {form.values.answers.map((_x, i) => (
                  <Flex align={"center"} gap={"md"} key={i} mb={30}>
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
                    <div style={{ width: "80%" }}>
                      <RichTextEditor
                        label={`answers.${i}.option`}
                        placeholder={t("option_placeholder") as string}
                        formContext={useFormContext}
                      ></RichTextEditor>
                    </div>
                    <UnstyledButton
                      onClick={() => {
                        form.insertListItem(
                          "answers",
                          {
                            option: "",
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
                          form.removeListItem("answers", i);
                        }}
                      >
                        <IconTrash color="red" />
                      </UnstyledButton>
                    )}
                  </Flex>
                ))}
                {typeof form.errors[`answers`] === "string" && (
                  <span style={{ color: "red" }}>{form.errors[`answers`]}</span>
                )}
              </Box>
            )}
            <Group mt={20}>
              <Button
                type="submit"
                loading={addQuestion.isPending}
                onClick={() => setIsReset(false)}
              >
                {t("save")}
              </Button>
              <Button
                type="submit"
                loading={addQuestion.isPending}
                onClick={() => setIsReset(true)}
              >
                {t("save_more")}
              </Button>
              <Button
                type="button"
                variant="outline"
                loading={addQuestion.isPending}
                onClick={() => cancelCreation()}
              >
                {t("cancel")}
              </Button>
            </Group>
          </form>
        </Card>
      </FormProvider>
    </Container>
  );
};

export default Create;
