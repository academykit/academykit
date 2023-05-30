import { useEffect, useState } from "react";
import { Group, TextInput, Switch, Select, Button, Grid } from "@mantine/core";
import { UserRole } from "@utils/enums";
import { useDepartmentSetting } from "@utils/services/adminService";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import * as Yup from "yup";
import errorType from "@utils/services/axiosError";
import { IUserProfile } from "@utils/services/types";
import queryStringGenerator from "@utils/queryStringGenerator";
import { PHONE_VALIDATION } from "@utils/constants";
import { useTranslation } from "react-i18next";
import useFormErrorHooks from "@hooks/useFormErrorHooks";

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    email: Yup.string()
      .email(t("invalid_email") as string)
      .required(t("email_required") as string),
    firstName: Yup.string()
      .max(100, t("first_name_character_required") as string)
      .required(t("first_name_required") as string),
    lastName: Yup.string()
      .max(100, t("last_name_character_required") as string)
      .required("last_name_required"),
    middleName: Yup.string()
      .max(100, t("middle_name_character_required") as string)
      .nullable()
      .notRequired(),
    role: Yup.string()
      .oneOf(["1", "2", "3", "4"], t("role_required") as string)
      .required(t("role_required") as string),
    mobileNumber: Yup.string()
      .nullable()
      .matches(PHONE_VALIDATION, {
        message: t("enter_valid_phone"),
        excludeEmptyString: true,
      }),
  });
};

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
  const { t } = useTranslation();
  const form = useForm<IUserProfile>({
    initialValues: item,
    validate: yupResolver(schema),
  });
  useFormErrorHooks(form);

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
        message: isEditing ? t("user_edited_success") : t("user_added_success"),
        title: t("successful"),
      });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        title: t("error"),
        color: "red",
      });
    }
    setOpened(!opened);
  };

  return (
    <form onSubmit={form.onSubmit(onSubmitForm)}>
      <Grid align={"center"}>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            withAsterisk
            label={t("firstname")}
            placeholder={t("user_firstname") as string}
            name="firstName"
            {...form.getInputProps("firstName")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            label={t("middlename")}
            placeholder={t("user_middlename") as string}
            {...form.getInputProps("middleName")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            withAsterisk
            label={t("lastname")}
            placeholder={t("user_lastname") as string}
            {...form.getInputProps("lastName")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            withAsterisk
            label={t("email")}
            type="email"
            placeholder={t("user_email") as string}
            {...form.getInputProps("email")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            label={t("mobilenumber")}
            placeholder={t("user_phone_number") as string}
            {...form.getInputProps("mobileNumber")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <TextInput
            label={t("profession")}
            placeholder={t("user_profession") as string}
            {...form.getInputProps("profession")}
          />
        </Grid.Col>
        <Grid.Col xs={6} lg={4}>
          <Switch
            label={t("user_status")}
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
            error={t("user_role_pick")}
            label={t("user_role")}
            placeholder={t("user_role_pick") as string}
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
            label={t("department")}
            placeholder={t("pick_department") as string}
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
          {t("submit")}
        </Button>
      </Group>
    </form>
  );
};
export default AddUpdateUserForm;
