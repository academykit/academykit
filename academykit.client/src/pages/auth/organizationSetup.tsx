import {
  Box,
  Button,
  Container,
  Paper,
  PasswordInput,
  TextInput,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import RoutePath from "@utils/routeConstants";
import { useCompanySetting } from "@utils/services/adminService";
import { useLogin } from "@utils/services/authService";
import errorType from "@utils/services/axiosError";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import useAuth from "../../hooks/useAuth";

const OrganizationSetup = () => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [currentStep, setCurrentStep] = useState(1);

  const form = useForm({
    initialValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const login = useLogin();
  const auth = useAuth();

  const onFormSubmit = async () => {
    try {
      showNotification({
        message: "",
      });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        title: t("error"),
        color: "red",
      });
    }
  };

  useEffect(() => {
    if (auth?.loggedIn) {
      navigate(RoutePath.login, { replace: true });
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
          name: companySettings.data.data.name,
          logo: companySettings.data.data.imageUrl,
        })
      );
      setHeader();
    }
  }, [companySettings.isSuccess]);

  return (
    <Container size={470} my={40} style={{ position: "relative" }}>
      <Box style={{ position: "absolute", width: "100%" }}>
        <form onSubmit={form.onSubmit(onFormSubmit)}>
          <Paper withBorder shadow="md" p={30} mt={30} radius="md">
            {currentStep === 1 && (
              <>
                <TextInput
                  mb={10}
                  withAsterisk
                  label={t("firstname")}
                  placeholder={t("user_firstname") as string}
                  name="firstName"
                  {...form.getInputProps("firstName")}
                  required
                />
                <TextInput
                  mb={10}
                  label={t("lastName")}
                  placeholder={t("user_lastName") as string}
                  {...form.getInputProps("lastName")}
                  required
                />
                <TextInput
                  mb={10}
                  {...form.getInputProps("email")}
                  label={t("email")}
                  placeholder={t("your_email") as string}
                  required
                />
                <PasswordInput
                  mb={10}
                  {...form.getInputProps("newPassword")}
                  label={t("password")}
                  placeholder={t("enter_password") as string}
                  required
                />

                <PasswordInput
                  mt={10}
                  {...form.getInputProps("confirmPassword")}
                  label={t("confirm_password")}
                  placeholder={t("repeat_password") as string}
                  required
                />
              </>
            )}
            {currentStep === 2 && (
              <>
                <TextInput
                  mb={10}
                  withAsterisk
                  label={t("group_name")}
                  placeholder={t("your_group_name") as string}
                  name="firstName"
                  {...form.getInputProps("firstName")}
                  required
                />
              </>
            )}

            {currentStep > 1 ? (
              <Button loading={login.isPending} fullWidth mt="xl" type="submit">
                {t("submit")}
              </Button>
            ) : (
              <Button
                onClick={() => setCurrentStep(currentStep + 1)}
                fullWidth
                mt="xl"
                type="button"
              >
                {t("next")}
              </Button>
            )}
          </Paper>
        </form>
      </Box>
    </Container>
  );
};
export default OrganizationSetup;
