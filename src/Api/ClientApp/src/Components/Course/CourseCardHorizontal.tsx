import DeleteModal from "@components/Ui/DeleteModal";
import useAuth from "@hooks/useAuth";
import {
  AspectRatio,
  Badge,
  Box,
  Button,
  Card,
  Center,
  Flex,
  Group,
  Image,
  TypographyStylesProvider,
  NavLink,
  Paper,
  Popover,
  Text,
  Title,
  useMantineTheme,
} from "@mantine/core";
import { useMediaQuery, useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import {
  IconCalendar,
  IconChevronRight,
  IconClock,
  IconDotsVertical,
} from "@tabler/icons";
import { color } from "@utils/constants";
import {
  CourseLanguage,
  CourseStatus,
  CourseUserStatus,
  CourseUserStatusValue,
  UserRole,
} from "@utils/enums";
import getCourseOgImageUrl from "@utils/getCourseOGImage";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import { ICourse, useDeleteCourse } from "@utils/services/courseService";
import moment from "moment";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const CourseCardHorizontal = ({
  course,
  search,
}: {
  course: ICourse;
  search: string;
}) => {
  const [deleteModal, setDeleteModal] = useToggle();
  const deleteCourse = useDeleteCourse(search);
  const handleDelete = async () => {
    try {
      await deleteCourse.mutateAsync(course.id);
      showNotification({
        title: t("success"),
        message: t("delete_course_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        color: "red",
        title: t("error"),
        message: error as string,
      });
    }
    setDeleteModal();
  };
  const theme = useMantineTheme();
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.md}px)`);
  const matchesSmall = useMediaQuery(`(min-width: ${theme.breakpoints.xs}px)`);
  const { t } = useTranslation();
  const auth = useAuth();

  return (
    <div style={{ position: "relative", height: matches ? "180px" : "350px" }}>
      <Link
        to={RoutePath.courses.description(course.slug).route}
        style={{
          textDecoration: "none",
          zIndex: 30,
          position: "absolute",
          height: "100%",
          width: "100%",
        }}
      ></Link>

      <Card
        my={10}
        radius={"md"}
        sx={{
          position: "absolute",
          top: 0,
          left: 0,
          width: "100%",
          overflow: "visible",
        }}
      >
        <DeleteModal
          title={`${t("want_to_delete")} "${course.name}" ${t("course?")}`}
          open={deleteModal}
          onClose={setDeleteModal}
          onConfirm={handleDelete}
        />

        <Group
          sx={() => ({
            flexWrap: !matches ? "wrap" : "nowrap",
            justifyContent: "center",
          })}
        >
          <Flex justify={"center"}>
            <Box
              sx={{
                heigh: matches ? "100px" : "300px",
                width: !matchesSmall ? "220px" : matches ? "240px" : "300px",
              }}
            >
              <AspectRatio ratio={16 / 9}>
                <Center>
                  <Image
                    src={getCourseOgImageUrl(
                      course.user,
                      course.name,
                      course.thumbnailUrl
                    )}
                    radius="md"
                    fit="cover"
                  />
                </Center>
              </AspectRatio>
            </Box>
          </Flex>
          <Group
            style={{
              width: "100%",
              flexDirection: "column",
              justifyContent: "space-around",
              alignItems: "stretch",
            }}
          >
            <Group sx={{ justifyContent: "space-between" }}>
              <Group spacing={10}>
                <Badge color="pink" variant="light">
                  {t(`${CourseLanguage[course.language]}`)}
                </Badge>
                <Badge color="blue" variant="light">
                  {course?.levelName}
                </Badge>
                {/* {auth?.auth && auth?.auth?.role > UserRole.Admin && ( */}
                <Badge>{CourseUserStatusValue[course.userStatus]}</Badge>
                {/* )} */}
                {((auth?.auth && auth?.auth?.role <= UserRole.Admin) ||
                  course.userStatus === CourseUserStatus.Author ||
                  course.userStatus === CourseUserStatus.Teacher) && (
                  <>
                    <Badge ml={10} color={color(course?.status)}>
                      {t(`${CourseStatus[course?.status]}`)}
                    </Badge>
                  </>
                )}
              </Group>
              {(course.userStatus === CourseUserStatus.Author ||
                course.userStatus === CourseUserStatus.Teacher ||
                (auth?.auth && auth.auth.role <= UserRole.Admin)) && (
                <Popover
                  position={"left-start"}
                  arrowSize={12}
                  styles={{
                    dropdown: { padding: 5 },
                  }}
                >
                  <Popover.Target>
                    <Button sx={{ zIndex: 50 }} variant="subtle" px={4}>
                      <IconDotsVertical />
                    </Button>
                  </Popover.Target>
                  <Popover.Dropdown>
                    <Paper>
                      <Group
                        p={0}
                        sx={{
                          flexDirection: "column",
                          alignItems: "start",
                        }}
                      >
                        <NavLink
                          variant="subtle"
                          label={t("manage")}
                          component={Link}
                          to={RoutePath.manageCourse.manage(course.slug).route}
                          rightSection={
                            <IconChevronRight size={12} stroke={1.5} />
                          }
                        ></NavLink>

                        <NavLink
                          onClick={() => setDeleteModal()}
                          variant="subtle"
                          label={t("delete")}
                          component={"button"}
                          rightSection={
                            <IconChevronRight size={12} stroke={1.5} />
                          }
                        ></NavLink>
                      </Group>
                    </Paper>
                  </Popover.Dropdown>
                </Popover>
              )}
            </Group>
            <Title size="xs" sx={{ textTransform: "uppercase" }} weight={700}>
              {course.name}
            </Title>

            <Group spacing={70}>
              {/* <Group>
                {!matches ? (
                  <Box>
                    <IconClock />
                  </Box>
                ) : (
                  <Text color="dimmed">Duration:</Text>
                )}
                <Text color={"dimmed"}>
                  {" "}
                  {moment
                    .utc(course.duration * 1000)
                    .format("H[h] mm[m] ss[s]")}
                </Text>
              </Group> */}
              <Group sx={{ justifyContent: "center", alignItems: "center" }}>
                {!matches ? (
                  <Box>
                    <IconCalendar />
                  </Box>
                ) : (
                  <Text color="dimmed">{t("created_on")}</Text>
                )}
                <Text color={"dimmed"}>
                  {moment(course.createdOn).format(theme.dateFormat)}
                </Text>
                <Text ml="sm" color={"dimmed"}>
                  {t("group")}
                </Text>
                <TypographyStylesProvider>
                  <Text lineClamp={1} color="dimmed">
                    {course.groupName}
                  </Text>
                </TypographyStylesProvider>
              </Group>
            </Group>
          </Group>
        </Group>
      </Card>
    </div>
  );
};

export default CourseCardHorizontal;
