import { Button, Group, Title } from '@mantine/core';
import RoutePath from '@utils/routeConstants';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const FeedbackDetails = ({
  name,
  id,
  hasFeedbackSubmitted,
  isTrainee,
}: {
  name: string;
  id: string;
  hasFeedbackSubmitted: boolean;
  isTrainee: boolean;
}) => {
  const { t } = useTranslation();
  return (
    <Group sx={{ flexDirection: 'column' }}>
      <Title>{name}</Title>
      {!hasFeedbackSubmitted ? (
        <Button component={Link} to={RoutePath.feedback.details(id).route}>
          {/* mock view without any answers present */}
          {isTrainee ? t('give_feedback') : t('view_feedback')}
        </Button>
      ) : (
        <Button component={Link} to={RoutePath.feedback.myDetails(id).route}>
          {t('view_feedback')}
        </Button>
      )}
    </Group>
  );
};

export default FeedbackDetails;
