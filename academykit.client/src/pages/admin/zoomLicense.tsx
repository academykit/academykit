import DeleteModal from "@components/Ui/DeleteModal";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  ActionIcon,
  Badge,
  Button,
  Drawer,
  Group,
  Paper,
  ScrollArea,
  Switch,
  Table,
  Text,
  TextInput,
  Title,
  useMantineColorScheme,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { useDisclosure } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconTrash } from "@tabler/icons-react";
import {
  IZoomLicense,
  updateZoomLicenseStatus,
  useAddZoomLicense,
  useDeleteZoomLicense,
  useUpdateZoomLicense,
  useZoomLicense,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { IUser } from "@utils/services/types";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import * as Yup from "yup";

interface IZoomLicensePost {
  licenseEmail: string;
  hostId: string;
  capacity: number;
}

export default function ZoomLicense() {
  const { t } = useTranslation();
  const [opened, { open, close }] = useDisclosure(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editItem, setEditItem] = useState<IZoomLicense<IUser>>();
  const getZoomLicense = useZoomLicense();
  const updateZoomLicense = useUpdateZoomLicense();
  const addZoomLicense = useAddZoomLicense();
  const { colorScheme } = useMantineColorScheme();

  const schema = () => {
    return Yup.object().shape({
      licenseEmail: Yup.string()
        .email(t("invalid_license_email") as string)
        .required(t("license_email_required") as string),
      hostId: Yup.string().required(t("host_id_required") as string),
      capacity: Yup.number()
        .integer()
        .nullable(false)
        .min(1, t("capacity_required") as string)
        .typeError(t("capacity_required") as string),
    });
  };

  const form = useForm<IZoomLicensePost>({
    initialValues: {
      licenseEmail: "",
      hostId: "",
      capacity: 0,
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  useEffect(() => {
    if (isEditing) {
      form.setValues({
        licenseEmail: editItem?.licenseEmail,
        hostId: editItem?.hostId,
        capacity: editItem?.capacity,
      });
    }
  }, [isEditing]);

  const Rows = ({ item }: { item: IZoomLicense<IUser> }) => {
    const [isChecked, setIsChecked] = useState<boolean>(item?.isActive);
    const [opened, setOpened] = useState(false);
    const deleteZoomLicense = useDeleteZoomLicense();
    const handleDelete = async () => {
      try {
        await deleteZoomLicense.mutateAsync(item.id);
        showNotification({
          title: t("successful"),
          message: t("zoom_license_deleted"),
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
            key={item.id}
            title={`${t("zoom_license_delete_confirmation")} "${item.licenseEmail
              }"?`}
            open={opened}
            onClose={() => setOpened(false)}
            onConfirm={handleDelete}
          />
        )}

        <Table.Td>
          <Group gap="sm">
            <Text size="sm" fw={500}>
              {item.licenseEmail}
            </Text>
          </Group>
        </Table.Td>

        <Table.Td>
          <Badge variant={colorScheme === "dark" ? "light" : "outline"}>
            {item.hostId}
          </Badge>
        </Table.Td>
        <Table.Td style={{ textAlign: "center" }}>{item.capacity}</Table.Td>
        <Table.Td style={{ textAlign: "center" }}>
          <Switch
            checked={isChecked}
            onChange={async () => {
              try {
                setIsChecked(!isChecked);

                await updateZoomLicenseStatus({
                  id: item.id,
                  status: !isChecked,
                });
                showNotification({
                  message: t("status_updated"),
                  title: t("successful"),
                });
                getZoomLicense.refetch();
              } catch (error) {
                const err = errorType(error);
                showNotification({
                  message: err,
                  title: t("error"),
                  color: "red",
                });
                setIsChecked(!isChecked);
              }
            }}
          />
        </Table.Td>
        <Table.Td>
          <Group gap={0} justify="center">
            <ActionIcon
              c="red"
              onClick={() => {
                opened ? close() : open();
                setIsEditing(true);
                setEditItem(item);
              }}
            >
              <IconEdit size={16} stroke={1.5} />
            </ActionIcon>
            <ActionIcon
              c="red"
              onClick={() => {
                setOpened(true);
              }}
            >
              <IconTrash size={16} stroke={1.5} />
            </ActionIcon>
          </Group>
        </Table.Td>
      </Table.Tr>
    );
  };

  const handleSubmit = async (values: IZoomLicensePost) => {
    try {
      if (isEditing) {
        await updateZoomLicense.mutateAsync({
          id: (editItem?.id as string) ?? "",
          data: values,
        });
        showNotification({
          title: t("successful"),
          message: t("zoom_license_update_success"),
        });
        form.reset();
      } else {
        await addZoomLicense.mutateAsync(values);
        showNotification({
          title: t("successful"),

          message: t("zoom_license_added"),
        });
        form.reset();
      }
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    } finally {
      close();
      setIsEditing(false);
    }
  };

  return (
    <ScrollArea>
      <Group
        style={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>{t("zoom_licenses")}</Title>
        <Button
          onClick={() => {
            open();
            form.reset();
          }}
        >
          {t("add_license")}
        </Button>
      </Group>

      <Drawer
        opened={opened}
        onClose={() => {
          close();
          setIsEditing(false);
          setEditItem(undefined);
        }}
        overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
      >
        <form onSubmit={form.onSubmit(handleSubmit)}>
          <TextInput
            placeholder={t("License_email") as string}
            name="licenseEmail"
            label={t("license_email")}
            withAsterisk
            {...form.getInputProps("licenseEmail")}
          />
          <TextInput
            placeholder={t("License_host_Id") as string}
            name="hostId"
            label={t("host_id")}
            withAsterisk
            {...form.getInputProps("hostId")}
          />
          <TextInput
            placeholder={t("license_capacity") as string}
            name="capacity"
            label={t("capacity")}
            type={"number"}
            withAsterisk
            {...form.getInputProps("capacity")}
          />
          <Group mt={10}>
            <Button
              type="submit"
              loading={addZoomLicense.isPending || updateZoomLicense.isPending}
            >
              {t("submit")}
            </Button>
          </Group>
        </form>
      </Drawer>

      <Paper>
        <Table
          style={{ minWidth: 800 }}
          verticalSpacing="sm"
          horizontalSpacing="md"
          striped
          highlightOnHover
          withTableBorder
          withColumnBorders
        >
          <Table.Thead>
            <Table.Tr>
              <Table.Th>{t("license_email")}</Table.Th>
              <Table.Th>{t("host_id")}</Table.Th>
              <Table.Th style={{ textAlign: "center" }}>
                {t("capacity")}
              </Table.Th>
              <Table.Th style={{ textAlign: "center" }}>
                {t("active_status")}
              </Table.Th>
              <Table.Th style={{ textAlign: "center" }}>
                {t("actions")}
              </Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {getZoomLicense.data?.data.items.map((item) => (
              <Rows item={item} key={item.id} />
            ))}
          </Table.Tbody>
        </Table>
      </Paper>
    </ScrollArea>
  );
}
