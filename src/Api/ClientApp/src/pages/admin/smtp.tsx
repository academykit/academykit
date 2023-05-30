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
import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation();

  return (
    <form
      onSubmit={form.onSubmit(async (values) => {
        try {
          await updateSMTP.mutateAsync(values);
          showNotification({
            message: t("update_success"),
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
          label={t("sender_email")}
          type={"email"}
          name="senderEmail"
          placeholder={t("sender_email_placeholder") as string}
          {...form.getInputProps("senderEmail")}
        />
        <TextInput
          mb={10}
          label={t("reply_email")}
          name="replyEmail"
          type={"email"}
          placeholder={t("reply_email_placeholder") as string}
          {...form.getInputProps("replyTo")}
        />
        <TextInput
          mb={10}
          label={t("sender_name")}
          name="senderName"
          placeholder={t("sender_name_placeholder") as string}
          {...form.getInputProps("senderName")}
        />
        <TextInput
          mb={10}
          label={t("user_name")}
          name="userName"
          placeholder={t("user_name_placeholder") as string}
          {...form.getInputProps("userName")}
        />
        <PasswordInput
          mb={10}
          label={t("password")}
          name="password"
          placeholder={t("password_placeholder") as string}
          {...form.getInputProps("password")}
        />
        <TextInput
          mb={10}
          label={t("port")}
          name="port"
          type={"number"}
          placeholder={t("port_placeholder") as string}
          {...form.getInputProps("mailPort")}
        />
        <TextInput
          mb={10}
          label={t("server")}
          name="server"
          placeholder={t("server_placeholder") as string}
          {...form.getInputProps("mailServer")}
        />
        <Button type="submit">{t("save")}</Button>
      </Container>
    </form>
  );
};
export default SMTP;
