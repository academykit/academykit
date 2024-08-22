import { Button, Group, Text, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";

import { showNotification } from "@mantine/notifications";
import {
  useActivateLicense,
  useCheckoutLicense,
} from "@utils/services/licenseService";
import { useTranslation } from "react-i18next";

const LicenseForm = () => {
  const { t } = useTranslation();
  const { mutateAsync } = useCheckoutLicense();

  const form = useForm({
    initialValues: {
      licenseKey: "",
    },
  });

  const activateLicense = useActivateLicense();

  const onSubmit = async ({ licenseKey }: { licenseKey: string }) => {
    try {
      await activateLicense.mutateAsync({ licenseKey: licenseKey });

      showNotification({
        message: t("license_activation_success"),
      });
    } catch (err) {
      showNotification({
        message: t("license_activation_fail"),
        color: "red",
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

  return (
    <Group justify="center">
      <form onSubmit={form.onSubmit(onSubmit)}>
        <Group justify="center">
          <Text style={{ fontWeight: "bold" }} size="lg">
            {t("license_key")}
          </Text>
        </Group>
        <TextInput
          my={15}
          style={{ minWidth: "350px" }}
          autoComplete="off"
          withAsterisk
          placeholder={t("license_key") as string}
          {...form.getInputProps("licenseKey")}
        />
        <Group justify="center">
          <Button loading={activateLicense.isPending} type="submit">
            {t("submit")}
          </Button>
          <Button variant="outline" type="button" onClick={handleCheckout}>
            {t("buy_key")}
          </Button>
        </Group>
      </form>
    </Group>
  );
};

export default LicenseForm;
