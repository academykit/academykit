import {
  TextInput,
  PasswordInput,
  Paper,
  Title,
  Container,
  Button,
  Center,
  Anchor,
  Box,
  Group,
  Transition,
} from "@mantine/core";
import {
  Link,
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { useEffect, useState } from "react";
import useAuth from "../../hooks/useAuth";
import {
  useLogin,
  useResetPassword,
  useResetPasswordToken,
} from "@utils/services/authService";
import Logo from "@components/Logo";
import RoutePath from "@utils/routeConstants";
import { useTimeout, useToggle } from "@mantine/hooks";
import { AxiosResponse } from "axios";
import errorType from "@utils/services/axiosError";

const ConfirmToken = () => {
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";
  const navigate = useNavigate();
  const [passwordReset, setPasswordReset] = useState("");

  const { mutateAsync } = useResetPasswordToken();
  const confirmReset = useResetPassword();

  const form = useForm({
    initialValues: {
      token: "",
    },
  });

  const passwordForm = useForm({
    initialValues: {
      confirmPassword: "",
      newPassword: "",
    },
  });

  const [toggle, setToggle] = useToggle();

  const login = useLogin();
  const auth = useAuth();
  const [params] = useSearchParams();

  const onFormSubmit = async (values: {
    token: string;
    email?: string | null;
  }) => {
    values = { ...values, email: params.get("email") };
    try {
      const response = await mutateAsync(values);
      setPasswordReset(response.data.data);

      setTimeout(() => setToggle(), 700);
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        title: "Error!",
        color: "red",
      });
    }
  };

  const onPasswordFormSubmit = async (values: {
    newPassword: string;
    confirmPassword: string;
  }) => {
    try {
      await confirmReset.mutateAsync({
        ...values,
        passwordChangeToken: passwordReset,
      });
      showNotification({
        message: "Password reset successfully!",
      });
      navigate(RoutePath.login);
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        color: "red",
      });
    }
    // setTimeout(() => , 700);
  };
  useEffect(() => {
    if (auth?.loggedIn) {
      navigate(RoutePath.login, { replace: true });
    }
  }, [auth?.auth]);

  return (
    <Container size={470} my={40} sx={{ position: "relative" }}>
      <Center m={"lg"}>
        <Link to={"/"}>
          <Logo />
        </Link>
      </Center>

      <Box sx={{ position: "absolute", width: "100%" }}>
        <Transition
          mounted={!toggle}
          transition="fade"
          duration={400}
          timingFunction="ease"
        >
          {(styles) => (
            <div style={styles}>
              <form onSubmit={form.onSubmit(onFormSubmit)}>
                <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                  <TextInput
                    {...form.getInputProps("token")}
                    label="Token"
                    placeholder="Add token sent on email"
                    required
                  />

                  <Group position="right" mt={10}>
                    <Link to={RoutePath.login}>
                      <Anchor
                        component="button"
                        align="end"
                        type="button"
                        color="dimmed"
                        size="xs"
                      >
                        Want to Login?
                      </Anchor>
                    </Link>
                  </Group>
                  <Button
                    loading={login.isLoading}
                    fullWidth
                    mt="xl"
                    type="submit"
                  >
                    Proceed
                  </Button>
                </Paper>
              </form>
            </div>
          )}
        </Transition>
      </Box>

      <Box sx={{ position: "absolute", width: "100%" }}>
        <Transition
          mounted={toggle}
          transition="pop"
          duration={400}
          timingFunction="ease"
        >
          {(styles) => (
            <div style={styles}>
              <form onSubmit={passwordForm.onSubmit(onPasswordFormSubmit)}>
                <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                  <PasswordInput
                    {...passwordForm.getInputProps("newPassword")}
                    label="Password"
                    placeholder="Add your new password"
                    required
                  />

                  <PasswordInput
                    mt={20}
                    {...passwordForm.getInputProps("confirmPassword")}
                    label="Confirm Password"
                    placeholder="Repeat Password"
                    required
                  />

                  <Group position="right" mt={10}>
                    <Link to={RoutePath.login}>
                      <Anchor
                        component="button"
                        align="end"
                        type="button"
                        color="dimmed"
                        size="xs"
                      >
                        Want to Login?
                      </Anchor>
                    </Link>
                  </Group>
                  <Button
                    loading={login.isLoading}
                    fullWidth
                    mt="xl"
                    type="submit"
                  >
                    Proceed
                  </Button>
                </Paper>
              </form>
            </div>
          )}
        </Transition>
      </Box>
    </Container>
  );
};
export default ConfirmToken;
