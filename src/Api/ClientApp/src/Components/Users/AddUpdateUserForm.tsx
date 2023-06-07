import { useEffect } from "react";
import { Group, TextInput, Switch, Select, Button, Grid } from "@mantine/core";
import { UserRole, UserStatus } from "@utils/enums";
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
  mobileNumber: Yup.string().nullable().matches(PHONE_VALIDATION, {
    message: "Please enter valid phone number.",
    excludeEmptyString: true,
  }),
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

  const { data } = useDepartmentSetting(
    queryStringGenerator({
      search: "",
      size: 200,
    })
  );

  useEffect(() => {
    form.setFieldValue("role", item?.role ?? 4);
    item?.departmentId &&
      form.setFieldValue("departmentId", item?.departmentId.toString() ?? "");
    form.setFieldValue(
      "isActive",
      item?.status === UserStatus.Active || item?.status === UserStatus.Pending
        ? true
        : false
    );
  }, [isEditing]);

  const onSubmitForm = async (data: typeof form.values) => {
    try {
      if (!isEditing) {
        await apiHooks.mutateAsync({
          ...data,
          role: Number(data?.role),
        });
      } else {
        const userData = { ...data };
        //@ts-ignore
        delete userData.isActive;
        const status =
          item?.status === UserStatus.Pending
            ? UserStatus.Pending
            : data.isActive
            ? UserStatus.Active
            : UserStatus.InActive;
        data = { ...userData, role: Number(data?.role), status };
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
        {isEditing && item?.status !== UserStatus.Pending && (
          <Grid.Col xs={6} lg={4}>
            <Switch
              label="User Status"
              {...form.getInputProps("isActive", { type: "checkbox" })}
            />
          </Grid.Col>
        )}
        <Grid.Col xs={6} lg={4}>
          <Select
            withAsterisk
            error="Pick at least one"
            label="User Role"
            placeholder="Pick one user role"
            data={[
              { value: UserRole.Admin.toString(), label: "Admin" },
              { value: UserRole.Trainer.toString(), label: "Trainer" },
              { value: UserRole.Trainee.toString(), label: "Trainee" },
            ]}
            {...form.getInputProps("role")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <Select
            label="Department"
            placeholder="Pick One Department"
            searchable
            data={
              data
                ? data.items.map((x) => ({
                    label: x.name,
                    value: x.id,
                  }))
                : [""]
            }
            {...form.getInputProps("departmentId")}
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
