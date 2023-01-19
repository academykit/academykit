import { Suspense, useState } from "react";
import {
  AspectRatio,
  Box,
  Button,
  Center,
  Container,
  createStyles,
  Grid,
  Loader,
  Tabs,
} from "@mantine/core";
import CourseContent from "@components/Course/CourseDescription/CourseContent/CourseContent";
import { useMediaQuery } from "@mantine/hooks";
import { IconFileDescription, IconMessage } from "@tabler/icons";
import { Link, Outlet, useNavigate, useParams } from "react-router-dom";
import {
  useCourseDescription,
  useGetCourseLesson,
} from "@utils/services/courseService";
import { CourseUserStatus, LessonType, UserRole } from "@utils/enums";
import ExamDetails from "@components/Course/Classes/ExamDetails";
import AssignmentDetails from "@components/Course/Classes/AssignmentDetails";
const VideoPlayer = lazyWithRetry(
  () => import("@components/VideoPlayer/VideoPlayer")
);
import Meetings from "@components/Course/Meetings";
import useAuth from "@hooks/useAuth";
import { useWatchHistory } from "@utils/services/watchHistory";
import RoutePath from "@utils/routeConstants";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import FeedbackDetails from "@components/Course/Classes/FeedbackDetails";
import lazyWithRetry from "@utils/lazyImportWithReload";

const PdfViewer = lazyWithRetry(
  () => import("@components/Course/Classes/PdfViewer")
);
const useStyle = createStyles((theme) => ({
  wrapper: {
    [theme.fn.largerThan(theme.breakpoints.md)]: {},
  },
  videoSection: {
    backgroundColor: "black",
    color: "white",

    [theme.fn.largerThan(theme.breakpoints.md)]: {
      height: "70vh",
      marginTop: "-8px",
    },
  },
  section: {
    background: theme.colorScheme == "dark" ? theme.colors.dark[6] : "white",
    overflowY: "auto",
    [theme.fn.largerThan(theme.breakpoints.md)]: {
      height: "70vh",
    },
  },
  errorSection: {
    display: "flex",
    flexDirection: "column",
    overflowY: "hidden",
    justifyContent: "center",
    alignItems: "center",
  },
  assignmentSection: {
    display: "flex",
    flexDirection: "column",
    overflowY: "hidden",
    justifyContent: "center",
    alignItems: "center",
  },
  fileSection: {},
  meetingSection: {
    display: "flex",
    flexDirection: "column",
    overflowY: "hidden",
    justifyContent: "center",
    alignItems: "center",
  },
}));

