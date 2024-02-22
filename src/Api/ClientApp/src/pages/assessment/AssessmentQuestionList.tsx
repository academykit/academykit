import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import { Button } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { useAssessmentQuestion } from '@utils/services/assessmentService';
import { t } from 'i18next';
import { useNavigate, useParams } from 'react-router-dom';
import AssessmentQuestionForm from './Assessment Details/AssessmentQuestionForm';
import AssessmentItem from './AssessmentItem';

const AssessmentQuestionList = ({
  searchParams,
  pagination,
}: IWithSearchPagination) => {
  const navigate = useNavigate();
  const params = useParams();
  const [addQuestion, setAddQuestion] = useToggle();
  const getAssessments = useAssessmentQuestion(
    searchParams,
    params.id as string
  );

  return (
    <>
      {getAssessments.data?.items.map((question, index) => (
        <AssessmentItem key={index} data={question} />
      ))}

      {getAssessments.data &&
        pagination(
          getAssessments.data?.totalPage,
          getAssessments.data?.items.length
        )}

      {addQuestion && (
        <AssessmentQuestionForm onCancel={() => setAddQuestion()} />
      )}

      <Button disabled={addQuestion} onClick={() => setAddQuestion()} mt={10}>
        {t('add_assessment_question')}
      </Button>
      <Button ml={10} variant="outline" onClick={() => navigate(-1)} mt={10}>
        {t('go_back_button')}
      </Button>
    </>
  );
};

export default withSearchPagination(AssessmentQuestionList);
