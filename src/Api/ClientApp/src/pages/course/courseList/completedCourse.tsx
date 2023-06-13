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
import { useTranslation } from "react-i18next";

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
          {data &&
            data.items?.length > 1 &&
            searchComponent(t("search_trainings") as string)}
        </Flex>
      </Container>

      {data?.items &&
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

export default withSearchPagination(CompletedCourseList);
