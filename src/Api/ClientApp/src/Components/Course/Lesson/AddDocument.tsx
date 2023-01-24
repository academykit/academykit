import {
  Button,
  Grid,
  Group,
  Modal,
  Paper,
  Switch,
  Textarea,
  TextInput,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
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

const schema = Yup.object().shape({
  name: Yup.string().required("File Name is required."),
});

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
  const [fileUrl, setFileUrl] = React.useState<string>(item?.documentUrl ?? "");

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
      isMandatory: item?.isMandatory ?? false,
    },
    validate: yupResolver(schema),
  });

  const submitForm = async (values: { name: string; description: string }) => {
    try {
      let fileData = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.Document,
        ...values,
        isMandatory,
        documentUrl: fileUrl,
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
      }
      showNotification({
        title: "Success",
        message: `File ${isEditing ? "Edited" : "Added"} successfully!`,
      });
      setAddLessonClick(true);
      setIsEditing(false);
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
    <React.Fragment>
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
          <FileUploadLesson currentFile={fileUrl} setUrl={setFileUrl} />
          <Textarea
            placeholder="File's Description"
            label="File Description"
            mb={10}
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
    </React.Fragment>
  );
};

export default AddDocument;
