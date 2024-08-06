import DeleteModal from "@components/Ui/DeleteModal";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  ActionIcon,
  Box,
  Button,
  Drawer,
  Group,
  Paper,
  Table,
  Text,
  TextInput,
  Title,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { useDisclosure } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconPencil, IconTrash } from "@tabler/icons-react";
import {
  useDeleteLevelSetting,
  useLevelSetting,
  usePostLevelSetting,
  useUpdateLevelSetting,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { IUser } from "@utils/services/types";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import * as Yup from "yup";

interface ILevel<T> {
  id: string;
  name: string;
  user: T;
}

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t("level_name_required") as string),
  });
};

const editSchema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    eName: Yup.string().required(t("level_name_required") as string),
  });
};

const Level = () => {
  const { t } = useTranslation();
  const [opened, { open, close }] = useDisclosure(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editItem, setEditItem] = useState<ILevel<IUser>>();
  const updateLevel = useUpdateLevelSetting();
  const getLevel = useLevelSetting();
  const postLevel = usePostLevelSetting();

  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  useEffect(() => {
    if (isEditing) {
      form.setValues({
        name: editItem?.name,
      });
    }
  }, [isEditing]);

  const Rows = ({ item }: { item: ILevel<IUser> }) => {
    const [opened, setOpened] = useState(false);

    const form = useForm({
      initialValues: {
        eName: item.name,
      },
      validate: yupResolver(editSchema()),
    });
    useFormErrorHooks(form);

    const deleteLevel = useDeleteLevelSetting();
    const handleDelete = async () => {
      try {
        await deleteLevel.mutateAsync(item.id);
        showNotification({
          message: t("delete_level"),
        });
      } catch (error) {
        const err = errorType(error);
        showNotification({
          message: err,
          color: "red",
        });
      }
      setOpened(false);
    };

    return (
      <Table.Tr key={item.id}>
        {opened && (
          <DeleteModal
            title={`${t("want_to_delete")} "${item?.name}" ${t("level?")}`}
            open={opened}
            onClose={() => setOpened(false)}
            onConfirm={handleDelete}
            loading={deleteLevel.isLoading}
          />
        )}
        <Table.Td>
          <Group gap="sm">
            <Text size="sm" fw={500}>
              {item.name}
            </Text>
          </Group>
        </Table.Td>
        <Table.Td>
          <Group gap={0} justify="center">
            <ActionIcon variant="subtle">
              <IconPencil
                size={16}
                stroke={1.5}
                onClick={() => {
                  opened ? close() : open();
                  setIsEditing(true);
                  setEditItem(item);
                }}
              />
            </ActionIcon>
            <ActionIcon variant="subtle" color="red">
              <IconTrash
                size={16}
                stroke={1.5}
                onClick={() => {
                  setOpened(true);
                }}
              />
            </ActionIcon>
          </Group>
        </Table.Td>
      </Table.Tr>
    );
  };

  return (
    <>
      <Group
        style={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>{t("levels")}</Title>
        {!opened && (
          <Button
            onClick={() => {
              open();
              form.reset();
            }}
          >
            {t("add_level")}
          </Button>
        )}
      </Group>

      <Drawer
        opened={opened}
        onClose={() => {
          close();
          setIsEditing(false);
        }}
        overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
      >
        <Box>
          <form
            onSubmit={form.onSubmit(async (values) => {
              try {
                if (isEditing) {
                  await updateLevel.mutateAsync({
                    id: editItem?.id as string,
                    ...values,
                  });
                  form.reset();
                  showNotification({
                    message: t("update_success"),
                  });
                } else {
                  await postLevel.mutateAsync(values);
                  showNotification({
                    message: t("add_level_success"),
                  });
                  form.reset();
                }
              } catch (error) {
                const err = errorType(error);

                showNotification({
                  title: t("error"),
                  message: err,
                  color: "red",
                });
              } finally {
                close();
                setIsEditing(false);
              }
            })}
          >
            <TextInput
              label={t("level_name")}
              name="levelName"
              withAsterisk
              placeholder={t("level_name_placeholder") as string}
              {...form.getInputProps("name")}
            />

            <Group mt={10}>
              <Button
                type="submit"
                loading={updateLevel.isLoading || postLevel.isLoading}
              >
                {t("submit")}
              </Button>
            </Group>
          </form>
        </Box>
      </Drawer>

      <Paper>
        <Table
          striped
          highlightOnHover
          withTableBorder
          withColumnBorders
          style={{ marginTop: "10px" }}
        >
          <Table.Thead>
            <Table.Tr>
              <Table.Th>{t("name")}</Table.Th>

              <Table.Th>
                <Text ta="center">{t("actions")}</Text>
              </Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {getLevel.data?.map((item: any) => (
              <Rows item={item} key={item.id} />
            ))}
          </Table.Tbody>
        </Table>
      </Paper>
    </>
  );
};

export default Level;
