import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import { Box, Button, Container } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { useAssignmentQuestion } from '@utils/services/assignmentService';
import AssignmentItem from './Component/AssignmentItem';
import EditAssignment from './Component/EditAssignment';
import { useTranslation } from 'react-i18next';

interface Props {
  lessonId: string;
}

const CreateAssignment = ({
  searchParams,
  lessonId,
}: Props & IWithSearchPagination) => {
  const [addQuestion, setAddQuestion] = useToggle();

  const questionList = useAssignmentQuestion(lessonId, searchParams);
  const { t } = useTranslation();
  return (
    <Container>
      {questionList.isSuccess && (
        <>
          {questionList.data.length > 0 ? (
            <Box>
              {questionList.data.map((x) => (
                <AssignmentItem
                  key={x.id}
                  data={x}
                  search={searchParams}
                  lessonId={lessonId}
                />
              ))}
            </Box>
          ) : (
            <Box mt={10}>{t('no_assignment_questions')}</Box>
          )}
          {addQuestion && (
            <EditAssignment
              onCancel={() => setAddQuestion()}
              lessonId={lessonId}
              search={searchParams}
            />
          )}

          {!addQuestion && (
            <Button mt={10} onClick={() => setAddQuestion()}>
              {t('add_question')}
            </Button>
          )}
        </>
      )}
    </Container>
  );
};

export default withSearchPagination(CreateAssignment);
