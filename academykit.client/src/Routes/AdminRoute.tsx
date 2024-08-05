import useAuth from "@hooks/useAuth";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { Navigate, Outlet, useLocation } from "react-router-dom";

const AdminAuthRoute = () => {
  const auth = useAuth();
  const location = useLocation();
  return auth?.auth && auth?.auth?.role <= UserRole.Admin ? (
    <Outlet />
  ) : auth?.loggedIn ? (
    <Navigate to={RoutePath[401]} state={{ from: location }} replace />
  ) : (
    <Navigate to={RoutePath.login} state={{ from: location }} replace />
  );
};

export const SuperAdminRoute = () => {
  const auth = useAuth();
  const location = useLocation();

  return auth?.auth && auth?.auth?.role === UserRole.SuperAdmin ? (
    <Outlet />
  ) : auth?.loggedIn ? (
    <Navigate to={RoutePath[403]} state={{ from: location }} replace />
  ) : (
    <Navigate to={RoutePath.login} state={{ from: location }} replace />
  );
};

export default AdminAuthRoute;
