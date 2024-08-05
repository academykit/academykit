import NotFound from "@pages/404";
import lazyWithRetry from "@utils/lazyImportWithReload";
import { Route, Routes } from "react-router-dom";

const AssessmentList = lazyWithRetry(
  () => import("@pages/assessment/AssessmentList")
);

const AssessmentRoute = () => {
  return (
    <Routes>
      <Route path={""} element={<AssessmentList />} />
      <Route path={"/*"} element={<NotFound />} />
    </Routes>
  );
};

export default AssessmentRoute;
