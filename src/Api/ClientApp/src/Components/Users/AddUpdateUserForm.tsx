import { useEffect, useState } from "react";
import { Group, TextInput, Switch, Select, Button, Grid } from "@mantine/core";
import { UserRole } from "@utils/enums";
import { useDepartmentSetting } from "@utils/services/adminService";
import { useForm, yupResolver } from "@mantine/form";
import axios from "axios";
import { showNotification } from "@mantine/notifications";
import * as Yup from "yup";
import errorType from "@utils/services/axiosError";
import { IUserProfile } from "@utils/services/types";
import queryStringGenerator from "@utils/queryStringGenerator";
import { PHONE_VALIDATION } from "@utils/constants";

const schema = Yup.object().shape({
  email: Yup.string().email("Invalid email").required("Email is required."),
  firstName: Yup.string()
    .max(100, "Firstname should have atmost 100 characters.")
    .required("First Name is required."),
  lastName: Yup.string()
    .max(100, "Lastname should have atmost 100 characters.")
    .required("Last Name is required."),
  middleName: Yup.string()
    .max(100, "Middlename should have atmost 100 characters.")
    .nullable()
    .notRequired(),
  role: Yup.string()
    .oneOf(["1", "2", "3", "4"], "Role is required.")
    .required("Role is required."),
  mobileNumber: Yup.string().matches(
    PHONE_VALIDATION,
    "Please enter valid phone number."
  ),
});

const AddUpdateUserForm = ({
  setOpened,
  opened,
  isEditing,
  apiHooks,
  item,
}: {
  setOpened: Function;
  opened: boolean;
  isEditing: boolean;
  apiHooks: any;
  item?: IUserProfile;
}) => {
  const form = useForm<IUserProfile>({
    initialValues: item,
    validate: yupResolver(schema),
  });

  const department = useDepartmentSetting(
    queryStringGenerator({
      search: "",
      size: 200,
    })
  );

  const [userStatus, setUserStatus] = useState<boolean>(item?.isActive ?? true);

  useEffect(() => {
    form.setFieldValue("role", item?.role ?? 5);
    item?.departmentId &&
      form.setFieldValue("department", item?.departmentId.toString() ?? "");
    form.setFieldValue("isActive", item?.isActive ?? false);
  }, [isEditing]);

  const onSubmitForm = async (data: IUserProfile) => {
    try {
      if (!isEditing) {
        await apiHooks.mutateAsync({
          ...data,
          role: Number(data?.role),
          isActive: userStatus,
        });
      } else {
        data = { ...data, role: Number(data?.role) };
        await apiHooks.mutateAsync({ id: item?.id as string, data });
      }
      showNotification({
        message: `User ${isEditing ? "Edited" : "Added"} successfully!`,
        title: "Success",
      });
      setOpened(!opened);
    } catch (error) {
      const err = errorType(error);

      if (axios.isAxiosError(error)) {
        showNotification({
          message: err,
          title: "Error!",
          color: "red",
        });
      } else {
        showNotification({
          message: "Unable to add group at this moment! please try again.",
          color: "red",
        });
      }
      setOpened(!opened);
    }
  };

  return (
    <form onSubmit={form.onSubmit(onSubmitForm)}>
      <Grid align={"center"}>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            withAsterisk
            label="First Name"
            placeholder="User's First Name"
            name="firstName"
            {...form.getInputProps("firstName")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            label="Middle Name"
            placeholder="User's Middle Name"
            {...form.getInputProps("middleName")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            withAsterisk
            label="Last Name"
            placeholder="User's Last Name"
            {...form.getInputProps("lastName")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            withAsterisk
            label="Email"
            type="email"
            placeholder="User's Email Name"
            {...form.getInputProps("email")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            label="Mobile Number"
            placeholder="User's Phone number"
            {...form.getInputProps("mobileNumber")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            label="Profession"
            placeholder="User's Profession Name"
            {...form.getInputProps("profession")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <Switch
            label="User Status"
            {...form.getInputProps("isActive")}
            checked={userStatus}
            onChange={(event) => {
              setUserStatus(event.currentTarget.checked);
              form.setFieldValue("isActive", event.target.checked);
            }}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <Select
            withAsterisk
            error="Pick at least one"
            label="User Role"
            placeholder="Pick one user role"
            data={[
              { value: UserRole.Admin, label: "Admin" },
              { value: UserRole.Trainer, label: "Trainer" },
              { value: UserRole.Trainee, label: "Trainee" },
            ]}
            {...form.getInputProps("role")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <Select
            label="Department"
            placeholder="Pick One Department"
            searchable
            {...form.getInputProps("departmentId")}
            data={
              department.data
                ? department.data.items.map((x) => ({
                    label: x.name,
                    value: x.id,
                  }))
                : [""]
            }
          />
        </Grid.Col>
      </Grid>

      <Group position="right" mt="md">
        <Button type="submit" loading={apiHooks.isLoading}>
          Submit
        </Button>
      </Group>
    </form>
  );
};
export default AddUpdateUserForm;