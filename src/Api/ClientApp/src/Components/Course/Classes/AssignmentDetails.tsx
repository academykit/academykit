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
import { DATE_FORMAT } from "@utils/constants";
import RoutePath from "@utils/routeConstants";
import { ICourseLesson } from "@utils/services/courseService";
import moment from "moment";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const AssignmentDetails = ({ lesson }: { lesson: ICourseLesson }) => {
  const theme = useMantineTheme();
  const user = useAuth();
  const { t } = useTranslation();
  console.log(lesson)
  return (
    <Group sx={{ flexDirection: "column" }}>
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
      ) : !lesson.startDate || moment().isAfter(lesson.startDate) ? (
        <Button
          component={Link}
          to={RoutePath.assignment.details(lesson.id).route}
        >
          {lesson.isCompleted ? t('resubmit') : t("start_assignment")}
        </Button>
      ) : (
        <Text>
          {t("assignment_yet_start")}{" "}
          {moment(lesson.startDate).format(DATE_FORMAT)}
        </Text>
      )}
      {lesson.assignmentReview && (
        <Box mb={20} sx={{ color: theme.white }}>
          <Title order={3}> {t("assignment_reviewed")}</Title>
          <Group sx={{ alignItems: "start" }} mt={20}>
            <UserShortProfile
              user={lesson.assignmentReview.teacher}
              size={"sm"}
            />
            <Paper
              sx={{
                background: theme.colors.dark[6],
                minWidth: "300px",
                maxWidth: "500px",
              }}
              p={10}
            >
              <Group>
                <Text color={"dimmed"}>
                  {t("obtained_mark")}:{"  "}
                </Text>
                <Text color={theme.white}>
                  {lesson.assignmentReview.mark}/100
                </Text>
              </Group>
              <Group>
                <Text color={"dimmed"}>
                  {t("review")}:{"  "}
                </Text>
                <Text color={theme.white}>
                  {lesson.assignmentReview.review}
                </Text>
              </Group>
            </Paper>
          </Group>
        </Box>
      )}
    </Group>
  );
};

export default AssignmentDetails;
