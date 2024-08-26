import AvatarEditor from "@components/Ui/AvatarEditor";
import { DynamicAutoFocusTextField } from "@components/Ui/CustomTextFieldWithAutoFocus";
import RichTextEditor from "@components/Ui/RichTextEditor/Index";
import TextViewer from "@components/Ui/RichTextViewer";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Button,
  Divider,
  Grid,
  Paper,
  Text,
  TextInput,
  Title,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { PHONE_VALIDATION } from "@utils/constants";
import { useUpdateUser } from "@utils/services/adminService";
import { useReAuth } from "@utils/services/authService";
import errorType from "@utils/services/axiosError";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useSearchParams } from "react-router-dom";
import * as Yup from "yup";

export interface FormValues {
  email: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
  mobileNumber: string | null;
  profession: string | null;
  address: string;
  imageUrl: string | null;
  bio: string | null;
  role: number;
  isActive: boolean;
}

const [FormProvider, useFormContext, useForm] = createFormContext<FormValues>();

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    firstName: Yup.string().required(t("first_name_required") as string),
    lastName: Yup.string().required(t("last_name_required") as string),
    email: Yup.string()
      .email(t("invalid_email") as string)
      .required(t("email_required") as string),
    mobileNumber: Yup.string()
      .nullable()
      .notRequired()
      .matches(PHONE_VALIDATION, {
        message: t("enter_valid_phone"),
        excludeEmptyString: true,
      }),
    bio: Yup.string().test(
      "asdf",
      t("bio_character_limit") as string,
      function (value: any) {
        const a = document.createElement("div");
        a.innerHTML = value;
        return a.innerText.length <= 200;
      }
    ),
  });
};
const UserInfo = () => {
  const userId = localStorage.getItem("id");
  const { data, isSuccess } = useReAuth();
  const [imageURL, setImageURL] = useState(data?.imageUrl ?? "");
  // const [viewMode, setViewMode] = useState(true);
  const [params, setParams] = useSearchParams();
  const viewMode = params.get("edit") == "1" ? false : true;

  const createEditMode = (editView: "0" | "1") => {
    // 0 = view
    // 1 = edit
    params.set("edit", editView);
    setParams(params);
  };

  const formData = useForm({
    initialValues: {
      firstName: "",
      middleName: "",
      lastName: "",
      email: "",
      mobileNumber: "",
      profession: "",
      address: "",
      bio: "",
      imageUrl: "",
      role: 1,
      isActive: false,
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(formData);

  useEffect(() => {
    if (isSuccess) {
      formData.setValues({
        email: data?.email,
        firstName: data?.firstName,
        middleName: data?.middleName ?? "",
        lastName: data?.lastName,
        mobileNumber: data?.mobileNumber ?? "",
        profession: data?.profession ?? "",
        address: data?.address ?? "",
        bio: data?.bio ?? "",
        role: data?.role ?? 1,
        isActive: data?.isActive ?? false,
        imageUrl: data?.imageUrl ?? "",
      });
      setImageURL(data?.imageUrl ?? "");
    }
  }, [isSuccess, params]); // reset on edit/view mode

  const updateUser = useUpdateUser(userId as string);
  const navigator = useNavigate();
  const { t } = useTranslation();

  const handleSubmit = async (data: FormValues) => {
    try {
      const withImage = { ...data };
      withImage.imageUrl = imageURL;
      await updateUser.mutateAsync({ id: userId as string, data });
      navigator(`/userProfile/${userId as string}`);
      showNotification({
        title: t("successful"),
        message: t("update_profile_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, title: t("error"), color: "red" });
    }
  };

  return (
    <Paper shadow={"xl"} radius="md" p="xl" withBorder>
      <Title>{t("profile_section")}</Title>
      <Divider mb={10} />
      <Text variant="text" size={"xl"}>
        {t("introduction")}
      </Text>
      <div style={{ marginBottom: "3px" }}>{t("recognize_you")}</div>
      <FormProvider form={formData}>
        <form onSubmit={formData.onSubmit(handleSubmit)}>
          <AvatarEditor
            url={imageURL}
            label={t("image") as string}
            formContext={useFormContext}
            disabled={viewMode}
          />

          <Grid>
            <Grid.Col span={{ xs: 6, lg: 4 }}>
              <DynamicAutoFocusTextField
                isViewMode={viewMode}
                readOnly={viewMode}
                withAsterisk
                label={t("firstname") as string}
                placeholder={t("your_firstname") as string}
                name="firstName"
                {...formData.getInputProps("firstName")}
              />
            </Grid.Col>
            <Grid.Col span={{ xs: 6, lg: 4 }} mt={7}>
              <TextInput
                styles={{ input: { border: viewMode ? "none" : "" } }}
                readOnly={viewMode}
                label={t("middleName") as string}
                name="middleName"
                placeholder={t("your_middleName") as string}
                {...formData.getInputProps("middleName")}
              />
            </Grid.Col>
            <Grid.Col span={{ xs: 6, lg: 4 }}>
              <TextInput
                styles={{ input: { border: viewMode ? "none" : "" } }}
                readOnly={viewMode}
                withAsterisk
                label={t("lastName")}
                name="lastName"
                placeholder={t("your_lastName") as string}
                {...formData.getInputProps("lastName")}
              />
            </Grid.Col>
            <Grid.Col span={{ xs: 6, lg: 4 }}>
              <TextInput
                styles={{ input: { border: viewMode ? "none" : "" } }}
                withAsterisk
                disabled
                label={t("email")}
                type="email"
                name="email"
                placeholder={t("your_email") as string}
                {...formData.getInputProps("email")}
              />
            </Grid.Col>
            <Grid.Col span={{ xs: 6, lg: 4 }} mt={7}>
              <TextInput
                styles={{ input: { border: viewMode ? "none" : "" } }}
                readOnly={viewMode}
                name="mobileNumber"
                label={t("mobileNumber")}
                placeholder={t("your_number") as string}
                {...formData.getInputProps("mobileNumber")}
              />
            </Grid.Col>
            <Grid.Col span={{ xs: 6, lg: 4 }} mt={7}>
              <TextInput
                styles={{ input: { border: viewMode ? "none" : "" } }}
                readOnly={viewMode}
                label={t("profession")}
                name="profession"
                placeholder={t("your_profession") as string}
                {...formData.getInputProps("profession")}
              />
            </Grid.Col>
            <Grid.Col span={{ xs: 6, lg: 4 }}>
              <TextInput
                styles={{ input: { border: viewMode ? "none" : "" } }}
                readOnly={viewMode}
                label={t("address")}
                placeholder={t("your_address") as string}
                {...formData.getInputProps("address")}
              />
            </Grid.Col>
            <Grid.Col span={{ xs: 12, lg: 12 }}>
              <Text size="sm">{t("bio")}</Text>
              {!viewMode && (
                <RichTextEditor
                  error={formData.errors?.bio}
                  placeholder={t("your_short_description") as string}
                  {...formData.getInputProps("bio")}
                />
              )}
              {viewMode && <TextViewer content={data?.bio as string} />}
            </Grid.Col>
            <Grid.Col span={{ lg: 12 }}>
              {viewMode && (
                <Button onClick={() => createEditMode("1")} type="button">
                  {t("edit")}
                </Button>
              )}

              {!viewMode && (
                <>
                  <Button
                    loading={updateUser.isPending}
                    type="submit"
                    style={{ marginRight: "10px" }}
                  >
                    {t("save")}
                  </Button>
                  <Button onClick={() => createEditMode("0")} variant="outline">
                    {t("cancel")}
                  </Button>
                </>
              )}
            </Grid.Col>
          </Grid>
        </form>
      </FormProvider>
    </Paper>
  );
};

export default UserInfo;
