import {
  TextInput,
  PasswordInput,
  Paper,
  Title,
  Container,
  Button,
  Center,
  Anchor,
  Group,
  Image,
} from "@mantine/core";
import { Link } from "react-router-dom";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { useEffect } from "react";
import useAuth from "../../hooks/useAuth";
import { useLogin } from "@utils/services/authService";
import Logo from "@components/Logo";
import RoutePath from "@utils/routeConstants";
import { IUserProfile } from "@utils/services/types";
import { useGeneralSetting } from "@utils/services/adminService";

const LoginPage = () => {
  const form = useForm({
    initialValues: {
      email: "",
      password: "",
    },
  });

  const login = useLogin();
  const auth = useAuth();
  const onFormSubmit = (values: { email: string; password: string }) => {
    login.mutate({ email: values.email, password: values.password });
  };

  useEffect(() => {
    if (login.isError) {
      showNotification({
        // @ts-ignore
        message:
          login.error?.response?.data?.message ?? "Something went wrong!",
        title: "Login Failed",
        color: "red",
      });
      login.reset();
    }
    if (login.isSuccess) {
      auth?.setToken(login.data?.data?.token);
      auth?.setRefreshToken(login?.data?.data?.refreshToken ?? null);
      auth?.setIsLoggedIn(true);
      auth?.setAuth({
        imageUrl: login.data.data.imageUrl,
        mobileNumber: "",
        firstName: login.data.data.firstName,
        lastName: login.data.data.firstName,
        id: login.data.data.userId,
        email: login.data.data.email,
      } as IUserProfile);
      showNotification({
        message: "Successfully logged in.",
        title: "Login Success",
      });
    }
  }, [login.isError, login.isSuccess]);
  const settings = useGeneralSetting();

  return (
    <Container size={420} my={40}>
      <Center m={"lg"}>
        <Link to={"/"}>
          <Image
            height={50}
            width={50}
            src={settings?.data?.data?.logoUrl}
          ></Image>
        </Link>
      </Center>
      <Title
        align="center"
        sx={(theme) => ({
          fontFamily: `Greycliff CF, ${theme.fontFamily}`,
          fontWeight: 900,
        })}
      >
        Welcome back!
      </Title>
      <form onSubmit={form.onSubmit(onFormSubmit)}>
        <Paper withBorder shadow="md" p={30} mt={30} radius="md">
          <TextInput
            {...form.getInputProps("email")}
            autoComplete={"email"}
            label="Email"
            type={"email"}
            placeholder="Your email address"
            required
            name="email"
          />
          <PasswordInput
            {...form.getInputProps("password")}
            label="Password"
            autoComplete={"password"}
            placeholder="Your password"
            required
            mt="md"
            name="password"
          />
          <Group position="right" mt={10}>
            <Link to={RoutePath.forgotPassword}>
              <Anchor
                component="button"
                align="end"
                type="button"
                color="dimmed"
                size="xs"
              >
                Forgot password?
              </Anchor>
            </Link>
          </Group>
          <Button loading={login.isLoading} fullWidth mt="xl" type="submit">
            Sign in
          </Button>
        </Paper>
      </form>
    </Container>
  );
};
export default LoginPage;
