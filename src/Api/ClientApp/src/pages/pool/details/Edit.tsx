import TextEditor from "@components/Ui/TextEditor";
import {
  Box,
  Button,
  Card,
  Checkbox,
  Container,
  Group,
  Loader,
  MultiSelect,
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
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import * as Yup from "yup";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
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
const Create = () => {
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
        answers,
      });

      form.setFieldValue("tags", data);
    }
  }, [getQuestion.isSuccess]);

  useEffect(() => {
    if (isSuccess) {
      setTagsList([
        ...tagsList,
        { label: addTagData.data.name, value: addTagData.data.id },
      ]);

      form.setFieldValue("tags", [...form.values.tags, addTagData?.data?.id]);
    }
  }, [isSuccess]);

  return (
    <Container fluid>
      <FormProvider form={form}>
        <Card mt={20}>
          <form onSubmit={form.onSubmit(onSubmit)}>
            <TextInput
              size={fieldSize}
              withAsterisk
              label={t("title_question")}
              placeholder={t("enter_question_title") as string}
              {...form.getInputProps("name")}
            ></TextInput>
            <Box mt={20}>
              <Text size={"md"}>{t("description")}</Text>
              <TextEditor label="description" formContext={useFormContext} />
            </Box>

            {tags.isSuccess ? (
              <MultiSelect
                searchable
                mt={10}
                labelProps="name"
                creatable
                sx={{ maxWidth: "500px" }}
                data={tagsList}
                value={[]}
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
              <TextEditor label="hints" formContext={useFormContext} />
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
                  <Group key={i} mb={30}>
                    <Checkbox
                      {...form.getInputProps(`answers.${i}.isCorrect`)}
                      name=""
                      checked={x.isCorrect}
                    ></Checkbox>
                    <TextEditor
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
                  </Group>
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

export default Create;
