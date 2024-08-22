import useLicenseValidation from "@hooks/useLicenseValidation";
import { Button, Group, Text, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";

import { showNotification } from "@mantine/notifications";
import {
  useActivatelicense,
  useCheckoutLicense,
} from "@utils/services/licenseService";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

const LicenseForm = () => {
  const { t } = useTranslation();
  const [isCheckout, setIsCheckout] = useState(false);

  const form = useForm({
    initialValues: {
      license_key: "",
    },
  });

  const activateLicense = useActivatelicense();
  const license = useLicenseValidation();
  const { data } = useCheckoutLicense(isCheckout);

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

  useEffect(() => {
    if (data?.checkoutUrl) {
      window.open(data.checkoutUrl, "_blank");
      setIsCheckout(false);
    }
  }, [data]);

  const handleCheckout = () => {
    setIsCheckout(true);
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
          <Button variant="outline" type="button" onClick={handleCheckout}>
            {t("Buy Key")}
          </Button>
        </Group>
      </form>
    </Group>
  );
};

export default LicenseForm;
