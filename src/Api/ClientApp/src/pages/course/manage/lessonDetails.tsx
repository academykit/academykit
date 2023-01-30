import { Button, Center, Group, Paper, Table } from "@mantine/core";
import { LessonType } from "@utils/enums";
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
    <>
      {lessonDetails.data?.items[0].lessonType === LessonType.Feedback && (
        <Group position="right" my="md">
          <Button>Export</Button>
        </Group>
      )}
      <Paper>
        <Table striped withBorder>
          <thead>
            <tr>
              <th>Trainees</th>
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
    </>
  );
};

export default LessonDetails;
