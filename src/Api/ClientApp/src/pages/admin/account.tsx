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

const schema = Yup.object().shape({
  currentPassword: Yup.string()
    .required("Current Password is required.")
    .label("current_password"),
  newPassword: Yup.string()
    .min(8, "Password must be 8 characters long.")
    .matches(/[0-9]/, "Password requires a number.")
    .matches(/[a-z]/, "Password requires a lowercase letter.")
    .matches(/[A-Z]/, "Password requires an uppercase letter.")
    .matches(/[^\w]/, "Password requires a symbol.")
    .required("New Password is required."),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref("newPassword"), null], "Password must match.")
    .required("Confirm Password is required."),
});

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
    validate: yupResolver(schema),
  });

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
        message: "Password Changed successfully!",
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
        title: "Error",
        color: "red",
      });
    }
  };

  return (
    <>
      <Modal
        opened={opened}
        onClose={() => setOpened(false)}
        title="Change Your Email Address"
      >
        <form onSubmit={changeEmailForm.onSubmit(onChangeEmail)}>
          <TextInput
            withAsterisk
            mb={10}
            label="Current Email"
            name="oldEmail"
            placeholder="Enter your Email"
            {...changeEmailForm.getInputProps("oldEmail")}
          />
          <PasswordInput
            placeholder="Enter your password"
            withAsterisk
            mb={10}
            name="password"
            label="Your Password"
            {...changeEmailForm.getInputProps("password")}
          />
          <TextInput
            withAsterisk
            mb={10}
            name="newEmail"
            label="Your New Email"
            placeholder="Enter your new Email"
            {...changeEmailForm.getInputProps("newEmail")}
          />
          <TextInput
            withAsterisk
            mb={10}
            name="confirmEmail"
            label="Confirm New Email"
            placeholder="Confirm your new Email"
            {...changeEmailForm.getInputProps("confirmEmail")}
          />
          <Button type="submit" mt={10} loading={changeEmail.isLoading}>
            Change Email
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
              label="Current Password"
              {...form.getInputProps("currentPassword")}
            />
            <PasswordInput
              mb={10}
              name="newPassword"
              withAsterisk
              label="New Password"
              {...form.getInputProps("newPassword")}
            />
            <PasswordInput
              mb={10}
              withAsterisk
              name="confirmPassword"
              label="Confirm Password"
              {...form.getInputProps("confirmPassword")}
            />
            <Button type="submit">Save</Button>
          </Container>
        </form>
        {/* email section */}

        <Container fluid style={{ width: "100%" }}>
          <TextInput
            label="Your Email"
            disabled
            sx={{
              marginBottom: "15px",
            }}
            placeholder="your@email.com"
            value={auth?.auth?.email}
          />
          <Button onClick={() => setOpened(true)}>Change Email</Button>
        </Container>
      </SimpleGrid>
    </>
  );
};

export default Account;
