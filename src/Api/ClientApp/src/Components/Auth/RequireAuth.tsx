import RoutePath from "@utils/routeConstants";
import { useEffect } from "react";
import { useLocation, Navigate, Outlet, useNavigate } from "react-router-dom";
import useAuth from "../../hooks/useAuth";

const RequireAuth = () => {
  const auth = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  useEffect(() => {
    if (!auth?.loggedIn) {
      navigate(RoutePath.login, { state: { from: location } });
    }
  }, [auth?.loggedIn]);

  return <>{auth?.loggedIn && <Outlet />}</>;
};

export default RequireAuth;
