import { Google, Microsoft } from "@components/Icons";
import Logo from "@components/Logo";
import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
import { BrandingContext } from "@context/BrandingThemeContext";
import {
  Anchor,
  Button,
  Center,
  Container,
  Group,
  Loader,
  Paper,
  PasswordInput,
  Text,
  Title,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { SignInType } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import {
  useCompanySetting,
  useGetSignInOptions,
} from "@utils/services/adminService";
import { useLogin } from "@utils/services/authService";
import { api } from "@utils/services/service-api";
import type { IUserProfile } from "@utils/services/types";
import type { AxiosError } from "axios";
import { useContext, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Link, useNavigate } from "react-router-dom";
import * as Yup from "yup";
import useAuth from "../../hooks/useAuth";

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    email: Yup.string()
      .trim()
      .email(t("invalid_email") as string)
      .required(t("email_required") as string),
  });
};

const LoginPage = () => {
  const login = useLogin();
  const { t } = useTranslation();
  const auth = useAuth();
  const { data: signInOptions } = useGetSignInOptions();
  const onFormSubmit = (values: { email: string; password: string }) => {
    login.mutate({ email: values.email, password: values.password });
  };
  const context = useContext(BrandingContext);
  const navigate = useNavigate();

  const signInMethods = {
    [SignInType.Google]: { component: Google, action: api.auth.googleSignIn },
    [SignInType.Microsoft]: {
      component: Microsoft,
      action: api.auth.microsoftSignIn,
    },
  };

  const form = useForm({
    initialValues: {
      email: "",
      password: "",
    },
    validate: yupResolver(schema()),
  });

  useEffect(() => {
    if (login.isError) {
      showNotification({
        message:
          ((login.error as AxiosError).response?.data as any)?.message ??
          t("something_wrong"),
        title: t("error"),
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
        message: t("login_success"),
        title: t("successful"),
      });
    }
  }, [login.isError, login.isSuccess]);
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
      if (!companySettings?.data?.data?.isSetupCompleted) {
        return navigate("/initial/setup", { replace: true });
      }
      const branding = JSON.parse(
        companySettings.data.data.customConfiguration ?? "{}"
      );
      localStorage.setItem(
        "app-info",
        JSON.stringify({
          name: companySettings.data.data?.name ?? "AcademyKit",
          logo: companySettings.data.data.imageUrl ?? "/favicon.png",
        })
      );
      localStorage.setItem("branding", branding.accent);
      localStorage.setItem("version", companySettings.data.data.appVersion);
      context?.toggleBrandingTheme(branding.accent ?? "#0E99AC"); // set the accent after fetching
      setHeader();
    }
  }, [companySettings.isSuccess]);

  const allowedSignInMethods =
    signInOptions?.data
      ?.filter((option) => option.isAllowed && option.signIn in signInMethods)
      .map(
        (option) => signInMethods[option.signIn as keyof typeof signInMethods]
      ) || [];

  return (
    <Container size={420} my={40}>
      {companySettings.isSuccess &&
      companySettings?.data?.data?.isSetupCompleted ? (
        <>
          <Logo
            height={50}
            width={140}
            url={companySettings?.data?.data?.imageUrl}
          />
          <Title
            ta="center"
            style={(theme) => ({
              fontFamily: theme.fontFamily,
              fontWeight: 900,
            })}
          >
            {t("welcome_back")}!
          </Title>
          <form onSubmit={form.onSubmit(onFormSubmit)}>
            <Paper withBorder shadow="md" p={30} mt={30} radius="md">
              <CustomTextFieldWithAutoFocus
                {...form.getInputProps("email")}
                autoComplete={"username"}
                label={t("email")}
                type={"email"}
                placeholder={t("your_email") as string}
                name="email"
              />
              <PasswordInput
                {...form.getInputProps("password")}
                label={t("password")}
                autoComplete={"password"}
                placeholder={t("your_password") as string}
                mt="md"
                name="password"
              />
              <Group justify="flex-end" mt={10}>
                <Link to={RoutePath.forgotPassword}>
                  <Anchor
                    component="button"
                    ta="end"
                    type="button"
                    c="dimmed"
                    size="xs"
                  >
                    {t("forgot_password")}?
                  </Anchor>
                </Link>
              </Group>
              <Button loading={login.isPending} fullWidth mt="xl" type="submit">
                {t("sign_in")}
              </Button>
            </Paper>
          </form>
          <Center my={18}>
            <Text size="sm">
              {t("create_new_agreement")}{" "}
              <Link to={"/"}>{t("terms_service")}</Link>,{" "}
              <Link to={"/"}>{t("privacy_policy")}</Link>,{" "}
              {t("and_our_default")}{" "}
              <Link to={"/"}>{t("notification_settings")}</Link>.
            </Text>
          </Center>
          {allowedSignInMethods.length > 0 && (
            <div
              style={{
                display: "flex",
                alignItems: "center",
                gap: 10,
                marginTop: 5,
              }}
            >
              <div
                style={{
                  flex: "1",
                  height: "1px",
                  width: "100%",
                  background: "black",
                }}
              />
              <Text size="sm" style={{ whiteSpace: "nowrap" }}>
                {t("or_sign_in_with")}
              </Text>
              <div
                style={{
                  flex: "1",
                  height: "1px",
                  width: "100%",
                  background: "black",
                }}
              />
            </div>
          )}
          <Center style={{ gap: 30, marginTop: 5 }}>
            {allowedSignInMethods.map(({ component: Icon, action }) => (
              <form key={action} action={action} method="get">
                <button
                  style={{
                    border: "none",
                    margin: 0,
                    padding: 0,
                    background: "transparent",
                    cursor: "pointer",
                  }}
                  type="submit"
                >
                  <Icon height={28} width={28} />
                </button>
              </form>
            ))}
          </Center>
        </>
      ) : (
        <div>
          <Loader />
        </div>
      )}
    </Container>
  );
};
export default LoginPage;
