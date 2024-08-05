import DeleteModal from "@components/Ui/DeleteModal";
import useAuth from "@hooks/useAuth";
import { Avatar, Box, Button, Group, Text, Textarea } from "@mantine/core";
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
import { useTranslation } from "react-i18next";
import classes from "./styles/reply.module.css";

const CommentReply = ({
  reply,
  courseId,
  commentId,
}: {
  reply: ICommentReply;
  courseId: string;
  commentId: string;
}) => {
  const deleteReply = useDeleteCommentReply(courseId, commentId);
  const editReply = useEditCommentReply(courseId, commentId);
  const { t } = useTranslation();
  const onDelete = async () => {
    try {
      await deleteReply.mutateAsync({
        commentId,
        courseId,
        replyId: reply.id,
      });
      showNotification({ message: t("delete_comment_reply_success") });
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
      if (Number(role) < UserRole.Trainee) {
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
        message: t("edit_reply_success"),
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
    <Box m={10} p={10}>
      <DeleteModal
        title={t("delete_reply_confirmation")}
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
          <Text size="xs" c="dimmed">
            {moment(reply.createdOn + "Z").fromNow()}
          </Text>
        </div>
      </Group>
      {edit ? (
        <Box mx={40}>
          <form onSubmit={form.onSubmit(submitForm)}>
            <Textarea
              {...form.getInputProps("content")}
              style={{ width: "100%" }}
              mt={20}
              mb={10}
              placeholder={t("your_comment") as string}
              withAsterisk
            />
            <Group>
              <Button
                loading={editReply.isLoading}
                size="sm"
                type="submit"
                disabled={!form.values.content.trim()}
                style={{ "&[data-disabled]": { pointerEvents: "all" } }}
              >
                {t("edit")}
              </Button>
              <Button
                size="sm"
                variant="outline"
                onClick={() => {
                  setEdit();
                  form.reset();
                }}
              >
                {t("cancel")}
              </Button>
            </Group>
          </form>
        </Box>
      ) : (
        <Text className={classes.body} size="sm">
          {reply.content}
        </Text>
      )}
      <Box style={{ display: "flex", justifyContent: "end" }}>
        {!edit && showEdit(reply.user, true) && (
          <Button variant="subtle" mx={4} onClick={() => setEdit()}>
            {t("edit")}
          </Button>
        )}
        {showEdit(reply.user) && (
          <Button
            onClick={() => setDeleteConfirmation()}
            variant="subtle"
            c="red"
            mx={4}
          >
            {t("delete")}
          </Button>
        )}
      </Box>
    </Box>
  );
};

export default CommentReply;
