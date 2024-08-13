import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
import TextEditorExtended from "@components/Ui/RichTextEditor/Extended";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Box,
  Button,
  Grid,
  Group,
  Paper,
  Switch,
  Text,
  Tooltip,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { LessonType } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  useCreateLesson,
  useUpdateLesson,
} from "@utils/services/courseService";
import { ILessonContent } from "@utils/services/types";
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useParams } from "react-router-dom";
import * as Yup from "yup";

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t("feedback_name_required") as string),
    content: Yup.string().required(t("content") as string),
  });
};

const AddContent = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setIsEditing,
}: {
  setAddState: (s: string) => void;
  item?: ILessonContent;
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

  const form = useForm({
    initialValues: {
      name: item?.name ?? "",
      content: item?.content ?? "",
      description: item?.description ?? "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const submitForm = async (values: {
    name: string;
    content: string;
    description: string;
  }) => {
    try {
      const data = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.Content,
        ...values,
        isMandatory,
      };
      if (!isEditing) {
        await lesson.mutateAsync(data as ILessonContent);
        form.reset();
      } else {
        await updateLesson.mutateAsync({
          ...data,
          lessonIdentity: item?.id,
        } as ILessonContent);
        setIsEditing(false);
      }
      showNotification({
        title: t("success"),
        message: `${t("content")} ${isEditing ? t("edited") : t("added")} ${t(
          "successfully"
        )}`,
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

  return (
    <React.Fragment>
      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Grid align={"center"}>
            <Grid.Col span={{ base: 12, lg: 6 }}>
              <CustomTextFieldWithAutoFocus
                withAsterisk
                label={t("lesson_name")}
                placeholder={t("lesson_name") as string}
                {...form.getInputProps("name")}
              />
            </Grid.Col>
            <Tooltip multiline label={t("mandatory_tooltip")} w={220}>
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
          <Box my={20}>
            <Text size={"sm"} mb={5}>
              {t("content")}
            </Text>
            <TextEditorExtended
              placeholder={t("content") as string}
              {...form.getInputProps("content")}
            />
          </Box>
          <Box my={20}>
            <Text size={"sm"} mb={5}>
              {t("description")}
            </Text>
            <TextEditorExtended
              placeholder={t("description") as string}
              {...form.getInputProps("description")}
            />
          </Box>
          <Group mt="md">
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
              <Button component={Link} to={`${item?.id}/feedback`}>
                {t("add_more_feedback")}
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </React.Fragment>
  );
};

export default AddContent;
