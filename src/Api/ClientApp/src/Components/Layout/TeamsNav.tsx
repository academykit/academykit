import { useLocation, useParams } from "react-router-dom";
import RoutePath from "@utils/routeConstants";
import NavOutlet from "./NavOutlet";
import { UserRole } from "@utils/enums";

function TeamsNav() {
  const params = useParams();

  const navLink = [
    {
      label: "Details",
      to: RoutePath.groups.details(params.id).route,
      role: UserRole.Trainee,
    },
    {
      label: "Members",
      to: RoutePath.groups.members(params.id).route,
      role: UserRole.Trainee,
    },
    {
      label: "Trainings",
      to: RoutePath.groups.courses(params.id).route,
      role: UserRole.Trainee,
    },
    {
      label: "Attachments",
      to: RoutePath.groups.attachments(params.id).route,
      role: UserRole.Trainee,
    },
  ];
  return <NavOutlet data={navLink} />;
}

export default TeamsNav;
