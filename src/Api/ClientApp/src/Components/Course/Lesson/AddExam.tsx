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
import { useNavigate, useParams } from "react-router-dom";
import * as Yup from "yup";

const schema = Yup.object().shape({
  name: Yup.string().required("Exam Name is required."),

  startDate: Yup.date()
    .required("Start Date is required.")
    .typeError("Start Date is required."),
  endDate: Yup.date()
    .required("End Date is required.")
    .typeError("End Date is required."),
  questionMarking: Yup.string().required("Question weightage is required."),
  startTime: Yup.string()
    .required("start time cannot be empty.")
    .typeError("Start Time is required."),
  endTime: Yup.string()
    .required("end time cannot be empty.")
    .typeError("End Time is required."),
  duration: Yup.number()
    .required("Duration is required.")
    .min(1, "Exam duration should at least be one."),
});

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
}: {
  setAddState: Function;
  item?: ILessonMCQ;
  isEditing?: boolean;
  sectionId: string;
}) => {
  const { id: slug } = useParams();
  const navigate = useNavigate();
  const lesson = useCreateLesson(slug as string);
  const updateLesson = useUpdateLesson(
    // item?.courseId || "",
    // item?.id,
    slug as string
  );

  const lessonDetails = useGetCourseLesson(
    item?.courseId || "",
    item?.id,
    isEditing
  );
  const [isMandatory, setIsMandatory] = useState<boolean>(false);
  const course = lessonDetails.data;

  useEffect(() => {
    if (lessonDetails.isSuccess && isEditing) {
      const data = course?.questionSet;
      const startDateTime = moment(data?.startTime + "z")
        .local()
        .toDate();
      const endDateTime = moment(data?.endTime + "z")
        .local()
        .toDate();

      form.setValues({
        name: course?.name ?? "",
        description: data?.description ?? "",
        negativeMarking: data?.negativeMarking ?? 0,
        questionMarking: data?.questionMarking ?? 0,
        passingWeightage: data?.passingWeightage ?? 1,
        allowedRetake: data?.allowedRetake ?? 0,
        duration: data?.duration ? data.duration / 60 : 1,
        startTime: startDateTime,
        startDate: startDateTime,
        endDate: endDateTime,
        endTime: endDateTime,
        isMandatory: course?.isMandatory,
      });
      setIsMandatory(course?.isMandatory ?? false);
    }
  }, [lessonDetails.isSuccess]);

  const form = useForm({
    initialValues: {
      name: "",
      description: "",
      negativeMarking: 0,
      questionMarking: 1,
      passingWeightage: 0,
      allowedRetake: 0,
      duration: 0,
      endDate: new Date(),
      endTime: new Date(),
      startTime: new Date(),
      startDate: new Date(),
      isMandatory: false,
    },
    validate: yupResolver(schema),
  });

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
      }
      showNotification({
        title: "Success!",
        message: `Lesson ${isEditing ? "Edited" : "Added"} successfully!`,
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
        title: "Error!",
      });
    }
  };

  return (
    <form onSubmit={form.onSubmit(handleSubmit)}>
      <Paper withBorder p="md">
        <Grid align={"center"}>
          <Grid.Col span={12} xs={6} lg={4}>
            <TextInput
              withAsterisk
              label="Exam Title"
              placeholder="Exam Title"
              name="title"
              {...form.getInputProps("name")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              label="Passing Percentage"
              max={100}
              min={0}
              placeholder="Question Set passing percentage"
              {...form.getInputProps("passingWeightage")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              withAsterisk
              label="Question Weightage"
              min={1}
              defaultValue={1}
              placeholder="Question Weightage"
              {...form.getInputProps("questionMarking")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <DatePicker
              placeholder="Pick date"
              withAsterisk
              label="Start date"
              minDate={moment(new Date()).toDate()}
              {...form.getInputProps("startDate")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <TimeInput
              label="Start Time"
              format="12"
              clearable
              withAsterisk
              {...form.getInputProps("startTime")}
            />
          </Grid.Col>

          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              label="Duration (minutes)"
              placeholder="Duration"
              min={1}
              withAsterisk
              {...form.getInputProps("duration")}
            />
          </Grid.Col>

          <Grid.Col span={12} xs={6} lg={4}>
            <DatePicker
              placeholder="Pick date"
              label="End date"
              withAsterisk
              minDate={form.values?.startDate}
              {...form.getInputProps("endDate")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <TimeInput
              label="End Time"
              format="12"
              clearable
              withAsterisk
              {...form.getInputProps("endTime")}
            />
          </Grid.Col>

          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              label="Negative Marking"
              placeholder="Negative Marking"
              {...form.getInputProps("negativeMarking")}
            />
          </Grid.Col>
          <Grid.Col span={12} xs={6} lg={4}>
            <NumberInput
              label="Number Of Retakes"
              placeholder="Retakes"
              min={0}
              {...form.getInputProps("allowedRetake")}
            />
          </Grid.Col>

          <Grid.Col span={6} lg={4}>
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

          <Grid.Col>
            <Textarea
              label="Description"
              placeholder="Exam's Description"
              {...form.getInputProps("description")}
            />
          </Grid.Col>
        </Grid>

        <Group position="left" mt="md">
          <Button
            type="submit"
            loading={updateLesson.isLoading || lesson.isLoading}
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
                navigate("questions/" + item?.slug);
              }}
            >
              Add More Questions
            </Button>
          )}
        </Group>
      </Paper>
    </form>
  );
};

export default AddExam;
