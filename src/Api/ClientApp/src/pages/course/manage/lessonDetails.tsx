import { Center, Paper, Table } from "@mantine/core";
import { useGetLessonStatisticsDetails } from "@utils/services/manageCourseService";
import { useParams } from "react-router-dom";
import CourseLessonDetails from "./Components/CourseLessonDetails";

const LessonDetails = () => {
  const { id, lessonId } = useParams();
  const lessonDetails = useGetLessonStatisticsDetails(
    id as string,
    lessonId as string
  );

  return (
    <Paper>
      <Table striped withBorder>
        <thead>
          <tr>
            <th>Students</th>
            <th>
              <Center>Status</Center>
            </th>

            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {lessonDetails.data?.items.map((x) => (
            <CourseLessonDetails element={x} key={x.lessonId} />
          ))}
        </tbody>
      </Table>
    </Paper>
  );
};

export default LessonDetails;
