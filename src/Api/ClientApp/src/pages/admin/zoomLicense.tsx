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
  Modal,
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
import { useState } from "react";
import * as Yup from "yup";

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

  const schema = Yup.object().shape({
    licenseEmail: Yup.string()
      .email("Invalid Licenses Email.")
      .required("Licenses Email is required."),
    hostId: Yup.string().required("Host ID is required."),
    capacity: Yup.number()
      .integer()
      .nullable(false)
      .min(1, "Capacity should be greater than 0."),
  });

  const form = useForm<IZoomLicensePost>({
    initialValues: {
      licenseEmail: "",
      hostId: "",
      capacity: 0,
    },
    validate: yupResolver(schema),
  });

  const Rows = ({ item }: { item: IZoomLicense<IUser> }) => {
    const [isChecked, setIsChecked] = useState<boolean>(item?.isActive);
    const [opened, setOpened] = useState(false);
    const deleteZoomLicense = useDeleteZoomLicense();
    const handleDelete = async () => {
      try {
        await deleteZoomLicense.mutateAsync(item.id);
        showNotification({
          message: "Zoom License delete successfully!",
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
        <DeleteModal
          title={`Are you sure you want to delete License with Email "${item.licenseEmail}"?`}
          open={opened}
          onClose={setOpened}
          onConfirm={handleDelete}
        />

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
                  message: "Status updated successfully!",
                  title: "Success!",
                });
                getZoomLicense.refetch();
              } catch (error) {
                const err = errorType(error);
                showNotification({
                  message: err,
                  title: "Error!",
                  color: "red",
                });
                setIsChecked(!isChecked);
              }
            }}
          />
        </td>
        <td>
          <Group spacing={0} position="center">
            {/* <ActionIcon>
              <IconPencil size={16} stroke={1.5} />
            </ActionIcon> */}
            <ActionIcon color="red">
              <IconTrash
                size={16}
                stroke={1.5}
                onClick={() => {
                  setOpened(true);
                }}
              />
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
        message: "Zoom License added successfully!",
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
        <Title>Zoom Licenses</Title>
        {!showAddForm && (
          <Button onClick={() => toggleAddForm()}>Add License</Button>
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
            <form onSubmit={form.onSubmit(handleSubmit)}>
              <TextInput
                name="licenseEmail"
                label="License Email"
                withAsterisk
                {...form.getInputProps("licenseEmail")}
              />
              <TextInput
                name="hostId"
                label="Host ID"
                withAsterisk
                {...form.getInputProps("hostId")}
              />
              <TextInput
                name="capacity"
                label="Capacity"
                type={"number"}
                withAsterisk
                {...form.getInputProps("capacity")}
              />
              <Group mt={10}>
                <Button type="submit">Submit</Button>
                {showAddForm && (
                  <Button onClick={() => toggleAddForm()} variant="outline">
                    Cancel
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
              <th>License Email</th>
              <th>Host ID</th>
              <th style={{ textAlign: "center" }}>Capacity</th>
              <th style={{ textAlign: "center" }}>Active Status</th>
              <th style={{ textAlign: "center" }}>Actions</th>
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
