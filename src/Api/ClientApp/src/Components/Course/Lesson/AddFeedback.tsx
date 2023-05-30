import React, { useState } from "react";
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
import { ILessonFeedback } from "@utils/services/types";
import { useParams } from "react-router-dom";
import errorType from "@utils/services/axiosError";
import * as Yup from "yup";
import CreateFeedback from "../FeedBack/CreateFeedBack";
import { useTranslation } from "react-i18next";

const schema = Yup.object().shape({
  name: Yup.string().required("Feedback Name is required."),
});

const AddFeedback = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setIsEditing,
}: {
  setAddState: Function;
  item?: ILessonFeedback;
  isEditing?: boolean;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
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
        type: LessonType.Feedback,
        ...values,
        isMandatory,
      };
      if (!isEditing) {
        const response: any = await lesson.mutateAsync(
          assignmentData as ILessonFeedback
        );
        setLessonId(response?.data?.id);
        form.reset();
        setOpened(true);
      } else {
        await updateLesson.mutateAsync({
          ...assignmentData,
          lessonIdentity: item?.id,
        } as ILessonFeedback);
        setIsEditing(false);
      }
      showNotification({
        title: t("success"),
        message: `${t("feedback")} ${isEditing ? t("edited") : t("added")} ${t("successfully")}`,
      });
    } catch (error: any) {
      const err = errorType(error);

      showNotification({
        title: t("error"),
        message: err,
        color: "red",
      });
    }
  };
const {t}= useTranslation();
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
        <CreateFeedback lessonId={lessonId ?? item?.id} />
      </Modal>

      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Grid align={"center"} justify="space-around">
            <Grid.Col span={12} lg={8}>
              <TextInput
                withAsterisk
                label="Feedback Title"
                placeholder="FeedBack's Title"
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
            placeholder="Feedback's Description"
            label="Feedback Description"
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
            {isEditing && (
              <Button
                onClick={() => {
                  setLessonId(item?.id ?? "");
                  setOpened(true);
                }}
              >
                Add More Feedback
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </React.Fragment>
  );
};

export default AddFeedback;
