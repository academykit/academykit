import { useEffect } from "react";
import { useLocation, Outlet, useNavigate } from "react-router-dom";
import useAuth from "../../hooks/useAuth";
import LanguageSelector from "@components/Ui/LanguageSelector";

const NotRequiredAuth = () => {
  const auth = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";

  useEffect(() => {
    if (auth?.loggedIn) {
      if (from === "/login") {
        navigate("/404", { replace: true });
      } else {
        navigate(from, { replace: true });
      }
    }
  }, [auth?.loggedIn]);

  return (
    <>
      {!auth?.loggedIn && (
        <>
          <Outlet />

          <div style={{ position: "fixed", bottom: "10px", right: "10px" }}>
            <LanguageSelector />
          </div>
        </>
      )}
    </>
  );
};

export default NotRequiredAuth;
