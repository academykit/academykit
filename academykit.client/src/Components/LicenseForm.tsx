import useLicenseValidation from "@hooks/useLicenseValidation";
import { Button, Group, Text, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";

import { showNotification } from "@mantine/notifications";
import { useActivatelicense } from "@utils/services/licenseService";
import { useTranslation } from "react-i18next";

const LicenseForm = () => {
  const { t } = useTranslation();

  const form = useForm({
    initialValues: {
      license_key: "",
    },
  });

  const activateLicense = useActivatelicense();
  const license = useLicenseValidation();

  const onSubmit = async ({ license_key }: { license_key: string }) => {
    try {
      await activateLicense.mutateAsync({ licenseKey: license_key });
      license?.setValid(true);
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
          {...form.getInputProps("license_key")}
        />
        <Group justify="center">
          <Button loading={activateLicense.isPending} type="submit">
            {t("submit")}
          </Button>
          <Button
            variant="outline"
            onClick={() => {
              window.open(
                "https://academykit.lemonsqueezy.com/buy/f83cb9f6-13d5-42f2-9a00-4ac5324e5cf6",
                "_blank"
              );
            }}
          >
            {t("Buy Key")}
          </Button>
        </Group>
      </form>
    </Group>
  );
};

export default LicenseForm;
