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

const schema = Yup.object().shape({
  name: Yup.string().required("Assignment's Title is required."),
  description: Yup.string().required("Assignment's Description is required."),
});

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
    validate: yupResolver(schema),
  });

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

            <Grid.Col span={6}>
              <DatePicker
                w={"100%"}
                placeholder="Pick Starting Date"
                label="Start date"
                icon={<IconCalendar size={16} />}
                minDate={moment(new Date()).toDate()}
                {...form.getInputProps("eventStartDate")}
              />
            </Grid.Col>
            <Grid.Col span={6}>
              <TimeInput
                label="Start Time"
                format="12"
                clearable
                {...form.getInputProps("startTime")}
              />
            </Grid.Col>

            <Grid.Col span={6}>
              <DatePicker
                w={"100%"}
                placeholder="Pick Ending Date"
                label="End date"
                minDate={form.values.eventStartDate}
                icon={<IconCalendar size={16} />}
                {...form.getInputProps("eventEndDate")}
              />
            </Grid.Col>
            <Grid.Col span={6}>
              <TimeInput
                label="End Time"
                format="12"
                clearable
                {...form.getInputProps("endTime")}
              />
            </Grid.Col>
            <Grid.Col>
              <Textarea
                placeholder="Assignment's Description"
                label="Assignment Description"
                mb={10}
                withAsterisk
                {...form.getInputProps("description")}
              />
            </Grid.Col>
          </Grid>
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
