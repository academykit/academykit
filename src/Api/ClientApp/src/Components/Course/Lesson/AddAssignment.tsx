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
import { ILessonAssignment } from "@utils/services/types";
import React, { useState } from "react";
import { useParams } from "react-router-dom";
import CreateAssignment from "@pages/assignment/create";
import errorType from "@utils/services/axiosError";
import * as Yup from "yup";

const schema = Yup.object().shape({
  name: Yup.string().required("Assignment's Title is required."),
  description: Yup.string().required("Assignment's Description is required."),
});

const AddAssignment = ({
  setAddState,
  item,
  isEditing,
  sectionId,
}: {
  setAddState: Function;
  item?: ILessonAssignment;
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
      isMandatory: item?.isMandatory ?? false,
    },
    validate: yupResolver(schema),
  });

  const submitForm = async (values: { name: string; description: string }) => {
    try {
      let assignmentData = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.Assignment,
        ...values,
        isMandatory,
      };
      if (!isEditing) {
        const response: any = await lesson.mutateAsync(
          assignmentData as ILessonAssignment
        );
        setLessonId(response?.data?.id);
        form.reset();
        setOpened(true);
      } else {
        await updateLesson.mutateAsync({
          ...assignmentData,
          lessonIdentity: item?.id,
        } as ILessonAssignment);
      }
      showNotification({
        title: "Success",
        message: `Assignment ${isEditing ? "Edited" : "Added"} successfully!`,
      });
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
      <Modal
        overflow="inside"
        opened={opened}
        // exitTransitionDuration={100}
        transition="slide-up"
        onClose={() => {
          setOpened(false);
          setAddState("");
        }}
        size="100%"
        style={{
          height: "100%",
        }}
        styles={{
          modal: {
            height: "100%",
            paddingBottom: 0,
          },
          inner: {
            paddingLeft: 0,
            paddingRight: 0,
            paddingBottom: 0,
            paddingTop: "100px",
            height: "100%",
          },
        }}
      >
        <CreateAssignment lessonId={lessonId} />
      </Modal>

      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Grid align={"center"} justify="space-around">
            <Grid.Col span={12} lg={8}>
              <TextInput
                label="Assignment Title"
                placeholder="Assignment's Title"
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
          <Textarea
            placeholder="Assignment's Description"
            label="Assignment Description"
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
            {isEditing && (
              <Button
                onClick={() => {
                  setLessonId(item?.id || "");
                  setOpened(true);
                }}
              >
                Add More Questions
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </React.Fragment>
  );
};

export default AddAssignment;
