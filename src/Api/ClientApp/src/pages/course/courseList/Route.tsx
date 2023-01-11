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
import React, { Suspense } from "react";
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

  return (
    <Container fluid>
      <Box
        my={10}
        sx={{ justifyContent: "space-between", alignItems: "center" }}
      >
        <Group position="apart">
          <Title sx={{ flexGrow: 2 }}>Trainings</Title>
          {role != UserRole.Trainee && (
            <Link to={RoutePath.courses.create}>
              <Button my={10} ml={5}>
                Create New Training
              </Button>
            </Link>
          )}
        </Group>
        <Tabs
          my={20}
          value={location.pathname}
          onTabChange={(value) => navigate(`${value}`)}
        >
          <Tabs.List>
            <Tabs.Tab value="/trainings/list">All Trainings</Tabs.Tab>
            <Tabs.Tab value="/trainings/list/completed">
              Completed Trainings
            </Tabs.Tab>
            {role <= UserRole.Admin && (
              <Tabs.Tab value="/trainings/list/review">
                Review Trainings
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
