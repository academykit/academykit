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
import { useTranslation } from "react-i18next";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import useCustomForm from "@hooks/useCustomForm";

const [FormProvider, useFormContext, useForm] =
  createFormContext<IGetSignature>();

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    fullName: Yup.string().required(t("signature_name_required") as string),
    designation: Yup.string().required(t("designation_required") as string),
    fileUrl: Yup.string().required(t("file_url_required") as string),
  });
};

const CreateSignature = ({
  data,
  onClose,
}: {
  data?: IGetSignature;
  onClose?: () => void;
}) => {
  const cForm = useCustomForm();
  const { id } = useParams();
  const createCertificate = useAddSignature(id as string);
  const deleteSignature = useDeleteSignature(id as string);
  const editSignature = useEditSignature(id as string);
  const [signatureUrl, setSignatureUrl] = useState(data?.fileUrl ?? "");
  const [confirmDelete, setConfirmDelete] = useToggle();
  const { t } = useTranslation();

  const form = useForm({
    initialValues: data,
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const edit = !!form.values.id;

  const handleSubmit = async (data: IGetSignature) => {
    try {
      await createCertificate.mutateAsync({ data, id: id as string });
      showNotification({
        title: t("success"),
        message: t("add_signature_success"),
      });
      onClose && onClose();
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: t("error"),
        message: err,
        color: "red",
      });
    }
  };
  const handelEdit = async (data: IGetSignature) => {
    try {
      await editSignature.mutateAsync({ data, id: id as string });
      showNotification({
        title: t("success"),
        message: t("edit_signature_success"),
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: t("error"),
        message: err,
        color: "red",
      });
    }
  };
  const handleDelete = async (sigId: string) => {
    try {
      await deleteSignature.mutateAsync({ id: id as string, sigId });
      onClose && onClose();
      showNotification({
        title: t("success"),
        message: t("delete_signature_success"),
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: t("error"),
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
          loading={deleteSignature.isLoading}
          title={t("sure_want_to_delete")}
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
              label={t("name")}
              placeholder={t("enter_name") as string}
              withAsterisk
              {...form.getInputProps(`fullName`)}
            />
            <TextInput
              w={"100%"}
              ml={5}
              label={t("designation")}
              withAsterisk
              placeholder={t("designation_placeholder") as string}
              {...form.getInputProps(`designation`)}
            />
          </Group>
          <Text size={"sm"}>
            {t("signature")} <sup style={{ color: "red" }}>*</sup>
          </Text>
          <ThumbnailEditor
            formContext={useFormContext}
            label={t("signature") as string}
            FormField={`fileUrl`}
            currentThumbnail={signatureUrl}
            width="48.5%"
          />
          <Group mt={30}>
            <Button
              disabled={!cForm?.isReady}
              loading={createCertificate.isLoading || editSignature.isLoading}
              type="submit"
            >
              {edit ? t("save_edit") : t("add")}
            </Button>
            {edit ? (
              <Button
                onClick={() => setConfirmDelete()}
                type="reset"
                variant="outline"
              >
                {t("delete")}
              </Button>
            ) : (
              <Button onClick={onClose} type="reset" variant="outline">
                {t("cancel")}
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </FormProvider>
  );
};

export default CreateSignature;
