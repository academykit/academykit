import { Button, Group, Title, Text } from '@mantine/core';
import { DATE_FORMAT } from '@utils/constants';
import RoutePath from '@utils/routeConstants';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const PhysicalTrainingDetail = ({
  name,
  id,
  hasFeedbackSubmitted,
  startDate,
}: {
  name: string;
  id: string;
  hasFeedbackSubmitted: boolean;
  startDate: string;
}) => {
  const { t } = useTranslation();
  const startTime = moment(startDate).format('HH:mm A');

  const updatedTime = moment(startTime, 'hh:mm A')
    .add(5, 'hours')
    .add(45, 'minutes')
    .format('hh:mm A');

  return (
    <Group sx={{ flexDirection: 'column' }}>
      <Title>{name}</Title>
      <Text>
        {t('start_date')}: {moment(startDate).format(DATE_FORMAT)}
      </Text>
      <Text>
        {t('start_time')}: {updatedTime}
      </Text>
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
