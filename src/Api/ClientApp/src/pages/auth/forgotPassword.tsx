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
  Image,
} from "@mantine/core";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { useEffect } from "react";
import useAuth from "../../hooks/useAuth";
import { useForgotPassword, useLogin } from "@utils/services/authService";
import Logo from "@components/Logo";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import { useCompanySetting } from "@utils/services/adminService";

const ForgotPassword = () => {
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";
  const navigate = useNavigate();
  const forgotPassword = useForgotPassword();
  const form = useForm({
    initialValues: {
      email: "",
    },
  });

  const login = useLogin();
  const auth = useAuth();
  const onFormSubmit = async (values: { email: string }) => {
    try {
      await forgotPassword.mutateAsync({ email: values.email });

      showNotification({
        message: "Please check your email!",
      });
      navigate(RoutePath.confirmToken + `?email=${values.email}`, {
        replace: true,
      });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        color: "red",
        message: err,
        title: "Error!",
      });
    }
  };
  useEffect(() => {
    if (auth?.loggedIn) {
      navigate(from, { replace: true });
    }
  }, [auth?.auth]);
  const companySettings = useCompanySetting();

  const setHeader = () => {
    const info =
      localStorage.getItem("app-info") &&
      JSON.parse(localStorage.getItem("app-info") ?? "");
    if (info) {
      let link = document.querySelector("link[rel~='icon']");
      document.title = info.name;
      if (!link) {
        link = document.createElement("link");
        // @ts-ignore
        link.rel = "icon";
        document.getElementsByTagName("head")[0].appendChild(info.logo);
      }
      // @ts-ignore
      link.href = info.logo;
    }
  };

  useEffect(() => {
    setHeader();

    if (companySettings.isSuccess) {
      localStorage.setItem(
        "app-info",
        JSON.stringify({
          name: companySettings.data.data.name,
          logo: companySettings.data.data.imageUrl,
        })
      );
      setHeader();
    }
  }, [companySettings.isSuccess]);

  return (
    <Container size={470} my={40}>
      <Center m={"lg"}>
        <Link to={"/"}>
          <Image
            height={50}
            width={50}
            src={companySettings?.data?.data?.imageUrl}
          ></Image>
        </Link>
      </Center>
      <Title
        align="center"
        sx={(theme) => ({
          fontFamily: `Greycliff CF, ${theme.fontFamily}`,
          fontWeight: 500,
        })}
      >
        Enter email to change password!
      </Title>
      <form onSubmit={form.onSubmit(onFormSubmit)}>
        <Paper withBorder shadow="md" p={30} mt={30} radius="md">
          <TextInput
            {...form.getInputProps("email")}
            label="Email"
            placeholder="Your email address"
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
            loading={forgotPassword.isLoading}
            fullWidth
            mt="xl"
            type="submit"
          >
            Proceed
          </Button>
        </Paper>
      </form>
    </Container>
  );
};
export default ForgotPassword;
