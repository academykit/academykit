import { Button, Container, PasswordInput, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import {
  ISMTPSettingUpdate,
  useCreateUpdateSMTPSetting,
  useSMTPSetting,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";

const SMTP = () => {
  const smtp = useSMTPSetting();
  const createUpdateSMTP = useCreateUpdateSMTPSetting();
  const { t } = useTranslation();

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
      senderEmail: (values: string) => !values && "Sender's Email is required.",
      senderName: (values: string) => !values && "Sender's Name is required.",
      userName: (values: string) => !values && "Username is required.",
      replyTo: (values: string) => !values && "Reply Email is required.",
      password: (values: string) => !values && "Password is required.",
      mailPort: (values: number) => !values && "Mail Port is required.",
      mailServer: (values: string) => !values && "Mail Server is required.",
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
          await createUpdateSMTP.mutateAsync(values);
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
        style={{
          marginLeft: "0px",
        }}
      >
        <TextInput
          mb={10}
          withAsterisk
          label={t("sender_email")}
          type={"email"}
          name="senderEmail"
          placeholder={t("sender_email_placeholder") as string}
          {...form.getInputProps("senderEmail")}
        />
        <TextInput
          mb={10}
          label={t("reply_email")}
          withAsterisk
          name="replyEmail"
          type={"email"}
          placeholder={t("reply_email_placeholder") as string}
          {...form.getInputProps("replyTo")}
        />
        <TextInput
          mb={10}
          label={t("sender_name")}
          withAsterisk
          name="senderName"
          placeholder={t("sender_name_placeholder") as string}
          {...form.getInputProps("senderName")}
        />
        <TextInput
          mb={10}
          label={t("user_name")}
          withAsterisk
          name="userName"
          placeholder={t("user_name_placeholder") as string}
          {...form.getInputProps("userName")}
        />
        <PasswordInput
          mb={10}
          label={t("password")}
          withAsterisk
          name="password"
          placeholder={t("password_placeholder") as string}
          {...form.getInputProps("password")}
        />
        <TextInput
          mb={10}
          label={t("port")}
          withAsterisk
          name="port"
          type={"number"}
          placeholder={t("port_placeholder") as string}
          {...form.getInputProps("mailPort")}
        />
        <TextInput
          mb={10}
          label={t("server")}
          withAsterisk
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
