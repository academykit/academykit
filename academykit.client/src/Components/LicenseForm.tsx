import { Button, Group, Text, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";

import { useTranslation } from "react-i18next";
import useLemonSqueeze from "./LemonSqueeze/useLemonSqueeze";

const LicenseForm = () => {
  const { t } = useTranslation();

  const lemon = useLemonSqueeze();

  const form = useForm({
    initialValues: {
      license_key: "",
    },
  });
  const onSubmit = async () => {};

  console.log("chekcout Data", lemon);
  return (
    <Group justify="center">
      <form onSubmit={form.onSubmit(onSubmit)}>
        <Text size="lg">{t("license_key")}</Text>
        {/* <Paper p={20} withBorder mt={20}> */}
        <TextInput
          autoComplete="off"
          withAsterisk
          placeholder={t("license_key") as string}
          {...form.getInputProps("license_key")}
        />
        <Group>
          <Button onClick={() => {}}>{t("submit")}</Button>
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
        {/* </Paper> */}
      </form>
    </Group>
  );
};

export default LicenseForm;
