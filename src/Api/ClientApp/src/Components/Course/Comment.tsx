import DeleteModal from '@components/Ui/DeleteModal';
import useAuth from '@hooks/useAuth';
import {
  Avatar,
  Box,
  Button,
  Group,
  Paper,
  Text,
  Textarea,
  Transition,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { UserRole } from '@utils/enums';
import errorType from '@utils/services/axiosError';
import {
  IComment,
  useDeleteComment,
  useEditComment,
} from '@utils/services/commentService';
import { IUser } from '@utils/services/types';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';
import CommentReplies from './CommentReplies';
import classes from './styles/comment.module.css';

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
      if (Number(role) < UserRole.Trainee) {
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

  const onDelete = async () => {
    try {
      await deleteComment.mutateAsync({
        commentId: comment.id,
        courseId: id as string,
      });
      showNotification({
        message: t('delete_comment_success'),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
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
        message: t('edit_comment_success'),
      });
      setEdit();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  const form = useForm({
    initialValues: {
      content: comment.content,
    },
  });

  moment.updateLocale('en', {
    relativeTime: {
      future: 'in %s',
      past: `%s`,
      s: `${t('few_seconds_ago')}`,
      ss: `%d ${t('seconds_ago')}`,
      m: `%d ${t('minute_ago')}`,
      mm: `%d ${t('minutes_ago')}`,
      h: `%d ${t('hour_ago')}`,
      hh: `%d ${t('hours_ago')}`,
      d: `%d ${t('day_ago')}`,
      dd: `%d ${t('days_ago')}`,
      M: `%d ${t('month_ago')}`,
      MM: `%d ${t('months_ago')}`,
      y: `%d ${t('year_ago')}`,
      yy: `%d ${t('years_ago')}`,
    },
  });

  return (
    <Paper withBorder p={10} my={'xs'} radius="md" shadow="md">
      <DeleteModal
        title={t('delete_comment_confirmation')}
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
          <Text size="xs" c="dimmed">
            {moment(comment.createdOn + 'Z').fromNow()}
          </Text>
        </div>
      </Group>
      {edit ? (
        <form onSubmit={form.onSubmit(onEdit)}>
          <Textarea
            mt={20}
            {...form.getInputProps('content')}
            style={{ minHeight: edit && '5rem' }}
            styles={{
              root: {
                border: 'none',
              },
            }}
          />

          <Group>
            <Button
              size="sm"
              type="submit"
              disabled={!form.values.content.trim()}
              style={{ '&[data-disabled]': { pointerEvents: 'all' } }}
            >
              {t('save')}
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => {
                setEdit();
                form.reset();
              }}
            >
              {t('cancel')}
            </Button>
          </Group>
        </form>
      ) : (
        <Text className={classes.body} size="sm">
          {comment?.content}
        </Text>
      )}
      <Box style={{ display: 'flex', justifyContent: 'end' }}>
        <Button variant="subtle" mx={4} onClick={() => setToggle()}>
          {toggle
            ? t('hide_reply')
            : `${t('show_reply')}(${comment.repliesCount}) `}
        </Button>

        {!edit && showEdit(comment.user, true) && (
          <Button variant="subtle" mx={4} onClick={() => setEdit()}>
            {t('edit')}
          </Button>
        )}
        {showEdit(comment.user) && comment.repliesCount <= 0 && (
          <Button
            loading={deleteComment.isLoading}
            variant="subtle"
            mx={4}
            c="red"
            onClick={() => setDeleteConfirmation()}
          >
            {t('delete')}
          </Button>
        )}
      </Box>
      <Transition
        mounted={toggle}
        transition={'pop-top-left'}
        duration={200}
        timingFunction="ease"
      >
        {(styles) => (
          <Box style={{ ...styles }} p="sm" mt={1}>
            {toggle ? (
              <CommentReplies
                replyCount={comment.repliesCount}
                commentId={comment.id}
                courseId={id as string}
              />
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
