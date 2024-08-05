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
  Radio,
  Select,
  Text,
  UnstyledButton,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconPlus, IconTrash } from "@tabler/icons-react";
import { QuestionType } from "@utils/enums";
import {
  IAddAssessment,
  IAssessmentQuestion,
  usePostAssessmentQuestion,
  useUpdateAssessmentQuestion,
} from "@utils/services/assessmentService";
import errorType from "@utils/services/axiosError";
import { t } from "i18next";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    questionName: Yup.string()
      .trim()
      .required(t("question_title_required") as string),
    type: Yup.string()
      .required(t("question_type_required") as string)
      .nullable(),
    assessmentQuestionOptions: Yup.array()
      .when(["type"], {
        is: QuestionType.MultipleChoice.toString(),
        then: Yup.array()
          .min(1, t("option_more_than_one") as string)
          .test(
            "test",
            t("multiple_choice_option_atleast") as string,
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

const [FormProvider, useFormContext, useForm] =
  createFormContext<IAddAssessment>();

interface IProps {
  onCancel: () => void;
  data?: IAssessmentQuestion;
}

const AssessmentQuestionForm = ({ onCancel, data }: IProps) => {
  const postAssessmentQuestion = usePostAssessmentQuestion();
  const updateAssessmentQuestion = useUpdateAssessmentQuestion();

  const params = useParams();
  const form = useForm({
    initialValues: {
      questionName: data ? data.questionName : "",
      description: data ? data.description : "",
      hints: data ? data.hints : "",
      type: data ? data.type.toString() : "1",
      assessmentQuestionOptions: data
        ? data.assessmentQuestionOptions
        : [{ option: "", isCorrect: false }],
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

  const onChangeRadioType = (index: number) => {
    form.values.assessmentQuestionOptions.forEach((_x, i) => {
      if (i === index) {
        return form.setFieldValue(
          `assessmentQuestionOptions.${index}.isCorrect`,
          true
        );
      }
      return form.setFieldValue(
        `assessmentQuestionOptions.${i}.isCorrect`,
        false
      );
    });
  };

  const onSubmit = async (values: typeof form.values) => {
    try {
      if (data) {
        await updateAssessmentQuestion.mutateAsync({
          id: data.id,
          data: {
            ...values,
            type: parseInt(values.type),
          },
        });

        form.reset();
        onCancel(); // close the form

        showNotification({
          title: t("successful"),
          message: t("question_updated_success"),
        });
      } else {
        await postAssessmentQuestion.mutateAsync({
          id: params.id as string,
          data: {
            ...values,
            type: parseInt(values.type),
          },
        });

        form.reset();
        onCancel(); // close the form

        showNotification({
          title: t("successful"),
          message: t("question_created_success"),
        });
      }
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <>
      <Container fluid>
        <FormProvider form={form}>
          <Card mt={20}>
            <form onSubmit={form.onSubmit(onSubmit)}>
              <CustomTextFieldWithAutoFocus
                size={"lg"}
                withAsterisk
                label={t("title_question")}
                placeholder={t("enter_question_title") as string}
                {...form.getInputProps("questionName")}
              />
              <Box mt={20}>
                <Text size={"lg"}>{t("description")}</Text>
                <RichTextEditor
                  label="description"
                  placeholder={t("question_description") as string}
                  formContext={useFormContext}
                />
              </Box>

              <Box mt={20}>
                <Text size={"lg"}>{t("hint")}</Text>
                <RichTextEditor
                  label="hints"
                  placeholder={t("question_hint") as string}
                  formContext={useFormContext}
                />
              </Box>

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
                  {form.values.assessmentQuestionOptions.map((_x, i) => (
                    <Flex align={"center"} gap={"md"} key={i} mb={30}>
                      {QuestionType.MultipleChoice.toString() ===
                      form.values.type ? (
                        <Checkbox
                          checked={
                            form.values.assessmentQuestionOptions[i].isCorrect
                          }
                          {...form.getInputProps(
                            `assessmentQuestionOptions.${i}.isCorrect`
                          )}
                        ></Checkbox>
                      ) : (
                        <Radio
                          onChange={() => onChangeRadioType(i)}
                          checked={
                            form.values.assessmentQuestionOptions[i].isCorrect
                          }
                        ></Radio>
                      )}
                      <div style={{ width: "80%" }}>
                        <RichTextEditor
                          label={`assessmentQuestionOptions.${i}.option`}
                          placeholder={t("option_placeholder") as string}
                          formContext={useFormContext}
                        ></RichTextEditor>
                      </div>
                      <UnstyledButton
                        onClick={() => {
                          form.insertListItem(
                            "assessmentQuestionOptions",
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
                      {form.values.assessmentQuestionOptions.length > 1 && (
                        <UnstyledButton
                          onClick={() => {
                            form.removeListItem("assessmentQuestionOptions", i);
                          }}
                        >
                          <IconTrash color="red" />
                        </UnstyledButton>
                      )}
                    </Flex>
                  ))}
                  {typeof form.errors[`assessmentQuestionOptions`] ===
                    "string" && (
                    <span style={{ color: "red" }}>
                      {form.errors[`assessmentQuestionOptions`]}
                    </span>
                  )}
                </Box>
              )}
              <Group mt={20}>
                <Button
                  type="submit"
                  loading={
                    postAssessmentQuestion.isLoading ||
                    updateAssessmentQuestion.isLoading
                  }
                >
                  {t("save")}
                </Button>
                <Button type="button" variant="outline" onClick={onCancel}>
                  {t("cancel")}
                </Button>
              </Group>
            </form>
          </Card>
        </FormProvider>
      </Container>
    </>
  );
};

export default AssessmentQuestionForm;
