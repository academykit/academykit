import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Container,
  Group,
  Loader,
  Tabs,
  Title,
} from "@mantine/core";
import NotFound from "@pages/404";
import { UserRole } from "@utils/enums";
import lazyWithRetry from "@utils/lazyImportWithReload";
import RoutePath from "@utils/routeConstants";
import { Suspense } from "react";
import { useTranslation } from "react-i18next";
import {
  Link,
  Outlet,
  Route,
  Routes,
  useLocation,
  useNavigate,
} from "react-router-dom";
const CoursePage = lazyWithRetry(
  () => import("@pages/course/courseList/course")
);
const CreatedCoursePage = lazyWithRetry(
  () => import("@pages/course/courseList/createdCourses")
);
const CompletedCourseList = lazyWithRetry(
  () => import("@pages/course/courseList/completedCourse")
);

const CourseListRoute = () => {
  return (
    <Routes>
      <Route element={<CourseListPageNav />}>
        <Route path={""} element={<CoursePage />} />
        <Route path={"/completed"} element={<CompletedCourseList />} />
        <Route path={"/review"} element={<CreatedCoursePage />} />
        <Route path={"/*"} element={<NotFound />} />
      </Route>
    </Routes>
  );
};

export default CourseListRoute;

const CourseListPageNav = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const auth = useAuth();

  const role = auth?.auth?.role ?? UserRole.Trainee;
  const { t } = useTranslation();
  return (
    <Container fluid>
      <Box
        my={10}
        style={{ justifyContent: "space-between", alignItems: "center" }}
      >
        <Group justify="space-between">
          <Title style={{ flexGrow: 2 }}>{t("trainings")}</Title>
          {role != UserRole.Trainee && (
            <Link to={RoutePath.courses.create}>
              <Button my={10} ml={5}>
                {t("new_training")}
              </Button>
            </Link>
          )}
        </Group>
        <Tabs
          my={20}
          value={location.pathname}
          onChange={(value) => navigate(`${value}`)}
        >
          <Tabs.List>
            <Tabs.Tab value="/trainings/list">{t("all_trainings")}</Tabs.Tab>
            <Tabs.Tab value="/trainings/list/completed">
              {t("completed_trainings")}
            </Tabs.Tab>
            {Number(role) <= UserRole.Admin && (
              <Tabs.Tab value="/trainings/list/review">
                {t("review_trainings")}
              </Tabs.Tab>
            )}
          </Tabs.List>
        </Tabs>
      </Box>
      <Suspense fallback={<Loader />}>
        <Outlet />
      </Suspense>
    </Container>
  );
};
