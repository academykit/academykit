import UserShortProfile from '@components/UserShortProfile';
import useAuth from '@hooks/useAuth';
import {
  createStyles,
  Image,
  Container,
  Title,
  Button,
  Group,
  AspectRatio,
  Center,
  Box,
  Loader,
  Badge,
  Modal,
  Textarea,
} from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import {
  CourseUserStatus,
  UserRole,
  CourseStatus,
  CourseUserStatusValue,
} from '@utils/enums';
import getCourseOgImageUrl from '@utils/getCourseOGImage';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import {
  useCourseDescription,
  useCourseStatus,
  useEnrollCourse,
} from '@utils/services/courseService';
import { Link, useParams } from 'react-router-dom';
import CourseContent from './CourseContent/CourseContent';
import { useTranslation } from 'react-i18next';
import { color } from '@utils/constants';
import TextViewer from '@components/Ui/RichTextViewer';
import { useToggle } from '@mantine/hooks';
import { useForm } from '@mantine/form';
import { useGeneralSetting } from '@utils/services/adminService';

const useStyles = createStyles((theme) => ({
  wrapper: {
    marginLeft: 40,
    marginRight: 40,
  },
  inner: {
    display: 'flex',
    justifyContent: 'space-between',
    paddingTop: '80px',
    paddingBottom: '80px',
    [theme.fn.smallerThan('sm')]: {
      flexDirection: 'column-reverse',
    },
  },

  content: {
    width: '60%',
    marginRight: '80px',

    [theme.fn.smallerThan('lg')]: {
      width: '50%',
    },
    [theme.fn.smallerThan('sm')]: {
      width: '100%',
    },
  },

  title: {
    color: theme.colorScheme === 'dark' ? theme.white : theme.black,
    fontFamily: `Greycliff CF, ${theme.fontFamily}`,
    fontSize: 42,
    lineHeight: 1.2,
    fontWeight: 800,

    [theme.fn.smallerThan('xs')]: {
      fontSize: 28,
    },
  },

  control: {
    [theme.fn.smallerThan('xs')]: {
      flex: 1,
    },
  },

  aside: {
    width: '40%',
    [theme.fn.smallerThan('lg')]: {
      width: '50%',
    },
    [theme.fn.smallerThan('sm')]: {
      width: '100%',
    },
  },

  highlight: {
    position: 'relative',
    backgroundColor: theme.fn.variant({
      variant: 'light',
      color: theme.primaryColor,
    }).background,
    borderRadius: theme.radius.sm,
    padding: '4px 12px',
  },
  CourseContentSmall: {
    display: 'none',
    [theme.fn.smallerThan('sm')]: {
      display: 'block',
    },
  },

  CourseContentLarge: {
    display: 'block',
    [theme.fn.smallerThan('sm')]: {
      display: 'none',
    },
  },
}));

