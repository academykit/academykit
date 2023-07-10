import { Badge, Group } from "@mantine/core";
import { LessonType } from "@utils/enums";
import { IStudentInfoLesson } from "@utils/services/manageCourseService";
import { useTranslation } from "react-i18next";

const videoType = {
  true: "Watched",
  false: "not_watched",
};
const examType = {
  true: "passed",
  false: "failed",
};
const liveSessionType = {
  true: "attended",
  false: "not_attended",
};
const documentType = {
  true: "viewed",
  false: "not_viewed",
};
const feedBackType = {
  true: "submitted",
  false: "not-submitted",
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
    default:
      return examType;
  }
};
const LessonStatusColor = ({
  status: { isPassed, isCompleted, lessonType: type },
}: {
  status: IStudentInfoLesson;
}) => {
  const { t } = useTranslation();

  return (
    <>
      <Group position="center">
        {isPassed ? (
          <Badge color={"green"}>{t(getType(type).true)}</Badge>
        ) : (
          <Badge color={"red"}>{t(getType(type).false)}</Badge>
        )}
        {isCompleted ? (
          <Badge color={"green"}>{t("completed")}</Badge>
        ) : (
          <Badge color={"red"}>{t("not_completed")}</Badge>
        )}
      </Group>
    </>
  );
};

export default LessonStatusColor;
