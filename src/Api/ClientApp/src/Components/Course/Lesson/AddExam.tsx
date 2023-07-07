import {
  Button,
  Grid,
  Group,
  Loader,
  NumberInput,
  Paper,
  Switch,
  Textarea,
  TextInput,
  Tooltip,
} from "@mantine/core";
import { DatePicker, TimeInput } from "@mantine/dates";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { LessonType } from "@utils/enums";
import { getDateTime } from "@utils/getDateTime";
import errorType from "@utils/services/axiosError";
import {
  useCreateLesson,
  useGetCourseLesson,
  useUpdateLesson,
} from "@utils/services/courseService";
import { ILessonMCQ } from "@utils/services/types";
import moment from "moment";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import * as Yup from "yup";
import useFormErrorHooks from "@hooks/useFormErrorHooks";

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    name: Yup.string().required(t("exam_name_required") as string),

    startDate: Yup.date()
      .required(t("start_date_required") as string)
      .typeError(t("start_date_required") as string),
    endDate: Yup.date()
      .required(t("end_date_required") as string)
      .typeError(t("start_date_required") as string),
    questionMarking: Yup.string().required(
      t("question_weightage_required") as string
    ),
    startTime: Yup.string()
      .required(t("start_time_not_empty") as string)
      .typeError(t("start_time_required") as string),
    endTime: Yup.string()
      .required(t("end_time_not_empty") as string)
      .typeError(t("end_time_required") as string),
    duration: Yup.number()
      .required(t("duration_required") as string)
      .min(1, t("exam_duration_atleast_one") as string),
  });
};
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

const AddExam = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setIsEditing,
}: {
  setAddState: Function;
  item?: ILessonMCQ;
  isEditing?: boolean;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const { id: slug } = useParams();
  const navigate = useNavigate();
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(slug as string);
  const { t } = useTranslation();
  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );

  const startDateTime = item?.questionSet?.startTime
    ? moment(item?.questionSet?.startTime + "z")
        .local()
        .toDate()
    : new Date();
  const endDateTime = item?.questionSet?.endTime
    ? moment(item?.questionSet?.endTime + "z")
        .local()
        .toDate()
    : new Date();

  const form = useForm({
    initialValues: {
      name: item?.name ?? "",
      description: item?.questionSet?.description ?? "",
      negativeMarking: item?.questionSet?.negativeMarking ?? 0,
      questionMarking: item?.questionSet?.questionMarking ?? 1,
      passingWeightage: item?.questionSet?.passingWeightage ?? 0,
      allowedRetake: item?.questionSet?.allowedRetake ?? 0,
      duration: item?.duration ? item?.duration / 60 : 1,
      endDate: endDateTime,
      endTime: endDateTime,
      startTime: startDateTime,
      startDate: startDateTime,
      isMandatory: item?.isMandatory ?? false,
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const handleSubmit = async (values: any) => {
    try {
      if (!isEditing) {
        const response: any = await lesson.mutateAsync({
          questionSet: strippedFormValue(values),
          sectionIdentity: sectionId,
          courseId: slug,
          type: LessonType.Exam,
          name: values.name,
          isMandatory: values.isMandatory,
        } as ILessonMCQ);
        navigate("questions/" + response?.data?.slug);
      } else {
        await updateLesson.mutateAsync({
          questionSet: strippedFormValue(values),
          sectionIdentity: sectionId,
          lessonIdentity: item?.id,
          courseId: slug,
          type: LessonType.Exam,
          name: values.name,
          isMandatory: values.isMandatory,
        } as ILessonMCQ);
        setIsEditing(false);
      }
      showNotification({
        title: t("success"),
        message: `${t("capital_lesson")} ${
          isEditing ? t("edited") : t("added")
        } ${t("successfully")}`,
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
        title: t("error"),
      });
    }
  };

  return (
    <form onSubmit={form.onSubmit(handleSubmit)}>
      <Paper withBorder p="md">
        <Grid align={"center"}>
          <Grid.Col span={12} xs={6} lg={4}>
            <TextInput
              autoFocus
              withAsterisk
              label={t("exam_title")}
              placeholder={t("exam_title") as string}
              name="title"
              {...form.getInputProps("name")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              label={t("passing_percentage")}
              max={100}
              min={0}
              placeholder={t("question_passing_percentage") as string}
              {...form.getInputProps("passingWeightage")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              withAsterisk
              label={t("question_weightage")}
              min={1}
              defaultValue={1}
              placeholder={t("question_weightage") as string}
              {...form.getInputProps("questionMarking")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <DatePicker
              placeholder={t("pick_date") as string}
              withAsterisk
              label={t("Start date")}
              minDate={moment(new Date()).toDate()}
              {...form.getInputProps("startDate")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <TimeInput
              label={t("start_time")}
              format="12"
              clearable
              withAsterisk
              {...form.getInputProps("startTime")}
            />
          </Grid.Col>

          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              label={t("duration_minutes")}
              placeholder={t("duration") as string}
              min={1}
              withAsterisk
              {...form.getInputProps("duration")}
            />
          </Grid.Col>

          <Grid.Col span={12} xs={6} lg={4}>
            <DatePicker
              placeholder={t("pick_date") as string}
              label={t("End date")}
              withAsterisk
              minDate={form.values?.startDate}
              {...form.getInputProps("endDate")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <TimeInput
              label={t("end_time")}
              format="12"
              clearable
              withAsterisk
              {...form.getInputProps("endTime")}
            />
          </Grid.Col>

          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              label={t("negative_marking")}
              placeholder={t("negative_marking") as string}
              min={0}
              step={0.05}
              precision={2}
              max={100}
              {...form.getInputProps("negativeMarking")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              label={t("number_retake")}
              placeholder={t("retakes") as string}
              min={1}
              {...form.getInputProps("allowedRetake")}
            />
          </Grid.Col>

          <Tooltip
            multiline
            label="Toggle this option to enforce mandatory completion of this lesson for trainees."
            width={220}
          >
            <Grid.Col span={6} lg={4}>
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

          <Grid.Col>
            <Textarea
              label={t("Description")}
              placeholder={t("exam_description") as string}
              {...form.getInputProps("description")}
            />
          </Grid.Col>
        </Grid>

        <Group position="left" mt="md">
          <Button
            type="submit"
            loading={updateLesson.isLoading || lesson.isLoading}
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
                navigate("questions/" + item?.slug);
              }}
            >
              {t("add_more_questions")}
            </Button>
          )}
        </Group>
      </Paper>
    </form>
  );
};

export default AddExam;
