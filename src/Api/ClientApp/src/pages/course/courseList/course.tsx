import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import { Box, Button, Container, Flex, Loader } from "@mantine/core";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useCourse } from "@utils/services/courseService";
import { CourseUserStatus } from "@utils/enums";

import { Link } from "react-router-dom";
import CourseList from "./component/List";

const filterValue = [
  {
    value: CourseUserStatus.Author.toString(),
    label: "Author",
  },
  {
    value: CourseUserStatus.Enrolled.toString(),
    label: "Enrolled",
  },
  {
    value: CourseUserStatus.NotEnrolled.toString(),
    label: "Not Enrolled",
  },
  {
    value: CourseUserStatus.Teacher.toString(),
    label: "Teacher",
  },
];

const CoursePage = ({
  filterComponent,
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { data, isSuccess, isLoading } = useCourse(searchParams);
  const auth = useAuth();
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
          {filterComponent(
            filterValue,
            "Enrollment Status",
            "Enrollmentstatus"
          )}
          {/* {role != UserRole.Trainee && (
            <Link to={RoutePath.courses.create}>
              <Button my={10} ml={5}>
                Create New Training
              </Button>
            </Link>
          )} */}
        </Flex>
      </Container>

      {data &&
        data?.items &&
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

export default withSearchPagination(CoursePage);