const Classes = () => {
  const navigate = useNavigate();
  const { classes, theme, cx } = useStyle();
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.md}px)`);
  const { id, tabValue, lessonId } = useParams();
  const [videoState, setVideoState] = useState<
    | "loading"
    | "completed"
    | "loaded"
    | "playing"
    | "paused"
    | "viewing"
    | "buffering"
  >("loading");

  const { data, isLoading } = useCourseDescription(id as string);
  const courseLesson = useGetCourseLesson(
    id as string,
    lessonId === "1" ? undefined : lessonId
  );
  const auth = useAuth();
  const watchHistory = useWatchHistory(
    id as string,
    lessonId === "1" ? undefined : lessonId
  );
  if (
    auth?.auth?.role !== UserRole.Admin &&
    auth?.auth?.role !== UserRole.SuperAdmin &&
    data?.userStatus === CourseUserStatus.NotEnrolled
  ) {
    navigate("/404", { replace: true });
  }
  if (isLoading) {
    return (
      <Center>
        <Loader />
      </Center>
    );
  }

  const goToNextLesson = (nextLesson: string) =>
    navigate(`${RoutePath.classes}/${id}/${nextLesson}`);
  const onCourseEnded = async (nextLesson: string) => {
    try {
      await watchHistory.mutateAsync({
        courseId: courseLesson.data?.courseId ?? "",
        lessonId: courseLesson.data?.id ?? "",
      });
      goToNextLesson(nextLesson);
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <Box p={0}>
      <Grid className={classes.wrapper}>
        <Grid.Col p={0} m={"auto"} span={matches ? 8 : 12}>
          <Suspense fallback={<Loader />}>
            {courseLesson.isLoading && (
              <Box
                className={classes.videoSection}
                sx={{
                  display: "flex",
                  overflowY: "hidden",
                  justifyContent: "center",
                  alignItems: "center",
                }}
              >
                <Box>
                  <Loader />
                </Box>
              </Box>
            )}
            {courseLesson.isError && (
              <Box className={cx(classes.videoSection, classes.errorSection)}>
                <Box>{errorType(courseLesson.error)}</Box>
                {courseLesson.error?.response?.status &&
                  courseLesson.error?.response?.status === 403 && (
                    <Button
                      component={Link}
                      mt={20}
                      to={`${RoutePath.classes}/${id}/1`}
                    >
                      View Previous Lesson
                    </Button>
                  )}
              </Box>
            )}

            {(courseLesson.data?.type == LessonType.Video ||
              courseLesson.data?.type == LessonType.RecordedVideo) && (
              <AspectRatio
                ratio={16 / 9}
                mt={matches ? 1 : -8}
                className={classes.videoSection}
              >
                <VideoPlayer
                  onEnded={() =>
                    onCourseEnded(courseLesson.data?.nextLessonSlug as string)
                  }
                  url={courseLesson.data.videoUrl}
                  setCurrentPlayerState={setVideoState}
                />
              </AspectRatio>
            )}
            {courseLesson.data?.type == LessonType.Assignment && (
              <Box
                className={cx(classes.videoSection, classes.assignmentSection)}
              >
                <AssignmentDetails lesson={courseLesson.data} />
              </Box>
            )}
            {courseLesson.data?.type === LessonType.LiveClass && (
              <Box
                className={cx(classes.videoSection, classes.meetingSection)}
                sx={{ overflowY: "hidden" }}
              >
                <Meetings data={courseLesson.data} />
              </Box>
            )}
            {courseLesson.data?.type == LessonType.Exam && (
              <Box
                className={classes.videoSection}
                sx={{ overflowY: "hidden" }}
              >
                <ExamDetails
                  exam={courseLesson.data.questionSet}
                  hasResult={courseLesson.data.hasResult}
                  remainingAttempt={courseLesson.data.remainingAttempt}
                />
              </Box>
            )}
            {courseLesson.data?.type === LessonType.Feedback && (
              <Box
                className={cx(classes.videoSection, classes.assignmentSection)}
              >
                <FeedbackDetails
                  name={courseLesson.data.name}
                  id={courseLesson.data.id}
                  hasFeedbackSubmitted={courseLesson.data.hasFeedbackSubmitted}
                />
              </Box>
            )}
            {courseLesson.data?.type === LessonType.Document && (
              <Box className={cx(classes.videoSection, classes.fileSection)}>
                <PdfViewer
                  lesson={courseLesson.data}
                  onEnded={() =>
                    onCourseEnded(courseLesson.data?.nextLessonSlug as string)
                  }
                />
              </Box>
            )}
          </Suspense>
        </Grid.Col>
        <Grid.Col span={matches ? 4 : 12} m={0}>
          <Box className={classes.section} p={10} mt={-8}>
            <CourseContent
              user={data?.user}
              courseName={data?.name}
              courseSlug={data?.slug || ""}
              duration={data?.duration || 0}
              sections={data?.sections || []}
              enrollmentStatus={data?.userStatus || 0}
            />
          </Box>
        </Grid.Col>
      </Grid>
      <Container fluid mt={30}>
        <Box>
          <Tabs
            defaultChecked={true}
            defaultValue={"description"}
            value={tabValue}
            onTabChange={(value) =>
              navigate(`${value}`, { preventScrollReset: true })
            }
          >
            <Tabs.List>
              <Tabs.Tab
                value="description"
                icon={<IconFileDescription size={14} />}
              >
                Description
              </Tabs.Tab>
              <Tabs.Tab value="comments" icon={<IconMessage size={14} />}>
                Comments
              </Tabs.Tab>
            </Tabs.List>

            <Box pt="xs">
              <Outlet />
            </Box>
          </Tabs>
        </Box>
      </Container>
    </Box>
  );
};

export default Classes;
