import UserShortProfile from "@components/UserShortProfile";
import { LessonStatDetails } from "@utils/services/manageCourseService";
import StudentLessonDetails from "./StudentDetails";
import LessonStatusColor from "./StudentDetails/LessonStatusColor";

const CourseLessonDetails = ({
  element,
  courseId,
}: {
  element: LessonStatDetails;
  courseId: string;
}) => {
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
