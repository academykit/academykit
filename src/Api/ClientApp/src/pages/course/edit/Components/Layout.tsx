import { useParams } from "react-router-dom";
import RoutePath from "@utils/routeConstants";
import NavOutlet from "@components/Layout/NavOutlet";
import { UserRole } from "@utils/enums";

function CourseEditNav() {
  const params = useParams();

  const navLink = [
    {
      label: "Statistics",
      to: RoutePath.manageCourse.manage(params.id).route,
      role: UserRole.Trainer,
    },

    {
      label: "Settings",
      to: RoutePath.manageCourse.dashboard(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: "Details",
      to: RoutePath.manageCourse.edit(params.id).routes(),
      role: UserRole.Trainer,
    },

    {
      label: "Lessons",
      to: RoutePath.manageCourse.lessons(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: "Teachers",
      to: RoutePath.manageCourse.teachers(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: "Certificates",
      to: RoutePath.manageCourse.certificate(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: "Lessons Stat",
      to: RoutePath.manageCourse.lessonsStat(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: "Students Stat",
      to: RoutePath.manageCourse.student(params.id).route,
      role: UserRole.Trainer,
    },
  ];

  return <NavOutlet data={navLink} hideBreadCrumb={2} />;
}

export default CourseEditNav;
