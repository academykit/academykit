import DeleteModal from "@components/Ui/DeleteModal";
import ThumbnailEditor from "@components/Ui/ThumbnailEditor";
import { Button, Group, Paper, Text, TextInput } from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  IGetSignature,
  useAddSignature,
  useDeleteSignature,
  useEditSignature,
} from "@utils/services/courseService";
import { useState } from "react";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const [FormProvider, useFormContext, useForm] =
  createFormContext<IGetSignature>();

const schema = Yup.object().shape({
  fullName: Yup.string().required("Signature name is required."),
  designation: Yup.string().required("Designation is required."),
  fileUrl: Yup.string().required("File Url is required!"),
});

const CreateSignature = ({
  data,
  onClose,
}: {
  data?: IGetSignature;
  onClose?: () => void;
}) => {
  const { id } = useParams();
  const createCertificate = useAddSignature(id as string);
  const deleteSignature = useDeleteSignature(id as string);
  const editSignature = useEditSignature(id as string);
  const [signatureUrl, setSignatureUrl] = useState(data?.fileUrl ?? "");
  const [confirmDelete, setConfirmDelete] = useToggle();

  const form = useForm({
    initialValues: data,
    validate: yupResolver(schema),
  });
  const edit = !!form.values.id;

  const handleSubmit = async (data: IGetSignature) => {
    try {
      await createCertificate.mutateAsync({ data, id: id as string });
      showNotification({
        title: "Success",
        message: "Signature added successfully!",
      });
      onClose && onClose();
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: "Error",
        message: err,
        color: "red",
      });
    }
  };

  const handelEdit = async (data: IGetSignature) => {
    try {
      await editSignature.mutateAsync({ data, id: id as string });
      showNotification({
        title: "Success",
        message: "Signature edited successfully!",
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: "Error",
        message: err,
        color: "red",
      });
    }
  };
  const handleDelete = async (sigId: string) => {
    try {
      await deleteSignature.mutateAsync({ id: id as string, sigId });
      showNotification({
        title: "Success",
        message: "Signature deleted successfully!",
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: "Error",
        message: err,
        color: "red",
      });
    }
    setConfirmDelete();
  };

  return (
    <FormProvider form={form}>
      {data && (
        <DeleteModal
          title={`Are you sure you want to delete?`}
          open={confirmDelete}
          onClose={setConfirmDelete}
          onConfirm={() => handleDelete(data.id)}
        />
      )}

      <form onSubmit={form.onSubmit(edit ? handelEdit : handleSubmit)}>
        <Paper p={10} style={{ marginBottom: "20px" }} withBorder>
          <Group noWrap mb={10}>
            <TextInput
              w={"100%"}
              label="Name"
              placeholder="Enter name"
              withAsterisk
              {...form.getInputProps(`fullName`)}
            />
            <TextInput
              w={"100%"}
              ml={5}
              label="Designation"
              withAsterisk
              placeholder="Enter the designation"
              {...form.getInputProps(`designation`)}
            />
          </Group>
          <Text size={"sm"}>
            Signature <sup style={{ color: "red" }}>*</sup>
          </Text>
          <ThumbnailEditor
            formContext={useFormContext}
            label="Signature"
            FormField={`fileUrl`}
            currentThumbnail={signatureUrl}
            width="48.5%"
          />
          <Group mt={30}>
            <Button
              loading={createCertificate.isLoading || editSignature.isLoading}
              type="submit"
            >
              {edit ? "Save Edit" : "Add"}
            </Button>
            {edit ? (
              <Button
                onClick={() => setConfirmDelete()}
                type="reset"
                variant="outline"
              >
                Delete
              </Button>
            ) : (
              <Button onClick={onClose} type="reset" variant="outline">
                Cancel
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </FormProvider>
  );
};

export default CreateSignature;
