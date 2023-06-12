import useAuth from "@hooks/useAuth";
import useNav from "@hooks/useNav";
import { Loader } from "@mantine/core";
import { CourseUserStatus, UserRole } from "@utils/enums";
import lazyWithRetry from "@utils/lazyImportWithReload";
import { useCourseDescription } from "@utils/services/courseService";
import { useEffect } from "react";
import { Routes, Route, useParams, useNavigate } from "react-router-dom";

const LessonDetails = lazyWithRetry(() => import("../manage/lessonDetails"));
const ManageCourse = lazyWithRetry(() => import("../manage/manage"));
const StudentDetails = lazyWithRetry(() => import("../manage/studentDetails"));
const Questions = lazyWithRetry(() => import("../question"));
const Certificate = lazyWithRetry(() => import("./certificate"));
const CourseEditNav = lazyWithRetry(() => import("./Components/Layout"));
const Dashboard = lazyWithRetry(() => import("./dashboard"));
const EditCourse = lazyWithRetry(() => import("./edit"));
const CourseLessons = lazyWithRetry(() => import("./lessons"));
const Teacher = lazyWithRetry(() => import("./teacher"));

const ManageLessons = lazyWithRetry(() => import("../manage/Lesson"));
const ManageStudents = lazyWithRetry(() => import("../manage/Student"));

const CourseRoute = () => {
  const params = useParams();
  const courseDetail = useCourseDescription(params.id as string);
  const auth = useAuth();
  const navigate = useNavigate();
  const nav = useNav();

  useEffect(() => {
    if (courseDetail.isError) {
      throw courseDetail.error;
    }
  }, [courseDetail.isError]);

  useEffect(() => {
    if (courseDetail.isSuccess) {
      nav.setBreadCrumb &&
        nav.setBreadCrumb([
          { href: "/trainings", title: "Tranning" },
          { href: "/trainings", title: "Tranning" },
          {
            href: `/trainings/stat/${courseDetail.data.slug}`,
            title: courseDetail?.data?.name ?? "",
          },
        ]);
      // }
    }
  }, [courseDetail.isSuccess, courseDetail.isRefetching]);

  if (courseDetail.isSuccess) {
    if (
      !(
        courseDetail.data.userStatus === CourseUserStatus.Author ||
        CourseUserStatus.Teacher ||
        (auth?.auth && auth.auth.role <= UserRole.Admin)
      )
    ) {
      return navigate("/403");
    }
  }

  return courseDetail.isLoading ? (
    <Loader />
  ) : courseDetail.isSuccess ? (
    <>
      <Routes>
        <Route element={<CourseEditNav />}>
          <Route path="/" element={<ManageCourse />} />
          <Route path="/students" element={<ManageStudents />} />
          <Route path="/students/:studentId" element={<StudentDetails />} />
          <Route path="/lessons-stat" element={<ManageLessons />} />
          <Route path="/lessons-stat/:lessonId" element={<LessonDetails />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/edit" element={<EditCourse />} />
          <Route path="/teachers" element={<Teacher />} />
          <Route path="/lessons" element={<CourseLessons />} />
          <Route path="/certificate" element={<Certificate />} />
        </Route>
        <Route path="/lessons/questions/:lessonSlug" element={<Questions />} />
      </Routes>
    </>
  ) : (
    <></>
  );
};

export default CourseRoute;