const CourseDescription = () => {
  const { classes } = useStyles();
  const { id } = useParams();
  const auth = useAuth();
  const courseStatus = useCourseStatus(id as string, '');
  const [isRejected, toggleRejected] = useToggle();
  const [confirmPublish, togglePublish] = useToggle();

  // to validate reject message
  const form = useForm({
    initialValues: {
      message: '',
    },
    validate: {
      message: (value) =>
        value.length === 0 ? 'Rejection message is required!' : null,
    },
  });

  const onPublish = async (message?: string) => {
    try {
      await courseStatus.mutateAsync({
        identity: id as string,
        status: message ? CourseStatus.Rejected : CourseStatus.Published,
        message: message ?? '',
      });
      showNotification({
        message: message
          ? t('training_rejected_success')
          : t('training_published_success'),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
    togglePublished();
  };
  const generalSettings = useGeneralSetting();
  const companyName = generalSettings.data?.data.companyName;
  const companyLogo = generalSettings.data?.data.logoUrl;

  const course = useCourseDescription(id as string);
  const { t } = useTranslation();
  const enrollCourse = useEnrollCourse(id as string);
  const onEnroll = async () => {
    try {
      await enrollCourse.mutateAsync({ id: id as string });
      showNotification({ message: t('enroll_course_success') });
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: 'red' });
    }
  };
  if (course.isError) {
    throw course.error;
  }

  if (course.isLoading) {
    return (
      <Center>
        <Loader />
      </Center>
    );
  }
  if (course.isError) {
    <Center>{t('unable_get_course')}</Center>;
  }

  const firstLessonSlugs = course?.data?.sections?.find(
    (item) => item.lessons && item?.lessons?.length > 0
  );

  const slug = firstLessonSlugs?.lessons
    ? firstLessonSlugs?.lessons[0].slug
    : '';

  const togglePublished = () => {
    togglePublish();
    toggleRejected(false);
    form.reset();
  };

  return (
    <div>
      <Modal
        opened={confirmPublish}
        onClose={togglePublished}
        title={
          isRejected
            ? t('leave_message_reject')
            : `${t('publish_confirmation')} "${course.data.name}"${t('?')}`
        }
      >
        {!isRejected ? (
          <Group mt={10}>
            <Button
              onClick={() => onPublish()}
              loading={courseStatus.isLoading}
            >
              {t('publish')}
            </Button>
            <Button
              variant="outline"
              onClick={() => {
                toggleRejected();
              }}
            >
              {t('reject')}
            </Button>
          </Group>
        ) : (
          <form onSubmit={form.onSubmit((value) => onPublish(value.message))}>
            <Group>
              <Textarea {...form.getInputProps('message')} w={'100%'} />
              <Button loading={courseStatus.isLoading} type="submit">
                {t('submit')}
              </Button>
              <Button variant="outline" onClick={() => toggleRejected()}>
                {t('cancel')}
              </Button>
            </Group>
          </form>
        )}
      </Modal>
      <Container fluid>
        <div className={classes.inner}>
          <div className={classes.content}>
            <Title className={classes.title}>
              {course.data?.name}
              <Badge ml={10}>
                {t(`${CourseUserStatusValue[course?.data?.userStatus]}`)}
              </Badge>
              {auth?.auth && auth?.auth?.role <= UserRole.Admin && (
                <>
                  <Badge ml={10} color={color(course?.data?.status)}>
                    {t(`${CourseStatus[course?.data?.status]}`)}
                  </Badge>
                </>
              )}
            </Title>

            <Group my={4}>
              {course.data?.user && (
                <UserShortProfile user={course?.data?.user} size={'md'} />
              )}
            </Group>
            <TextViewer content={course.data?.description} />
          </div>
          <div className={classes.aside}>
            <AspectRatio ratio={16 / 9} mx="auto">
              <Image
                src={getCourseOgImageUrl({
                  author: course?.data?.user,
                  title: course?.data?.name,
                  thumbnailUrl: course?.data.thumbnailUrl,
                  companyName: companyName,
                  companyLogo: companyLogo,
                })}
              />
            </AspectRatio>
            <Center>
              <Group my={30}>
                {auth?.auth && auth?.auth?.role <= UserRole.Admin ? (
                  <>
                    {slug && (
                      <Link
                        to={`${RoutePath.classes}/${course?.data?.slug}/${slug}/description`}
                      >
                        <Button
                          radius="xl"
                          size="md"
                          className={classes.control}
                        >
                          {t('preview')}
                        </Button>
                      </Link>
                    )}
                  </>
                ) : course.data?.userStatus === CourseUserStatus.NotEnrolled ? (
                  <Button
                    radius="xl"
                    size="md"
                    className={classes.control}
                    loading={enrollCourse.isLoading}
                    onClick={onEnroll}
                  >
                    {t('enroll_course')}
                  </Button>
                ) : course.data?.userStatus === CourseUserStatus.Author ||
                  course.data?.userStatus === CourseUserStatus.Teacher ? (
                  <>
                    {slug && (
                      <Link
                        to={`${RoutePath.classes}/${course?.data?.slug}/${slug}/description`}
                      >
                        <Button
                          radius="xl"
                          size="md"
                          className={classes.control}
                        >
                          {t('preview')}
                        </Button>
                      </Link>
                    )}
                  </>
                ) : (
                  <>
                    {slug && (
                      <Link
                        to={`${RoutePath.classes}/${course?.data?.slug}/${slug}/description`}
                      >
                        <Button
                          radius="xl"
                          size="md"
                          className={classes.control}
                        >
                          {t('watch_course')}
                        </Button>
                      </Link>
                    )}
                  </>
                )}
                {auth?.auth &&
                  auth?.auth?.role <= UserRole.Admin &&
                  course.data?.status === CourseStatus.Review && (
                    <Button
                      onClick={() => togglePublished()}
                      radius="xl"
                      size="md"
                      className={classes.control}
                    >
                      {t('publish')}
                    </Button>
                  )}

                {auth?.auth &&
                  auth?.auth?.role <= UserRole.Trainer &&
                  course.data?.status === CourseStatus.Draft && (
                    <Link to={RoutePath.manageCourse.edit(id).route}>
                      <Button radius="xl" size="md" className={classes.control}>
                        {t('edit')}
                      </Button>
                    </Link>
                  )}
                {auth?.auth &&
                  course.data?.status !==
                    (CourseStatus.Draft || CourseStatus.Review) &&
                  (auth?.auth?.role <= UserRole.Admin ||
                    course.data?.userStatus === CourseUserStatus.Author ||
                    course.data?.userStatus === CourseUserStatus.Teacher) && (
                    <Link to={RoutePath.manageCourse.dashboard(id).route}>
                      <Button radius="xl" size="md" className={classes.control}>
                        {t('manage')}
                      </Button>
                    </Link>
                  )}
              </Group>
            </Center>

            <Box className={classes.CourseContentLarge}>
              {course.data?.sections && (
                <CourseContent
                  courseName={course.data?.name}
                  courseSlug={course.data?.slug}
                  sections={course.data?.sections}
                  duration={course.data?.duration}
                  enrollmentStatus={course.data?.userStatus}
                />
              )}
            </Box>
          </div>
        </div>
        <Box className={classes.CourseContentSmall}>
          {course.data?.sections && (
            <CourseContent
              courseSlug={course.data.slug}
              sections={course.data?.sections}
              duration={course.data?.duration}
              enrollmentStatus={course.data?.userStatus}
            />
          )}
        </Box>
      </Container>
    </div>
  );
};
export default CourseDescription;
