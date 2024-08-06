import useAuth from "@hooks/useAuth";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { Navigate, Outlet, useLocation } from "react-router-dom";

const TeacherRouteGuard = () => {
  const auth = useAuth();
  const location = useLocation();
  return auth?.auth && auth?.auth?.role <= UserRole.Trainer ? (
    <Outlet />
  ) : auth?.loggedIn ? (
    <Navigate to={RoutePath[401]} state={{ from: location }} />
  ) : (
    <Navigate to={RoutePath.login} state={{ from: location }} />
  );
};

export default TeacherRouteGuard;
