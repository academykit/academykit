import React, { useEffect, useMemo, useState } from "react";
import {
  Button,
  Grid,
  Group,
  NumberInput,
  Select,
  Switch,
  Textarea,
  TextInput,
} from "@mantine/core";
import { DatePicker, TimeInput } from "@mantine/dates";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { LessonType } from "@utils/enums";
import { useActiveZoomLicense } from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import {
  useCreateLesson,
  useGetCourseLesson,
  useUpdateLesson,
} from "@utils/services/courseService";
import { ILessonMeeting } from "@utils/services/types";
import moment from "moment";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const schema = Yup.object().shape({
  name: Yup.string().required("Meeting Name is required."),
  meetingStartDate: Yup.string()
    .required("Start Date is required.")
    .typeError("Start Date is required."),
  meetingStartTime: Yup.string()
    .required("Start Time is required.")
    .typeError("Start Time is required."),
  meetingDuration: Yup.string()
    .required("Meeting Duration is required.")
    .typeError("Meeting Duration is required."),
  zoomLicenseId: Yup.string().required("Zoom License is required."),
});

const AddMeeting = ({
  setAddState,
  item,
  isEditing,
  sectionId,
  setAddLessonClick,
  setIsEditing,
}: {
  setAddState: React.Dispatch<React.SetStateAction<string>>;
  item?: ILessonMeeting;
  isEditing?: boolean;
  sectionId?: string;
  setAddLessonClick: React.Dispatch<React.SetStateAction<boolean>>;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const { id: slug } = useParams();
  const lesson = useCreateLesson(slug as string);
  const [dateTime, setDateTime] = useState<string>("");
  const lessonDetails = useGetCourseLesson(
    item?.courseId || "",
    item?.id,
    isEditing
  );
  const [isMandatory, setIsMandatory] = useState<boolean>(
    item?.isMandatory ?? false
  );

  const updateLesson = useUpdateLesson(
    slug as string,
    item?.courseId,
    item?.id
  );

  useEffect(() => {
    if (lessonDetails.isSuccess && isEditing) {
      const data = lessonDetails.data;
      const startDateTime = moment(data?.meeting?.startDate + "z")
        .local()
        .toDate();

      form.setValues({
        name: data?.name ?? "",
        meetingDuration: data ? Number(data?.meeting?.duration) / 60 : 0,
        zoomLicenseId: data?.meeting?.zoomLicenseId ?? "",
        meetingStartDate: startDateTime,
        meetingStartTime: startDateTime,
        isMandatory: data?.isMandatory,
        description: data?.description ?? "",
      });
      changeZoomLiscense();
    }
  }, [lessonDetails.isSuccess]);

  const form = useForm({
    initialValues: {
      name: "",
      meetingStartDate: new Date(),
      meetingStartTime: new Date(),
      meetingDuration: 1,
      zoomLicenseId: "",
      isMandatory: false,
      description: "",
    },
    validate: yupResolver(schema),
  });

  const meeting = useActiveZoomLicense(
    dateTime,
    form.values.meetingDuration,
    lessonDetails?.data?.id
  );

  const selectItem = meeting.data?.data
    ? meeting.data.data.map((e) => {
        return { value: e.id, label: e.licenseEmail };
      })
    : [
        {
          label: "Please select different time!",
          value: "null",
          disabled: true,
        },
      ];

  const changeZoomLiscense = () => {
    const { meetingDuration, meetingStartTime, meetingStartDate } = form.values;
    if (meetingDuration && meetingStartTime && meetingStartDate) {
      const time = new Date(meetingStartTime).toLocaleTimeString();
      const date = new Date(meetingStartDate).toLocaleDateString();
      setDateTime(() => date + " " + time);
    } else {
      form.setFieldValue("zoomLicenseId", "");
    }
  };

  const handleSubmit = async (values: any) => {
    const meeting = {
      ...values,
      meetingStartDate: isEditing
        ? new Date(values?.meetingStartTime).toISOString()
        : new Date(dateTime).toISOString(),
    };
    delete meeting.isMandatory;
    delete meeting.meetingStartTime;

    try {
      if (isEditing) {
        await updateLesson.mutateAsync({
          meeting,
          name: values.name,
          courseId: slug,
          type: LessonType.LiveClass,
          lessonIdentity: item?.id,
          sectionIdentity: sectionId,
          isMandatory: values.isMandatory,
          description: values.description,
        } as ILessonMeeting);
        setIsEditing(false);
      } else {
        await lesson.mutateAsync({
          meeting,
          name: values.name,
          courseId: slug,
          type: LessonType.LiveClass,
          sectionIdentity: sectionId,
          isMandatory: values.isMandatory,
          description: values.description,
        } as ILessonMeeting);
      }
      showNotification({
        message: `Lesson ${isEditing ? "Edited" : "Added"} successfully!`,
        title: "Success!",
      });
      setAddLessonClick(true);
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
      <Grid align="center">
        <Grid.Col span={12} lg={6}>
          <TextInput
            label="Meeting name"
            placeholder="Meeting's Name"
            {...form.getInputProps("name")}
            withAsterisk
          />
        </Grid.Col>
        <Grid.Col span={6} lg={3}>
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
      <Group grow>
        <DatePicker
          placeholder="Pick date"
          label="Start date"
          withAsterisk
          {...form.getInputProps("meetingStartDate")}
        />
        <TimeInput
          label="Start Time"
          format="12"
          clearable
          withAsterisk
          {...form.getInputProps("meetingStartTime")}
        />
      </Group>
      <Group grow mt={5} mb={10}>
        <NumberInput
          label="Meeting Duration (minutes)"
          placeholder="Meeting Duration in Minutes"
          withAsterisk
          min={1}
          {...form.getInputProps("meetingDuration")}
        />

        <Select
          onClick={changeZoomLiscense}
          onKeyDown={(e) => {
            if (e.code === "Space") {
              changeZoomLiscense();
            }
          }}
          defaultValue="Pick one license"
          label="Zoom License"
          placeholder="Pick one License"
          disabled={
            !(
              form.values.meetingDuration &&
              form.values.meetingStartDate &&
              form.values.meetingStartTime
            )
          }
          data={selectItem}
          withAsterisk
          {...form.getInputProps("zoomLicenseId")}
        />
      </Group>
      <Textarea
        label="Description"
        placeholder="Description for Live Class"
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
      </Group>
    </form>
  );
};

export default AddMeeting;
