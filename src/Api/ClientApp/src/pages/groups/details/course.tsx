import CourseCard from "@components/Course/CourseCard";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import { Box, Button, Container, Flex, Group, Loader } from "@mantine/core";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useGroupCourse } from "@utils/services/groupService";
import { Link, useParams } from "react-router-dom";

const GroupCourse = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const { data, isLoading, error } = useGroupCourse(id as string, searchParams);
  const auth = useAuth();

  if (error) {
    throw error;
  }
  return (
    <Container fluid>
      {auth?.auth && auth.auth?.role < UserRole.Trainee && (
        <Group position="right" mb={10}>
          <Link to={RoutePath.courses.create + `?group=${id}`}>
            <Button>Create New Training</Button>
          </Link>
        </Group>
      )}

      {searchComponent("Search for group trainings")}
      <Flex wrap="wrap" mt={15}>
        {data?.items &&
          (data.totalCount > 0 ? (
            data.items.map((x) => (
              <div key={x.id}>
                <CourseCard course={x} />
              </div>
            ))
          ) : (
            <Box>No Trainings Found!</Box>
          ))}
      </Flex>
      {isLoading && <Loader />}
      {data && data.totalPage > 1 && pagination(data?.totalPage)}
    </Container>
  );
};

export default withSearchPagination(GroupCourse);
