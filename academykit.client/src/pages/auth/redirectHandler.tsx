import useAuth from "@hooks/useAuth";
import { Container, Loader } from "@mantine/core";
import { REFRESH_TOKEN_STORAGE, TOKEN_STORAGE } from "@utils/constants";
import { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const RedirectHandler = () => {
  // const [loading, setLoading] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();

  const auth = useAuth();

  const queryParams = new URLSearchParams(location.search);
  const userId = queryParams.get("userId");
  const token = queryParams.get("token");
  const refresh = queryParams.get("refresh");

  useEffect(() => {
    // setLoading(true);
    if (token && refresh && userId) {
      token && localStorage.setItem(TOKEN_STORAGE, token);
      refresh && localStorage.setItem(REFRESH_TOKEN_STORAGE, refresh);
      userId && localStorage.setItem("id", userId);
      auth?.setToken(token);
      auth?.setRefreshToken(refresh ?? null);
      auth?.setIsLoggedIn(true);
      navigate("/");
      // setLoading(false);
    }
  }, []);

  return (
    <Container size={470} my={40}>
      <Loader />
    </Container>
  );
};
export default RedirectHandler;
