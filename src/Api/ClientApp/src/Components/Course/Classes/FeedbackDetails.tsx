/* eslint-disable */
import { Box, Button, Group, Title } from '@mantine/core';
import RoutePath from '@utils/routeConstants';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const FeedbackDetails = ({
  name,
  id,
  hasFeedbackSubmitted,
}: {
  name: string;
  id: string;
  hasFeedbackSubmitted: boolean;
}) => {
  const { t } = useTranslation();
  return (
    <Group sx={{ flexDirection: 'column' }}>
      <Title>{name}</Title>
      {!hasFeedbackSubmitted ? (
        <Button component={Link} to={RoutePath.feedback.details(id).route}>
          {t('give_feedback')}
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
