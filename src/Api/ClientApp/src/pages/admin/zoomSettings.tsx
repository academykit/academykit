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
import React, { useEffect, useState } from "react";

const mapData = {
  apiKey: "API Key",
  apiSecret: "API Secret",
  sdkKey: "SDK Key",
  sdkSecret: "SDK Secret",
  webhookSecret: "Webhook Secret",
  webhookVerification: "Webhook Verification Key",
  isRecordingEnabled: "Recording Enabled?",
};

const Row = ({ label, data }: { label: string; data: string }) => {
  return (
    <Box mt={10} ml={10}>
      {data && (
        <>
          <Text fz="md" weight={"bold"}>
            {
              //@ts-ignore

              mapData[label]
            }
          </Text>
          <Text fz="sm">{String(data)}</Text>
        </>
      )}
    </Box>
  );
};

const ReadonlyData = ({
  data,
  form,
}: {
  data: IZoomSetting | undefined;
  form: any;
}) => {
  return (
    <>
      {Object.keys(form?.values).map((key, index) => (
        <Row key={index} label={key} data={form?.values[key]} />
      ))}
    </>
  );
};

const ZoomSettings = () => {
  const zoom = useZoomSetting();
  const updateZoom = useUpdateZoomSetting(zoom.data?.data.id);
  const [isChecked, setIsChecked] = useState<boolean | undefined>();
  const [edit, setEdit] = useState(true);

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
              message: "Successfully Changed Zoom settings!",
            });
            setEdit(!edit);
          } catch (error) {
            const err = errorType(error);

            showNotification({
              title: "Error!",
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
              label="API Key"
              name="apiKey"
              placeholder="Please enter your API Key"
              mb={10}
              {...form.getInputProps("apiKey")}
            />
            <TextInput
              label="API Secret"
              name="apiSecret"
              placeholder="Please enter your client secret"
              mb={10}
              {...form.getInputProps("apiSecret")}
            />
            <TextInput
              label="SDK Key"
              name="sdkKey"
              placeholder="Please enter your SDK Key"
              mb={10}
              {...form.getInputProps("sdkKey")}
            />
            <TextInput
              label="SDK Secret"
              name="sdkSecret"
              placeholder="Please enter your SDK Secret"
              mb={10}
              {...form.getInputProps("sdkSecret")}
            />
            <TextInput
              label="Webhook Secret"
              name="webhookSecret"
              placeholder="Please enter your Webhook Secret"
              mb={10}
              {...form.getInputProps("webhookSecret")}
            />
            <TextInput
              label="Webhook Verification Key"
              name="webhookVerification"
              placeholder="Please enter your Webhook verification key"
              mb={10}
              {...form.getInputProps("webhookVerification")}
            />
            <Switch
              sx={{ input: { cursor: "pointer" } }}
              checked={isChecked}
              label="Recording Enabled"
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
        {edit && <ReadonlyData data={zoom.data?.data} form={form} />}
        <Group mt={30} mb={15} ml={10}>
          {edit && <Button onClick={() => setEdit(!edit)}>Edit</Button>}
          {!edit && <Button type="submit">Save</Button>}
        </Group>
      </form>
    </Paper>
  );
};

export default ZoomSettings;
