import UserShortProfile from '@components/UserShortProfile';
import { Table } from '@mantine/core';
import { LessonStatDetails } from '@utils/services/manageCourseService';
import StudentLessonDetails from './StudentDetails';
import LessonStatusColor from './StudentDetails/LessonStatusColor';

const CourseLessonDetails = ({
  element,
  courseId,
}: {
  element: LessonStatDetails;
  courseId: string;
}) => {
  return (
    <Table.Tr key={element.lessonId}>
      <Table.Td>
        <UserShortProfile user={element.user} size="sm" />
      </Table.Td>
      <Table.Td>
        <LessonStatusColor status={element} />
      </Table.Td>

      <Table.Td>
        <StudentLessonDetails
          studentInfo={element}
          courseId={courseId as string}
          studentId={element.user.id}
        />
      </Table.Td>
    </Table.Tr>
  );
};

export default CourseLessonDetails;
