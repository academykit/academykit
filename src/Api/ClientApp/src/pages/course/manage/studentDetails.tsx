import { Center, Paper, Table } from "@mantine/core";
import { useGetStudentStatisticsDetails } from "@utils/services/manageCourseService";
import { useParams } from "react-router-dom";
import CourseStudentLessons from "./Components/CourseStudentLessons";

const StudentDetails = () => {
  const { id, studentId } = useParams();
  const studentDetails = useGetStudentStatisticsDetails(
    id as string,
    studentId as string
  );

  if (studentDetails.isError) {
    throw studentDetails.error;
  }

  return (
    <Paper>
      <Table striped withBorder>
        <thead>
          <tr>
            <th>Lesson</th>
            <th>
              <Center>Lesson Status</Center>
            </th>
            <th>Lesson Type</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {studentDetails.data?.map((x) => (
            <CourseStudentLessons
              element={x}
              key={x.lessonId}
              studentId={studentId ?? ""}
              courseId={id as string}
            />
          ))}
        </tbody>
      </Table>
    </Paper>
  );
};

export default StudentDetails;
