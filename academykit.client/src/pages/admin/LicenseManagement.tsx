import { Button, Container, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
// import { useLicenseKey, useUpdateLicenseKey } from "@utils/services/licenseService";
import errorType from "@utils/services/axiosError";
import { t } from "i18next";

const LicenseManagement = () => {
  // const formData = useLicenseKey();
  // const updateLicenseKey = useUpdateLicenseKey();

  const form = useForm({
    initialValues: {
      key: "",
    },
  });

  // useEffect(() => {
  //     form.setValues({
  //         key: formData.data?.key ?? "",
  //     });
  // }, [formData.isSuccess]);

  const handleSubmit = async () => {
    try {
      // await updateLicenseKey.mutateAsync({
      //     data: values,
      // });
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

  return (
    <>
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Container
          size={450}
          style={{
            marginLeft: "0px",
          }}
        >
          <TextInput
            autoFocus
            mb={10}
            label={t("license_key")}
            placeholder={t("enter_license_key") as string}
            {...form.getInputProps("key")}
          />

          <Button type="submit">{t("submit")}</Button>
        </Container>
      </form>
    </>
  );
};

export default LicenseManagement;
