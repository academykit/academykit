import Comment from "@components/Course/Comment";
import { Box, Button, Loader, Textarea } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import { useGetComments, usePostComment } from "@utils/services/commentService";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

const Comments = () => {
  const { id } = useParams();
  const { data, isLoading } = useGetComments(id as string);
  const { t } = useTranslation();
  const form = useForm({
    initialValues: {
      content: "",
    },
  });
  const postComment = usePostComment(id as string);

  const onSubmit = async ({ content }: { content: string }) => {
    try {
      await postComment.mutateAsync({
        content,
        courseId: id as string,
      });
      form.reset();
      showNotification({ message: t("add_comment_success") });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  if (isLoading) {
    return <Loader />;
  }

  return (
    <Box mb={"lg"}>
      <form onSubmit={form.onSubmit(onSubmit)}>
        <Textarea
          placeholder={t("your_comment_here") as string}
          {...form.getInputProps("content")}
          mt={20}
          style={{ minHeight: "5rem" }}
        />
        <Button
          loading={postComment.isPending}
          type="submit"
          mb={"xs"}
          disabled={!form.values.content.trim()}
          style={{ "&[data-disabled]": { pointerEvents: "all" } }}
        >
          {t("post")}
        </Button>
      </form>
      {data?.items.map((x) => <Comment comment={x} key={x.id} />)}
    </Box>
  );
};

export default Comments;
