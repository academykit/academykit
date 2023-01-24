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
  name: Yup.string().required("Video Name is required."),
});

const [FormProvider, useFormContext, useForm] = createFormContext();

const AddLecture = ({
  setAddState,
  item,
  setAddLessonClick,
  isEditing,
  sectionId,
  setIsEditing,
}: {
  setAddState: Function;
  item?: ILessons;
  setAddLessonClick: Function;
  isEditing?: boolean;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const { id: slug } = useParams();
  const [videoUrl, setVideoUrl] = React.useState<string>(item?.videoUrl ?? "");
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(slug as string);

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
      setIsEditing(false);
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
                label="Video Name"
                placeholder="Video Name"
                withAsterisk
                {...form.getInputProps("name")}
              />
            </Grid.Col>
            <Grid.Col span={4}>
              <Switch
                label="Is Mandatory"
                {...form.getInputProps("isMandatory")}
              />
            </Grid.Col>
          </Grid>
          <Text size={"sm"} mt={10}>
            Video <span style={{ color: "red" }}>*</span>
          </Text>
          <LessonVideoUpload
            setUrl={setVideoUrl}
            currentVideo={videoUrl}
            marginy={1}
          />
          <Textarea
            placeholder="Video's Description"
            label="Video Description"
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
    </FormProvider>
  );
};

export default AddLecture;
