import { Button, Group, Title, Text } from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import { DATE_FORMAT } from '@utils/constants';
import errorType from '@utils/services/axiosError';
import { usePostAttendance } from '@utils/services/physicalTrainingService';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';

const PhysicalTrainingDetail = ({
  name,
  id,
  hasAttended,
  startDate,
  lessonSlug,
}: {
  name: string;
  id: string;
  hasAttended: boolean | null;
  startDate: string;
  lessonSlug: string;
}) => {
  const { id: slug } = useParams();
  const { t } = useTranslation();
  const attendance = usePostAttendance(slug as string, lessonSlug);
  const startTime = moment(startDate).format('HH:mm A');

  const updatedTime = moment(startTime, 'hh:mm A')
    .add(5, 'hours')
    .add(45, 'minutes')
    .format('hh:mm A');

  const handleAttendance = async () => {
    try {
      const response = await attendance.mutateAsync({ identity: id });
      showNotification({
        message: response.data.message,
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  return (
    <Group sx={{ flexDirection: 'column' }}>
      <Title>{name}</Title>
      <Text>
        {t('start_date')}: {moment(startDate).format(DATE_FORMAT)}
      </Text>
      <Text>
        {t('start_time')}: {updatedTime}
      </Text>
      {!hasAttended ? (
        <Button
          onClick={() => handleAttendance()}
          loading={attendance.isLoading || attendance.isSuccess}
        >
          {t('mark_as_attended')}
        </Button>
      ) : (
        <Text>{t('attended')}</Text>
      )}
    </Group>
  );
};

export default PhysicalTrainingDetail;
