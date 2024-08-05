import RoutePath from "@utils/routeConstants";
import { useEffect } from "react";
import { useLocation, Outlet, useNavigate } from "react-router-dom";
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
