import { Button, Container, Select, Switch, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { AiModelEnum } from "@utils/enums";
import { useAIMaster, useUpdateAISetup } from "@utils/services/aiService";
import errorType from "@utils/services/axiosError";
import { t } from "i18next";
import { useEffect } from "react";

const AIMasterSetup = () => {
  const formData = useAIMaster();
  const updateAISetup = useUpdateAISetup();

  const form = useForm({
    initialValues: {
      key: "",
      isActive: false,
      aiModel: "",
    },
  });

  useEffect(() => {
    form.setValues({
      key: formData.data?.key ?? "",
      isActive: formData.data?.isActive ?? false,
      aiModel: formData.data?.aiModel ? formData.data.aiModel.toString() : "",
    });
  }, [formData.isSuccess]);

  const handleSubmit = async (values: typeof form.values) => {
    try {
      await updateAISetup.mutateAsync({
        data: { ...values, aiModel: Number(values.aiModel) },
      });
      showNotification({
        message: t("update_ai_setup_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        color: "red",
        message: error,
      });
    }
  };

  const getAIModels = () => {
    return Object.entries(AiModelEnum)
      .splice(0, Object.entries(AiModelEnum).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
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
            label={t("ai_key")}
            placeholder={t("enter_ai_key") as string}
            {...form.getInputProps("key")}
          />

          <Select
            mb={10}
            clearable
            label={t("ai_model")}
            placeholder={t("ai_model_placeholder") as string}
            data={getAIModels()}
            {...form.getInputProps("aiModel")}
          />

          <Switch
            mb={10}
            label={t("isActive")}
            {...form.getInputProps("isActive", { type: "checkbox" })}
          />
          <Button loading={updateAISetup.isPending} type="submit">
            {t("submit")}
          </Button>
        </Container>
      </form>
    </>
  );
};

export default AIMasterSetup;
