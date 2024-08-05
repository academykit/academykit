import NavOutlet from "@components/Layout/NavOutlet";
import {
  IconBooks,
  IconCertificate,
  IconChartInfographic,
  IconClipboard,
  IconGraph,
  IconTool,
  IconUsersGroup,
} from "@tabler/icons-react";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

function CourseEditNav() {
  const params = useParams();
  const { t } = useTranslation();

  const navLink = [
    {
      label: t("statistics"),
      to: RoutePath.manageCourse.manage(params.id).route,
      role: UserRole.Trainer,
      icon: <IconGraph size={14} />,
    },

    {
      label: t("settings"),
      to: RoutePath.manageCourse.dashboard(params.id).route,
      role: UserRole.Trainer,
      icon: <IconTool size={14} />,
    },
    {
      label: t("details"),
      to: RoutePath.manageCourse.edit(params.id).routes(),
      role: UserRole.Trainer,
      icon: <IconClipboard size={14} />,
    },

    {
      label: t("lessons"),
      to: RoutePath.manageCourse.lessons(params.id).route,
      role: UserRole.Trainer,
      icon: <IconBooks size={14} />,
      // isActive: (pathName: string) =>
      //   pathName.includes(
      //     RoutePath.manageCourse.lessons(params.id).route ||
      //       RoutePath.manageCourse.lessons(params.id).route +
      //         `${params.lessonId}/${'assignment' || 'feedback'}`
      //   ),
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
      icon: <IconCertificate size={14} />,
    },
    {
      label: t("lesson_stats"),
      to: RoutePath.manageCourse.lessonsStat(params.id).route,
      role: UserRole.Trainer,
      // additional active path for lesson stats
      isActive: (pathName: string) =>
        pathName.includes(
          RoutePath.manageCourse.lessonsStat(params.id).route ||
            RoutePath.manageCourse.lessonsStat(params.id).route +
              `${params.lessonId}`
        ),
      icon: <IconChartInfographic size={14} />,
    },
    {
      label: t("trainee"),
      to: RoutePath.manageCourse.student(params.id).route,
      role: UserRole.Trainer,
      icon: <IconUsersGroup size={14} />,
    },
  ];

  return <NavOutlet data={navLink} hideBreadCrumb={2} />;
}

export default CourseEditNav;
