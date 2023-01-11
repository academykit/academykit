import TextEditor from "@components/Ui/TextEditor";
import {
  Box,
  Button,
  Checkbox,
  Container,
  Group,
  Paper,
  Select,
  Text,
  TextInput,
  UnstyledButton,
} from "@mantine/core";
import { createFormContext } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconPlus, IconTrash } from "@tabler/icons";
import { FeedbackType, ReadableEnum } from "@utils/enums";

import errorType from "@utils/services/axiosError";
import {
  ICreateFeedback,
  IFeedbackQuestions,
  useAddFeedbackQuestion,
  useEditFeedbackQuestion,
} from "@utils/services/feedbackService";
const fieldSize = "md";
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
  });

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
          title: "Successful",
          message: "Successfully edited feedback question.",
        });
      } else {
        await addFeedbackQuestions.mutateAsync({ data });
        showNotification({
          title: "Successful",
          message: "Successfully added feedback question.",
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
            ></Select>
            {(form.values.type === FeedbackType.MultipleChoice.toString() ||
              form.values.type === FeedbackType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>Options</Text>
                {form.values.answers &&
                  form.values.answers.map((x, i) => (
                    <Group key={i} mb={30}>
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
                      {typeof form.errors[`answers.${i}.option`] ===
                        "string" && (
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
              <Button size="sm" type="reset" onClick={onCancel}>
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
