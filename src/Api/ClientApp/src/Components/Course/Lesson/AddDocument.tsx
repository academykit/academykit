import {
  Button,
  Grid,
  Group,
  Text,
  Paper,
  Switch,
  Textarea,
  TextInput,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { LessonType } from "@utils/enums";
import {
  useCreateLesson,
  useUpdateLesson,
} from "@utils/services/courseService";
import { ILessonAssignment, ILessonFile } from "@utils/services/types";
import React, { useState } from "react";
import { useParams } from "react-router-dom";
import errorType from "@utils/services/axiosError";
import * as Yup from "yup";
import CreateFeedback from "../FeedBack/CreateFeedBack";
import FileUploadLesson from "@components/Ui/FileUploadLesson";
import { useTranslation } from "react-i18next";
import useFormErrorHooks from "@hooks/useFormErrorHooks";

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t("file_name_required") as string),
    documentUrl: Yup.string().required(t("file_required") as string),
  });
};

const [FormProvider, useFormContext, useForm] = createFormContext();

const AddDocument = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setAddLessonClick,
  setIsEditing,
}: {
  setAddState: Function;
  item?: ILessonAssignment;
  setAddLessonClick: Function;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
  isEditing?: boolean;
  sectionId: string;
}) => {
  const { id: slug } = useParams();
  const lesson = useCreateLesson(slug as string);

  const updateLesson = useUpdateLesson(
    // item?.courseId || "",
    // item?.id,
    slug as string
  );

  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );

  const [opened, setOpened] = useState(false);
  const [lessonId, setLessonId] = useState("");

  const form = useForm({
    initialValues: {
      name: item?.name ?? "",
      description: item?.description ?? "",
      documentUrl: item?.documentUrl ?? "",
      isMandatory: item?.isMandatory ?? false,
    },
    validate: yupResolver(schema),
  });
  useFormErrorHooks(form);

  const submitForm = async (values: any) => {
    try {
      let fileData = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.Document,
        ...values,
        isMandatory,
      };
      if (!isEditing) {
        const response: any = await lesson.mutateAsync(fileData as ILessonFile);
        setLessonId(response?.data?.id);
        form.reset();
        setOpened(true);
      } else {
        await updateLesson.mutateAsync({
          ...fileData,
          lessonIdentity: item?.id,
        } as ILessonFile);
        setIsEditing(false);
      }
      showNotification({
        title: "Success",
        message: `File ${isEditing ? "Edited" : "Added"} successfully!`,
      });
      setAddLessonClick(true);
    } catch (error: any) {
      const err = errorType(error);

      showNotification({
        title: "Error!",
        message: err,
        color: "red",
      });
    }
  };

  return (
    <FormProvider form={form}>
      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Grid align={"center"} justify="space-around">
            <Grid.Col span={12} lg={8}>
              <TextInput
                withAsterisk
                label="File Title"
                placeholder="File Name"
                {...form.getInputProps("name")}
              />
            </Grid.Col>
            <Grid.Col span={4}>
              <Switch
                label="Is Mandatory"
                {...form.getInputProps("isMandatory")}
                checked={isMandatory}
                onChange={() => {
                  setIsMandatory(() => !isMandatory);
                  form.setFieldValue("isMandatory", !isMandatory);
                }}
              />
            </Grid.Col>
          </Grid>
          <Text size={"sm"} mt={10}>
            File <span style={{ color: "red" }}>*</span>
          </Text>
          <FileUploadLesson
            currentFile={item?.documentUrl}
            formContext={useFormContext}
          />
          <Textarea
            placeholder="File's Description"
            label="File Description"
            my={form.errors["documentUrl"] ? 20 : 10}
            {...form.getInputProps("description")}
          />
          <Group position="left" mt="md">
            <Button
              type="submit"
              loading={lesson.isLoading || updateLesson.isLoading}
            >
              Submit
            </Button>
            {!isEditing && (
              <Button
                onClick={() => {
                  setAddState("");
                }}
                variant="outline"
              >
                Close
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </FormProvider>
  );
};

export default AddDocument;
