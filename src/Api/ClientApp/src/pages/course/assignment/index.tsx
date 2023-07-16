import { Button, Container, createStyles, Divider, Title } from '@mantine/core';

import { useAssignmentQuestion } from '@utils/services/assignmentService';
import { useNavigate, useParams } from 'react-router-dom';
import AssignmentForm from './Component/AssignmentForm';
import { useTranslation } from 'react-i18next';

const useStyles = createStyles((theme) => ({
  root: {
    paddingTop: 80,
    paddingBottom: 80,
  },

  title: {
    fontWeight: 900,
    fontSize: 34,
    marginBottom: theme.spacing.md,
    fontFamily: `Greycliff CF, ${theme.fontFamily}`,

    [theme.fn.smallerThan('sm')]: {
      fontSize: 32,
    },
  },

  control: {
    [theme.fn.smallerThan('sm')]: {
      width: '100%',
    },
  },
}));

const AssignmentPage = () => {
  const { classes } = useStyles();
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
