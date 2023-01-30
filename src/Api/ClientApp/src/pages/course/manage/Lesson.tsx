import ProgressBar from "@components/Ui/ProgressBar";
import {
  Table,
  ScrollArea,
  Badge,
  Paper,
  Center,
  Tooltip,
  Button,
  Anchor,
  Loader,
  Box,
} from "@mantine/core";
import { IconEye } from "@tabler/icons";
import { LessonType } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { ICourseLesson } from "@utils/services/courseService";
import {
  ILessonStats,
  useGetLessonStatistics,
} from "@utils/services/manageCourseService";
import { Link, useParams } from "react-router-dom";

function TableReviews() {
  const slug = useParams();
  const course_id = slug.id as string;

  const getLessonStatistics = useGetLessonStatistics(course_id);

  const Rows = ({ item }: { item: ILessonStats }) => {
    return (
      <tr key={item?.id}>
        <td>
          <Anchor
            component={Link}
            to={`${RoutePath.classes}/${course_id}/${item.slug}`}
          >
            {item.name}
          </Anchor>
        </td>
        <td>{LessonType[item.lessonType]}</td>
        <td>
          <ProgressBar
            total={item?.enrolledStudent}
            positive={item?.lessonWatched}
          />
        </td>
        <td>
          <Center>
            {item?.isMandatory ? (
              <Badge color="green" variant="outline">
                yes
              </Badge>
            ) : (
              <Badge color="red" variant="outline">
                no
              </Badge>
            )}
          </Center>
        </td>
        <td
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Tooltip label={`View Details for ${item.name} lesson`}>
            <Button component={Link} variant="subtle" to={`${item.slug}`}>
              <IconEye />
            </Button>
          </Tooltip>
        </td>
      </tr>
    );
  };

  if (getLessonStatistics.isLoading) return <Loader />;

  if (getLessonStatistics.data?.length === 0)
    return <Box>No Lessons found</Box>;

  return (
    <ScrollArea>
      <Paper>
        <Table
          sx={{ minWidth: 800 }}
          verticalSpacing="xs"
          striped
          highlightOnHover
        >
          <thead>
            <tr>
              <th>Lesson Name</th>
              <th>Lesson Type</th>
              <th>
                <Center>Progress</Center>
              </th>
              <th>
                <Center>Is mandatory</Center>
              </th>
              <th>
                <Center>Action</Center>
              </th>
            </tr>
          </thead>
          <tbody>
            {getLessonStatistics.data?.map((item: ILessonStats) => (
              <Rows item={item} key={item.id} />
            ))}
          </tbody>
        </Table>
      </Paper>
    </ScrollArea>
  );
}
export default TableReviews;
