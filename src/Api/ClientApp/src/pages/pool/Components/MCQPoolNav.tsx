import { useParams } from "react-router-dom";
import RoutePath from "@utils/routeConstants";
import NavOutlet from "@components/Layout/NavOutlet";
import { UserRole } from "@utils/enums";

function MCQPoolNav() {
  const params = useParams();

  const navLink = [
    {
      label: "Details",
      to: RoutePath.pool.details(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: "Teachers",
      to: RoutePath.pool.teachers(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: "Questions",
      to: RoutePath.pool.questions(params.id).route,
      role: UserRole.Trainer,
    },
  ];

  return <NavOutlet data={navLink} hideBreadCrumb={4} />;
}

export default MCQPoolNav;
