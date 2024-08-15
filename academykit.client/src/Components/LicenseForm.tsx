import { Button, Group, Text, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";
import { useTranslation } from "react-i18next";

const LicenseForm = () => {
    const { t } = useTranslation();

    const form = useForm({
        initialValues: {
            license_key: "",
        },
    });
    const onSubmit = async () => { };

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
                    <Button onClick={() => { }}>{t("submit")}</Button>
                    <Button
                        variant="outline"
                        onClick={() => { }}
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
