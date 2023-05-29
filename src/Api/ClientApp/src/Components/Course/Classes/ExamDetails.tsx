import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Group,
  MantineProvider,
  Text,
  Title,
  useMantineTheme,
} from "@mantine/core";
import UserResults from "@pages/course/exam/Components/UserResults";
import { useQueryClient } from "@tanstack/react-query";
import RoutePath from "@utils/routeConstants";
import { useGetCourseLesson } from "@utils/services/courseService";
import { api } from "@utils/services/service-api";
import moment from "moment";
import { useEffect } from "react";
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
      queryClient.invalidateQueries([
        api.lesson.courseLesson(
          id as string,
          lessonId === "1" ? undefined : lessonId
        ),
      ]);

      window.history.pushState(
        { fromJs: true },
        "",
        `${window.location.pathname}`
      );
    }
  }, [invalidate]);

  const theme = useMantineTheme();
  return (
    <Group
      p={10}
      sx={{
        height: "70vh",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      {exam && (
        <Group sx={{ justifyContent: "space-around", width: "100%" }}>
          <Box>
            <Title lineClamp={3} align="justify">
              {exam?.name}
            </Title>
            {exam?.startTime && (
              <Text>
                Start Date: {moment(exam?.startTime).format(theme.dateFormat)}{" "}
              </Text>
            )}
            {exam?.duration ? (
              <Text>Duration: {exam?.duration / 60} minute(s) </Text>
            ) : (
              ""
            )}
            <Text>Total Retake: {exam?.allowedRetake}</Text>
            <Text>Remaining Retakes: {data?.remainingAttempt}</Text>
            {exam?.negativeMarking ? (
              <Text>Negative marking {exam?.negativeMarking}</Text>
            ) : (
              ""
            )}
          </Box>
          <div>
            <Box sx={{ overflow: "auto", maxHeight: "60vh" }} px={10}>
              {data.hasResult && (
                <MantineProvider
                  theme={{
                    colorScheme: "dark",
                  }}
                >
                  <UserResults lessonId={exam?.slug} studentId={userId} />
                </MantineProvider>
              )}
            </Box>
            {moment().isBetween(exam?.startTime + "z", exam?.endTime + "z") ? (
              <>
                {data.remainingAttempt > 0 ? (
                  <Button
                    mt={10}
                    component={Link}
                    to={RoutePath.exam?.details(exam?.slug).route}
                    state={window.location.pathname}
                  >
                    Start Exam
                  </Button>
                ) : (
                  <Text mt={15}>You have exceeded attempt count.</Text>
                )}
              </>
            ) : (
              <Box mt={10}>
                {moment.utc().isBefore(exam?.startTime + "z")
                  ? `Starts ${moment(exam?.startTime + "z")
                      .utc()
                      .fromNow()}`
                  : `Ended ${moment(exam?.endTime + "z")
                      .utc()
                      .fromNow()}`}
              </Box>
            )}
          </div>
        </Group>
      )}
    </Group>
  );
};

export default ExamDetails;
