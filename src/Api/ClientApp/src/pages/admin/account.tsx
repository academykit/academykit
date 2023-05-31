import useAuth from "@hooks/useAuth";
import {
  Button,
  Container,
  Modal,
  PasswordInput,
  SimpleGrid,
  TextInput,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { useChangeEmail, useChangePassword } from "@utils/services/authService";
import errorType from "@utils/services/axiosError";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import { useNavigate } from "react-router-dom";
import * as Yup from "yup";

export interface IPasswordResetRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface IChangeEmailRequest {
  oldEmail: string;
  newEmail: string;
  confirmEmail: string;
  password: string;
}

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    currentPassword: Yup.string()
      .required(t("current_password_required") as string)
      .label(t("current_password")),
    newPassword: Yup.string()
      .min(8, t("password_length_required") as string)
      .matches(/[0-9]/, t("password_number_required") as string)
      .matches(/[a-z]/, t("password_lowercase_required") as string)
      .matches(/[A-Z]/, t("password_uppercase_required") as string)
      .matches(/[^\w]/, t("password_symbol_required") as string)
      .required("New Password is required."),
    confirmPassword: Yup.string()
      .oneOf([Yup.ref(t("new_password")), null], t("password_match") as string)
      .required(t("password_confirm_required") as string),
  });
};

const changeEmailSchema = Yup.object().shape({
  oldEmail: Yup.string().email("Invalid Email.").required("Email is required."),
  newEmail: Yup.string().email("Invalid Email.").required("Email is required."),
  confirmEmail: Yup.string()
    .oneOf([Yup.ref("newEmail"), null], "Email must match.")
    .required("Confirm Email is required."),
  password: Yup.string().required("Password is required."),
});

const Account = () => {
  const [opened, setOpened] = useState(false);
  const { mutateAsync, isLoading } = useChangePassword();
  const changeEmail = useChangeEmail();
  const auth = useAuth();
  const navigate = useNavigate();

  const form = useForm({
    initialValues: {
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const changeEmailForm = useForm({
    initialValues: {
      oldEmail: "",
      newEmail: "",
      confirmEmail: "",
      password: "",
    },
    validate: yupResolver(changeEmailSchema),
  });

  const onSubmitForm = async (value: IPasswordResetRequest) => {
    try {
      await mutateAsync(value);
      form.reset();
      showNotification({
        message: t("change_password_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  const onChangeEmail = async (values: IChangeEmailRequest) => {
    try {
      const response = await changeEmail.mutateAsync(values);
      navigate("/verify?token=" + response?.data?.resendToken);
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        title: t("error"),
        color: "red",
      });
    }
  };
  const { t } = useTranslation();
  return (
    <>
      <Modal
        opened={opened}
        onClose={() => setOpened(false)}
        title={t("change_email_address")}
      >
        <form onSubmit={changeEmailForm.onSubmit(onChangeEmail)}>
          <TextInput
            withAsterisk
            mb={10}
            label={t("current_email")}
            name="oldEmail"
            placeholder={t("enter_email") as string}
            {...changeEmailForm.getInputProps("oldEmail")}
          />
          <PasswordInput
            placeholder={t("enter_password") as string}
            withAsterisk
            mb={10}
            name="password"
            label={t("your_password")}
            {...changeEmailForm.getInputProps("password")}
          />
          <TextInput
            withAsterisk
            mb={10}
            name="newEmail"
            label={t("new_email")}
            placeholder={t("your_new_email") as string}
            {...changeEmailForm.getInputProps("newEmail")}
          />
          <TextInput
            withAsterisk
            mb={10}
            name="confirmEmail"
            label={t("confirm_new_email")}
            placeholder={t("confirm_your_new_email") as string}
            {...changeEmailForm.getInputProps("confirmEmail")}
          />
          <Button type="submit" mt={10} loading={changeEmail.isLoading}>
            {t("change_email")}
          </Button>
        </form>
      </Modal>
      <SimpleGrid cols={2} breakpoints={[{ maxWidth: 600, cols: 1 }]}>
        <form onSubmit={form.onSubmit(onSubmitForm)} style={{ width: "100%" }}>
          <Container
            fluid
            sx={{
              marginLeft: "0px",
            }}
          >
            <PasswordInput
              mb={10}
              withAsterisk
              name="currentPassword"
              label={t("current_password")}
              placeholder={t("enter_current_password") as string}
              {...form.getInputProps("currentPassword")}
            />
            <PasswordInput
              mb={10}
              name="newPassword"
              withAsterisk
              label={t("new_password")}
              placeholder={t("enter_new_password") as string}
              {...form.getInputProps("newPassword")}
            />
            <PasswordInput
              mb={10}
              withAsterisk
              name="confirmPassword"
              placeholder={t("confirm_new_password") as string}
              label={t("confirm_password")}
              {...form.getInputProps("confirmPassword")}
            />
            <Button type="submit">{t("save")}</Button>
          </Container>
        </form>
        {/* email section */}

        <Container fluid style={{ width: "100%" }}>
          <TextInput
            label={t("your_email")}
            disabled
            sx={{
              marginBottom: "15px",
            }}
            placeholder={t("placeholder_email") as string}
            value={auth?.auth?.email}
          />
          <Button onClick={() => setOpened(true)}> {t("change_email")} </Button>
        </Container>
      </SimpleGrid>
    </>
  );
};

export default Account;
