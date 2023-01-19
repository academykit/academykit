import React from "react";
import LoginPage from "@pages/auth/loginPage";
import { Route, Routes } from "react-router-dom";
import RoutePath from "@utils/routeConstants";
import ConfirmToken from "@pages/auth/confirmToken";
import NotFound from "@pages/404";
import ServerError from "@pages/500";
import RequireAuth from "@components/Auth/RequireAuth";
import NotRequiredAuth from "@components/Auth/NotRequireAuth";
import { Suspense } from "react";
import UnAuthorize from "@pages/401";
import Forbidden from "@pages/403";
import { Loader } from "@mantine/core";
import Verify from "@pages/verify";
import ChangeEmail from "@components/Users/ChangeEmail";
import GroupAttachment from "@pages/groups/details/attachment";
import TeacherRouteGuard from "./TeacherRoute";
import ZoomMettingMessage from "@components/ZoomMeeting";
import { Navigate } from "react-router-dom";
import Layout from "@components/Layout/Layout";
import AdminAuthRoute from "./AdminRoute";
import lazyWithRetry from "@utils/lazyImportWithReload";
import MyTrainingExternal from "@pages/admin/Component/training/myTrainingExternal";
import CertificateList from "@pages/admin/Component/training/certificateList";
const MyFeedback = lazyWithRetry(
  () => import("@pages/course/feedback/myfeedback")
);
const ClassesRoute = lazyWithRetry(
  () => import("@pages/course/classes/classesRoute")
);

const AssignmentResult = lazyWithRetry(
  () => import("@pages/course/assignment/result")
);
const ForgotPassword = lazyWithRetry(
  () => import("@pages/auth/forgotPassword")
);
const FeedbackResult = lazyWithRetry(
  () => import("@pages/course/feedback/result")
);

const Dashboard = lazyWithRetry(() => import("@pages/user/dashboard"));

const CreateCoursePage = lazyWithRetry(() => import("@pages/course/create"));
const GroupsPage = lazyWithRetry(() => import("@pages/groups"));
const TeamsNav = lazyWithRetry(() => import("@components/Layout/TeamsNav"));
const GroupDetail = lazyWithRetry(() => import("@pages/groups/details"));
const GroupMember = lazyWithRetry(
  () => import("@pages/groups/details/members")
);
const UsersList = lazyWithRetry(() => import("@pages/admin/users"));
const MCQPool = lazyWithRetry(() => import("@pages/pool"));
const MCQPoolRoute = lazyWithRetry(() => import("@pages/pool/details/Route"));
const CourseListRoute = lazyWithRetry(
  () => import("@pages/course/courseList/Route")
);
const CourseRoute = lazyWithRetry(() => import("@pages/course/edit/Route"));
const UserInfo = lazyWithRetry(() => import("@components/Users/UserInfo"));
const UserProfile = lazyWithRetry(
  () => import("@components/Users/UserProfile")
);
const Classes = lazyWithRetry(() => import("@pages/course/classes/classes"));

const AdminRoute = lazyWithRetry(() => import("@pages/admin/AdminRoute"));

const GroupCourse = lazyWithRetry(() => import("@pages/groups/details/course"));
const LessonExam = lazyWithRetry(() => import("@pages/course/exam"));
const ExamResult = lazyWithRetry(() => import("@pages/course/exam/result"));

const MeetingRoute = lazyWithRetry(
  () => import("@components/Course/Meetings/Route")
);
const CourseDescriptionPage = lazyWithRetry(
  () => import("@pages/course/courseDescription")
);

const AssignmentPage = lazyWithRetry(() => import("@pages/course/assignment"));
const FeedbackPage = lazyWithRetry(() => import("@pages/course/feedback"));

