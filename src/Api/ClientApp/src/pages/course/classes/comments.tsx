import Comment from "@components/Course/Comment";
import { Box, Button, Loader, Textarea } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import RichTextEditor from "@mantine/rte";
import errorType from "@utils/services/axiosError";
import { useGetComments, usePostComment } from "@utils/services/commentService";
import { useParams } from "react-router-dom";

const Comments = () => {
  const { id } = useParams();
  const { data, isLoading, isError, error } = useGetComments(id as string);
  const form = useForm({
    initialValues: {
      content: "",
    },
  });
  const postComment = usePostComment(id as string);
  if (isLoading) {
    return <Loader />;
  }

  const onSubmit = async ({ content }: { content: string }) => {
    try {
      await postComment.mutateAsync({
        content,
        courseId: id as string,
      });
      form.reset();
      showNotification({ message: "Comment added successfully" });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <Box>
      {data?.items.map((x) => (
        <Comment comment={x} key={x.id} />
      ))}
      <form onSubmit={form.onSubmit(onSubmit)}>
        <Textarea
          placeholder="Write your comment here"
          {...form.getInputProps("content")}
          mt={20}
          sx={{ minHeight: "5rem" }}
        />
        <Button loading={postComment.isLoading} type="submit" mt={10}>
          Post
        </Button>
      </form>
    </Box>
  );
};

export default Comments;