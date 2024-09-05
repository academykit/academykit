import Logo from "@components/Logo";
import {
  Anchor,
  Button,
  Container,
  Group,
  Paper,
  TextInput,
  Title,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { checkValidUrl } from "@utils/checkValidUrl";
import RoutePath from "@utils/routeConstants";
import { useCompanySetting } from "@utils/services/adminService";
import { useForgotPassword } from "@utils/services/authService";
import errorType from "@utils/services/axiosError";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Link, useLocation, useNavigate } from "react-router-dom";
import * as Yup from "yup";
import useAuth from "../../hooks/useAuth";

const ForgotPassword = () => {
  const schema = () => {
    const { t } = useTranslation();
    return Yup.object().shape({
      email: Yup.string()
        .trim()
        .email(t("invalid_email") as string)
        .required(t("email_required") as string),
    });
  };
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";
  const navigate = useNavigate();
  const forgotPassword = useForgotPassword();
  const { t } = useTranslation();
  const form = useForm({
    validate: yupResolver(schema()),
    initialValues: {
      email: "",
    },
  });

  const auth = useAuth();
  const onFormSubmit = async (values: { email: string }) => {
    try {
      await forgotPassword.mutateAsync({ email: values.email });

      showNotification({
        message: t("check_email"),
      });
      navigate(RoutePath.confirmToken + `?email=${values.email}`, {
        replace: true,
      });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        color: "red",
        message: err,
        title: t("error"),
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
      let link = document.querySelector("link[rel~='icon']") as HTMLLinkElement;
      document.title = info.name;
      if (!link) {
        link = document.createElement("link");
        link.rel = "icon";
        document.getElementsByTagName("head")[0].appendChild(link);
      }
      link.href = info.logo;
    }
  };

  useEffect(() => {
    setHeader();

    if (companySettings.isSuccess) {
      localStorage.setItem(
        "app-info",
        JSON.stringify({
          name: companySettings.data.data?.name ?? "AcademyKit",
          logo: checkValidUrl(companySettings.data.data.imageUrl)
            ? companySettings.data.data.imageUrl
            : "/favicon.png",
        })
      );
      setHeader();
    }
  }, [companySettings.isSuccess]);

  return (
    <Container size={470} my={40}>
      <Logo
        height={50}
        width={50}
        url={companySettings?.data?.data?.imageUrl}
      />
      <Title
        ta="center"
        style={(theme) => ({
          fontFamily: theme.fontFamily,
          fontWeight: 500,
        })}
      >
        {t("change_email_title")}!
      </Title>
      <form onSubmit={form.onSubmit(onFormSubmit)}>
        <Paper withBorder shadow="md" p={30} mt={30} radius="md">
          <TextInput
            {...form.getInputProps("email")}
            label={t("email")}
            placeholder={t("your_email") as string}
          />

          <Group justify="flex-end" mt={10}>
            <Link to={RoutePath.login}>
              <Anchor
                component="button"
                ta="end"
                type="button"
                c="dimmed"
                size="xs"
              >
                {t("want_login")}?
              </Anchor>
            </Link>
          </Group>
          <Button
            loading={forgotPassword.isPending}
            fullWidth
            mt="xl"
            type="submit"
          >
            {t("proceed")}
          </Button>
        </Paper>
      </form>
    </Container>
  );
};
export default ForgotPassword;
