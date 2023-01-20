import ProgressBar from "@components/Ui/ProgressBar";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  Anchor,
  Table,
  Title,
  useMantineTheme,
  Box,
  Paper,
  Badge,
  ActionIcon,
  Tooltip,
} from "@mantine/core";
import { IconEdit } from "@tabler/icons";
import { CourseLanguage } from "@utils/enums";
import { useCourse, useMyCourse } from "@utils/services/courseService";
import moment from "moment";
import { Link, useParams } from "react-router-dom";
import RoutePath from "@utils/routeConstants";

const UserCourse = (props: IWithSearchPagination) => {
  const { id } = useParams();
  const { data } = useMyCourse(id as string);
  const authorCourse = useCourse("Enrollmentstatus=1");

  const theme = useMantineTheme();

  return (
    <>
      <>
        <Title mt={10} size={30} mb={10}>
          Attended Trainings{" "}
        </Title>
        {data && data.totalCount > 0 && (
          <Paper>
            <Table>
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Enrolled Date</th>
                  <th>Progress</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((x) => (
                  <tr>
                    <td>
                      <Anchor
                        component={Link}
                        to={RoutePath.courses.description(x.slug).route}
                      >
                        {x.name}
                      </Anchor>
                    </td>
                    <td>{moment(x.createdOn).format(theme.dateFormat)}</td>

                    <td>
                      <ProgressBar total={100} positive={x.percentage} />
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </Paper>
        )}
        {data && data.totalCount === 0 && <Box mt={5}>No Trainings Found</Box>}
      </>
      <>
        <Title mt={20} size={30} mb={10}>
          My Trainings
        </Title>

        {authorCourse.data && authorCourse.data.totalCount > 0 && (
          <Paper>
            <Table striped>
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Created Date</th>
                  <th>Language / Level</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {authorCourse.data.items.map((x) => (
                  <tr>
                    <td>
                      <Anchor
                        component={Link}
                        to={RoutePath.courses.description(x.slug).route}
                      >
                        {x.name}
                      </Anchor>
                    </td>
                    <td>{moment(x.createdOn).format(theme.dateFormat)}</td>
                    <td>
                      <Badge color="pink" variant="light">
                        {CourseLanguage[x.language]}
                      </Badge>{" "}
                      /
                      <Badge color="blue" variant="light">
                        {x?.levelName}
                      </Badge>
                    </td>
                    <td>
                      <Tooltip label={"Edit this course"}>
                        <ActionIcon
                          component={Link}
                          to={RoutePath.manageCourse.edit(x.slug).route}
                        >
                          <IconEdit />
                        </ActionIcon>
                      </Tooltip>
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </Paper>
        )}
        {authorCourse.data && authorCourse.data.totalCount === 0 && (
          <Box mt={5}>No Trainings Found</Box>
        )}
      </>
    </>
  );
};

export default withSearchPagination(UserCourse);
