import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Button,
  FocusTrap,
  Group,
  Paper,
  Space,
  TextInput,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import { useAddGroup } from "@utils/services/groupService";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import * as Yup from "yup";

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t("group_name_required") as string),
  });
};
const AddGroups = ({ onCancel }: { onCancel: () => void }) => {
  const navigate = useNavigate();

  const { t } = useTranslation();
  const { mutateAsync, isLoading, data, isSuccess } = useAddGroup();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  if (isSuccess) {
    navigate(`./${data.data.slug}/members`);
  }

  const onSubmitForm = async (name: string) => {
    try {
      await mutateAsync(name);
      showNotification({
        title: t("successful"),
        message: t("group_add_success"),
      });
      onCancel();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  return (
    <Paper>
      <form onSubmit={form.onSubmit(({ name }) => onSubmitForm(name))}>
        <FocusTrap active={true}>
          <TextInput
            data-autofocus
            autoComplete="off"
            mb={10}
            label={t("group_name")}
            placeholder={t("your_group_name") as string}
            withAsterisk
            {...form.getInputProps("name")}
          />
        </FocusTrap>
        <Space h="md" />
        <Group justify="flex-end">
          <Button loading={isLoading} type="submit">
            {t("submit")}
          </Button>
        </Group>
      </form>
    </Paper>
  );
};

export default AddGroups;
