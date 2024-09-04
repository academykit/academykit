import { Button, Container, Group, Text, TextInput } from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { LICENSE_KEY } from "@utils/constants";
// import { useLicenseKey, useUpdateLicenseKey } from "@utils/services/licenseService";
import errorType from "@utils/services/axiosError";
import {
  useCheckoutLicense,
  useUpdateLicense,
} from "@utils/services/licenseService";
import { t } from "i18next";
import * as Yup from "yup";

const LicenseManagement = () => {
  // const formData = useLicenseKey();
  // const updateLicenseKey = useUpdateLicenseKey();
  const licenseToken = localStorage.getItem(LICENSE_KEY);
  const { mutateAsync, isPending } = useCheckoutLicense();
  const updateLicenseKey = useUpdateLicense();

  // useEffect(() => {
  //     form.setValues({
  //         key: formData.data?.key ?? "",
  //     });
  // }, [formData.isSuccess]);

  const schema = () => {
    return Yup.object().shape({
      licenseKey: Yup.string().required(t("level_name_required") as string),
    });
  };

  const form = useForm({
    initialValues: {
      licenseKey: licenseToken ?? "",
    },
    validate: yupResolver(schema()),
  });

  const handleSubmit = async (values: { licenseKey: string }) => {
    try {
      await updateLicenseKey.mutateAsync({
        licenseKey: values.licenseKey,
      });
      showNotification({
        message: t("update_license_key_success"),
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

  return (
    <>
      <Text fw={700} size="xl">
        {t("license_management")}
      </Text>
      <Text size="sm" mb={10}>
        {t("license_activated_on")}
      </Text>
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
