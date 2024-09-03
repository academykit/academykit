import lazyWithRetry from "@utils/lazyImportWithReload";
import { Route, Routes } from "react-router-dom";

const AssessmentNavLayout = lazyWithRetry(
  () => import("../component/AssessmentNavLayout")
);
const AssessmentQuestionList = lazyWithRetry(
  () => import("../AssessmentQuestionList")
);
const AssessmentSetting = lazyWithRetry(() => import("../AssessmentSetting"));
const EditAssessment = lazyWithRetry(() => import("../EditAssessment"));
const ManageAssessmentStudents = lazyWithRetry(
  () => import("@pages/assessment/ManageAssessmentStudents")
);

const AssessmentDetailRoutes = () => {
  return (
    <>
      <Routes>
        <Route element={<AssessmentNavLayout />}>
          <Route path="/edit" element={<EditAssessment />} />
          <Route path="/questions" element={<AssessmentQuestionList />} />
          <Route path="/statistics" element={<ManageAssessmentStudents />} />
          <Route
            path="/statistics/:studentId"
            element={<div>coming soon</div>}
          />
          <Route path="/setting" element={<AssessmentSetting />} />
        </Route>
      </Routes>
    </>
  );
};

export default AssessmentDetailRoutes;
