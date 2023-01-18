import DeleteModal from "@components/Ui/DeleteModal";
import useAuth from "@hooks/useAuth";
import {
  createStyles,
  Text,
  Avatar,
  Group,
  Box,
  Button,
  Textarea,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  ICommentReply,
  useDeleteCommentReply,
  useEditCommentReply,
} from "@utils/services/commentService";
import { IUser } from "@utils/services/types";
import moment from "moment";

const useStyles = createStyles((theme) => ({
  wrapper: {
    border: 1,
    backgroundColor:
      theme.colorScheme === "dark"
        ? theme.colors.dark[4]
        : theme.colors.gray[4],
    borderRadius: theme.radius.md,
  },
  body: {
    paddingLeft: 54,
    paddingTop: theme.spacing.sm,
  },
}));

const CommentReply = ({
  reply,
  courseId,
  commentId,
}: {
  reply: ICommentReply;
  courseId: string;
  commentId: string;
}) => {
  const { classes } = useStyles();

  const deleteReply = useDeleteCommentReply(courseId, commentId);
  const editReply = useEditCommentReply(courseId, commentId);

  const onDelete = async () => {
    try {
      await deleteReply.mutateAsync({
        commentId,
        courseId,
        replyId: reply.id,
      });
      showNotification({ message: "Successfully deleted comment reply." });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  const [deleteConfirmation, setDeleteConfirmation] = useToggle();
  const [edit, setEdit] = useToggle();
  const auth = useAuth();
  const showEdit = (user: IUser, edit = false) => {
    if (edit) {
      if (user.id === auth?.auth?.id) {
        return true;
      }
    } else {
      const role = auth?.auth?.role ?? UserRole.Trainee;
      if (role < UserRole.Trainee) {
        return true;
      } else {
        if (auth?.auth?.id === user.id) return true;
        return false;
      }
    }
  };
  const form = useForm({
    initialValues: {
      content: reply.content,
    },
  });
  const submitForm = async ({ content }: { content: string }) => {
    try {
      await editReply.mutateAsync({
        commentId,
        courseId,
        content,
        replyId: reply.id,
      });
      setEdit();
      showNotification({
        message: "Successfully edited reply.",
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <Box m={10} p={10} className={classes.wrapper}>
      <DeleteModal
        title={"Are you sure you want to delete this reply?"}
        open={deleteConfirmation}
        onClose={setDeleteConfirmation}
        onConfirm={onDelete}
      />

      <Group>
        <Avatar
          src={reply.user.imageUrl}
          alt={reply.user.fullName}
          radius="xl"
        />
        <div>
          <Text size="sm">{reply.user.fullName}</Text>
          <Text size="xs" color="dimmed">
            {moment(reply.createdOn + "Z").fromNow()}
          </Text>
        </div>
      </Group>
      {edit ? (
        <Box mx={40}>
          <form onSubmit={form.onSubmit(submitForm)}>
            <Textarea
              {...form.getInputProps("content")}
              sx={{ width: "100%" }}
              mt={20}
              mb={10}
              placeholder="Your comment"
              withAsterisk
            />
            <Group>
              <Button loading={editReply.isLoading} size="sm" type="submit">
                Edit
              </Button>
              <Button size="sm" variant="outline" onClick={() => setEdit()}>
                Cancel
              </Button>
            </Group>
          </form>
        </Box>
      ) : (
        <Text className={classes.body} size="sm">
          {reply.content}
        </Text>
      )}
      <Box sx={{ display: "flex", justifyContent: "end" }}>
        {!edit && showEdit(reply.user, true) && (
          <Button variant="subtle" mx={4} onClick={() => setEdit()}>
            Edit
          </Button>
        )}
        {showEdit(reply.user) && (
          <Button
            onClick={() => setDeleteConfirmation()}
            variant="subtle"
            mx={4}
          >
            Delete
          </Button>
        )}
      </Box>
    </Box>
  );
};

export default CommentReply;
