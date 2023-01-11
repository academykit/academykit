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
import RichTextEditor from "@mantine/rte";
import { IconEdit, IconTrash } from "@tabler/icons";
import { FeedbackType, ReadableEnum } from "@utils/enums";

import errorType from "@utils/services/axiosError";
import {
  IFeedbackQuestions,
  useDeleteFeedbackQuestion,
} from "@utils/services/feedbackService";
import EditFeedback from "./EditFeedBack";

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

  const deleteHandler = async () => {
    try {
      await deleteFeedback.mutateAsync({ feedbackId: data.id });
      showNotification({
        message: "Successfully deleted feedback question.",
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
        title={"Are you sure you want to delete this feedback question?"}
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
        {data.hint && (
          <Box my={10}>
            <Text size={"sm"}>{"Hint"}</Text>
            <RichTextEditor mb={5} value={data.hint} readOnly={true} />
          </Box>
        )}
        <Select
          mt={20}
          placeholder={"Feedback Type"}
          label="Feedback Type"
          data={getQuestionType()}
          value={data.type.toString()}
          onChange={() => {}}
          disabled
        ></Select>
        <Box my={20}>
          {(data.type === FeedbackType.MultipleChoice ||
            data.type === FeedbackType.SingleChoice) && (
            <>
              <Text>Options</Text>
              {data.feedbackQuestionOptions?.map((x) => (
                <Group my={10} key={x.id}>
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

export default FeedbackItem;
