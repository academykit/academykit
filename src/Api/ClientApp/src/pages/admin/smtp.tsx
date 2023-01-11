import { Button, Container, PasswordInput, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import {
  ISMTPSettingUpdate,
  useSMTPSetting,
  useUpdateSMTPSetting,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { useEffect } from "react";

const SMTP = () => {
  const smtp = useSMTPSetting();
  const updateSMTP = useUpdateSMTPSetting(smtp.data?.data.id);
  const form = useForm<ISMTPSettingUpdate>({
    initialValues: {
      senderEmail: "",
      senderName: "",
      userName: "",
      replyTo: "",
      password: "",
      mailPort: 0,
      mailServer: "",
    },
    validate: {
      senderEmail: (values: string) => !values && "Sender's Email is required!",
      senderName: (values: string) => !values && "Sender's Name is required!",
      userName: (values: string) => !values && "Username is required!",
      replyTo: (values: string) => !values && "Reply Email is required!",
      password: (values: string) => !values && "Password is required!",
      mailPort: (values: number) => !values && "Mail Port is required!",
      mailServer: (values: string) => !values && "Mail Server is required!",
    },
  });

  useEffect(() => {
    form.setValues({
      senderEmail: smtp.data?.data?.senderEmail || "",
      password: smtp.data?.data?.password || "",
      mailPort: smtp.data?.data?.mailPort || 0,
      mailServer: smtp.data?.data?.mailServer || "",
      senderName: smtp.data?.data?.senderName || "",
      userName: smtp.data?.data?.userName || "",
      replyTo: smtp.data?.data?.replyTo || "",
    });
  }, [smtp.isSuccess]);

  return (
    <form
      onSubmit={form.onSubmit(async (values) => {
        try {
          await updateSMTP.mutateAsync(values);
          showNotification({
            message: "Updated Successfully!",
          });
        } catch (error) {
          const err = errorType(error);
          showNotification({
            message: err,
            color: "red",
          });
        }
      })}
    >
      <Container
        size={450}
        sx={{
          marginLeft: "0px",
        }}
      >
        <TextInput
          mb={10}
          label="Sender Email"
          type={"email"}
          name="senderEmail"
          placeholder="Please enter your sender's email"
          {...form.getInputProps("senderEmail")}
        />
        <TextInput
          mb={10}
          label="Reply Email"
          name="replyEmail"
          type={"email"}
          placeholder="Please enter your reply email"
          {...form.getInputProps("replyTo")}
        />
        <TextInput
          mb={10}
          label="Sender Name"
          name="senderName"
          placeholder="Please enter your sender's name"
          {...form.getInputProps("senderName")}
        />
        <TextInput
          mb={10}
          label="Username"
          name="userName"
          placeholder="Please enter your username"
          {...form.getInputProps("userName")}
        />
        <PasswordInput
          mb={10}
          label="Password"
          name="password"
          placeholder="Please enter your password"
          {...form.getInputProps("password")}
        />
        <TextInput
          mb={10}
          label="Port"
          name="port"
          type={"number"}
          placeholder="Please enter your port"
          {...form.getInputProps("mailPort")}
        />
        <TextInput
          mb={10}
          label="Server"
          name="server"
          placeholder="Please enter your server url"
          {...form.getInputProps("mailServer")}
        />
        <Button type="submit">Save</Button>
      </Container>
    </form>
  );
};
export default SMTP;
