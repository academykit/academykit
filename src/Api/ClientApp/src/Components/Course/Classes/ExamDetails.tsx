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
import RoutePath from "@utils/routeConstants";
import { ICourseMcq } from "@utils/services/courseService";
import moment from "moment";
import { Link } from "react-router-dom";

const ExamDetails = ({
  exam,
  hasResult,
}: {
  exam: ICourseMcq;
  hasResult: boolean;
}) => {
  const auth = useAuth();
  const userId = auth?.auth?.id ?? "";

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
      <Group sx={{ justifyContent: "space-around", width: "100%" }}>
        <Box>
          <Title lineClamp={3} align="justify">
            {exam.name}
          </Title>
          {exam.startTime && (
            <Text>
              Starts at {moment(exam.startTime).format(theme.dateFormat)}{" "}
            </Text>
          )}
          {exam.duration ? (
            <Text>Duration: {exam.duration / 60} minute(s) </Text>
          ) : (
            ""
          )}
          {exam.allowedRetake && <Text>Retake: {exam.allowedRetake}</Text>}
          {exam.negativeMarking ? (
            <Text>Negative marking {exam.negativeMarking}</Text>
          ) : (
            ""
          )}
        </Box>
        <div>
          <Box sx={{ overflow: "auto", maxHeight: "60vh" }} px={10}>
            {hasResult && (
              <MantineProvider
                theme={{
                  colorScheme: "dark",
                }}
              >
                <UserResults lessonId={exam.slug} studentId={userId} />
              </MantineProvider>
            )}
          </Box>
          {moment().isBetween(exam.startTime + "z", exam.endTime + "z") ? (
            <Button
              mt={10}
              component={Link}
              to={RoutePath.exam.details(exam.slug).route}
            >
              Start Exam
            </Button>
          ) : (
            <Box>
              {moment.utc().isBefore(exam.startTime + "z")
                ? "Starts "
                : "Ended "}
              {moment(exam.startTime + "z")
                .utc()
                .fromNow()}
            </Box>
          )}
        </div>
      </Group>
    </Group>
  );
};

export default ExamDetails;
