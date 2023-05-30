import { Button, Card, Group, Radio, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { EFileStorageType } from "@utils/enums";
import {
  IFileStorage,
  useUpdateFileStorage,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { useState } from "react";
import { useTranslation } from "react-i18next";

const FileStorageUI = ({ data }: { data: IFileStorage[] }) => {
  const form = useForm({ initialValues: data });
  const { t } = useTranslation();
  const [activeIndex, setActiveIndex] = useState(
    data.findIndex((x) => x.isActive)
  );
  const fileStorage = useUpdateFileStorage();
  const submitHandler = async (data: IFileStorage[]) => {
    try {
      await fileStorage.mutateAsync(data);
      showNotification({
        message: "Successfully updated file storage",
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
    <form onSubmit={form.onSubmit(submitHandler)}>
      <Group>
        {/* @ts-ignore */}
        <Radio
          onChange={() => {
            form.setFieldValue(`0.isActive`, true);
            form.setFieldValue(`1.isActive`, false);

            setActiveIndex(0);
          }}
          checked={form.values[0].isActive}
          label={t(`${EFileStorageType[form.values[0].type]}`)}
        />
        {/* @ts-ignore */}
        <Radio
          onChange={() => {
            setActiveIndex(1);
            form.setFieldValue(`0.isActive`, false);
            form.setFieldValue(`1.isActive`, true);
          }}
          checked={form.values[1].isActive}
          label={t(`${EFileStorageType[form.values[1].type]}`)}
        />
      </Group>
      <Card>
        {form.values[activeIndex].values.map((x, index) => (
          <TextInput
            key={x.key}
            mt={10}
            {...form.getInputProps(`${activeIndex}.values.${index}.value`)}
            label={x.key}
          />
        ))}
        <Button mt={20} type="submit">
          {t("save")}
        </Button>
      </Card>
    </form>
  );
};

export default FileStorageUI;
