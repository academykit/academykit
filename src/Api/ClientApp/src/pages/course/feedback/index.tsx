import FeedbackForm from '@components/Course/FeedBack/ViewFeedback';
import { Title, Button, Container, createStyles } from '@mantine/core';

import { useFeedbackQuestion } from '@utils/services/feedbackService';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';

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

const FeedbackPage = () => {
  const { id } = useParams();
  const { classes } = useStyles();
  const { t } = useTranslation();
  const feedback = useFeedbackQuestion(id as string, '');
  const navigate = useNavigate();

  if (feedback.data && feedback.data?.length < 1) {
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
            {t('go_back_button')}
          </Button>
        </div>
      </Container>
    );
  }

  return (
    <Container my={50}>
      {feedback.isSuccess && (
        <FeedbackForm item={feedback.data} lessonId={id as string} />
      )}
    </Container>
  );
};

export default FeedbackPage;
