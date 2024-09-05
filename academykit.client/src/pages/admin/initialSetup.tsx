import Logo from "@components/Logo";
import {
  Box,
  Button,
  Container,
  Group,
  Loader,
  Paper,
  PasswordInput,
  Stepper,
  Text,
  TextInput,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { checkValidUrl } from "@utils/checkValidUrl";
import RoutePath from "@utils/routeConstants";
import {
  ISetupInitial,
  useCompanySetting,
  useInitialSetup,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import * as yup from "yup";
import useAuth from "../../hooks/useAuth";

const InitialSetup = () => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [active, setActive] = useState(0);

  const validationSchema = yup.object({
    companyName: yup.string().required(t("company_name_required")),
    ...(active === 1 && {
      firstName: yup.string().required(t("first_name_required")),
      lastName: yup.string().required(t("last_name_required")),
      email: yup
        .string()
        .email(t("invalid_email"))
        .required(t("email_required")),
      password: yup
        .string()
        .min(8, t("password_length_required"))
        .required(t("password_required")),
      confirmPassword: yup
        .string()
        .oneOf([yup.ref("password")], t("password_match"))
        .required(t("password_confirm_required")),
    }),
  });

  const form = useForm({
    initialValues: {
      companyName: "",
      companyAddress: "",
      logoUrl: "",
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
    validate: yupResolver(validationSchema),
  });

  const auth = useAuth();
  const initialSetup = useInitialSetup();

  const onFormSubmit = async (value: ISetupInitial) => {
    try {
      await initialSetup.mutateAsync(value);
      showNotification({
        message: "Initial Setup Successfully.",
      });
      window.location.reload();
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
      JSON.parse(localStorage.getItem("app-info") ?? "Academy kit");
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
      if (companySettings.data?.data?.isSetupCompleted) {
        return navigate("/login");
      }
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

  const nextStep = () => {
    if (form.validate().hasErrors) {
      return;
    }
    setActive(1);
  };

  const prevStep = () =>
    setActive((current) => (current > 0 ? current - 1 : current));

  return (
    <Container my={40} style={{ position: "relative" }}>
      {companySettings.isSuccess &&
      !companySettings?.data?.data?.isSetupCompleted ? (
        <>
          <Logo
            height={100}
            width={100}
            url={companySettings?.data?.data?.imageUrl}
          />
          <form onSubmit={form.onSubmit(onFormSubmit)}>
            <Paper p={30} radius="md">
              <Stepper active={active}>
                <Stepper.Step
                  label="Step 1"
                  description={t("organization_detail")}
                >
                  <Box size={100} p={20}>
                    <Text mb={10}>{t("logo_url_setup_later")}</Text>
                    <TextInput
                      mb={10}
                      withAsterisk
                      label={t("company_name")}
                      placeholder={t("enter_company_name") as string}
                      name="companyName"
                      {...form.getInputProps("companyName")}
                      required
                    />
                    <TextInput
                      mb={10}
                      {...form.getInputProps("companyAddress")}
                      label={t("company_address")}
                      placeholder={t("enter_company_address") as string}
                    />
                    <TextInput
                      mb={10}
                      {...form.getInputProps("logoUrl")}
                      label={t("logo_url")}
                      placeholder={t("enter_logo_url") as string}
                    />
                  </Box>
                </Stepper.Step>
                <Stepper.Step label="Step 2" description={t("user_detail")}>
                  <Box p={20}>
                    <Text mb={10}>{t("user_is_super_admin")}</Text>
                    <TextInput
                      mb={10}
                      withAsterisk
                      label={t("firstname")}
                      placeholder={t("name_placeholder") as string}
                      name="Name"
                      {...form.getInputProps("firstName")}
                      required
                    />
                    <TextInput
                      mb={10}
                      label={t("lastName")}
                      placeholder={t("name_placeholder") as string}
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
                      {...form.getInputProps("password")}
                      label={t("password")}
                      placeholder={t("password_placeholder") as string}
                      required
                    />

                    <PasswordInput
                      mt={10}
                      {...form.getInputProps("confirmPassword")}
                      label={t("confirm_password")}
                      placeholder={t("confirm_password") as string}
                      required
                    />
                  </Box>
                </Stepper.Step>
              </Stepper>
              <Group justify="flex-end" mt="xl">
                <Button
                  variant="default"
                  onClick={active === 0 ? nextStep : prevStep}
                >
                  {active === 0 ? t("next") : t("back")}
                </Button>
                {active === 1 && (
                  <Button type="submit" onClick={nextStep}>
                    {t("submit")}
                  </Button>
                )}
              </Group>
            </Paper>
          </form>
        </>
      ) : (
        <div>
          <Loader />
        </div>
      )}
    </Container>
  );
};
export default InitialSetup;
