import DeleteModal from "@components/Ui/DeleteModal";
import {
  Box,
  Button,
  createStyles,
  Flex,
  Group,
  Paper,
  Select,
  Text,
  Title,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconTrash } from "@tabler/icons";
import { FeedbackType, ReadableEnum } from "@utils/enums";

import errorType from "@utils/services/axiosError";
import {
  IFeedbackQuestions,
  useDeleteFeedbackQuestion,
} from "@utils/services/feedbackService";
import EditFeedback from "./EditFeedBack";
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

const FeedbackItem = ({
  data,
  search,
  lessonId,
}: {
  data: IFeedbackQuestions;
  search: string;
  lessonId: string;
}) => {
  const { classes } = useStyle();
  const [edit, setEdit] = useToggle();
  const getQuestionType = () => {
    return Object.entries(FeedbackType)
      .splice(0, Object.entries(FeedbackType).length / 2)
      .map(([key, value]) => ({
        value: key,
        label:
          ReadableEnum[value as keyof typeof ReadableEnum] ?? value.toString(),
      }));
  };
  const deleteFeedback = useDeleteFeedbackQuestion(lessonId, search);
  const [confirmDelete, setConfirmDelete] = useToggle();
  const { t } = useTranslation();
  const deleteHandler = async () => {
    try {
      await deleteFeedback.mutateAsync({ feedbackId: data.id });
      showNotification({
        message: t("edit_feedback_question_success"),
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
      <EditFeedback
        lessonId={lessonId}
        search={search}
        onCancel={() => {
          setEdit();
        }}
        feedbackQuestion={data}
      />
    );
  }
  return (
    <Flex gap={"lg"} className={classes.wrapper}>
      <DeleteModal
        title={t("delete_feedback_question_confirmation")}
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
        {data.hint && (
          <Box my={10}>
            <Text size={"sm"}>{t("hint")}</Text>
            <TextViewer content={data.hint} />
          </Box>
        )}
        <Select
          mt={20}
          placeholder={t("feedback_type") as string}
          label={t("feedback_type")}
          data={getQuestionType()}
          value={data.type.toString()}
          onChange={() => {}}
          disabled
        ></Select>
        <Box my={20}>
          {(data.type === FeedbackType.MultipleChoice ||
            data.type === FeedbackType.SingleChoice) && (
            <>
              <Text>{t("options")}</Text>
              {data.feedbackQuestionOptions?.map((x) => (
                <Group my={10} key={x.id}>
                  <TextViewer content={x.option}></TextViewer>
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

export default FeedbackItem;
