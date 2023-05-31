import { useLocation, useParams } from "react-router-dom";
import RoutePath from "@utils/routeConstants";
import NavOutlet from "./NavOutlet";
import { UserRole } from "@utils/enums";
import { useTranslation } from "react-i18next";

function TeamsNav() {
  const params = useParams();
  const { t } = useTranslation();
  const navLink = [
    {
      label: t("details"),
      to: RoutePath.groups.details(params.id).route,
      role: UserRole.Trainee,
    },
    {
      label: t("members"),
      to: RoutePath.groups.members(params.id).route,
      role: UserRole.Trainee,
    },
    {
      label: t("trainings"),
      to: RoutePath.groups.courses(params.id).route,
      role: UserRole.Trainee,
    },
    {
      label: t("attachments"),
      to: RoutePath.groups.attachments(params.id).route,
      role: UserRole.Trainee,
    },
  ];
  return <NavOutlet data={navLink} />;
}

export default TeamsNav;
