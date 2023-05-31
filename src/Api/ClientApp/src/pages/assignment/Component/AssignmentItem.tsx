import DeleteModal from "@components/Ui/DeleteModal";
import {
  Box,
  Button,
  Checkbox,
  createStyles,
  Flex,
  Group,
  Modal,
  Paper,
  Select,
  Text,
  Title,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import RichTextEditor from "@mantine/rte";
import { IconEdit, IconTrash } from "@tabler/icons";
import { QuestionType, ReadableEnum } from "@utils/enums";
import {
  IAssignmentQuestion,
  useDeleteAssignmentQuestion,
} from "@utils/services/assignmentService";
import errorType from "@utils/services/axiosError";
import EditAssignment from "./EditAssignment";
import { useTransition } from "react";
import { useTranslation } from "react-i18next";

const useStyle = createStyles((theme) => ({
  wrapper: {
    ":hover": {
      ".action": {
        display: "flex",
      },
    },
    ".action": {
      display: "none",
    },
  },
}));

const AssignmentItem = ({
  data,
  search,
  lessonId,
}: {
  data: IAssignmentQuestion;
  search: string;
  lessonId: string;
}) => {
  const { classes } = useStyle();
  const [edit, setEdit] = useToggle();
  const getQuestionType = () => {
    return Object.entries(QuestionType)
      .splice(0, Object.entries(QuestionType).length / 2)
      .map(([key, value]) => ({
        value: key,
        label:
          ReadableEnum[value as keyof typeof ReadableEnum] ?? value.toString(),
      }));
  };
  const deleteQuestion = useDeleteAssignmentQuestion(lessonId, search);
  const [confirmDelete, setConfirmDelete] = useToggle();
  const { t } = useTranslation();
  const deleteHandler = async () => {
    try {
      await deleteQuestion.mutateAsync({ assignmentId: data.id });
      showNotification({
        message: t("delete_assignment_question_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
    setConfirmDelete();
  };
  if (edit) {
    return (
      <EditAssignment
        lessonId={lessonId}
        search={search}
        onCancel={() => {
          setEdit();
        }}
        assignmentQuestion={data}
      />
    );
  }
  return (
    <Flex gap={"lg"} className={classes.wrapper}>
      <DeleteModal
        title={`Are you sure you want to delete this assignment question?`}
        open={confirmDelete}
        onClose={setConfirmDelete}
        onConfirm={deleteHandler}
      />

      <Paper shadow={"lg"} sx={{ width: "100%" }} my={20} withBorder p={20}>
        <Title>{data.name}</Title>

        {data.description && (
          <Box my={10}>
            <Text>{"Description"}</Text>
            <RichTextEditor mb={5} value={data.description} readOnly={true} />
          </Box>
        )}
        {data?.hints && (
          <Box my={10}>
            <Text size={"sm"}>{"Hint"}</Text>
            <RichTextEditor mb={5} value={data?.hints} readOnly={true} />
          </Box>
        )}
        <Select
          mt={20}
          placeholder={"Question Type"}
          label="Question Type"
          data={getQuestionType()}
          value={data.type.toString()}
          onChange={() => {}}
          disabled
        ></Select>
        <Box my={20}>
          {(data.type === QuestionType.MultipleChoice ||
            data.type === QuestionType.SingleChoice) && (
            <>
              <Text>Options</Text>
              {data.assignmentQuestionOptions?.map((x) => (
                <Group my={10} key={x.id}>
                  <Checkbox onChange={() => {}} checked={x.isCorrect} />
                  <RichTextEditor
                    w={"90%"}
                    readOnly
                    value={x.option}
                  ></RichTextEditor>
                </Group>
              ))}
            </>
          )}
        </Box>
      </Paper>
      <Flex
        className={"action"}
        direction={"column"}
        align="center"
        justify={"center"}
        gap={20}
      >
        <Button variant="subtle" onClick={() => setEdit()}>
          <IconEdit />
        </Button>
        <Button variant="subtle" color="red" onClick={() => setConfirmDelete()}>
          <IconTrash />
        </Button>
      </Flex>
    </Flex>
  );
};

export default AssignmentItem;
