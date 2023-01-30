import { Center, Paper, Table, Box, Loader } from "@mantine/core";
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

  if (studentDetails.data?.length === 0)
    return <Box>Trainee has not started any lesson.</Box>;

  if (studentDetails.isLoading) return <Loader />;

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
          {studentDetails.data?.map((x, i) => (
            <CourseStudentLessons
              element={x}
              key={x.lessonId + i}
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
