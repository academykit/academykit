import {
  Box,
  Button,
  Container,
  Group,
  Paper,
  Switch,
  Text,
  TextInput,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import {
  IZoomSetting,
  useUpdateZoomSetting,
  useZoomSetting,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { useEffect, useState } from "react";
import { TFunction } from "i18next";
import { useTranslation } from "react-i18next";

const mapData = {
  apiKey: "zoom_api_key",
  apiSecret: "zoom_api_secret",
  sdkKey: "zoom_sdk_key",
  sdkSecret: "zoom_sdk_secret",
  webhookSecret: "zoom_webhook_secret",
  webhookVerification: "zoom_webhook_verification",
  isRecordingEnabled: "zoom_recording_enabled",
};

const Row = ({
  label,
  data,
  t,
}: {
  label: string;
  data: string;
  t: TFunction;
}) => {
  return (
    <Box mt={10} ml={10}>
      {data && (
        <>
          <Text fz="md" weight={"bold"}>
            {
              //@ts-ignore

              t(`${mapData[label]}`)
            }
          </Text>
          <Text fz="sm">{String(data)}</Text>
        </>
      )}
    </Box>
  );
};

const ReadonlyData = ({ t, form }: { t: TFunction; form: any }) => {
  return (
    <>
      {Object.keys(form?.values).map((key, index) => (
        <Row key={index} label={key} data={form?.values[key]} t={t} />
      ))}
    </>
  );
};

const ZoomSettings = () => {
  const zoom = useZoomSetting();
  const updateZoom = useUpdateZoomSetting(zoom.data?.data.id);
  const [isChecked, setIsChecked] = useState<boolean | undefined>();
  const [edit, setEdit] = useState(true);
  const { t } = useTranslation();

  const form = useForm({
    initialValues: {
      apiKey: zoom.data?.data?.apiKey || "",
      apiSecret: zoom.data?.data?.apiSecret || "",
      sdkKey: zoom.data?.data?.sdkKey || "",
      sdkSecret: zoom.data?.data?.sdkSecret || "",
      isRecordingEnabled: zoom.data?.data?.isRecordingEnabled || false,
      webhookSecret: zoom.data?.data?.webhookSecret || "",
      webhookVerification: zoom.data?.data?.webhookVerificationKey || "",
    },
  });

  useEffect(() => {
    form.setValues({
      apiKey: zoom.data?.data?.apiKey || "",
      apiSecret: zoom.data?.data?.apiSecret || "",
      sdkKey: zoom.data?.data?.sdkKey || "",
      sdkSecret: zoom.data?.data?.sdkSecret || "",
      isRecordingEnabled: zoom.data?.data?.isRecordingEnabled || false,
      webhookSecret: zoom.data?.data?.webhookSecret || "",
      webhookVerification: zoom.data?.data?.webhookVerificationKey || "",
    });
    setIsChecked(zoom.data?.data.isRecordingEnabled);
  }, [zoom.isSuccess]);
  return (
    <Paper p={"20"} withBorder>
      <form
        onSubmit={form.onSubmit(async (values) => {
          try {
            await updateZoom.mutateAsync(values);
            showNotification({
              message: t("change_zoom_settings_success"),
            });
            setEdit(!edit);
          } catch (error) {
            const err = errorType(error);

            showNotification({
              title: t("error"),
              color: "red",
              message: err,
            });
          }
        })}
      >
        {!edit && (
          <Container
            size={450}
            sx={{
              marginLeft: "0px",
            }}
          >
            <TextInput
              label={t("zoom_api_key")}
              name="apiKey"
              placeholder={t("enter_zoom_api_key") as string}
              mb={10}
              {...form.getInputProps("apiKey")}
            />
            <TextInput
              label={t("zoom_api_secret")}
              name="apiSecret"
              placeholder={t("enter_zoom_api_secret") as string}
              mb={10}
              {...form.getInputProps("apiSecret")}
            />
            <TextInput
              label={t("zoom_sdk_key")}
              name="sdkKey"
              placeholder={t("enter_zoom_sdk_key") as string}
              mb={10}
              {...form.getInputProps("sdkKey")}
            />
            <TextInput
              label={t("zoom_sdk_secret")}
              name="sdkSecret"
              placeholder={t("enter_zoom_sdk_secret") as string}
              mb={10}
              {...form.getInputProps("sdkSecret")}
            />
            <TextInput
              label={t("zoom_webhook_secret")}
              name="webhookSecret"
              placeholder={t("enter_zoom_webhook_secret") as string}
              mb={10}
              {...form.getInputProps("webhookSecret")}
            />
            <TextInput
              label={t("zoom_webhook_verification")}
              name="webhookVerification"
              placeholder={t("enter_zoom_webhook_verification") as string}
              mb={10}
              {...form.getInputProps("webhookVerification")}
            />
            <Switch
              sx={{ input: { cursor: "pointer" } }}
              checked={isChecked}
              label={t("zoom_recording_enabled")}
              labelPosition="left"
              onChange={(e) => {
                setIsChecked(e.currentTarget.checked);
                form.setFieldValue(
                  "isRecordingEnabled",
                  e.currentTarget.checked
                );
              }}
            />
          </Container>
        )}
        {edit && <ReadonlyData form={form} t={t} />}
        <Group mt={30} mb={15} ml={10}>
          {edit && <Button onClick={() => setEdit(!edit)}>{t("edit")}</Button>}
          {!edit && <Button type="submit">{t("save")}</Button>}
          {!edit && (
            <Button onClick={() => setEdit(true)} variant="outline">
              {t("cancel")}
            </Button>
          )}
        </Group>
      </form>
    </Paper>
  );
};

export default ZoomSettings;
