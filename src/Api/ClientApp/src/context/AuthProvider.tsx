import { AppShell, Button, Center, Group, Loader, Modal } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { useQueryClient } from "@tanstack/react-query";
import { REFRESH_TOKEN_STORAGE, TOKEN_STORAGE } from "@utils/constants";
import { useLogout, useReAuth } from "@utils/services/authService";
import { IUserProfile } from "@utils/services/types";
import React, { createContext, useState, FC } from "react";
import { useEffect } from "react";

export interface IAuthContext {
  auth: IUserProfile | null;
  token: string | null;
  refreshToken: string | null;
  loggedIn: boolean;
  setAuth: React.Dispatch<React.SetStateAction<IUserProfile | null>>;
  logout: () => void;
  setToken: React.Dispatch<React.SetStateAction<string | null>>;
  setIsLoggedIn: React.Dispatch<React.SetStateAction<boolean>>;
  setRefreshToken: React.Dispatch<React.SetStateAction<string | null>>;
}

const AuthContext = createContext<IAuthContext | null>(null);

export const AuthProvider: FC<React.PropsWithChildren> = ({ children }) => {
  const reAuth = useReAuth();
  const useLogoutQuery = useLogout();
  const [loggedIn, setIsLoggedIn] = useState(false);
  const [token, setToken] = useState<string | null>(
    localStorage.getItem(TOKEN_STORAGE)
  );
  const [refreshToken, setRefreshToken] = useState<string | null>(
    localStorage.getItem(REFRESH_TOKEN_STORAGE) as string
  );

  const [auth, setAuth] = useState<IUserProfile | null>(null);
  const [ready, setReady] = useState(false);

  useEffect(() => {
    if (reAuth.isSuccess) {
      setAuth(reAuth.data);
      setIsLoggedIn(true);
      setReady(() => true);
    }
    if (reAuth.isError) {
      localStorage.removeItem(REFRESH_TOKEN_STORAGE);
      localStorage.removeItem(TOKEN_STORAGE);
      localStorage.removeItem("id");

      setIsLoggedIn(false);
      setToken(null);
      setRefreshToken(null);
      setAuth(null);
      setReady(() => true);
    }
  }, [reAuth.isSuccess, reAuth.isError, reAuth.isFetching]);

  useEffect(() => {
    if (useLogoutQuery.isSuccess) {
      window.location.reload();
    }
  }, [useLogoutQuery.isSuccess]);

  const [showLogout, setShowLogout] = useToggle();
  const confirmLogout = async () => {
    try {
      await useLogoutQuery.mutateAsync();
      setShowLogout();
    } catch (err) {
      setShowLogout();
      showNotification({
        title: "Error!",
        message: "Unable to logout at this time",
        color: "red",
      });
    }
  };
  const logout = () => {
    setShowLogout();
  };

  return (
    <AuthContext.Provider
      value={{
        auth,
        setAuth,
        logout,
        token,
        refreshToken,
        loggedIn,
        setToken,
        setIsLoggedIn,
        setRefreshToken,
      }}
    >
      <Modal
        onClose={() => setShowLogout()}
        opened={showLogout}
        title="Are you sure you want to Logout?"
      >
        <Group position="center">
          <Button onClick={confirmLogout}>Sure</Button>
          <Button variant="outline" onClick={() => setShowLogout()}>
            Cancel
          </Button>
        </Group>
      </Modal>
      {!ready && localStorage.getItem("token") ? (
        <AppShell>
          <Center>
            <Loader />
          </Center>
        </AppShell>
      ) : (
        children
      )}
    </AuthContext.Provider>
  );
};

export default AuthContext;