import AvatarEditor from "@components/Ui/AvatarEditor";
import {
  Button,
  createStyles,
  Divider,
  Grid,
  Paper,
  Text,
  TextInput,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import RichTextEditor from "@mantine/rte";
import { PHONE_VALIDATION } from "@utils/constants";
import { useUpdateUser } from "@utils/services/adminService";
import { useReAuth } from "@utils/services/authService";
import errorType from "@utils/services/axiosError";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
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

const schema = Yup.object().shape({
  firstName: Yup.string().required("First Name is required."),
  lastName: Yup.string().required("Last Name is required."),
  email: Yup.string().email("Invalid Email").required("Email is required."),
  mobileNumber: Yup.string()
    .nullable()
    .notRequired()
    .matches(PHONE_VALIDATION, {
      message: "Please enter valid phone number.",
      excludeEmptyString: true,
    }),
});
const UserInfo = () => {
  const userId = localStorage.getItem("id");
  const { data, isLoading, isSuccess } = useReAuth();

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
    validate: yupResolver(schema),
  });
  const updateUser = useUpdateUser(userId as string);
  const [imageURL, setImageURL] = useState(data?.imageUrl ?? "");
  const navigator = useNavigate();
  const handleSubmit = async (data: FormValues) => {
    try {
      const withImage = { ...data };
      withImage.imageUrl = imageURL;
      await updateUser.mutateAsync({ id: userId as string, data: withImage });
      navigator(`/userProfile/${userId as string}`);
      showNotification({
        title: "Successful",
        message: "Profile Successfully Updated.",
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, title: "Error!", color: "red" });
    }
  };

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
  }, [isSuccess]);

  return (
    <Paper shadow={"xl"} radius="md" p="xl" withBorder>
      Profile Section
      <Divider mb={10} />
      <Text variant="text" size={"xl"}>
        Introduction
      </Text>
      <div style={{ marginBottom: "3px" }}>
        Let other learners and instructors recognize you.
      </div>
      <FormProvider form={formData}>
        <form onSubmit={formData.onSubmit(handleSubmit)}>
          <AvatarEditor
            formContext={useFormContext}
            url={imageURL}
            label={"image"}
            onUploadSuccess={(imageURLUp: string) => {
              setImageURL(imageURLUp);
              formData.setFieldValue("imageUrl", imageURLUp);
            }}
            onRemoveSuccess={() => {
              formData.setFieldValue("imageUrl", null);
            }}
          />

          <Grid>
            <Grid.Col xs={6} lg={4}>
              <TextInput
                withAsterisk
                label="First Name"
                placeholder="Your First Name"
                name="firstName"
                {...formData.getInputProps("firstName")}
              />
            </Grid.Col>
            <Grid.Col xs={6} lg={4}>
              <TextInput
                label="Middle Name"
                name="middleName"
                placeholder="Your Middle Name"
                {...formData.getInputProps("middleName")}
              />
            </Grid.Col>
            <Grid.Col xs={6} lg={4}>
              <TextInput
                withAsterisk
                label="Last Name"
                name="lastName"
                placeholder="Your Last Name"
                {...formData.getInputProps("lastName")}
              />
            </Grid.Col>
            <Grid.Col xs={6} lg={4}>
              <TextInput
                withAsterisk
                disabled
                label="Email"
                type="email"
                name="email"
                placeholder="Your Email Name"
                {...formData.getInputProps("email")}
              />
            </Grid.Col>
            <Grid.Col xs={6} lg={4}>
              <TextInput
                name="mobileNumber"
                label="Mobile Number"
                placeholder="Your Phone number"
                {...formData.getInputProps("mobileNumber")}
              />
            </Grid.Col>
            <Grid.Col xs={6} lg={4}>
              <TextInput
                label="Profession"
                name="profession"
                placeholder="Your Profession"
                {...formData.getInputProps("profession")}
              />
            </Grid.Col>
            <Grid.Col xs={6} lg={4}>
              <TextInput
                label="Address"
                placeholder="Your Address "
                {...formData.getInputProps("address")}
              />
            </Grid.Col>
            <Grid.Col xs={12} lg={12}>
              <Text size="sm">Bio</Text>
              <RichTextEditor
                placeholder="Your bio "
                {...formData.getInputProps("bio")}
              />
            </Grid.Col>
            <Grid.Col lg={12}>
              <Button loading={updateUser.isLoading} type="submit">
                Save
              </Button>
            </Grid.Col>
          </Grid>
        </form>
      </FormProvider>
    </Paper>
  );
};

export default UserInfo;
