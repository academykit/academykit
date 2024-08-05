import { Loader } from "@mantine/core";
import lazyWithRetry from "@utils/lazyImportWithReload";
import { Suspense } from "react";
import { Navigate, Route, Routes } from "react-router-dom";

const CourseDescriptionSection = lazyWithRetry(
  () => import("@pages/course/classes/description")
);

const Comments = lazyWithRetry(() => import("@pages/course/classes/comments"));

const ClassesRoute = () => {
  return (
    <Suspense fallback={<Loader />}>
      <Routes>
        <Route index element={<CourseDescriptionSection />} />
        <Route path="comments" element={<Comments />} />
        <Route path="description" element={<CourseDescriptionSection />} />
        <Route path="*" element={<Navigate replace to={"/404"} />} />
      </Routes>
    </Suspense>
  );
};

export default ClassesRoute;
