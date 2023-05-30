import { useParams } from "react-router-dom";
import RoutePath from "@utils/routeConstants";
import NavOutlet from "@components/Layout/NavOutlet";
import { UserRole } from "@utils/enums";
import { useTranslation } from "react-i18next";

function CourseEditNav() {
  const params = useParams();
  const { t } = useTranslation();

  const navLink = [
    {
      label: t("statistics"),
      to: RoutePath.manageCourse.manage(params.id).route,
      role: UserRole.Trainer,
    },

    {
      label: t("settings"),
      to: RoutePath.manageCourse.dashboard(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: t("details"),
      to: RoutePath.manageCourse.edit(params.id).routes(),
      role: UserRole.Trainer,
    },

    {
      label: t("lessons"),
      to: RoutePath.manageCourse.lessons(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: t("trainers"),
      to: RoutePath.manageCourse.teachers(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: t("certificates"),
      to: RoutePath.manageCourse.certificate(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: t("lesson_stats"),
      to: RoutePath.manageCourse.lessonsStat(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: t("trainee"),
      to: RoutePath.manageCourse.student(params.id).route,
      role: UserRole.Trainer,
    },
  ];

  return <NavOutlet data={navLink} hideBreadCrumb={2} />;
}

export default CourseEditNav;
