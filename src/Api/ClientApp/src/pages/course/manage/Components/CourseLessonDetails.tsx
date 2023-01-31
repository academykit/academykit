import UserShortProfile from "@components/UserShortProfile";
import { UnstyledButton } from "@mantine/core";
import { LessonStatDetails } from "@utils/services/manageCourseService";
import { useParams } from "react-router-dom";
import StudentLessonDetails from "./StudentDetails";
import LessonStatusColor from "./StudentDetails/LessonStatusColor";

const CourseLessonDetails = ({ element }: { element: LessonStatDetails }) => {
  const { id: courseId } = useParams();
  return (
    <tr key={element.lessonId}>
      <td>
        <UserShortProfile user={element.user} size="sm" />
      </td>
      <td>
        <LessonStatusColor
          isCompleted={element.isCompleted}
          isPassed={element.isPassed}
          type={element.lessonType}
        />
      </td>

      <td>
        <StudentLessonDetails
          questionSetId={element.questionSetId}
          type={element.lessonType}
          studentId={element.user.id}
          lessonId={element.lessonId}
          isCompleted={element.isPassed}
          courseId={courseId as string}
          lessonName={element.lessonName}
        />
      </td>
    </tr>
  );
};

export default CourseLessonDetails;
