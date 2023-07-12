import TextEditor from "@components/Ui/TextEditor";
import {
  Box,
  Button,
  Card,
  Checkbox,
  Container,
  Flex,
  Group,
  Loader,
  MultiSelect,
  Radio,
  Select,
  Text,
  TextInput,
  UnstyledButton,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconPlus, IconTrash } from "@tabler/icons";
import { QuestionType, ReadableEnum } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import errorType from "@utils/services/axiosError";
import {
  IAddQuestionType,
  useEditQuestion,
  useGetQuestion,
} from "@utils/services/questionService";
import { useAddTag, useTags } from "@utils/services/tagService";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import * as Yup from "yup";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
const [FormProvider, useFormContext, useForm] =
  createFormContext<IAddQuestionType>();

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t("question_title_required") as string),

    answers: Yup.array()

      .when(["type"], {
        is: QuestionType.MultipleChoice.toString(),
        then: Yup.array()
          .min(1, t("option_more_than_one") as string)
          .test(
            t("test"),
            t("multiple_choice_option_atleast ") as string,
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
        then: Yup.array()
          .test(
            t("test"),
            t("single_choice_option_atleast") as string,
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
                .required(t("option_required") as string),
            })
          ),
      }),
  });
};
const EditQuestion = () => {
  const { t } = useTranslation();
  const { id, slug } = useParams();
  const navigate = useNavigate();
  const { mutate, data: addTagData, isSuccess } = useAddTag();
  const getQuestion = useGetQuestion(id as string, slug as string);
  const editQuestion = useEditQuestion(id as string, slug as string);
  const [tagsList, setTagsList] = useState<{ value: string; label: string }[]>(
    []
  );

  const form = useForm({
    initialValues: {
      name: "",
      description: "",
      hints: "",
      tags: [],
      type: "",
      answers: [{ option: "", isCorrect: false }],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const fieldSize = "md";
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

  const onSubmit = async (data: IAddQuestionType) => {
    try {
      await editQuestion.mutateAsync({
        poolId: id as string,
        questionId: slug as string,
        data,
      });
      navigate(-1);
      showNotification({
        title: t("successful"),
        message: t("question_edit_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        title: t("error"),
        message: error,
        color: "red",
      });
    }
  };
  const [searchParams, setSearchParams] = useState("");

  const tags = useTags(
    queryStringGenerator({
      search: searchParams,
      size: 10000,
    })
  );
  const [first, setFirst] = useState(true);

  useEffect(() => {
    if (tags.isSuccess && tags.isFetched) {
      setTagsList(() =>
        tags.data.items.map((x) => ({ label: x.name, value: x.id }))
      );
    }
  }, [tags.isSuccess]);

  useEffect(() => {
    if (getQuestion.isSuccess) {
      const data = getQuestion.data.tags.map((e) => e.tagId);
      const answers = getQuestion.data.questionOptions.map((e) => ({
        option: e.option,
        isCorrect: e.isCorrect,
      }));

      form.setValues({
        name: getQuestion.data.name,
        description: getQuestion.data.description,
        hints: getQuestion.data.hints,
        type: getQuestion.data.type.toString(),
        answers: answers,
        tags: data,
      });

      setTimeout(() => {
        setFirst(false);
      }, 1000);
    }
  }, [getQuestion.data]);

  useEffect(() => {
    if (isSuccess) {
      setTagsList([
        ...tagsList,
        { label: addTagData.data.name, value: addTagData.data.id },
      ]);

      form.setFieldValue("tags", [...form.values.tags, addTagData?.data?.id]);
    }
  }, [isSuccess]);

  useEffect(() => {
    if (!first) {
      form.values.answers.forEach((x, i) => {
        return form.setFieldValue(`answers.${i}.isCorrect`, false);
      });
    }
  }, [form.values.type]);

  const onChangeRadioType = (index: number) => {
    form.values.answers.forEach((x, i) => {
      if (i === index) {
        return form.setFieldValue(`answers.${index}.isCorrect`, true);
      }
      return form.setFieldValue(`answers.${i}.isCorrect`, false);
    });
  };

  return (
    <Container fluid>
      <FormProvider form={form}>
        <Card mt={20}>
          <form onSubmit={form.onSubmit(onSubmit)}>
            <CustomTextFieldWithAutoFocus
              size={fieldSize}
              withAsterisk
              label={t("title_question")}
              placeholder={t("enter_question_title") as string}
              {...form.getInputProps("name")}
            />
            <Box mt={20}>
              <Text size={"md"}>{t("description")}</Text>
              <TextEditor
                placeholder={t("question_description") as string}
                label="description"
                formContext={useFormContext}
              />
            </Box>

            {tags.isSuccess ? (
              <MultiSelect
                searchable
                mt={10}
                labelProps="name"
                creatable
                sx={{ maxWidth: "500px" }}
                data={tagsList}
                // value={[]}
                {...form.getInputProps("tags")}
                getCreateLabel={(query) => `+ Create ${query}`}
                onCreate={(query) => {
                  mutate(query);
                }}
                size={"md"}
                label={t("tags")}
                placeholder={t("select_tags") as string}
              />
            ) : (
              <Loader />
            )}

            <Box mt={20}>
              <Text size={"md"}>{t("hint")}</Text>
              <TextEditor
                placeholder={t("question_hint") as string}
                label={t("hint") as string}
                formContext={useFormContext}
              />
            </Box>
            <Select
              mt={20}
              placeholder={t("select_question_type") as string}
              size={fieldSize}
              withAsterisk
              label={t("question_type")}
              data={getQuestionType()}
              {...form.getInputProps("type")}
            ></Select>
            {(form.values.type === QuestionType.MultipleChoice.toString() ||
              form.values.type === QuestionType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>{t("options")}</Text>
                {form.values.answers.map((x, i) => (
                  <Flex
                    align={"center"}
                    justify={"center"}
                    gap={"md"}
                    key={i}
                    mb={30}
                  >
                    {QuestionType.MultipleChoice.toString() ===
                    form.values.type ? (
                      <Checkbox
                        checked={x.isCorrect}
                        {...form.getInputProps(`answers.${i}.isCorrect`)}
                        name=""
                      ></Checkbox>
                    ) : (
                      <Radio
                        onChange={() => onChangeRadioType(i)}
                        checked={x.isCorrect}
                        // {...form.getInputProps(`answers.${i}.isCorrect`)}
                      ></Radio>
                    )}
                    <TextEditor
                      placeholder={t("option_placeholder") as string}
                      label={`answers.${i}.option`}
                      formContext={useFormContext}
                    ></TextEditor>
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
                    {typeof form.errors[`answers.${i}.option`] === "string" && (
                      <span style={{ color: "red" }}>
                        {form.errors[`answers.${i}.option`]}
                      </span>
                    )}
                  </Flex>
                ))}
                {typeof form.errors[`answers`] === "string" && (
                  <span style={{ color: "red" }}>{form.errors[`answers`]}</span>
                )}
              </Box>
            )}
            <Group mt={20}>
              <Button size="sm" type="submit" loading={editQuestion.isLoading}>
                {t("save")}
              </Button>
            </Group>
          </form>
        </Card>
      </FormProvider>
    </Container>
  );
};

export default EditQuestion;
