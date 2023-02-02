import React, { useEffect } from "react";
import { createFormContext, yupResolver } from "@mantine/form";
import { TextInput, Button, Textarea, Container, Text } from "@mantine/core";
import ThumbnailEditor from "@components/Ui/ThumbnailEditor";
import {
  useGeneralSetting,
  useUpdateGeneralSetting,
} from "@utils/services/adminService";
import { showNotification } from "@mantine/notifications";
import * as Yup from "yup";
import errorType from "@utils/services/axiosError";
import useAuth from "@hooks/useAuth";
import { PHONE_VALIDATION } from "@utils/constants";

const schema = Yup.object().shape({
  companyName: Yup.string().required("Company Name is required."),
  companyAddress: Yup.string().required("Company Address required."),
  companyContactNumber: Yup.string().nullable().matches(PHONE_VALIDATION, {
    message: "Please enter valid phone number.",
    excludeEmptyString: true,
  }),
  emailSignature: Yup.string().required("Signature is required."),
  logoUrl: Yup.string().required("Company Logo is required!"),
});

interface ICompanySettings {
  thumbnail?: string;
  companyName: string;
  companyAddress: string;
  companyContactNumber: string;
  emailSignature: string;
  logoUrl?: string | undefined;
}

const [FormProvider, useFormContext, useForm] =
  createFormContext<ICompanySettings>();

const GeneralSettings = () => {
  const generalSettings = useGeneralSetting();
  const updateGeneral = useUpdateGeneralSetting(generalSettings.data?.data.id);
  const data = generalSettings.data?.data;

  useEffect(() => {
    form.setValues({
      logoUrl: data?.logoUrl || "",
      companyName: data?.companyName || "",
      companyAddress: data?.companyAddress || "",
      companyContactNumber: data?.companyContactNumber || "",
      emailSignature: data?.emailSignature || "",
    });
  }, [generalSettings.isSuccess]);

  const form = useForm({
    initialValues: {
      logoUrl: "",
      companyName: "",
      companyAddress: "",
      companyContactNumber: "",
      emailSignature: "",
    },
    validate: yupResolver(schema),
  });

  const handleSubmit = async (values: any) => {
    try {
      await updateGeneral.mutateAsync(values);
      showNotification({
        message: "Settings updated successfully!",
      });
      window.scrollTo(0, 0);
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        color: "red",
      });
    }
  };

  return (
    <FormProvider form={form}>
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Container
          size={450}
          sx={{
            marginLeft: "0px",
          }}
        >
          Company Logo <sup style={{ color: "red" }}>*</sup>
          <ThumbnailEditor
            formContext={useFormContext}
            label="image"
            FormField="logoUrl"
            currentThumbnail={data?.logoUrl}
          />
          <Text c="dimmed" size="xs">
            Note : Image minimum resolution size should be 640 * 360
          </Text>
          <TextInput
            label="Company Name"
            withAsterisk
            mt={20}
            name="companyName"
            placeholder="Please enter your company name"
            {...form.getInputProps("companyName")}
          />
          <TextInput
            label="Company Address"
            withAsterisk
            name="companyAddress"
            placeholder="Please enter your company address"
            {...form.getInputProps("companyAddress")}
          />
          <TextInput
            label="Company Contact Number"
            withAsterisk
            type={"number"}
            name="ContactNumber"
            placeholder="Please enter your company contact number"
            {...form.getInputProps("companyContactNumber")}
          />
          <Textarea
            mt="md"
            label="Mail Signature"
            withAsterisk
            name="signature"
            placeholder="Your mail signature"
            {...form.getInputProps("emailSignature")}
          />
          <Button mt={10} type="submit" loading={updateGeneral.isLoading}>
            Submit
          </Button>
        </Container>
      </form>
    </FormProvider>
  );
};

export default GeneralSettings;
