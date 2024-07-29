import DeleteModal from '@components/Ui/DeleteModal';
import TextViewer from '@components/Ui/RichTextViewer';
import {
  Box,
  Button,
  Checkbox,
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
import { QuestionType } from '@utils/enums';
import {
  IAssignmentQuestion,
  useDeleteAssignmentQuestion,
} from '@utils/services/assignmentService';
import errorType from '@utils/services/axiosError';
import { useTranslation } from 'react-i18next';
import EditAssignment from './EditAssignment';

const AssignmentItem = ({
  data,
  search,
  lessonId,
  onEditChange,
}: {
  data: IAssignmentQuestion;
  search: string;
  lessonId: string;
  onEditChange: () => void;
}) => {
  const [edit, setEdit] = useToggle();
  const { t } = useTranslation();
  const getQuestionType = () => {
    return Object.entries(QuestionType)
      .splice(0, Object.entries(QuestionType).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
  };
  const deleteQuestion = useDeleteAssignmentQuestion(lessonId, search);
  const [confirmDelete, setConfirmDelete] = useToggle();
  const deleteHandler = async () => {
    try {
      await deleteQuestion.mutateAsync({ assignmentId: data.id });
      showNotification({
        message: t('delete_assignment_question_success'),
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
      <EditAssignment
        lessonId={lessonId}
        search={search}
        onCancel={() => {
          setEdit();
          onEditChange();
        }}
        assignmentQuestion={data}
      />
    );
  }
  return (
    <Flex gap={'lg'}>
      <DeleteModal
        title={t('delete_assignment_question_confirmation')}
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
            <TextViewer
              key={data.id + data.description}
              content={data.description}
            />
          </Box>
        )}
        {data?.hints && (
          <Box my={10}>
            <Text size={'sm'}>{t('hint')}</Text>
            <TextViewer key={data.id + data.hints} content={data?.hints} />
          </Box>
        )}
        <Select
          mt={20}
          placeholder={t('question_type') as string}
          label={t('question_type')}
          data={getQuestionType()}
          value={data.type.toString()}
          onChange={() => {}}
          disabled
        ></Select>
        <Box my={20}>
          {(data.type === QuestionType.MultipleChoice ||
            data.type === QuestionType.SingleChoice) && (
            <>
              <Text>{t('options')}</Text>
              {data.assignmentQuestionOptions?.map((x) => (
                <Flex
                  align={'center'}
                  justify={'center'}
                  gap={'md'}
                  my={10}
                  key={x.id}
                >
                  <Checkbox readOnly checked={x.isCorrect} />
                  <TextViewer
                    styles={{ root: { flexGrow: 1 } }}
                    content={x.option}
                  ></TextViewer>
                </Flex>
              ))}
            </>
          )}
        </Box>
      </Paper>
    </Flex>
  );
};

export default AssignmentItem;
