import { Button, Container, Divider, Title } from '@mantine/core';
import { useAssignmentQuestion } from '@utils/services/assignmentService';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import classes from '../styles/assignment.module.css';
import AssignmentForm from './Component/AssignmentForm';

const AssignmentPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t } = useTranslation();
  const assignment = useAssignmentQuestion(id as string, '');

  if (assignment.isSuccess && assignment.data?.length === 0) {
    return (
      <Container className={classes.root}>
        <div>
          <Title className={classes.title}>{t('no_question_found')}</Title>

          <Button
            variant="outline"
            size="md"
            mt="xl"
            onClick={() => navigate(-1)}
            className={classes.control}
          >
            {t('back_course')}
          </Button>
        </div>
      </Container>
    );
  }

  return (
    <Container my={50}>
      <Divider />
      {assignment.isSuccess && (
        <AssignmentForm item={assignment.data} lessonId={id as string} />
      )}
    </Container>
  );
};

export default AssignmentPage;
