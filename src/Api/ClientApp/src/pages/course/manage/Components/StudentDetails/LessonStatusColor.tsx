import { Badge, Group } from "@mantine/core";
import { LessonType } from "@utils/enums";
import { IStudentInfoLesson } from "@utils/services/manageCourseService";
import { useTranslation } from "react-i18next";

const videoType = {
  true: "Watched",
  false: "Not Watched",
};
const examType = {
  true: "Passed",
  false: "Failed",
};
const liveSessionType = {
  true: "Attended",
  false: "Not Attended",
};
const documentType = {
  true: "Viewed",
  false: "Not Viewed",
};
const feedBackType = {
  true: "Submitted",
  false: "Not Submitted",
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
const { t } = useTranslation();
const LessonStatusColor = ({
  status: { isPassed, isCompleted, lessonType: type },
}: {
  status: IStudentInfoLesson;
}) => (
  <>
    <Group position="center">
      {isPassed ? (
        <Badge color={"green"}>{getType(type).true}</Badge>
      ) : (
        <Badge color={"red"}>{getType(type).false}</Badge>
      )}
      {isCompleted ? (
        <Badge color={"green"}>{t("completed")}</Badge>
      ) : (
        <Badge color={"red"}>{t("not_completed")}</Badge>
      )}
    </Group>
  </>
);

export default LessonStatusColor;
