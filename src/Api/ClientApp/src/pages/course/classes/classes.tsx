import AssignmentDetails from '@components/Course/Classes/AssignmentDetails';
import ExamDetails from '@components/Course/Classes/ExamDetails';
import FeedbackDetails from '@components/Course/Classes/FeedbackDetails';
import PhysicalTrainingDetail from '@components/Course/Classes/PhysicalTrainingDetail';
import CourseContent from '@components/Course/CourseDescription/CourseContent/CourseContent';
import Meetings from '@components/Course/Meetings';
import useAuth from '@hooks/useAuth';
import {
  AspectRatio,
  Box,
  Button,
  Center,
  Container,
  Grid,
  Loader,
  Tabs,
} from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconFileDescription, IconMessage } from '@tabler/icons';
import { CourseUserStatus, LessonType, UserRole } from '@utils/enums';
import lazyWithRetry from '@utils/lazyImportWithReload';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import {
  useCourseDescription,
  useGetCourseLesson,
} from '@utils/services/courseService';
import { useWatchHistory } from '@utils/services/watchHistory';
import { AxiosError } from 'axios';
import cx from 'clsx';
import { Suspense, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Outlet, useNavigate, useParams } from 'react-router-dom';
import classes from '../styles/classes.module.css';
const VideoPlayer = lazyWithRetry(
  () => import('@components/VideoPlayer/VideoPlayer')
);

const PdfViewer = lazyWithRetry(
  () => import('@components/Course/Classes/PdfViewer')
);

const Classes = () => {
  const navigate = useNavigate();
  const matches = useMediaQuery(`(min-width: 62em)`);
  const params = useParams();
  const tab = params['*'];
  const { t } = useTranslation();
  const [, setVideoState] = useState<
    | 'loading'
    | 'completed'
    | 'loaded'
    | 'playing'
    | 'paused'
    | 'viewing'
    | 'buffering'
  >('loading');

  const { data, isLoading } = useCourseDescription(params.id as string);
  const auth = useAuth();
  const watchHistory = useWatchHistory(
    params.id as string,
    params.lessonId === '1' ? undefined : params.lessonId
  );
  const courseLesson = useGetCourseLesson(
    params.id as string,
    params.lessonId === '1' ? undefined : params.lessonId
  );

  const goToNextLesson = (nextLesson: string) =>
    navigate(`${RoutePath.classes}/${params.id}/${nextLesson}/description`);
  const onCourseEnded = async (nextLesson: string) => {
    try {
      await watchHistory.mutateAsync({
        courseId: courseLesson.data?.courseId ?? '',
        lessonId: courseLesson.data?.id ?? '',
      });
      if (nextLesson) {
        goToNextLesson(nextLesson);
      }
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  if (
    Number(auth?.auth?.role) !== UserRole.Admin &&
    Number(auth?.auth?.role) !== UserRole.SuperAdmin &&
    data?.userStatus === CourseUserStatus.NotEnrolled
  ) {
    navigate('/404', { replace: true });
  }

  if (isLoading) {
    return (
      <Center>
        <Loader />
      </Center>
    );
  }

  // finding the latest incomplete lesson i.e., current lesson
  const currentLesson = data?.sections.map((section) =>
    section.lessons?.find((lesson) => !lesson.isCompleted)
  );

  return (
    <Box p={0}>
      <Grid className={classes.wrapper}>
        <Grid.Col p={0} m={'auto'} span={matches ? 8 : 12}>
          <Suspense fallback={<Loader />}>
            {courseLesson.isLoading && (
              <Box
                className={classes.videoSection}
                style={{
                  display: 'flex',
                  overflowY: 'hidden',
                  justifyContent: 'center',
                  alignItems: 'center',
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

                {(courseLesson.error as AxiosError)?.response?.status &&
                  (courseLesson.error as AxiosError)?.response?.status ===
                    403 && (
                    <Button
                      component={Link}
                      mt={20}
                      to={`${RoutePath.classes}/${params.id}/${
                        currentLesson &&
                        currentLesson[0] &&
                        currentLesson[0].slug
                      }/description`}
                    >
                      {t('view_previous_lesson')}
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
                style={{ overflowY: 'hidden' }}
              >
                <Meetings data={courseLesson.data} />
              </Box>
            )}
            {courseLesson.data?.type == LessonType.Exam && (
              <Box
                className={classes.videoSection}
                style={{ overflowY: 'hidden' }}
              >
                <ExamDetails
                  id={params.id as string}
                  lessonId={
                    params.lessonId === '1' ? undefined : params.lessonId
                  }
                />
              </Box>
            )}
            {courseLesson.data?.type === LessonType.Feedback && (
              <Box
                className={cx(classes.videoSection, classes.assignmentSection)}
              >
                <FeedbackDetails
                  isTrainee={courseLesson.data.isTrainee}
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
            {courseLesson.data?.type === LessonType.Physical && (
              <Box
                className={cx(classes.videoSection, classes.assignmentSection)}
              >
                <PhysicalTrainingDetail
                  lessonSlug={courseLesson.data.slug}
                  name={courseLesson.data.name}
                  id={courseLesson.data.id}
                  hasAttended={courseLesson.data.hasAttended}
                  startDate={courseLesson.data.startDate}
                  isTrainee={courseLesson.data.isTrainee}
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
              courseSlug={data?.slug || ''}
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
            defaultValue={t('description')}
            value={tab}
            onChange={(value) =>
              navigate(`${value}`, { preventScrollReset: true })
            }
          >
            <Tabs.List>
              <Tabs.Tab
                value="description"
                leftSection={<IconFileDescription size={14} />}
              >
                {t('description')}
              </Tabs.Tab>
              <Tabs.Tab
                value="comments"
                leftSection={<IconMessage size={14} />}
              >
                {t('comments')}
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
