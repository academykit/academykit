import DeleteModal from '@components/Ui/DeleteModal';
import TextViewer from '@components/Ui/RichTextViewer';
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
import { IconEdit, IconTrash } from '@tabler/icons-react';
import { AssessmentType, FeedbackType, ReadableEnum } from '@utils/enums';
import {
  IAssessmentQuestion,
  useDeleteAssessmentQuestion,
} from '@utils/services/assessmentService';
import errorType from '@utils/services/axiosError';
import { t } from 'i18next';
import AssessmentQuestionForm from './Assessment Details/AssessmentQuestionForm';

interface IProps {
  data: IAssessmentQuestion;
}

const AssessmentItem = ({ data }: IProps) => {
  const [edit, setEdit] = useToggle();
  const [confirmDelete, setConfirmDelete] = useToggle();
  const deleteAssessmentQuestion = useDeleteAssessmentQuestion();

  const getQuestionType = () => {
    return Object.entries(AssessmentType)
      .splice(0, Object.entries(AssessmentType).length / 2)
      .map(([key, value]) => ({
        value: key,
        label:
          ReadableEnum[value as keyof typeof ReadableEnum] ?? value.toString(),
      }));
  };

  const deleteHandler = async () => {
    try {
      await deleteAssessmentQuestion.mutateAsync(data.id);
      showNotification({
        message: t('delete_question_success'),
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
      <AssessmentQuestionForm onCancel={() => setEdit(false)} data={data} />
    );
  }

  return (
    <>
      <DeleteModal
        title={t('delete_assessment_question_confirmation')}
        open={confirmDelete}
        onClose={setConfirmDelete}
        onConfirm={deleteHandler}
        loading={deleteAssessmentQuestion.isLoading}
      />

      <Paper shadow={'lg'} style={{ width: '100%' }} my={20} withBorder p={20}>
        <Flex justify={'space-between'}>
          <Title>{data.questionName}</Title>
          <Group>
            <Button
              variant="subtle"
              onClick={() => {
                setEdit();
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
        {data.hints && (
          <Box my={10}>
            <Text size={'sm'}>{t('hint')}</Text>
            <TextViewer key={data.id} content={data.hints} />
          </Box>
        )}
        <Select
          mt={20}
          placeholder={t('assessment_type') as string}
          label={t('assessment_type')}
          data={getQuestionType()}
          value={data.type.toString()}
          disabled
        ></Select>
        <Box my={20}>
          {(data.type === FeedbackType.MultipleChoice ||
            data.type === FeedbackType.SingleChoice) && (
            <>
              <Text>{t('options')}</Text>
              {data.assessmentQuestionOptions?.map((x) => (
                <Group my={10} key={x.id}>
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
    </>
  );
};

export default AssessmentItem;
