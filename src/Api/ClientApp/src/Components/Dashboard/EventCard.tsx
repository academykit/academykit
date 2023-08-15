import { Badge, Group, Indicator, Paper, Text } from '@mantine/core';
import { LessonType } from '@utils/enums';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const EventCard = ({
  lessonName,
  trainingName,
  lessonType,
  date,
}: {
  lessonName: string;
  trainingName: string;
  lessonType: number;
  date: string;
}) => {
  const { t } = useTranslation();

  // #C5F6FA
  // #E9FAC8
  // #D6EFED
  // #E3F4F4
  // #D2DAFF
  // #FBF4F9

  const videoType = {
    color: '#C5F6FA',
  };
  const examType = {
    color: '#E9FAC8',
  };
  const liveSessionType = {
    color: '#D6EFED',
  };
  const documentType = {
    color: '#E3F4F4',
  };
  const feedBackType = {
    color: '#D2DAFF',
  };
  const assignmentType = {
    color: '#E1D4BB',
  };
  const physicalType = {
    color: '#FBF4F9',
  };

  const getTypeColor = (type: number) => {
    switch (type) {
      case LessonType.Exam:
        return examType;
      case LessonType.Assignment:
        return assignmentType;
      case LessonType.Document:
        return documentType;
      case LessonType.RecordedVideo:
        return videoType;
      case LessonType.Video:
        return videoType;
      case LessonType.LiveClass:
        return liveSessionType;
      case LessonType.Feedback:
        return feedBackType;
      case LessonType.Physical:
        return physicalType;
      default:
        return examType;
    }
  };

  return (
    <>
      <Indicator
        ml={8}
        size={15}
        disabled={true}
        processing
        color={'green'}
        position="top-start"
      >
        <Paper
          mt={10}
          p={10}
          radius={'md'}
          component={Link}
          to={'/'}
          bg={getTypeColor(lessonType).color}
        >
          <Text size="lg" weight="bolder" lineClamp={2} color="black">
            {lessonName}
          </Text>
          <Text size="sm" lineClamp={2} color="black">
            {trainingName}
          </Text>
          <Group mt={'sm'}>
            <Badge color="blue" variant="outline">
              {t(`${LessonType[lessonType]}`)}
            </Badge>
            <Text size="sm" color="black">
              {moment(date).format('LL')}
            </Text>
          </Group>
        </Paper>
      </Indicator>
    </>
  );
};

export default EventCard;
