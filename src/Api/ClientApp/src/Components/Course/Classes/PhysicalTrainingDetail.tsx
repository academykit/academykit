import { Button, Group, Title, Text } from '@mantine/core';
import RoutePath from '@utils/routeConstants';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const PhysicalTrainingDetail = ({
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
      <Text>{t('start_date')}: </Text>
      <Text>{t('start_time')}: </Text>
      {!hasFeedbackSubmitted ? (
        <Button>{t('mark_as_attended')}</Button>
      ) : (
        <Button component={Link} to={RoutePath.feedback.myDetails(id).route}>
          {t('view_feedback')}
        </Button>
      )}
    </Group>
  );
};

export default PhysicalTrainingDetail;
