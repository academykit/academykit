import { Badge, Group } from '@mantine/core';
import { LessonType } from '@utils/enums';
import { IStudentInfoLesson } from '@utils/services/manageCourseService';
import { useTranslation } from 'react-i18next';

const videoType = {
  true: 'Watched',
  false: 'not_watched',
  empty: 'not_watched',
};
const examType = {
  true: 'passed',
  false: 'failed',
  empty: 'not_attempted',
};
const liveSessionType = {
  true: 'attended',
  false: 'not_attended',
  empty: 'not_attended',
};
const documentType = {
  true: 'viewed',
  false: 'not_viewed',
  empty: 'not_viewed',
};
const feedBackType = {
  true: 'submitted',
  false: 'not_submitted',
  empty: 'not_submitted',
};
const physicalType = {
  true: 'attended',
  false: 'review',
  empty: 'not_attended',
};

export const getType = (type: LessonType) => {
  switch (type) {
    case LessonType.Exam:
      return examType;
    case LessonType.Assignment:
      return examType;
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
const LessonStatusColor = ({
  status: {
    isPassed,
    isAssignmentReviewed,
    isCompleted,
    lessonType: type,
    attendenceReviewed,
  },
}: {
  status: IStudentInfoLesson;
}) => {
  const { t } = useTranslation();

  return (
    <>
      <Group position="center">
        {type !== LessonType.Assignment &&
          type !== LessonType.Physical &&
          // Logic for other lesson types
          (isPassed === true ? (
            <Badge color={'green'}>{t(getType(type).true)}</Badge>
          ) : isPassed === false ? (
            <Badge color={'red'}>{t(getType(type).false)}</Badge>
          ) : (
            <Badge color={'red'}>{t(getType(type).empty)}</Badge>
          ))}

        {/* show isPassed badge only when assignment is reviewed */}
        {type == LessonType.Assignment &&
          (isPassed
            ? isAssignmentReviewed && (
                <Badge color={'green'}>{t(getType(type).true)}</Badge>
              )
            : isAssignmentReviewed && (
                <Badge color={'red'}>{t(getType(type).false)}</Badge>
              ))}

        {/* Physical training review/attended/not-attended badges */}
        {type == LessonType.Physical &&
          (attendenceReviewed == true && isPassed == true ? (
            <Badge color={'green'}>{t(getType(type).true)}</Badge>
          ) : attendenceReviewed == true && isPassed == null ? (
            <Badge color={'red'}>{t(getType(type).empty)}</Badge>
          ) : attendenceReviewed == false && isPassed == null ? (
            <Badge color={'orange'}>{t(getType(type).false)}</Badge>
          ) : (
            <Badge color={'red'}>{t(getType(type).empty)}</Badge>
          ))}

        {/* show review status badge until it is not reviewed i.e. isAssignmentReviewed=true */}
        {/* null is not submitted
            true is reviewed
            false is in-review 
        */}
        {isAssignmentReviewed === null
          ? !isAssignmentReviewed &&
            type == LessonType.Assignment && (
              <Badge color="red">{t('not_submitted')}</Badge>
            )
          : isAssignmentReviewed
          ? !isAssignmentReviewed &&
            type == LessonType.Assignment && (
              <Badge color="green">{t('reviewed')}</Badge>
            )
          : !isAssignmentReviewed &&
            type == LessonType.Assignment && (
              <Badge color="orange">{t('in_review')}</Badge>
            )}

        {isCompleted ? (
          <Badge color={'green'}>{t('completed')}</Badge>
        ) : (
          <Badge color={'red'}>{t('not_completed')}</Badge>
        )}
      </Group>
    </>
  );
};

export default LessonStatusColor;
