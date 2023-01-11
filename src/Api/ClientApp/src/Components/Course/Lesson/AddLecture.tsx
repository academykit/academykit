import React from "react";
import LessonVideoUpload from "@components/Ui/LessonVideoUpload";
import {
  Button,
  Grid,
  Group,
  Paper,
  Switch,
  Text,
  Textarea,
  TextInput,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { LessonType } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  ILessons,
  useCreateLesson,
  useUpdateLesson,
} from "@utils/services/courseService";
import { ILessonLecture } from "@utils/services/types";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const schema = Yup.object().shape({
  name: Yup.string().required("Lecture Name is required."),
  description: Yup.string().required("Lecture's Description is required"),
});

const [FormProvider, useFormContext, useForm] = createFormContext();

const AddLecture = ({
  setAddState,
  item,
  setAddLessonClick,
  isEditing,
  sectionId,
}: {
  setAddState: Function;
  item?: ILessons;
  setAddLessonClick: Function;
  isEditing?: boolean;
  sectionId: string;
}) => {
  const { id: slug } = useParams();
  const [videoUrl, setVideoUrl] = React.useState<string>(item?.videoUrl ?? "");
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(slug as string);

  const [isMandatory, setIsMandatory] = React.useState<EventTarget | boolean>(
    item?.isMandatory ?? false
  );

  const form = useForm({
    initialValues: {
      videoUrl: item?.videoUrl ?? "",
      name: item?.name ?? "",
      description: item?.description ?? "",
      isMandatory: item?.isMandatory ?? false,
    },
    validate: yupResolver(schema),
  });

  const handleSubmit = async (values: any) => {
    form.setFieldValue("videoUrl", videoUrl);
    try {
      if (isEditing) {
        await updateLesson.mutateAsync({
          courseId: slug as string,
          sectionIdentity: sectionId,
          lessonIdentity: item?.id,
          type: LessonType.Video,
          ...values,
          videoUrl,
        } as ILessonLecture);
      } else {
        await lesson.mutateAsync({
          courseId: slug as string,
          sectionIdentity: sectionId,
          type: LessonType.Video,
          ...values,
          videoUrl,
        } as ILessonLecture);
      }
      showNotification({
        title: "Success!",
        message: `Lesson ${isEditing ? "Edited" : "Added"} successfully`,
      });
      setAddLessonClick(true);
    } catch (error: any) {
      const err = errorType(error);
      showNotification({
        color: "red",
        title: "Error!",
        message: err,
      });
    }
  };

  return (
    <FormProvider form={form}>
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Paper withBorder p="md">
          <Grid align="center" justify={"space-around"}>
            <Grid.Col span={12} lg={8}>
              <TextInput
                sx={{ width: "100%" }}
                label="Lecture Name"
                placeholder="Lecture Name"
                withAsterisk
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
          <Text mt={10}>Lecture</Text>
          <LessonVideoUpload
            setUrl={setVideoUrl}
            currentVideo={videoUrl}
            marginy={1}
          />
          <Textarea
            placeholder="Lecture's Description"
            label="Lecture Description"
            mb={10}
            withAsterisk
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

export default AddLecture;
