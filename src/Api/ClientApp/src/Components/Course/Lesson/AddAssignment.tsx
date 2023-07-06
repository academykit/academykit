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
import { DatePicker, TimeInput } from "@mantine/dates";
import { IconCalendar } from "@tabler/icons";
import { getDateTime } from "@utils/getDateTime";
import moment from "moment";
import { useTranslation } from "react-i18next";
import useFormErrorHooks from "@hooks/useFormErrorHooks";

const strippedFormValue = (value: any) => {
  const val = { ...value };
  delete val.isMandatory;
  delete val.isRequired;
  const startTime = getDateTime(val.startDate, val.startTime);
  const endTime = getDateTime(val.endDate, val.endTime);
  val.startTime = startTime.utcDateTime;
  val.endTime = endTime.utcDateTime;
  delete val.startDate;
  delete val.endDate;

  return val;
};

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t("assignment_title_required") as string),
    startTime: Yup.date()
      .required(t("start_time_required") as string)
      .typeError(t("start_time_required") as string),
    eventStartDate: Yup.date()
      .required(t("start_date_required") as string)
      .typeError(t("start_date_required") as string),
    eventEndDate: Yup.date()
      .required(t("end_date_required") as string)
      .typeError(t("end_date_required") as string),
    endTime: Yup.date()
      .required(t("end_time_required") as string)
      .typeError(t("end_time_required") as string),
  });
};

interface SubmitType {
  name: string;
  description: string;
  isMandatory?: boolean;
  eventStartDate?: Date;
  eventEndDate?: Date;
  startTime?: Date;
  endTime?: Date;
}

const AddAssignment = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setIsEditing,
}: {
  setAddState: Function;
  item?: ILessonAssignment;
  isEditing?: boolean;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const { id: slug } = useParams();
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(slug as string);
  const { t } = useTranslation();

  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );

  const [opened, setOpened] = useState(false);
  const [lessonId, setLessonId] = useState("");

  const startDateTime = item?.startDate
    ? moment(item?.startDate + "z")
        .local()
        .toDate()
    : new Date();

  const endDateTime = item?.endDate
    ? moment(item?.endDate + "z")
        .local()
        .toDate()
    : new Date();

  const form = useForm({
    initialValues: {
      name: item?.name ?? "",
      description: item?.description ?? "",
      isMandatory: item?.isMandatory ?? false,
      eventStartDate: startDateTime ?? new Date(),
      eventEndDate: endDateTime ?? new Date(),
      endTime: endDateTime ?? new Date(),
      startTime: startDateTime ?? new Date(),
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const submitForm = async (values: SubmitType) => {
    const val = { ...values };
    delete val.eventEndDate;
    delete val.endTime;
    delete val.eventStartDate;
    delete val.startTime;

    const startDate =
      values?.eventStartDate &&
      getDateTime(
        values?.eventStartDate?.toString() ?? "",
        values?.startTime?.toString() ?? ""
      );
    const endDate =
      values?.eventEndDate &&
      getDateTime(
        values?.eventEndDate?.toString() ?? "",
        values?.endTime?.toString() ?? ""
      );
    try {
      let assignmentData = {
        courseId: slug,
        sectionIdentity: sectionId,
        type: LessonType.Assignment,
        ...val,
        startDate: startDate && startDate.utcDateTime,
        endDate: endDate && endDate.utcDateTime,
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
        setIsEditing(false);
      }
      showNotification({
        title: t("success"),
        message: `${t("assignment")} ${
          isEditing ? t("edited") : t("added")
        } ${t("successfully")}`,
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
        {opened && <CreateAssignment lessonId={lessonId} />}
      </Modal>

      <form onSubmit={form.onSubmit(submitForm)}>
        <Paper withBorder p="md">
          <Grid align={"center"} justify="space-around">
            <Grid.Col span={12} lg={8}>
              <TextInput
                autoFocus
                label={t("assignment_title")}
                placeholder={t("assignment_title") as string}
                withAsterisk
                {...form.getInputProps("name")}
              />
            </Grid.Col>
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

            <Grid.Col span={6}>
              <DatePicker
                w={"100%"}
                placeholder={t("pick_start_date") as string}
                label={t("start_date")}
                icon={<IconCalendar size={16} />}
                minDate={moment(new Date()).toDate()}
                withAsterisk
                {...form.getInputProps("eventStartDate")}
              />
            </Grid.Col>
            <Grid.Col span={6}>
              <TimeInput
                label={t("start_time")}
                format="12"
                withAsterisk
                clearable
                {...form.getInputProps("startTime")}
              />
            </Grid.Col>

            <Grid.Col span={6}>
              <DatePicker
                w={"100%"}
                placeholder={t("pick_end_date") as string}
                label={t("end_date")}
                minDate={form.values.eventStartDate}
                icon={<IconCalendar size={16} />}
                withAsterisk
                {...form.getInputProps("eventEndDate")}
              />
            </Grid.Col>
            <Grid.Col span={6}>
              <TimeInput
                label={t("end_time")}
                format="12"
                clearable
                withAsterisk
                {...form.getInputProps("endTime")}
              />
            </Grid.Col>
            <Grid.Col>
              <Textarea
                placeholder={t("assignment_description") as string}
                label={t("assignment_description")}
                mb={10}
                {...form.getInputProps("description")}
              />
            </Grid.Col>
          </Grid>
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
                  setLessonId(item?.id || "");
                  setOpened(true);
                }}
              >
                {t("add_more_questions")}
              </Button>
            )}
          </Group>
        </Paper>
      </form>
    </React.Fragment>
  );
};

export default AddAssignment;
