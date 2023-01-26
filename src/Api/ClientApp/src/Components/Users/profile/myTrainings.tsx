import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import { useReAuth } from "@utils/services/authService";
import { useCourse } from "@utils/services/courseService";
import {
  Title,
  Anchor,
  Badge,
  Tooltip,
  ActionIcon,
  Paper,
  Table,
  useMantineTheme,
  Box,
  Loader,
  Pagination,
} from "@mantine/core";
import { IconEdit } from "@tabler/icons";
import RoutePath from "@utils/routeConstants";
import { CourseLanguage, UserRole } from "@utils/enums";
import moment from "moment";
import { Link } from "react-router-dom";
import { useState } from "react";
const MyTrainings = () => {
  const auth = useReAuth();
  const theme = useMantineTheme();
  const [page, setPage] = useState(1);
  const authorCourse = useCourse(`Enrollmentstatus=1&size=12&page=${page}`);

  return (
    <>
      {auth.data && auth.data?.role !== UserRole.Trainee && (
        <div>
          <Title mt={20} size={30} mb={10}>
            My Trainings
          </Title>

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
                {authorCourse.data &&
                  authorCourse.data.totalCount > 0 &&
                  authorCourse.data.items.map((x) => (
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
                {authorCourse.isLoading && <Loader />}
              </tbody>
            </Table>
          </Paper>
          {
            authorCourse.data && authorCourse.data.totalPage > 1 && (
              <Pagination
                my={20}
                total={authorCourse.data.totalPage}
                page={page}
                onChange={setPage}
              />
            )
            // pagination(authorCourse.data.totalPage)
          }

          {authorCourse.data && authorCourse.data.totalCount === 0 && (
            <Box mt={5}>No Trainings Found</Box>
          )}
        </div>
      )}
    </>
  );
};

export default MyTrainings;
