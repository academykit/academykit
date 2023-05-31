import TextEditor from "@components/Ui/TextEditor";
import {
  Box,
  Button,
  Checkbox,
  Container,
  Flex,
  Group,
  Paper,
  Select,
  Text,
  TextInput,
  UnstyledButton,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconPlus, IconTrash } from "@tabler/icons";
import { QuestionType, ReadableEnum } from "@utils/enums";
import {
  IAssignmentQuestion,
  ICreateAssignment,
  useAddAssignmentQuestion,
  useEditAssignmentQuestion,
} from "@utils/services/assignmentService";
import errorType from "@utils/services/axiosError";
import { useMemo } from "react";
import { useTranslation } from "react-i18next";
import * as Yup from "yup";
import useFormErrorHooks from "@hooks/useFormErrorHooks";

const getQuestionType = () => {
  return Object.entries(QuestionType)
    .splice(0, Object.entries(QuestionType).length / 2)
    .map(([key, value]) => ({
      value: key,
      label:
        ReadableEnum[value as keyof typeof ReadableEnum] ?? value.toString(),
    }));
};

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t("question_title_required") as string),
    type: Yup.string()
      .required(t("question_type_required") as string)
      .nullable(),

    answers: Yup.array()
      .when(["type"], {
        is: QuestionType.MultipleChoice.toString(),
        then: Yup.array()
          .min(2, t("more_option_required") as string)
          .test(
            t("test"),
            t("one_option_selected_on_multiple_choice") as string,
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
          .min(2, t("more_option_required") as string)
          .test(
            t("test"),
            t("one_option_selected_on_single_choice") as string,
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
      name: assignmentQuestion ? assignmentQuestion.name : "",
      description: assignmentQuestion ? assignmentQuestion.description : "",
      hints: assignmentQuestion ? assignmentQuestion?.hints || "" : "",
      type: assignmentQuestion ? assignmentQuestion.type.toString() : "",
      // @ts-ignore
      answers: assignmentQuestion
        ? assignmentQuestion.assignmentQuestionOptions?.map((x) => ({
            option: x.option,
            isCorrect: x.isCorrect,
          }))
        : [{ option: "", isCorrect: false }],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const data = useMemo(() => getQuestionType(), []);

  const addAssignmentQuestion = useAddAssignmentQuestion(lessonId, search);
  const editAssignment = useEditAssignmentQuestion(lessonId, search);
  const { t } = useTranslation();
  const onSubmit = async (data: ICreateAssignment) => {
    try {
      if (assignmentQuestion) {
        const dat = { ...data, lessonId: lessonId };
        await editAssignment.mutateAsync({
          data: dat,
          assignmentId: assignmentQuestion.id,
        });
        showNotification({
          message: t("edit_assignment_question_success"),
        });
      } else {
        await addAssignmentQuestion.mutateAsync({ data });
        showNotification({
          message: t("add_assignment_question_success"),
        });
      }
      form.reset();
      onCancel();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  return (
    <Container fluid>
      <FormProvider form={form}>
        <form onSubmit={form.onSubmit(onSubmit)}>
          <Paper p={20} withBorder mt={20}>
            <TextInput
              size={"lg"}
              withAsterisk
              label="Title for question"
              placeholder="Enter Title of Question"
              {...form.getInputProps("name")}
            ></TextInput>
            <Box mt={20}>
              <Text size={"lg"}>Description</Text>
              <TextEditor formContext={useFormContext} />
            </Box>

            <Box mt={20}>
              <Text size={"lg"}>Hint</Text>
              <TextEditor label="hints" formContext={useFormContext} />
            </Box>
            <Select
              mt={20}
              placeholder={"Please Select Question Type"}
              withAsterisk
              size={"lg"}
              label="Question Type"
              {...form.getInputProps("type")}
              data={data}
              onClick={() => {
                assignmentQuestion &&
                  form.setFieldValue("answers", [
                    { option: "", isCorrect: false, isSelected: false },
                  ]);
              }}
            ></Select>
            {(form.values.type === QuestionType.MultipleChoice.toString() ||
              form.values.type === QuestionType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>Options</Text>
                {form.values.answers &&
                  form.values.answers.map((x, i) => (
                    <div
                      key={i}
                      style={{ marginBottom: "30px", marginTop: "10px" }}
                    >
                      <Flex align="center">
                        <Checkbox
                          {...form.getInputProps(`answers.${i}.isCorrect`)}
                          mr={10}
                          checked={
                            form.values.answers &&
                            form?.values.answers[i].isCorrect
                          }
                          name="isCorrect"
                        ></Checkbox>
                        <TextEditor
                          label={`answers.${i}.option`}
                          formContext={useFormContext}
                        ></TextEditor>
                        <UnstyledButton
                          ml={10}
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
                        {form.values.answers &&
                          form.values.answers.length > 1 && (
                            <UnstyledButton
                              ml={10}
                              onClick={() => {
                                form.removeListItem("answers", i);
                              }}
                            >
                              <IconTrash color="red" />
                            </UnstyledButton>
                          )}
                      </Flex>
                      {typeof form.errors[`answers.${i}.option`] ===
                        "string" && (
                        <span style={{ color: "red" }}>
                          {form.errors[`answers.${i}.option`]}
                        </span>
                      )}
                    </div>
                  ))}
                {typeof form.errors[`answers`] === "string" && (
                  <span style={{ color: "red" }}>{form.errors[`answers`]}</span>
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
                Save
              </Button>
              <Button
                size="sm"
                type="reset"
                onClick={onCancel}
                variant="outline"
              >
                Cancel
              </Button>
            </Group>
          </Paper>
        </form>
      </FormProvider>
    </Container>
  );
};

export default EditAssignment;