const AppRoutes = () => {
  return (
    <Suspense
      fallback={
        <div>
          <Loader />
        </div>
      }
    >
      <Routes>
        <Route element={<NotRequiredAuth />}>
          <Route path={RoutePath.login} element={<LoginPage />} />
          <Route path={RoutePath.forgotPassword} element={<ForgotPassword />} />
          <Route path={RoutePath.confirmToken} element={<ConfirmToken />} />
        </Route>
        <Route path={RoutePath[404]} element={<NotFound />} />
        <Route path={RoutePath[500]} element={<ServerError />} />
        <Route path={RoutePath[401]} element={<UnAuthorize />} />
        <Route path={RoutePath[403]} element={<Forbidden />} />
        <Route element={<RequireAuth />}>
          <Route path={RoutePath.verify} element={<Verify />} />
          <Route path={RoutePath.verifyChangeEmail} element={<ChangeEmail />} />
          <Route element={<Layout />}>
            <Route path="*" element={<MainRoutes />} />
          </Route>
        </Route>
      </Routes>
    </Suspense>
  );
};

const MainRoutes = () => {
  return (
    <Suspense fallback={<Loader />}>
      <Routes>
        <Route path={RoutePath.userDashboard} element={<Dashboard />} />
        <Route
          path={RoutePath.courses.courseList + "*"}
          element={<CourseListRoute />}
        />

        {/* <Route
          path={"/user/certificate" + `/:id`}
          element={<MyTrainingExternal />}
        /> */}
        <Route
          path={RoutePath.courses.base}
          element={<Navigate to={RoutePath.courses.courseList} replace />}
        />

        <Route element={<TeacherRouteGuard />}>
          <Route path={RoutePath.pool.base} element={<MCQPool />} />
          <Route
            path={RoutePath.courses.create}
            element={<CreateCoursePage />}
          />
          <Route
            path={RoutePath.pool.base + "/:id/*"}
            element={<MCQPoolRoute />}
          />
          <Route
            path={RoutePath.manageCourse.description().signature + "/*"}
            element={<CourseRoute />}
          />
        </Route>

        <Route path={RoutePath.groups.base} element={<GroupsPage />} />
        <Route element={<AdminAuthRoute />}>
          <Route path={RoutePath.users} element={<UsersList />} />
        </Route>
        <Route path={RoutePath.userInfo + `/:id`} element={<UserInfo />} />
        <Route
          path={RoutePath.userProfile + `/:id`}
          element={<UserProfile />}
        />

        <Route
          path={"/meet/:courseId/:lessonId"}
          element={<ZoomMettingMessage />}
        />

        <Route
          path={RoutePath.courses.description().signature}
          element={<CourseDescriptionPage />}
        />

        <Route path="/settings/*" element={<AdminRoute />} />

        <Route
          path={RoutePath.classes + "/:id/:lessonId/*"}
          element={<Classes />}
        >
          <Route path="*" element={<ClassesRoute />} />
        </Route>

        <Route
          path={RoutePath.meeting.base + "/*"}
          element={<MeetingRoute />}
        />

        <Route element={<TeamsNav />}>
          <Route
            path={RoutePath.groups.details().signature}
            element={<GroupDetail />}
          />
          <Route
            path={RoutePath.groups.members().signature}
            element={<GroupMember />}
          />
          <Route
            path={RoutePath.groups.courses().signature}
            element={<GroupCourse />}
          />
          <Route
            path={RoutePath.groups.attachments().signature}
            element={<GroupAttachment />}
          />{" "}
        </Route>
        <Route
          path={RoutePath.exam.details().signature}
          element={<LessonExam />}
        />
        <Route
          path={RoutePath.exam.resultOne().signature}
          element={<ExamResult />}
        />
        <Route
          path={RoutePath.assignment.details().signature}
          element={<AssignmentPage />}
        />
        <Route
          path={RoutePath.assignment.result().signature}
          element={<AssignmentResult />}
        />
        <Route
          path={RoutePath.feedback.details().signature}
          element={<FeedbackPage />}
        />
        <Route
          path={RoutePath.feedback.myDetails().signature}
          element={<MyFeedback />}
        />
        <Route
          path={RoutePath.feedback.result().signature}
          element={<FeedbackResult />}
        />

        <Route path={"*"} element={<NotFound />} />
      </Routes>
    </Suspense>
  );
};

export default AppRoutes;
