import DeleteModal from "@components/Ui/DeleteModal";
import {
  Badge,
  Table,
  Group,
  Text,
  ActionIcon,
  ScrollArea,
  useMantineTheme,
  Switch,
  Button,
  TextInput,
  Paper,
  Title,
  Transition,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconTrash } from "@tabler/icons";
import {
  IZoomLicense,
  updateZoomLicenseStatus,
  useAddZoomLicense,
  useDeleteZoomLicense,
  useZoomLicense,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { IUser } from "@utils/services/types";
import { TFunction } from "i18next";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import * as Yup from "yup";
import useFormErrorHooks from "@hooks/useFormErrorHooks";

interface IZoomLicensePost {
  licenseEmail: string;
  hostId: string;
  capacity: number;
}

export default function ZoomLicense() {
  const theme = useMantineTheme();
  const getZoomLicense = useZoomLicense();
  const addZoomLicense = useAddZoomLicense();
  const [showAddForm, toggleAddForm] = useToggle();
  const { t } = useTranslation();

  const schema = () => {
    return Yup.object().shape({
      licenseEmail: Yup.string()
        .email(t("invalid_license_email") as string)
        .required(t("license_email_required") as string),
      hostId: Yup.string().required(t("host_id_required") as string),
      capacity: Yup.number()
        .integer()
        .nullable(false)
        .min(1, t("capacity_required") as string),
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
      <tr key={item.id}>
        {opened && (
          <DeleteModal
            key={item.id}
            title={`${t("zoom_license_delete_confirmation")} "${
              item.licenseEmail
            }"?`}
            open={opened}
            onClose={setOpened}
            onConfirm={handleDelete}
          />
        )}

        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              {item.licenseEmail}
            </Text>
          </Group>
        </td>

        <td>
          <Badge variant={theme.colorScheme === "dark" ? "light" : "outline"}>
            {item.hostId}
          </Badge>
        </td>
        <td style={{ textAlign: "center" }}>{item.capacity}</td>
        <td style={{ textAlign: "center" }}>
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
        </td>
        <td>
          <Group spacing={0} position="center">
            <ActionIcon
              color="red"
              onClick={() => {
                setOpened(true);
              }}
            >
              <IconTrash size={16} stroke={1.5} />
            </ActionIcon>
          </Group>
        </td>
      </tr>
    );
  };

  const handleSubmit = async (values: IZoomLicensePost) => {
    try {
      await addZoomLicense.mutateAsync(values);
      showNotification({
        title: t("successful"),

        message: t("zoom_license_added"),
      });
      form.reset();
      toggleAddForm();
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    }
  };

  return (
    <ScrollArea>
      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>{t("zoom_licenses")}</Title>
        {!showAddForm && (
          <Button onClick={() => toggleAddForm()}>{t("add_license")}</Button>
        )}
        {/* <Button onClick={() => toggleAddForm()}>
          {!showAddForm ? "Add License" : "Cancel"}
        </Button> */}
      </Group>
      <Transition
        mounted={showAddForm}
        transition={"slide-down"}
        duration={200}
        timingFunction="ease"
      >
        {() => (
          <Paper shadow={"sm"} radius="md" p="xl" withBorder mb={20}>
            <form key={`${showAddForm}`} onSubmit={form.onSubmit(handleSubmit)}>
              <TextInput
                name="licenseEmail"
                label={t("license_email")}
                withAsterisk
                {...form.getInputProps("licenseEmail")}
              />
              <TextInput
                name="hostId"
                label={t("host_id")}
                withAsterisk
                {...form.getInputProps("hostId")}
              />
              <TextInput
                name="capacity"
                label={t("capacity")}
                type={"number"}
                withAsterisk
                {...form.getInputProps("capacity")}
              />
              <Group mt={10}>
                <Button type="submit">{t("submit")}</Button>
                {showAddForm && (
                  <Button
                    onClick={() => {
                      form.reset();
                      toggleAddForm();
                    }}
                    variant="outline"
                  >
                    {t("cancel")}
                  </Button>
                )}
              </Group>
            </form>
          </Paper>
        )}
      </Transition>
      <Paper>
        <Table
          sx={{ minWidth: 800 }}
          verticalSpacing="sm"
          horizontalSpacing="md"
          striped
          highlightOnHover
          withBorder
        >
          <thead>
            <tr>
              <th>{t("license_email")}</th>
              <th>{t("host_id")}</th>
              <th style={{ textAlign: "center" }}>{t("capacity")}</th>
              <th style={{ textAlign: "center" }}>{t("active_status")}</th>
              <th style={{ textAlign: "center" }}>{t("actions")}</th>
            </tr>
          </thead>
          <tbody>
            {getZoomLicense.data?.data.items.map((item) => (
              <Rows item={item} key={item.id} />
            ))}
          </tbody>
        </Table>
      </Paper>
    </ScrollArea>
  );
}
