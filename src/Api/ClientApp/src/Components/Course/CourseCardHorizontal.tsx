import DeleteModal from '@components/Ui/DeleteModal';
import useAuth from '@hooks/useAuth';
import {
  Anchor,
  AspectRatio,
  Badge,
  Box,
  Button,
  Card,
  Center,
  Flex,
  Group,
  Image,
  Menu,
  Text,
  useMantineTheme,
} from '@mantine/core';
import { useMediaQuery, useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import {
  IconCalendar,
  IconChevronRight,
  IconDotsVertical,
  IconGraph,
  IconSettings,
  IconTrash,
  IconUsers,
} from '@tabler/icons';
import { DATE_FORMAT, color } from '@utils/constants';
import {
  CourseLanguage,
  CourseStatus,
  CourseUserStatus,
  CourseUserStatusValue,
  UserRole,
} from '@utils/enums';
import getCourseOgImageUrl from '@utils/getCourseOGImage';
import RoutePath from '@utils/routeConstants';
import { useGeneralSetting } from '@utils/services/adminService';
import errorType from '@utils/services/axiosError';
import { ICourse, useDeleteCourse } from '@utils/services/courseService';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import classes from './styles/horizontalCard.module.css';

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
        title: t('success'),
        message: t('delete_course_success'),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        color: 'red',
        title: t('error'),
        message: error as string,
      });
    }
    setDeleteModal();
  };
  const theme = useMantineTheme();
  const matches = useMediaQuery(`(max-width: ${theme.breakpoints.md}px)`);
  const matchesSmall = useMediaQuery(`(min-width: ${theme.breakpoints.xs}px)`);
  const { t } = useTranslation();
  const auth = useAuth();

  const generalSettings = useGeneralSetting();
  const companyName = generalSettings.data?.data.companyName;
  const companyLogo = generalSettings.data?.data.logoUrl;

  return (
    <Box className={classes.container}>
      {/* <Link
        to={RoutePath.courses.description(course.slug).route}
        style={{
          textDecoration: 'none',
          zIndex: 30,
          position: 'absolute',
          height: '100%',
          width: '100%',
        }}
      ></Link> */}

      <Card
        my={10}
        radius={'md'}
        style={{
          position: 'absolute',
          top: 0,
          left: 0,
          width: '100%',
          overflow: 'visible',
        }}
      >
        <DeleteModal
          title={`${t('want_to_delete')} "${course.name}" ${t('course?')}`}
          open={deleteModal}
          onClose={setDeleteModal}
          onConfirm={handleDelete}
        />

        <Flex gap={'lg'} className={classes.inner}>
          <Center>
            <Box
              style={{
                heigh: matches ? '100px' : '300px',
                width: !matchesSmall ? '220px' : matches ? '240px' : '300px',
              }}
            >
              <AspectRatio ratio={16 / 9}>
                <Center>
                  <Image
                    src={getCourseOgImageUrl({
                      author: course.user,
                      title: course.name,
                      thumbnailUrl: course.thumbnailUrl,
                      companyName: companyName,
                      companyLogo: companyLogo,
                    })}
                    radius="md"
                    fit="cover"
                  />
                </Center>
              </AspectRatio>
            </Box>
          </Center>
          <Group
            style={{
              width: '100%',
              flexDirection: 'column',
              justifyContent: 'space-around',
              alignItems: 'stretch',
            }}
          >
            <Group style={{ justifyContent: 'space-between' }}>
              <Group gap={10}>
                <Badge color="pink" variant="light">
                  {t(`${CourseLanguage[course.language]}`)}
                </Badge>
                <Badge color="blue" variant="light">
                  {course?.levelName}
                </Badge>
                {/* {auth?.auth && auth?.auth?.role > UserRole.Admin && ( */}
                <Badge color="cyan" variant="light">
                  {t(`${CourseUserStatusValue[course.userStatus]}`)}
                </Badge>
                {/* )} */}
                {((auth?.auth && Number(auth?.auth?.role) <= UserRole.Admin) ||
                  course.userStatus === CourseUserStatus.Author ||
                  course.userStatus === CourseUserStatus.Teacher) && (
                  <>
                    <Badge
                      variant="light"
                      ml={10}
                      color={color(course?.status)}
                    >
                      {t(`${CourseStatus[course?.status]}`)}
                    </Badge>
                  </>
                )}
              </Group>
              {(course.userStatus === CourseUserStatus.Author ||
                course.userStatus === CourseUserStatus.Teacher ||
                (auth?.auth && Number(auth.auth.role) <= UserRole.Admin)) && (
                <Menu
                  shadow="md"
                  width={200}
                  trigger="hover"
                  withArrow
                  position="right"
                >
                  <Menu.Target>
                    <Button style={{ zIndex: 50 }} variant="subtle" px={4}>
                      <IconDotsVertical />
                    </Button>
                  </Menu.Target>
                  <Menu.Dropdown>
                    <Menu.Label>{t('manage')}</Menu.Label>
                    <Menu.Item
                      leftSection={<IconSettings size={14} />}
                      component={Link}
                      to={RoutePath.manageCourse.manage(course.slug).route}
                      rightSection={<IconChevronRight size={12} stroke={1.5} />}
                    >
                      {t('statistics')}
                    </Menu.Item>
                    <Menu.Item
                      leftSection={<IconGraph size={14} />}
                      component={Link}
                      to={RoutePath.manageCourse.lessonsStat(course.slug).route}
                      rightSection={<IconChevronRight size={12} stroke={1.5} />}
                    >
                      {t('lesson_stats')}
                    </Menu.Item>
                    <Menu.Item
                      leftSection={<IconUsers size={14} />}
                      component={Link}
                      to={RoutePath.manageCourse.student(course.slug).route}
                      rightSection={<IconChevronRight size={12} stroke={1.5} />}
                    >
                      {t('trainee')}
                    </Menu.Item>
                    <Menu.Divider />
                    <Menu.Item
                      c="red"
                      leftSection={<IconTrash size={14} />}
                      onClick={() => setDeleteModal()}
                    >
                      {t('delete')}
                    </Menu.Item>
                  </Menu.Dropdown>
                </Menu>
              )}
            </Group>
            <Anchor
              component={Link}
              to={RoutePath.courses.description(course.slug).route}
              size="lg"
              fw="bold"
            >
              {course.name}
            </Anchor>

            <Group gap={70}>
              {/* <Group>
                {!matches ? (
                  <Box>
                    <IconClock />
                  </Box>
                ) : (
                  <Text c="dimmed">Duration:</Text>
                )}
                <Text color={"dimmed"}>
                  {" "}
                  {moment
                    .utc(course.duration * 1000)
                    .format("H[h] mm[m] ss[s]")}
                </Text>
              </Group> */}
              <Group style={{ justifyContent: 'center', alignItems: 'center' }}>
                {!matches ? (
                  <IconCalendar />
                ) : (
                  <Text c="dimmed">{t('created_on')}</Text>
                )}
                <Text c={'dimmed'}>
                  {moment(course.createdOn).format(DATE_FORMAT)}
                </Text>{' '}
                <Badge
                  h={34}
                  title={t('group')}
                  component={Link}
                  to={'/groups/' + course.groupId}
                  style={{ maxWidth: '230px', cursor: 'pointer' }}
                  leftSection={<IconUsers size={14} />}
                  variant="light"
                >
                  {course.groupName}
                </Badge>
              </Group>
            </Group>
          </Group>
        </Flex>
      </Card>
    </Box>
  );
};

export default CourseCardHorizontal;
