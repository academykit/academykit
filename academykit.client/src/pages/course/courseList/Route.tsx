import NotFound from "@pages/404";
import lazyWithRetry from "@utils/lazyImportWithReload";
import { Route, Routes } from "react-router-dom";
const CoursePage = lazyWithRetry(
  () => import("@pages/course/courseList/course")
);

const CourseListRoute = () => {
  return (
    <Routes>
      <Route path={""} element={<CoursePage />} />
      <Route path={"/*"} element={<NotFound />} />
    </Routes>
  );
};

export default CourseListRoute;
