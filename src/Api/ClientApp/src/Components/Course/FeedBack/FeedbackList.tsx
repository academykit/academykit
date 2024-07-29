import DeleteModal from '@components/Ui/DeleteModal';
import {
    Box,
    Button,
    Flex,
    Group,
    Paper,
    Select,
    Text,
    Title,
} from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconDragDrop, IconEdit, IconTrash } from '@tabler/icons-react';
import { FeedbackType, ReadableEnum } from '@utils/enums';

import TextViewer from '@components/Ui/RichTextViewer';
import errorType from '@utils/services/axiosError';
import {
    IFeedbackQuestions,
    useDeleteFeedbackQuestion,
} from '@utils/services/feedbackService';
import { useTranslation } from 'react-i18next';
import classes from '../styles/feedbackList.module.css';
import EditFeedback from './EditFeedBack';

const FeedbackItem = ({
  data,
  search,
  lessonId,
  onEditChange,
}: {
  data: IFeedbackQuestions;
  search: string;
  lessonId: string;
  onEditChange: () => void;
}) => {
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
  const { t } = useTranslation();
  const deleteHandler = async () => {
    try {
      await deleteFeedback.mutateAsync({ feedbackId: data.id });
      showNotification({
        message: t('edit_feedback_question_success'),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
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
          onEditChange();
        }}
        feedbackQuestion={data}
      />
    );
  }
  return (
    <Flex gap={'lg'} className={classes.wrapper}>
      <DeleteModal
        title={t('delete_feedback_question_confirmation')}
        open={confirmDelete}
        onClose={setConfirmDelete}
        onConfirm={deleteHandler}
      />

      <Paper shadow={'lg'} style={{ width: '100%' }} my={20} withBorder p={20}>
        <Flex justify={'space-between'}>
          <Title>{data.name}</Title>
          <Group>
            <IconDragDrop />
            <Button
              variant="subtle"
              onClick={() => {
                setEdit();
                onEditChange();
              }}
            >
              <IconEdit />
            </Button>
            <Button variant="subtle" c="red" onClick={() => setConfirmDelete()}>
              <IconTrash />
            </Button>
          </Group>
        </Flex>

        {data.description && (
          <Box my={10}>
            <Text>{t('description')}</Text>
            <TextViewer key={data.id} content={data.description} />
          </Box>
        )}
        {data.hint && (
          <Box my={10}>
            <Text size={'sm'}>{t('hint')}</Text>
            <TextViewer key={data.id} content={data.hint} />
          </Box>
        )}
        <Select
          mt={20}
          placeholder={t('feedback_type') as string}
          label={t('feedback_type')}
          data={getQuestionType()}
          value={data.type.toString()}
          onChange={() => {}}
          disabled
        ></Select>
        <Box my={20}>
          {(data.type === FeedbackType.MultipleChoice ||
            data.type === FeedbackType.SingleChoice) && (
            <>
              <Text>{t('options')}</Text>
              {data.feedbackQuestionOptions?.map((x) => (
                <Group my={10} key={x.id} id="hehe">
                  <div style={{ width: '100%' }}>
                    <TextViewer
                      content={x.option}
                      styles={{ root: { flexGrow: 1 } }}
                    ></TextViewer>
                  </div>
                </Group>
              ))}
            </>
          )}
        </Box>
      </Paper>
    </Flex>
  );
};

export default FeedbackItem;
