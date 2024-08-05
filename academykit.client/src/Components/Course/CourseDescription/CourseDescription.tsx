import TextViewer from '@components/Ui/RichTextViewer';
import UserShortProfile from '@components/UserShortProfile';
import useAuth from '@hooks/useAuth';
import {
  AspectRatio,
  Badge,
  Box,
  Button,
  Center,
  Container,
  Flex,
  Group,
  Image,
  Loader,
  Modal,
  Textarea,
  Title,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { color } from '@utils/constants';
import {
  CourseStatus,
  CourseUserStatus,
  CourseUserStatusValue,
  UserRole,
} from '@utils/enums';
import getCourseOgImageUrl from '@utils/getCourseOGImage';
import RoutePath from '@utils/routeConstants';
import { useGeneralSetting } from '@utils/services/adminService';
import errorType from '@utils/services/axiosError';
import {
  useCourseDescription,
  useCourseStatus,
  useEnrollCourse,
} from '@utils/services/courseService';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';
import classes from '../styles/courseDescription.module.css';
import CourseContent from './CourseContent/CourseContent';

const CourseDescription = () => {
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
            <Flex wrap={'wrap'} align={'baseline'}>
              <Box maw={{ base: '100%', md: 300, lg: 500 }}>
                <Title
                  className={classes.title}
                  style={{
                    whiteSpace: 'nowrap',
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                  }}
                >
                  {course.data?.name}
                </Title>
              </Box>
              <>
                {auth?.auth &&
                auth?.auth?.role <= UserRole.Admin &&
                course?.data?.userStatus !== CourseUserStatus.Author ? (
                  <></>
                ) : (
                  <Badge ml={10}>
                    {t(`${CourseUserStatusValue[course?.data?.userStatus]}`)}
                  </Badge>
                )}
                {auth?.auth && Number(auth?.auth?.role) <= UserRole.Admin && (
                  <>
                    <Badge ml={10} color={color(course?.data?.status)}>
                      {t(`${CourseStatus[course?.data?.status]}`)}
                    </Badge>
                  </>
                )}
              </>
            </Flex>

            <Group my={4}>
              {course.data?.user && (
                <UserShortProfile user={course?.data?.user} size={'md'} />
              )}
            </Group>
            <TextViewer content={course.data?.description} />
          </div>
          <div className={classes.aside}>
            <AspectRatio ratio={16 / 8} mx="auto">
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
                {auth?.auth && Number(auth?.auth?.role) <= UserRole.Admin ? (
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
                    disabled={!course.data.isEligible}
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
                  Number(auth?.auth?.role) <= UserRole.Admin &&
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
                  Number(auth?.auth?.role) <= UserRole.Trainer &&
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
                  (Number(auth?.auth?.role) <= UserRole.Admin ||
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

            <Box className={classes.CourseContentLarge} maw={'90%'}>
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
