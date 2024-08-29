import {
  Box,
  Button,
  Container,
  Group,
  Paper,
  PasswordInput,
  Stepper,
  TextInput,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import RoutePath from "@utils/routeConstants";
import { useCompanySetting } from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import * as yup from "yup";
import useAuth from "../../hooks/useAuth";

const OrganizationSetup = () => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [active, setActive] = useState(0);

  const validationSchema = yup.object({
    companyName: yup.string().required("Name is required"),
    ...(active === 1 && {
      name: yup.string().required("Name is required"),
      email: yup.string().email("Invalid email").required("Email is required"),
      password: yup
        .string()
        .min(6, "Password must be at least 6 characters")
        .required("Password is required"),
      confirmPassword: yup
        .string()
        .oneOf([yup.ref("password")], "Passwords must match")
        .required("Confirm password is required"),
    }), // Assuming it's only required in step 2
  });

  const form = useForm({
    initialValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
    validate: yupResolver(validationSchema),
  });

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

  const nextStep = () => {
    if (form.validate().hasErrors) {
      console.log("form errors", form.errors);
      return; // Prevent step change if there are validation errors
    }
    setActive(1);
  };

  const prevStep = () =>
    setActive((current) => (current > 0 ? current - 1 : current));

  return (
    <Container size={470} my={40} style={{ position: "relative" }}>
      <Box style={{ position: "absolute", width: "100%" }}>
        <form onSubmit={form.onSubmit(onFormSubmit)}>
          <Paper withBorder shadow="md" p={30} mt={30} radius="md">
            <Stepper active={active}>
              <Stepper.Step label="Step 1" description="Organization Detail">
                <>
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
                    {...form.getInputProps("address")}
                    label={t("address")}
                    placeholder={t("enter_address") as string}
                  />
                  <TextInput
                    mb={10}
                    {...form.getInputProps("logoUrl")}
                    label={t("logo_url")}
                    placeholder={t("enter_logo_url") as string}
                  />
                </>
              </Stepper.Step>
              <Stepper.Step label="Step 2" description="User Detail">
                <>
                  <>
                    <TextInput
                      mb={10}
                      withAsterisk
                      label={t("name")}
                      placeholder={t("name_placeholder") as string}
                      name="Name"
                      {...form.getInputProps("name")}
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
                </>
              </Stepper.Step>
            </Stepper>
            <Group justify="flex-end" mt="xl">
              <Button
                variant="default"
                onClick={active === 0 ? nextStep : prevStep}
              >
                {active === 0 ? "Next" : "Back"}
              </Button>
              {active === 1 && (
                <Button type="submit" onClick={nextStep}>
                  Submit
                </Button>
              )}
            </Group>
          </Paper>
        </form>
      </Box>
    </Container>
  );
};
export default OrganizationSetup;
