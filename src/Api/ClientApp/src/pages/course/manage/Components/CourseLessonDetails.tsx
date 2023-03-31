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
        <LessonStatusColor status={element} />
      </td>

      <td>
        <StudentLessonDetails
          studentInfo={element}
          courseId={courseId as string}
          studentId={element.user.id}
        />
      </td>
    </tr>
  );
};

export default CourseLessonDetails;
