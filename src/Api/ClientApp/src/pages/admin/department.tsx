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
  Flex,
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
import { useTranslation } from "react-i18next";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import * as Yup from "yup";
import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
interface IDepartment<T> {
  id: string;
  name: string;
  isActive: boolean;
  user: T;
}
const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t("department_name_required") as string),
  });
};

const Department = ({
  searchParams,
  pagination,
  searchComponent,
  filterComponent,
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
    const [opened, setOpened] = useState(false);
    const [editModal, setEditModal] = useState(false);

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
          message: t("delete_department_success"),
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
        <Modal
          opened={editModal}
          onClose={() => {
            form.reset();
            setEditModal(false);
          }}
        >
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
                    message: t("update_department_success"),
                  });
                } catch (error) {
                  const err = errorType(error);

                  showNotification({
                    title: t("error"),
                    message: err,
                    color: "red",
                  });
                }
              })}
            >
              <CustomTextFieldWithAutoFocus
                label={t("department_name")}
                name="departmentName"
                withAsterisk
                placeholder={t("department_name_placeholder") as string}
                {...form.getInputProps("dName")}
                mb={10}
              />
              <Switch
                sx={{ input: { cursor: "pointer" } }}
                checked={form.values.isDepartmentActive}
                label={t("department_enabled")}
                labelPosition="left"
                onChange={(e) => {
                  form.setFieldValue(
                    "isDepartmentActive",
                    e.currentTarget.checked
                  );
                }}
              />

              <Group mt={20}>
                <Button type="submit">{t("submit")}</Button>

                <Button
                  variant="outline"
                  onClick={() => {
                    form.reset();
                    setEditModal(false);
                  }}
                >
                  {t("cancel")}
                </Button>
              </Group>
            </form>
          </Box>
        </Modal>

        {opened && (
          <DeleteModal
            title={`${t("sure_to_delete")} "${item?.name}" ${t("department?")}`}
            open={opened}
            onClose={setOpened}
            onConfirm={handleDelete}
          />
        )}

        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              {item?.name}
            </Text>
          </Group>
        </td>
        <td style={{ textAlign: "center" }}>
          {item?.isActive ? (
            <Badge color={"green"}>{t("active")}</Badge>
          ) : (
            <Badge color={"red"}>{t("inactive")}</Badge>
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

  const form = useForm({
    initialValues: { name: "", isActive: false },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const getDepartment = useDepartmentSetting(searchParams);
  const postDepartment = usePostDepartmentSetting();

  const [showAddForm, toggleAddForm] = useToggle();
  const { classes } = useStyles();
  const { t } = useTranslation();

  return (
    <>
      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>{t("departments")}</Title>
        {!showAddForm && (
          <Button onClick={() => toggleAddForm()}>{t("add_department")}</Button>
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
                      message: t("add_department_success"),
                    });
                  } catch (error) {
                    const err = errorType(error);

                    showNotification({
                      title: t("error"),
                      message: err,
                      color: "red",
                    });
                  }
                })}
              >
                <TextInput
                  label={t("department_name")}
                  name="departmentName"
                  withAsterisk
                  placeholder={t("department_name_placeholder") as string}
                  {...form.getInputProps("name")}
                  mb={10}
                />
                <Switch
                  sx={{ input: { cursor: "pointer" } }}
                  label={t("department_enabled")}
                  labelPosition="left"
                  onChange={(e) => {
                    form.setFieldValue("isActive", e.currentTarget.checked);
                  }}
                />

                <Group mt={20} ml={10}>
                  <Button type="submit">{t("submit")}</Button>
                  {showAddForm && (
                    <Button
                      variant="outline"
                      onClick={() => {
                        form.reset();
                        toggleAddForm();
                      }}
                    >
                      {t("cancel")}
                    </Button>
                  )}
                </Group>
              </form>
            </Box>
          </Paper>
        )}
      </Transition>
      <Flex mb={10}>
        {searchComponent(t("search_department") as string)}
        <Flex style={{ width: "210px" }}>
          {filterComponent(
            [
              { value: "true", label: t("active") },
              { value: "false", label: t("inactive") },
            ],
            t("department_status"),
            "IsActive"
          )}
        </Flex>
      </Flex>

      {getDepartment.data && getDepartment.data.totalCount > 0 ? (
        <Paper>
          <Table striped highlightOnHover withBorder sx={{ marginTop: "10px" }}>
            <thead>
              <tr>
                <th>{t("name")}</th>
                <th>
                  <Text align="center">{t("department_status")}</Text>
                </th>
                <th>
                  <Text align="center">{t("actions")}</Text>
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
        <Box mt={10}>{t("no_department")}</Box>
      )}

      {getDepartment.data &&
        pagination(
          getDepartment.data?.totalPage,
          getDepartment.data?.items.length
        )}
    </>
  );
};

export default withSearchPagination(Department);
