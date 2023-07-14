import { Button, Group, Title } from "@mantine/core";
import { ICourseLesson } from "@utils/services/courseService";
import moment from "moment";
import { useTranslation } from "react-i18next";

const Meetings = ({ data }: { data: ICourseLesson }) => {
  const EndTime = moment
    .utc(data.meeting.startDate)
    .add(data.meeting.duration, "seconds");
  const StartTime = moment.utc(data.meeting.startDate);
  const { t } = useTranslation();
  return (
    <Group px={40} sx={{ flexDirection: "column" }}>
      <Title lineClamp={3} align="justify">
        {data.name}
      </Title>
      {t("class_duration")} : {Number(data?.meeting?.duration) / 60}
      {t("minutes")}
      {moment().isBetween(StartTime, EndTime) ? (
        <Button
          component={"a"}
          href={`/meet.html?l=${data.slug}&c=${data.courseId}`}
        >
          {t("join_meeting")}
        </Button>
      ) : (
        <div style={{ display: "block" }}>
          {moment().isBefore(StartTime)
            ? `${t("starts")} ${moment(
                moment.utc(data.meeting.startDate).local()
              ).fromNow()}`
            : `${t("started")} ${moment(
                moment.utc(data.meeting.startDate).local()
              ).fromNow()}`}
        </div>
      )}
    </Group>
  );
};

export default Meetings;
