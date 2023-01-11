import { Button, Group, Title } from "@mantine/core";
import { ICourseLesson } from "@utils/services/courseService";
import moment from "moment";

const Meetings = ({ data }: { data: ICourseLesson }) => {
  const EndTime = moment
    .utc(data.meeting.startDate)
    .add(data.meeting.duration, "minutes");
  const StartTime = moment.utc(data.meeting.startDate);
  return (
    <Group px={40} sx={{ flexDirection: "column" }}>
      <Title lineClamp={3} align="justify">
        {data.name}
      </Title>
      Class Duration: {Number(data?.meeting?.duration) / 60} Minutes
      {moment().isBetween(StartTime, EndTime) ? (
        <Button
          component={"a"}
          href={`/meet.html?l=${data.slug}&c=${data.courseId}`}
        >
          Join Meeting
        </Button>
      ) : (
        <div style={{ display: "block" }}>
          {moment().isBefore(StartTime)
            ? `Starts ${moment(
                moment.utc(data.meeting.startDate).local()
              ).fromNow()}`
            : `Started ${moment(
                moment.utc(data.meeting.startDate).local()
              ).fromNow()}`}
        </div>
      )}
    </Group>
  );
};

export default Meetings;
