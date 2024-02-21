import { Button, Container, Loader, Title } from '@mantine/core';
import { useAssessmentExamQuestions } from '@utils/services/assessmentService';
import { t } from 'i18next';
import { useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Exam from './component/Exam';

const AssessmentExam = () => {
  const navigate = useNavigate();
  const params = useParams();
  const assessmentExam = useAssessmentExamQuestions(params.id as string);

  useEffect(() => {
    assessmentExam.refetch();
  }, []);

  if (assessmentExam.isLoading) {
    return <Loader />;
  }
  if (assessmentExam.isError) {
    throw assessmentExam.error;
  }

  if (assessmentExam.data.questions.length === 0) {
    return (
      <Container fluid>
        <div>
          <Title>{t('no_question_found')}</Title>

          <Button
            variant="outline"
            size="md"
            mt="xl"
            onClick={() => navigate('/')}
          >
            {t('go_back')}
          </Button>
        </div>
      </Container>
    );
  }

  return <Exam data={assessmentExam.data} assessmentId={params.id as string} />;
};

export default AssessmentExam;
