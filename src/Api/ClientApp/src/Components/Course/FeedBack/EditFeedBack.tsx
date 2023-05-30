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
import { FeedbackType, ReadableEnum } from "@utils/enums";
import * as Yup from "yup";
import errorType from "@utils/services/axiosError";
import {
  ICreateFeedback,
  IFeedbackQuestions,
  useAddFeedbackQuestion,
  useEditFeedbackQuestion,
} from "@utils/services/feedbackService";
import React from "react";
import { useTranslation } from "react-i18next";
const fieldSize = "md";

const schema = Yup.object().shape({
  name: Yup.string().required("Title for feedback is required."),
  type: Yup.string().required("Feedback type is required.").nullable(),

  answers: Yup.array()
    .when(["type"], {
      is: FeedbackType.MultipleChoice.toString(),
      then: Yup.array()
        .min(1, "Options should be more than one.")
        .test(
          "test",
          "Options should be more than one.",
          function (value: any) {
            const a = value.length > 1;
            return a;
          }
        )
        .of(
          Yup.object().shape({
            option: Yup.string().trim().required("Options is required."),
          })
        ),
    })
    .when(["type"], {
      is: FeedbackType.SingleChoice.toString(),
      then: Yup.array()
        .test(
          "test",
          "Options should be more than one.",
          function (value: any) {
            const length: number = value && value.length;
            return length > 1;
          }
        )
        .of(
          Yup.object().shape({
            option: Yup.string().trim().required("Options is required."),
          })
        ),
    }),
});

const getQuestionType = () => {
  return Object.entries(FeedbackType)
    .splice(0, Object.entries(FeedbackType).length / 2)
    .map(([key, value]) => ({
      value: key,
      label:
        ReadableEnum[value as keyof typeof ReadableEnum] ?? value.toString(),
    }));
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
      name: feedbackQuestion ? feedbackQuestion.name : "",
      type: feedbackQuestion ? feedbackQuestion.type.toString() : "",
      // @ts-ignore
      answers: feedbackQuestion
        ? feedbackQuestion.feedbackQuestionOptions?.map((x) => ({
            option: x.option,
          }))
        : [{ option: "" }],
    },
    validate: yupResolver(schema),
  });
  const { t } = useTranslation();
  const addFeedbackQuestions = useAddFeedbackQuestion(lessonId, search);
  const editFeedbackQuestion = useEditFeedbackQuestion(lessonId, search);

  const onSubmit = async (data: ICreateFeedback) => {
    try {
      if (feedbackQuestion) {
        await editFeedbackQuestion.mutateAsync({
          data,
          feedbackId: feedbackQuestion.id,
        });
        showNotification({
          title: t("successful"),
          message: t("edit_feedback_question_success"),
        });
      } else {
        await addFeedbackQuestions.mutateAsync({ data });
        showNotification({
          title: t("successful"),
          message: t("add_feedback_question_success"),
        });
        form.reset();
      }
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
              size={fieldSize}
              withAsterisk
              label="Title for Feedback"
              placeholder="Enter Title of Feedback"
              {...form.getInputProps("name")}
            ></TextInput>

            <Select
              mt={20}
              placeholder={"Please Enter Feedback Type"}
              size={fieldSize}
              label="Feedback Type"
              {...form.getInputProps("type")}
              data={getQuestionType()}
              onClick={() => {
                feedbackQuestion &&
                  form.setFieldValue("answers", [
                    { option: "", isSelected: false },
                  ]);
              }}
              withAsterisk
            ></Select>
            {(form.values.type === FeedbackType.MultipleChoice.toString() ||
              form.values.type === FeedbackType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>Options</Text>
                {form.values.answers &&
                  form.values.answers.map((x, i) => (
                    <div key={i} style={{ marginBottom: "30px" }}>
                      <Flex>
                        <TextEditor
                          label={`answers.${i}.option`}
                          formContext={useFormContext}
                        ></TextEditor>
                        <UnstyledButton
                          mx={10}
                          onClick={() => {
                            form.insertListItem(
                              "answers",
                              {
                                option: "",
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
                  addFeedbackQuestions.isLoading
                  // || editAssignment.isLoading
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

export default EditFeedback;
