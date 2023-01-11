import UserShortProfile from "@components/UserShortProfile";
import useAuth from "@hooks/useAuth";
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
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import RichTextEditor from "@mantine/rte";
import {
  CourseUserStatus,
  UserRole,
  CourseStatus,
  CourseUserStatusValue,
} from "@utils/enums";
import getCourseOgImageUrl from "@utils/getCourseOGImage";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import {
  useCourseDescription,
  useCourseStatus,
  useEnrollCourse,
} from "@utils/services/courseService";
import { Link, useParams } from "react-router-dom";
import CourseContent from "./CourseContent/CourseContent";

const useStyles = createStyles((theme) => ({
  wrapper: {
    marginLeft: 40,
    marginRight: 40,
  },
  inner: {
    display: "flex",
    justifyContent: "space-between",
    paddingTop: theme.spacing.xl,
    paddingBottom: theme.spacing.xl * 4,
    [theme.fn.smallerThan("sm")]: {
      flexDirection: "column-reverse",
    },
  },

  content: {
    width: "60%",
    marginRight: theme.spacing.xl * 3,

    [theme.fn.smallerThan("lg")]: {
      width: "50%",
    },
    [theme.fn.smallerThan("sm")]: {
      width: "100%",
    },
  },

  title: {
    color: theme.colorScheme === "dark" ? theme.white : theme.black,
    fontFamily: `Greycliff CF, ${theme.fontFamily}`,
    fontSize: 42,
    lineHeight: 1.2,
    fontWeight: 800,

    [theme.fn.smallerThan("xs")]: {
      fontSize: 28,
    },
  },

  control: {
    [theme.fn.smallerThan("xs")]: {
      flex: 1,
    },
  },

  aside: {
    width: "40%",
    [theme.fn.smallerThan("lg")]: {
      width: "50%",
    },
    [theme.fn.smallerThan("sm")]: {
      width: "100%",
    },
  },

  highlight: {
    position: "relative",
    backgroundColor: theme.fn.variant({
      variant: "light",
      color: theme.primaryColor,
    }).background,
    borderRadius: theme.radius.sm,
    padding: "4px 12px",
  },
  CourseContentSmall: {
    display: "none",
    [theme.fn.smallerThan("sm")]: {
      display: "block",
    },
  },

  CourseContentLarge: {
    display: "block",
    [theme.fn.smallerThan("sm")]: {
      display: "none",
    },
  },
}));

const CourseDescription = () => {
  const { classes } = useStyles();
  const { id } = useParams();
  const auth = useAuth();
  const courseStatus = useCourseStatus(id as string);
  const onPublish = async () => {
    try {
      await courseStatus.mutateAsync({
        id: id as string,
        status: CourseStatus.Published,
      });
      showNotification({
        message: "Training has been successfully published.",
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  const course = useCourseDescription(id as string);

  const enrollCourse = useEnrollCourse(id as string);
  const onEnroll = async () => {
    try {
      await enrollCourse.mutateAsync({ id: id as string });
      showNotification({ message: "Course enrolled successfully." });
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: "red" });
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
    <Center>Unable to get course details</Center>;
  }

  return (
    <div>
      <Container fluid>
        <div className={classes.inner}>
          <div className={classes.content}>
            <Title className={classes.title}>
              {course.data?.name}
              <Badge ml={10}>
                {CourseUserStatusValue[course.data?.userStatus]}
              </Badge>
              {auth?.auth && auth?.auth?.role <= UserRole.Admin && (
                <>
                  <Badge ml={10} color={"teal"}>
                    {CourseStatus[course.data?.status]}
                  </Badge>
                </>
              )}
            </Title>

            <Group my={4}>
              {course.data?.user && (
                <UserShortProfile user={course?.data?.user} size={"md"} />
              )}
            </Group>
            <RichTextEditor
              mt={20}
              color="dimmed"
              readOnly
              value={course.data?.description}
            />
          </div>
          <div className={classes.aside}>
            <AspectRatio ratio={16 / 9} mx="auto">
              <Image
                src={getCourseOgImageUrl(
                  course?.data?.user,
                  course?.data?.name,
                  course?.data.thumbnailUrl
                )}
              />
            </AspectRatio>
            <Center>
              <Group my={30}>
                {auth?.auth && auth?.auth?.role <= UserRole.Admin ? (
                  <Link
                    to={`${RoutePath.classes}/${course?.data?.slug}/${
                      course?.data?.sections &&
                      course?.data?.sections[0]?.lessons &&
                      course?.data?.sections[0]?.lessons[0]?.slug
                    }`}
                  >
                    <Button radius="xl" size="md" className={classes.control}>
                      Preview
                    </Button>
                  </Link>
                ) : course.data?.userStatus === CourseUserStatus.NotEnrolled ? (
                  <Button
                    radius="xl"
                    size="md"
                    className={classes.control}
                    loading={enrollCourse.isLoading}
                    onClick={onEnroll}
                  >
                    Enroll Course
                  </Button>
                ) : course.data?.userStatus === CourseUserStatus.Author ? (
                  <Link
                    to={`${RoutePath.classes}/${course?.data?.slug}/${
                      course?.data?.sections &&
                      course?.data?.sections[0]?.lessons &&
                      course?.data?.sections[0]?.lessons[0]?.slug
                    }`}
                  >
                    <Button radius="xl" size="md" className={classes.control}>
                      Preview
                    </Button>
                  </Link>
                ) : (
                  <Link
                    to={`${RoutePath.classes}/${course?.data?.slug}/${
                      course?.data?.sections &&
                      course?.data?.sections[0]?.lessons &&
                      course?.data?.sections[0]?.lessons[0]?.slug
                    }`}
                  >
                    <Button radius="xl" size="md" className={classes.control}>
                      Watch Course
                    </Button>
                  </Link>
                )}
                {auth?.auth &&
                  auth?.auth?.role <= UserRole.Admin &&
                  course.data?.status === CourseStatus.Review && (
                    <Button
                      onClick={() => onPublish()}
                      radius="xl"
                      size="md"
                      className={classes.control}
                    >
                      Publish
                    </Button>
                  )}
                {auth?.auth &&
                  auth?.auth?.role <= UserRole.Trainer &&
                  course.data?.status === CourseStatus.Draft && (
                    <Link to={RoutePath.manageCourse.edit(id).route}>
                      <Button radius="xl" size="md" className={classes.control}>
                        Edit
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
