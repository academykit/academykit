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
import { useState } from 'react';

const CommentReplies = ({
  commentId,
  courseId,
  replyCount,
}: {
  commentId: string;
  courseId: string;
  replyCount: number;
}) => {
  const form = useForm({
    initialValues: {
      content: '',
    },
  });
  const addCommentReply = usePostCommentReply(courseId, commentId);
  const commentReplies = useGetCommentReplies(courseId, commentId, replyCount);
  const { t } = useTranslation();

  const initialVisibleReplies = 3;
  const [visibleReplies, setVisibleReplies] = useState(initialVisibleReplies);
  const hasMoreReplies =
    (commentReplies.data?.items.length as number) > initialVisibleReplies;

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

  const showMoreReplies = () => {
    setVisibleReplies(
      commentReplies.data?.items.length ?? initialVisibleReplies
    );
  };

  const showLessReplies = () => {
    setVisibleReplies(initialVisibleReplies);
  };

  const visibleRepliesData = commentReplies.data?.items.slice(
    0,
    visibleReplies
  );

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
      {visibleRepliesData?.map((x) => (
        <CommentReply
          key={x.id}
          reply={x}
          commentId={commentId}
          courseId={courseId}
        />
      ))}

      {/* show button when replies are more than 3 */}
      {hasMoreReplies && visibleReplies > initialVisibleReplies ? (
        <Button variant="subtle" mx={4} onClick={showLessReplies}>
          See Less
        </Button>
      ) : (
        hasMoreReplies && (
          <Button variant="subtle" mx={4} onClick={showMoreReplies}>
            See More
          </Button>
        )
      )}
    </>
  );
};

export default CommentReplies;
