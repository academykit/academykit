import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  Box,
  Container,
  Group,
  Loader,
  Paper,
  ScrollArea,
  Table,
} from "@mantine/core";
import { useCourse } from "@utils/services/courseService";
import CourseRow from "./Component/CourseRow";

const AdminCourseList = ({
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  const { data, isSuccess, isLoading, isError, error } =
    useCourse(searchParams);

  return (
    <Container fluid>
      <Group my={10}>All the trainings are listed below:</Group>
      {searchComponent("Search Courses")}

      <ScrollArea>
        {data &&
          (data.totalCount > 0 ? (
            <Paper mt={10}>
              <Table striped highlightOnHover withBorder>
                <thead>
                  <tr>
                    <th>Trainings Name</th>
                    <th>Created Date</th>
                    <th>Author</th>

                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map((x) => (
                    <CourseRow course={x} key={x.id} search={searchParams} />
                  ))}
                </tbody>
              </Table>
            </Paper>
          ) : (
            <Box>No Trainings Found!</Box>
          ))}
      </ScrollArea>
      {isLoading && <Loader />}
      {isError && <Box>Something Went Wrong.</Box>}
      {data && pagination(data.totalPage)}
    </Container>
  );
};

export default withSearchPagination(AdminCourseList);
