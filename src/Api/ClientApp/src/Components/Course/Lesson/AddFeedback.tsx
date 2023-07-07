import React, { useState } from "react";
import {
  Button,
  Grid,
  Group,
  Modal,
  Paper,
  ScrollArea,
  Switch,
  Textarea,
  TextInput,
  Tooltip,
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
import useFormErrorHooks from "@hooks/useFormErrorHooks";

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t("feedback_name_required") as string),
  });
};

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
  const { t } = useTranslation();

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
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

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
        message: `${t("feedback")} ${isEditing ? t("edited") : t("added")} ${t(
          "successfully"
        )}`,
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
  return (
    <React.Fragment>
      <Modal
        scrollAreaComponent={ScrollArea.Autosize}
        opened={opened}
        // exitTransitionDuration={100}
        transitionProps={{
          transition: "slide-up",
        }}
        onClose={() => {
          setOpened(false);
          setAddState("");
        }}
        size="100%"
        style={{
          height: "100%",
        }}
        styles={{
          inner: {
            paddingLeft: 0,
            paddingRight: 0,
            paddingBottom: 0,
            paddingTop: "100px",
            height: "100%",
          },
        }}
      >
        {opened && <CreateFeedback lessonId={lessonId ?? item?.id} />}
      </Modal>

      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Grid align={"center"} justify="space-around">
            <Grid.Col span={12} lg={8}>
              <TextInput
                withAsterisk
                label={t("feedback_title")}
                placeholder={t("feedback_title") as string}
                {...form.getInputProps("name")}
              />
            </Grid.Col>
            <Tooltip
              multiline
              label="Toggle this option to enforce mandatory completion of this lesson for trainees."
              width={220}
            >
              <Grid.Col span={4}>
                <Switch
                  label={t("is_mandatory")}
                  {...form.getInputProps("isMandatory")}
                  checked={isMandatory}
                  onChange={() => {
                    setIsMandatory(() => !isMandatory);
                    form.setFieldValue("isMandatory", !isMandatory);
                  }}
                />
              </Grid.Col>
            </Tooltip>
          </Grid>
          <Textarea
            placeholder={t("feedback_description") as string}
            label={t("feedback_description")}
            mb={10}
            {...form.getInputProps("description")}
          />
          <Group position="left" mt="md">
            <Button
              type="submit"
              loading={lesson.isLoading || updateLesson.isLoading}
            >
              {t("submit")}
            </Button>
            {!isEditing && (
              <Button
                onClick={() => {
                  setAddState("");
                }}
                variant="outline"
              >
                {t("close")}
              </Button>
            )}
            {isEditing && (
              <Button
                onClick={() => {
                  setLessonId(item?.id ?? "");
                  setOpened(true);
                }}
              >
                {t("add_more_feedback")}
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </React.Fragment>
  );
};

export default AddFeedback;
