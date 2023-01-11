import { Button, Group, Loader, Textarea } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  useGetCommentReplies,
  usePostCommentReply,
} from "@utils/services/commentService";
import CommentReply from "./CommentReply";

const CommentReplies = ({
  commentId,
  courseId,
}: {
  commentId: string;
  courseId: string;
}) => {
  const form = useForm({
    initialValues: {
      content: "",
    },
  });
  const addCommentReply = usePostCommentReply(courseId, commentId);
  const commentReplies = useGetCommentReplies(courseId, commentId);

  const submitHandler = async ({ content }: { content: string }) => {
    try {
      await addCommentReply.mutateAsync({
        commentId,
        courseId,
        content,
      });
      form.reset();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  if (commentReplies.isLoading) {
    return <Loader />;
  }
  return (
    <>
      {commentReplies.data?.items.map((x) => (
        <CommentReply
          key={x.id}
          reply={x}
          commentId={commentId}
          courseId={courseId}
        />
      ))}

      <form onSubmit={form.onSubmit(submitHandler)}>
        <Group mx={14}>
          <Textarea
            {...form.getInputProps("content")}
            sx={{ width: "100%" }}
            placeholder="Your comment"
            withAsterisk
          />
          <Button type="submit" loading={addCommentReply.isLoading}>
            Reply
          </Button>
        </Group>
      </form>
    </>
  );
};

export default CommentReplies;
