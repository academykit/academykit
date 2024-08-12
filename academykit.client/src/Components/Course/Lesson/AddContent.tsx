import TextEditorExtended from "@components/Ui/RichTextEditor/Extended";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import { Box, Button, Group, Paper, Text } from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import {
  useCreateLesson,
  useUpdateLesson,
} from "@utils/services/courseService";
import { ILessonFeedback } from "@utils/services/types";
import React from "react";
import { useTranslation } from "react-i18next";
import { Link, useParams } from "react-router-dom";
import * as Yup from "yup";

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t("feedback_name_required") as string),
    url: Yup.string().required(t("url") as string),
  });
};

const AddContent = ({
  setAddState,
  item,
  isEditing,
}: {
  setAddState: (s: string) => void;
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

  const form = useForm({
    initialValues: {
      name: item?.name ?? "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const submitForm = async () => {};
  return (
    <React.Fragment>
      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Box my={20}>
            <Text size={"sm"}>{t("external_url_description")}</Text>
            <TextEditorExtended
              placeholder={t("external_url_description") as string}
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
