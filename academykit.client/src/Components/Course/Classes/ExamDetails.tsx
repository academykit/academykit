import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Group,
  MantineProvider,
  Text,
  Title,
} from "@mantine/core";
import UserResults from "@pages/course/exam/Components/UserResults";
import { useQueryClient } from "@tanstack/react-query";
import { DATE_FORMAT } from "@utils/constants";
import { CourseStatus } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useGetCourseLesson } from "@utils/services/courseService";
import { api } from "@utils/services/service-api";
import moment from "moment";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Link, useSearchParams } from "react-router-dom";

const ExamDetails = ({
  id,
  lessonId,
}: {
  id: string;
  lessonId: string | undefined;
}) => {
  const auth = useAuth();
  const queryClient = useQueryClient();
  const { t } = useTranslation();
  const [searchParams] = useSearchParams();
  const userId = auth?.auth?.id ?? "";
  const { data } = useGetCourseLesson(
    id as string,
    lessonId === "1" ? undefined : lessonId
  );
  const invalidate = searchParams.get("invalidate");

  const exam = data?.questionSet;

  useEffect(() => {
    if (invalidate) {
      queryClient.invalidateQueries({
        queryKey: [
          api.lesson.courseLesson(
            id as string,
            lessonId === "1" ? undefined : lessonId
          ),
        ],
      });

      window.history.pushState(
        { fromJs: true },
        "",
        `${window.location.pathname}`
      );
    }
  }, [invalidate]);

  // render 'in [time] [s/min/hr/...]' if time left
  // render '[time] [s/min/hr/...] ago' if meeting ended
  moment.updateLocale("en", {
    relativeTime: {
      future: `${t("in")} %s`,
      past: `%s`,
      s: `%d ${t("in_second")}`,
      ss: `%d ${t("in_seconds")}`,
      m: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_minute")}`
        : `%d ${t("minute_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_minute")}`
          : `%d ${t("minute_ago")}`,
      mm: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_minutes")}`
        : `%d ${t("minutes_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_minutes")}`
          : `%d ${t("minutes_ago")}`,
      h: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_hour")}`
        : `%d ${t("hour_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_hour")}`
          : `%d ${t("hour_ago")}`,
      hh: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_hours")}`
        : `%d ${t("hours_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_hours")}`
          : `%d ${t("hours_ago")}`,
      d: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_day")}`
        : `%d ${t("day_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_day")}`
          : `%d ${t("day_ago")}`,
      dd: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_days")}`
        : `%d ${t("days_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_days")}`
          : `%d ${t("days_ago")}`,
      M: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_month")}`
        : `%d ${t("month_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_month")}`
          : `%d ${t("month_ago")}`,
      MM: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_months")}`
        : `%d ${t("months_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_months")}`
          : `%d ${t("months_ago")}`,
      y: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_year")}`
        : `%d ${t("year_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_year")}`
          : `%d ${t("year_ago")}`,
      yy: moment().isBefore(exam?.startTime + "Z")
        ? `%d ${t("in_years")}`
        : `%d ${t("years_ago")}` && moment().isBefore(exam?.endTime + "Z")
          ? `%d ${t("in_years")}`
          : `%d ${t("years_ago")}`,
    },
  });

  const formattedStartTime = moment(exam?.startTime).format("HH:mm A");
  const formattedEndTime = moment(exam?.endTime).format("HH:mm A");

  return (
    <Group
      p={10}
      style={{
        height: "70vh",
        justifyContent: "center",
        alignItems: "center",
        overflow: "scroll",
      }}
      pos={"relative"}
    >
      {exam && (
        <>
          {moment.utc().isBefore(exam?.endTime + "Z") && (
            <Box pos={"absolute"} top={40} right={50}>
              <Text>{`${t("ends")} ${moment(exam?.endTime + "Z")
                .utc()
                .fromNow()}`}</Text>
            </Box>
          )}
          <Group style={{ justifyContent: "space-around", width: "100%" }}>
            <Box>
              <Box w={350}>
                <Title
                  ta="justify"
                  display={"block"}
                  style={{
                    lineClamp: 1,
                    whiteSpace: "nowrap",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                  }}
                >
                  {exam?.name}
                </Title>
              </Box>
              {exam?.startTime && (
                <Text>
                  {t("active_from")}:{" "}
                  {moment(exam?.startTime).format(DATE_FORMAT)} (
                  {moment(formattedStartTime, "hh:mm A")
                    .add(5, "hours")
                    .add(45, "minutes")
                    .format("hh:mm A")}
                  )
                </Text>
              )}
              {exam?.endTime && (
                <Text>
                  {t("active_till")}:{" "}
                  {moment(exam?.endTime).format(DATE_FORMAT)} (
                  {moment(formattedEndTime, "hh:mm A")
                    .add(5, "hours")
                    .add(45, "minutes")
                    .format("hh:mm A")}
                  )
                </Text>
              )}

              {exam?.duration ? (
                <Text>
                  {t("duration")}: {exam?.duration / 60} minute(s){" "}
                </Text>
              ) : (
                ""
              )}
              <Text>
                {t("total_retake")}: {exam?.allowedRetake}
              </Text>
              <Text>
                {t("remaining_retakes")}: {data?.remainingAttempt}
              </Text>
              {exam?.totalQuestions > 0 && (
                <Text>
                  {t("total_question")} {exam?.totalQuestions}
                </Text>
              )}
              {exam?.negativeMarking ? (
                <Text>
                  {t("negative_marking")}:{" "}
                  <span style={{ color: "red" }}>{exam?.negativeMarking}</span>
                </Text>
              ) : (
                ""
              )}
              {exam?.totalMarks > 0 && (
                <Text>
                  {t("total_marks")}: {exam?.totalMarks}
                </Text>
              )}
              {exam?.passingWeightage > 0 && (
                <Text>
                  {t("pass_mark")}:{" "}
                  {exam?.totalMarks * (exam?.passingWeightage / 100)}
                </Text>
              )}
            </Box>
            <div>
              <Box style={{ overflow: "auto", maxHeight: "60vh" }} px={10}>
                {data.hasResult && (
                  <MantineProvider>
                    <UserResults
                      lessonId={exam?.slug}
                      studentId={userId}
                      isTrainee={data.isTrainee}
                    />
                  </MantineProvider>
                )}
              </Box>
              {moment().isBetween(
                exam?.startTime + "Z",
                exam?.endTime + "Z"
              ) ? (
                <>
                  {data.status != CourseStatus.Completed ? (
                    <>
                      {data.remainingAttempt > 0 ? (
                        <Button
                          mt={10}
                          component={Link}
                          to={RoutePath.exam?.details(exam?.slug).route}
                          state={window.location.pathname}
                        >
                          {data.isTrainee ? t("start_exam") : t("view_exam")}
                        </Button>
                      ) : (
                        <Text mt={15}>{t("attempt_exceeded")}</Text>
                      )}
                    </>
                  ) : (
                    <Text mt={15}>{t("exam_course_completed")}</Text>
                  )}
                </>
              ) : (
                <Box mt={10}>
                  {moment.utc().isBefore(exam?.startTime + "Z")
                    ? `${t("starts")} ${moment(exam?.startTime + "Z")
                        .utc()
                        .fromNow()}`
                    : `${t("ended")} ${moment(exam?.endTime + "Z")
                        .utc()
                        .fromNow()}`}
                </Box>
              )}
            </div>
          </Group>
        </>
      )}
    </Group>
  );
};

export default ExamDetails;
