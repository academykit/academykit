import { data } from "@components/Dashboard/ChartTest";
import DeleteModal from "@components/Ui/DeleteModal";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  ActionIcon,
  Badge,
  Box,
  Button,
  Container,
  createStyles,
  Group,
  Modal,
  Paper,
  Switch,
  Table,
  Text,
  TextInput,
  Title,
  Transition,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { randomId, useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconPencil, IconTrash } from "@tabler/icons";
import {
  useDeleteDepartmentSetting,
  useDepartmentSetting,
  usePostDepartmentSetting,
  useUpdateDepartmentSetting,
  useUpdateDepartmentSettingStatus,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { IUser } from "@utils/services/types";
import { useState } from "react";
import * as Yup from "yup";
interface IDepartment<T> {
  id: string;
  name: string;
  isActive: boolean;
  user: T;
}

const Department = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const useStyles = createStyles((theme) => ({
    paper: {
      [theme.fn.smallerThan("md")]: {
        width: "100%",
      },
      [theme.fn.smallerThan("lg")]: {
        width: "100%",
      },
      width: "50%",
      marginBottom: "20px",
    },
  }));
  const Rows = ({ item }: { item: IDepartment<IUser> }) => {
    const departmentSettingStatus = useUpdateDepartmentSettingStatus(
      item.id,
      !item.isActive
    );
    const [opened, setOpened] = useState(false);
    const [editModal, setEditModal] = useState(false);
    const [isChecked, setIsChecked] = useState(item.isActive);

    const form = useForm({
      initialValues: {
        dName: item.name,
        isDepartmentActive: item.isActive,
      },
    });

    const updateDepartment = useUpdateDepartmentSetting(item.id);
    const deleteDepartment = useDeleteDepartmentSetting();
    const handleDelete = async () => {
      try {
        await deleteDepartment.mutateAsync(item.id);
        showNotification({
          message: "Department deleted successfully!",
        });
      } catch (error: any) {
        showNotification({
          message: error?.response?.data?.message,
          color: "red",
        });
      }
    };
    return (
      <tr key={item.id}>
        <Modal opened={editModal} onClose={() => setEditModal(false)}>
          <Box>
            <form
              onSubmit={form.onSubmit(async (values) => {
                try {
                  await updateDepartment.mutateAsync({
                    id: item.id,
                    name: values.dName,
                    isActive: values.isDepartmentActive,
                  });

                  showNotification({
                    message: "Successfully updated department!",
                  });
                } catch (error) {
                  const err = errorType(error);

                  showNotification({
                    title: "Error!",
                    message: err,
                    color: "red",
                  });
                }
              })}
            >
              <TextInput
                label="Department Name"
                name="departmentName"
                withAsterisk
                placeholder="Enter Department Name"
                {...form.getInputProps("dName")}
                mb={10}
              />
              <Switch
                sx={{ input: { cursor: "pointer" } }}
                checked={isChecked}
                label="Department Enabled"
                labelPosition="left"
                onChange={(e) => {
                  setIsChecked(e.currentTarget.checked);
                  form.setFieldValue(
                    "isDepartmentActive",
                    e.currentTarget.checked
                  );
                }}
              />

              <Group mt={20}>
                <Button type="submit">Submit</Button>

                <Button variant="outline" onClick={() => setEditModal(false)}>
                  Cancel
                </Button>
              </Group>
            </form>
          </Box>
        </Modal>

        <DeleteModal
          title={`Are you sure you want to delete "${item?.name}" department?`}
          open={opened}
          onClose={setOpened}
          onConfirm={handleDelete}
        />

        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              {item?.name}
            </Text>
          </Group>
        </td>
        <td style={{ textAlign: "center" }}>
          {item?.isActive ? (
            <Badge color={"green"}>Active</Badge>
          ) : (
            <Badge color={"red"}>InActive</Badge>
          )}
        </td>
        <td>
          <Group spacing={0} position="center">
            <ActionIcon onClick={() => setEditModal(true)}>
              <IconPencil size={16} stroke={1.5} />
            </ActionIcon>
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

  const schema = Yup.object().shape({
    name: Yup.string().required(" Department Name is required."),
  });
  const form = useForm({
    initialValues: { name: "", isActive: false },
    validate: yupResolver(schema),
  });
  const getDepartment = useDepartmentSetting(searchParams);
  const postDepartment = usePostDepartmentSetting();

  const [showAddForm, toggleAddForm] = useToggle();
  const { classes } = useStyles();

  return (
    <>
      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>Departments</Title>
        {!showAddForm && (
          <Button onClick={() => toggleAddForm()}>Add Department</Button>
        )}
      </Group>

      <Transition
        mounted={showAddForm}
        transition={"slide-down"}
        duration={200}
        timingFunction="ease"
      >
        {(style) => (
          <Paper
            shadow={"sm"}
            radius="md"
            p="xl"
            withBorder
            className={classes.paper}
          >
            <Box mt={10}>
              <form
                onSubmit={form.onSubmit(async (values) => {
                  try {
                    await postDepartment.mutateAsync(values);
                    form.reset();
                    toggleAddForm();
                    showNotification({
                      message: "Successfully added department!",
                    });
                  } catch (error) {
                    const err = errorType(error);

                    showNotification({
                      title: "Error!",
                      message: err,
                      color: "red",
                    });
                  }
                })}
              >
                <TextInput
                  label="Department Name"
                  name="departmentName"
                  withAsterisk
                  placeholder="Enter Department Name"
                  {...form.getInputProps("name")}
                  mb={10}
                />
                <Switch
                  sx={{ input: { cursor: "pointer" } }}
                  label="Department Enabled"
                  labelPosition="left"
                  onChange={(e) => {
                    form.setFieldValue("isActive", e.currentTarget.checked);
                  }}
                />

                <Group mt={20} ml={10}>
                  <Button type="submit">Submit</Button>
                  {showAddForm && (
                    <Button variant="outline" onClick={() => toggleAddForm()}>
                      Cancel
                    </Button>
                  )}
                </Group>
              </form>
            </Box>
          </Paper>
        )}
      </Transition>
      {searchComponent("Search Department")}

      {getDepartment.data && getDepartment.data.totalCount > 0 ? (
        <Paper>
          <Table striped highlightOnHover withBorder sx={{ marginTop: "10px" }}>
            <thead>
              <tr>
                <th>Name</th>
                <th>
                  <Text align="center">Department Status</Text>
                </th>
                <th>
                  <Text align="center">Actions</Text>
                </th>
              </tr>
            </thead>
            <tbody>
              {getDepartment.data?.items.map((item: any) => (
                <Rows item={item} key={item.id} />
              ))}
            </tbody>
          </Table>
        </Paper>
      ) : (
        <Box mt={10}>No Department Found!</Box>
      )}

      {getDepartment.data && pagination(getDepartment.data?.totalPage)}
    </>
  );
};

export default withSearchPagination(Department);
