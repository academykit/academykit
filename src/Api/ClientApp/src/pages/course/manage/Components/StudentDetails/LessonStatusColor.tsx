import { Badge, Group } from "@mantine/core";
import { LessonType } from "@utils/enums";

const videoType = {
  true: "watched",
  false: "not watched",
};
const examType = {
  true: "Passed",
  false: "Failed",
};
const liveSessionType = {
  true: "Attend",
  false: "Not Attend",
};
const documentType = {
  true: "Attend",
  false: "Not Attend",
};
const feedBackType = {
  true: "Done",
  false: "Not Done",
};
const getType = (type: LessonType) => {
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
  type,
  isPassed,
  isCompleted,
}: {
  type: LessonType;
  isPassed: boolean;
  isCompleted: boolean;
}) => (
  <>
    <Group position="center">
      {isPassed ? (
        <Badge color={"green"}>{getType(type).true}</Badge>
      ) : (
        <Badge color={"red"}>{getType(type).false}</Badge>
      )}
      {isCompleted ? (
        <Badge color={"green"}>Completed</Badge>
      ) : (
        <Badge color={"red"}>Not Completed</Badge>
      )}
    </Group>
  </>
);

export default LessonStatusColor;
