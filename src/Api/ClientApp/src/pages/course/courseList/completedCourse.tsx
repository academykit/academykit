import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import { Box, Button, Container, Flex, Loader } from "@mantine/core";
import { CourseStatus, CourseUserStatus, UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useCourse } from "@utils/services/courseService";
import { useEffect } from "react";
import { Link } from "react-router-dom";
import CourseList from "./component/List";

const CompletedCourseList = ({
  setInitialSearch,
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  useEffect(() => {
    setInitialSearch([
      {
        key: "Status",
        value: CourseStatus.Completed.toString(),
      },
    ]);
  }, []);
  const auth = useAuth();
  const { data, isSuccess, isLoading } = useCourse(searchParams);
  const role = auth?.auth?.role ?? UserRole.Trainee;

  return (
    <Container fluid>
      <Container fluid>
        <Flex
          pb={20}
          sx={{
            justifyContent: "end",
            alignItems: "center",
          }}
        >
          {searchComponent("Search for trainings")}
          {/* {role != UserRole.Trainee && (
            <Link to={RoutePath.courses.create}>
              <Button my={10} variant="outline" ml={5}>
                Create New Training
              </Button>
            </Link>
          )} */}
        </Flex>
      </Container>

      {data?.items &&
        (data.totalCount >= 1 ? (
          <CourseList role={role} courses={data.items} search={searchParams} />
        ) : (
          <Box>No Trainings Found!</Box>
        ))}
      {isLoading && <Loader />}
      {data && pagination(data.totalPage)}
    </Container>
  );
};

export default withSearchPagination(CompletedCourseList);
