import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
import RichTextEditor from "@components/Ui/RichTextEditor/Index";
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
import { ILessonExternalUrl } from "@utils/services/types";
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const AddExternalUrl = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setIsEditing,
  setAddLessonClick,
}: {
  setAddState: (s: string) => void;
  item?: ILessonExternalUrl;
  isEditing?: boolean;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
  setAddLessonClick: (b: boolean) => void;
}) => {
  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );
  const { id: slug } = useParams();
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(slug as string);
  const { t } = useTranslation();

  const schema = () => {
    return Yup.object().shape({
      name: Yup.string().required(t("lesson_name_required") as string),
      externalUrl: Yup.string()
        .url(t("url_invalid") as string)
        .required(t("externalUrl_required") as string),
    });
  };

  const form = useForm({
    initialValues: {
      name: item?.name ?? "",
      externalUrl: item?.externalUrl ?? "",
      description: item?.description ?? "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const submitForm = async (values: {
    name: string;
    externalUrl: string;
    description: string;
  }) => {
    try {
      const data = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.ExternalUrl,
        ...values,
        isMandatory,
      };
      if (!isEditing) {
        await lesson.mutateAsync(data as ILessonExternalUrl);
        form.reset();
      } else {
        await updateLesson.mutateAsync({
          ...data,
          lessonIdentity: item?.id,
        } as ILessonExternalUrl);
        setIsEditing(false);
      }
      showNotification({
        title: t("success"),
        message: `${t("external_URL")} ${isEditing ? t("edited") : t("added")} ${t(
          "successfully"
        )}`,
      });
      setAddLessonClick(true);
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
            <Grid.Col span={{ base: 12, lg: 6 }}>
              <CustomTextFieldWithAutoFocus
                withAsterisk
                label={t("URL")}
                placeholder={t("externalUrl") as string}
                {...form.getInputProps("externalUrl")}
              />
            </Grid.Col>
          </Grid>
          <Box my={20}>
            <Text size={"sm"}>{t("external_url_description")}</Text>
            <RichTextEditor
              placeholder={t("external_url_description") as string}
              {...form.getInputProps("description")}
            />
          </Box>
          <Group mt="md">
            <Button
              type="submit"
              loading={lesson.isPending || updateLesson.isPending}
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
          </Group>
        </Paper>
      </form>
    </React.Fragment>
  );
};

export default AddExternalUrl;
