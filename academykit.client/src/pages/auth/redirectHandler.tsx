import useAuth from "@hooks/useAuth";
import { Container, Loader } from "@mantine/core";
import { REFRESH_TOKEN_STORAGE, TOKEN_STORAGE } from "@utils/constants";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const RedirectHandler = () => {
  const search = window.location.search;
  const navigate = useNavigate();
  const auth = useAuth();

  const params = new URLSearchParams(search);
  const userId = params.get("userId")?.replace(/ /g, "+");
  const token = params.get("token")?.replace(/ /g, "+");
  const refresh = params.get("refresh")?.replace(/ /g, "+");
  useEffect(() => {
    if (token && refresh && userId) {
      token && localStorage.setItem(TOKEN_STORAGE, token);
      refresh && localStorage.setItem(REFRESH_TOKEN_STORAGE, refresh);
      userId && localStorage.setItem("id", userId);
      auth?.setToken(token);
      auth?.setRefreshToken(refresh ?? null);
      auth?.setIsLoggedIn(true);
      navigate("/");
    }
  }, []);

  return (
    <Container size={470} my={40}>
      <Loader />
    </Container>
  );
};
export default RedirectHandler;
