import { Button, Group, Loader, Textarea } from '@mantine/core';
import { useForm } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import errorType from '@utils/services/axiosError';
import {
  useGetCommentReplies,
  usePostCommentReply,
} from '@utils/services/commentService';
import CommentReply from './CommentReply';
import { useTranslation } from 'react-i18next';

const CommentReplies = ({
  commentId,
  courseId,
}: {
  commentId: string;
  courseId: string;
}) => {
  const form = useForm({
    initialValues: {
      content: '',
    },
  });
  const addCommentReply = usePostCommentReply(courseId, commentId);
  const commentReplies = useGetCommentReplies(courseId, commentId);
  const { t } = useTranslation();

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
        color: 'red',
      });
    }
  };
  if (commentReplies.isLoading) {
    return <Loader />;
  }
  return (
    <>
      <form onSubmit={form.onSubmit(submitHandler)}>
        <Group mx={14}>
          <Textarea
            {...form.getInputProps('content')}
            sx={{ width: '100%' }}
            placeholder={t('your_comment') as string}
            withAsterisk
          />
          <Button
            type="submit"
            loading={addCommentReply.isLoading}
            disabled={!form.values.content.trim()}
            sx={{ '&[data-disabled]': { pointerEvents: 'all' } }}
          >
            {t('reply')}
          </Button>
        </Group>
      </form>
      {commentReplies.data?.items.map((x) => (
        <CommentReply
          key={x.id}
          reply={x}
          commentId={commentId}
          courseId={courseId}
        />
      ))}
    </>
  );
};

export default CommentReplies;
