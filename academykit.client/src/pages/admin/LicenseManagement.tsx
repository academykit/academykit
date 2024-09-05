import {
  Badge,
  Button,
  Container,
  Group,
  Stack,
  Text,
  TextInput,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconCalendar, IconClock, IconLicense } from "@tabler/icons-react";
// import { useLicenseKey, useUpdateLicenseKey } from "@utils/services/licenseService";
import errorType from "@utils/services/axiosError";
import {
  getLicenses,
  useCheckoutLicense,
  useUpdateLicense,
} from "@utils/services/licenseService";
import { t } from "i18next";
import { useEffect } from "react";
import * as Yup from "yup";

const LicenseManagement = () => {
  const { mutateAsync, isPending } = useCheckoutLicense();
  const { data: licenses } = getLicenses(true);
  const updateLicenseKey = useUpdateLicense();

  const schema = () => {
    return Yup.object().shape({
      licenseKey: Yup.string().required(t("level_name_required") as string),
    });
  };

  const form = useForm({
    initialValues: {
      licenseKey: "",
    },
    validate: yupResolver(schema()),
  });

  useEffect(() => {
    if (licenses?.length) {
      form.setValues({
        licenseKey: licenses[0].licenseKey,
      });
    }
  }, [licenses]);

  const handleSubmit = async (values: { licenseKey: string }) => {
    try {
      await updateLicenseKey.mutateAsync({
        licenseKey: values.licenseKey,
      });
      showNotification({
        message: t("license_updated_successfully"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        color: "red",
        message: error,
      });
    }
  };

  const handleCheckout = async () => {
    try {
      await mutateAsync();
    } catch (err) {
      showNotification({
        message: t("license_activation_fail"),
        color: "red",
      });
    }
  };

  const activeLicense = licenses?.length ? licenses[0] : null;

  return (
    <>
      <Text fw={700} size="xl">
        {t("license_management")}
      </Text>
      {activeLicense ? (
        <Stack gap="xs" p={"lg"} mt={30} mb="md">
          <Group gap="xs">
            <IconCalendar size={18} />
            <Text size="sm">
              {t("license_activated_on")}:{" "}
              <strong>{activeLicense.activatedOn.split("T")[0]}</strong>
            </Text>
          </Group>
          <Group gap="xs">
            <IconClock size={18} />
            <Text size="sm">
              {t("license_expires_in")}:{" "}
              <strong>{activeLicense.expiredOn.split("T")[0]}</strong>
            </Text>
          </Group>
          <Group gap="xs">
            <IconLicense size={18} />
            <Text size="sm">
              {t("current_plan")}:{" "}
              <Badge color="blue">{activeLicense.variantName}</Badge>
            </Text>
          </Group>
        </Stack>
      ) : (
        <Text c="red" mb="md">
          {t("no_active_license")}
        </Text>
      )}
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Container
          size={450}
          style={{
            marginLeft: "0px",
            marginTop: "10px",
          }}
        >
          <TextInput
            autoFocus
            mb={10}
            label={t("license_key")}
            placeholder={t("enter_new_license_key") as string}
            {...form.getInputProps("licenseKey")}
          />

          <Group>
            <Button type="submit">{t("upgrade")}</Button>
            <Button
              loading={isPending}
              variant="outline"
              type="button"
              onClick={handleCheckout}
            >
              {t("buy_key")}
            </Button>
          </Group>
        </Container>
      </form>
    </>
  );
};

export default LicenseManagement;
