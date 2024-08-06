import NavOutlet from "@components/Layout/NavOutlet";
import { IconClipboard, IconListNumbers } from "@tabler/icons-react";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import i18next from "i18next";
import { useParams } from "react-router-dom";

function MCQPoolNav() {
  const params = useParams();

  const navLink = [
    {
      label: i18next.t("details"),
      to: RoutePath.pool.details(params.id).route,
      role: UserRole.Trainer,
      icon: <IconClipboard size={14} />,
    },
    {
      label: i18next.t("trainers"),
      to: RoutePath.pool.teachers(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: i18next.t("questions"),
      to: RoutePath.pool.questions(params.id).route,
      role: UserRole.Trainer,
      icon: <IconListNumbers size={14} />,
    },
  ];

  return <NavOutlet data={navLink} hideBreadCrumb={4} />;
}

export default MCQPoolNav;
