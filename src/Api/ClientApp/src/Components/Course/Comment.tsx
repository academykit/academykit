import useAuth from "@hooks/useAuth";
import {
  createStyles,
  Text,
  Avatar,
  Group,
  Paper,
  Box,
  Button,
  Transition,
  Textarea,
} from "@mantine/core";
import { UserRole } from "@utils/enums";
import { useForm } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  IComment,
  useDeleteComment,
  useEditComment,
} from "@utils/services/commentService";
import { IUser } from "@utils/services/types";
import moment from "moment";
import { useParams } from "react-router-dom";
import CommentReplies from "./CommentReplies";
import DeleteModal from "@components/Ui/DeleteModal";
import { useTranslation } from "react-i18next";
const useStyles = createStyles((theme) => ({
  comment: {
    padding: `${theme.spacing.lg}px ${theme.spacing.xl}px`,
    // backgroundColor:
    //   theme.colorScheme === "dark"
    //     ? theme.colors.dark[1]
    //     : theme.colors.gray[2],
  },

  body: {
    paddingLeft: 54,
    paddingTop: theme.spacing.sm,
    fontSize: theme.fontSizes.sm,
  },

  content: {
    "& > p:last-child": {
      marginBottom: 0,
    },
  },
  editor: {
    backgroundColor:
      theme.colorScheme === "dark"
        ? theme.colors.dark[1]
        : theme.colors.gray[2],
  },

  replies: {
    backgroundColor:
      theme.colorScheme === "dark"
        ? theme.colors.dark[3]
        : theme.colors.gray[1],

    borderRadius: theme.radius.md,
  },
}));

const Comment = ({ comment }: { comment: IComment }) => {
  const auth = useAuth();
  const { t } = useTranslation();

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
  const [toggle, setToggle] = useToggle();
  const { id } = useParams();
  const [deleteConfirmation, setDeleteConfirmation] = useToggle();
  const [edit, setEdit] = useToggle();

  const deleteComment = useDeleteComment(id as string);
  const editComment = useEditComment(id as string, comment.id);
  const { classes, cx } = useStyles();
  const onDelete = async () => {
    try {
      await deleteComment.mutateAsync({
        commentId: comment.id,
        courseId: id as string,
      });
      showNotification({
        message: t("delete_comment_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  const onEdit = async ({ content }: { content: string }) => {
    try {
      await editComment.mutateAsync({
        commentId: comment.id,
        courseId: id as string,
        content,
      });
      showNotification({
        message: t("edit_comment_success"),
      });
      setEdit();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  const form = useForm({
    initialValues: {
      content: comment.content,
    },
  });

  return (
    <Paper
      withBorder
      my={5}
      radius="md"
      className={classes.comment}
      shadow="md"
    >
      <DeleteModal
        title={t("delete_comment_confirmation")}
        open={deleteConfirmation}
        onClose={setDeleteConfirmation}
        onConfirm={onDelete}
      />

      <Group>
        <Avatar
          src={comment.user.imageUrl}
          alt={comment.user.fullName}
          radius="xl"
        />
        <div>
          <Text size="sm">{comment.user.fullName}</Text>
          <Text size="xs" color="dimmed">
            {moment(comment.createdOn + "Z").fromNow()}
          </Text>
        </div>
      </Group>
      {edit ? (
        <form onSubmit={form.onSubmit(onEdit)}>
          <Textarea
            autoFocus={true}
            className={cx({ [classes.editor]: !edit })}
            mt={20}
            mb={10}
            {...form.getInputProps("content")}
            sx={{ minHeight: edit && "5rem" }}
            readOnly={edit ? false : true}
            styles={{
              root: {
                border: "none",
              },
            }}
          />
          {edit && (
            <Group>
              <Button
                size="sm"
                type="submit"
                disabled={!form.values.content.trim()}
              >
                {t("save")}
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={() => {
                  setEdit();
                  form.reset();
                }}
              >
                {t("cancel")}
              </Button>
            </Group>
          )}
        </form>
      ) : (
        <Text className={classes.body} size="sm">
          {comment?.content}
        </Text>
      )}
      <Box sx={{ display: "flex", justifyContent: "end" }}>
        <Button variant="subtle" mx={4} onClick={() => setToggle()}>
          {toggle ? "Hide Reply" : `Show Reply(${comment.repliesCount}) `}
        </Button>

        {!edit && showEdit(comment.user, true) && (
          <Button variant="subtle" mx={4} onClick={() => setEdit()}>
            {t("edit")}
          </Button>
        )}
        {showEdit(comment.user) && comment.repliesCount <= 0 && (
          <Button
            loading={deleteComment.isLoading}
            variant="subtle"
            mx={4}
            onClick={() => setDeleteConfirmation()}
          >
            {t("delete")}
          </Button>
        )}
      </Box>
      <Transition
        mounted={toggle}
        transition={"pop-top-left"}
        duration={200}
        timingFunction="ease"
      >
        {(styles) => (
          <Box
            style={{ ...styles }}
            mx={10}
            className={classes.replies}
            p="lg"
            mt={10}
          >
            {toggle ? (
              <CommentReplies commentId={comment.id} courseId={id as string} />
            ) : (
              <></>
            )}
          </Box>
        )}
      </Transition>
    </Paper>
  );
};
export default Comment;
