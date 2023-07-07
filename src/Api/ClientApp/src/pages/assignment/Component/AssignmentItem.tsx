import DeleteModal from "@components/Ui/DeleteModal";
import {
  Box,
  Button,
  Checkbox,
  createStyles,
  Flex,
  Paper,
  Select,
  Text,
  Title,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconTrash } from "@tabler/icons";
import { QuestionType, ReadableEnum } from "@utils/enums";
import {
  IAssignmentQuestion,
  useDeleteAssignmentQuestion,
} from "@utils/services/assignmentService";
import errorType from "@utils/services/axiosError";
import EditAssignment from "./EditAssignment";
import { useTranslation } from "react-i18next";
import TextViewer from "@components/Ui/RichTextViewer";

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
        title={t("delete_assignment_question_confirmation")}
        open={confirmDelete}
        onClose={setConfirmDelete}
        onConfirm={deleteHandler}
      />

      <Paper shadow={"lg"} sx={{ width: "100%" }} my={20} withBorder p={20}>
        <Title>{data.name}</Title>

        {data.description && (
          <Box my={10}>
            <Text>{t("description")}</Text>
            <TextViewer content={data.description} />
          </Box>
        )}
        {data?.hints && (
          <Box my={10}>
            <Text size={"sm"}>{t("hint")}</Text>
            <TextViewer content={data?.hints} />
          </Box>
        )}
        <Select
          mt={20}
          placeholder={t("question_type") as string}
          label={t("question_type")}
          data={getQuestionType()}
          value={data.type.toString()}
          onChange={() => {}}
          disabled
        ></Select>
        <Box my={20}>
          {(data.type === QuestionType.MultipleChoice ||
            data.type === QuestionType.SingleChoice) && (
            <>
              <Text>{t("options")}</Text>
              {data.assignmentQuestionOptions?.map((x) => (
                <Flex
                  align={"center"}
                  justify={"center"}
                  gap={"md"}
                  my={10}
                  key={x.id}
                >
                  <Checkbox onChange={() => {}} checked={x.isCorrect} />
                  <TextViewer
                    sx={{ width: "90%" }}
                    content={x.option}
                  ></TextViewer>
                </Flex>
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
