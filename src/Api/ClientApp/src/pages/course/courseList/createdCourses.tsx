import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import { Box, Button, Container, Flex, Loader } from "@mantine/core";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useCourse } from "@utils/services/courseService";
import { useEffect } from "react";
import { Link } from "react-router-dom";
import CourseList from "./component/List";
import { CourseStatus } from "@utils/enums";
import { useTranslation } from "react-i18next";

const ReviewedCourse = ({
  setInitialSearch,
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  useEffect(() => {
    setInitialSearch([
      {
        key: "Status",
        value: CourseStatus.Review.toString(),
      },
    ]);
  }, []);
  const auth = useAuth();
  const { data, isSuccess, isLoading } = useCourse(searchParams);
  const role = auth?.auth?.role ?? UserRole.Trainee;
  const { t } = useTranslation();
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
          {searchComponent(t("search_trainings") as string)}
          {/* {role != UserRole.Trainee && (
            <Link to={RoutePath.courses.create}>
              <Button my={10} variant="outline" ml={5}>
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
          <Box>{t("no_trainings_found")}</Box>
        ))}
      {isLoading && <Loader />}
      {data && pagination(data.totalPage)}
    </Container>
  );
};

export default withSearchPagination(ReviewedCourse);
