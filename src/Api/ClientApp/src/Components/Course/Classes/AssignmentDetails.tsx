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
import RoutePath from "@utils/routeConstants";
import { ICourseLesson } from "@utils/services/courseService";
import { Link } from "react-router-dom";

const AssignmentDetails = ({ lesson }: { lesson: ICourseLesson }) => {
  const theme = useMantineTheme();
  const user = useAuth();
  return (
    <Group sx={{ flexDirection: "column" }}>
      <Title>{lesson.name}</Title>

      {lesson.hasReviewedAssignment ? (
        <Button
          component={Link}
          to={RoutePath.assignment.result(lesson.id, user?.auth?.id).route}
        >
          View Result
        </Button>
      ) : (
        <Button
          component={Link}
          to={RoutePath.assignment.details(lesson.id).route}
        >
          Start Assignment
        </Button>
      )}
      {lesson.assignmentReview && (
        <Box mb={20} sx={{ color: theme.white }}>
          <Title order={3}> Your assignment has been reviewed.</Title>
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
                <Text color={"dimmed"}>Obtained Mark:{"  "}</Text>
                <Text color={theme.white}>
                  {lesson.assignmentReview.mark}/100
                </Text>
              </Group>
              <Group>
                <Text color={"dimmed"}>Review:{"  "}</Text>
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
