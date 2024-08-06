import UserShortProfile from "@components/UserShortProfile";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Group,
  Paper,
  Text,
  Title,
  useMantineTheme,
} from "@mantine/core";
// import { DATE_FORMAT } from '@utils/constants';
import RoutePath from "@utils/routeConstants";
import { ICourseLesson } from "@utils/services/courseService";
import moment from "moment";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const AssignmentDetails = ({ lesson }: { lesson: ICourseLesson }) => {
  const theme = useMantineTheme();
  const user = useAuth();
  const { t } = useTranslation();
  const lesson_start_date = moment.utc(
    lesson.startDate,
    "YYYY-MM-DD[T]HH:mm[Z]"
  );
  const current_time = moment.utc(moment().toDate(), "YYYY-MM-DD[T]HH:mm[Z]");

  return (
    <Group style={{ flexDirection: "column" }}>
      <Title>{lesson.name}</Title>

      {lesson.hasReviewedAssignment ? (
        <Button
          component={Link}
          to={RoutePath.assignment.result(lesson.id, user?.auth?.id).route}
        >
          {t("view_result")}
        </Button>
      ) : lesson.assignmentExpired ? (
        <Text>{t("assignment_expired")}</Text>
      ) : !lesson.startDate || current_time.isAfter(lesson_start_date) ? (
        <Button
          component={Link}
          to={RoutePath.assignment.details(lesson.id).route}
        >
          {lesson.isCompleted && lesson.isTrainee
            ? t("resubmit")
            : lesson.isTrainee && t("start_assignment")}

          {!lesson.isTrainee && t("view_assignment")}
        </Button>
      ) : (
        <Text>
          {t("assignment_yet_start")}{" "}
          {moment(lesson.startDate)
            .add(5, "hours")
            .add(45, "minutes")
            .format("MMM DD, YYYY hh:mm A")}
        </Text>
      )}
      {lesson.assignmentReview && (
        <Box mb={20} style={{ color: theme.white }}>
          <Title order={3}> {t("assignment_reviewed")}</Title>
          <Group style={{ alignItems: "start" }} mt={20}>
            <UserShortProfile
              user={lesson.assignmentReview.teacher}
              size={"sm"}
              color="#C1C2C5"
            />
            <Paper
              style={{
                background: theme.colors.dark[6],
                minWidth: "300px",
                maxWidth: "500px",
              }}
              p={10}
            >
              <Group>
                <Text c={"dimmed"}>
                  {t("obtained_mark")}:{"  "}
                </Text>
                <Text c={theme.white}>{lesson.assignmentReview.mark}/100</Text>
              </Group>
              <Group>
                <Text c={"dimmed"}>
                  {t("review")}:{"  "}
                </Text>
                <Text c={theme.white}>{lesson.assignmentReview.review}</Text>
              </Group>
            </Paper>
          </Group>
        </Box>
      )}
    </Group>
  );
};

export default AssignmentDetails;
