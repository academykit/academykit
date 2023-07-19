import useAuth from '@hooks/useAuth';
import {
  Box,
  Button,
  Card,
  Flex,
  Image,
  Text,
  Avatar,
  AspectRatio,
  Menu,
  Progress,
  Group,
} from '@mantine/core';
import {
  IconChevronRight,
  IconDotsVertical,
  IconGraph,
  IconSettings,
  IconUsers,
} from '@tabler/icons';
import { UserRole } from '@utils/enums';
import getCourseOgImageUrl from '@utils/getCourseOGImage';
import { getInitials } from '@utils/getInitialName';
import RoutePath from '@utils/routeConstants';
import { DashboardCourses } from '@utils/services/dashboardService';
import { IUser } from '@utils/services/types';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const StudentAvatar = ({ data }: { data: IUser }) => {
  return (
    <Avatar src={data.imageUrl} radius="xl">
      {' '}
      {!data.imageUrl && getInitials(data.fullName ?? '')}
    </Avatar>
  );
};

const TrainingCards = ({ data }: { data: DashboardCourses }) => {
  const auth = useAuth();
  const role = auth?.auth?.role;
  const { t } = useTranslation();

  return (
    <Card
      withBorder
      p={2}
      padding={0}
      component={Link}
      to={RoutePath.courses.description(data.slug).route}
    >
      <Flex sx={{ justifyContent: 'start', alignItems: 'start' }} gap={'sm'}>
        <Box sx={{ height: 65, width: 160 }}>
          <AspectRatio ratio={16 / 9}>
            <Image
              src={getCourseOgImageUrl(data.user, data.name, data.thumbnailUrl)}
              radius="sm"
              height={'100%'}
              width={'100%'}
              fit="contain"
            />
          </AspectRatio>
        </Box>
        <Box w={'100%'}>
          <Flex justify={'space-between'} sx={{ width: '100%' }}>
            <Text lineClamp={1} h={'95%'} weight="bold">
              {data.name}
            </Text>
            <div>
              {(role === UserRole.Admin || role === UserRole.SuperAdmin) && (
                <Menu
                  shadow="md"
                  width={200}
                  trigger="hover"
                  withArrow
                  position="left"
                >
                  <Menu.Target>
                    <Button sx={{ zIndex: 50 }} variant="subtle" px={4}>
                      <IconDotsVertical />
                    </Button>
                  </Menu.Target>
                  <Menu.Dropdown>
                    <Menu.Label>{t('manage')}</Menu.Label>
                    <Menu.Item
                      icon={<IconSettings size={14} />}
                      component={Link}
                      to={RoutePath.manageCourse.manage(data.slug).route}
                      rightSection={<IconChevronRight size={12} stroke={1.5} />}
                    >
                      {t('statistics')}
                    </Menu.Item>
                    <Menu.Item
                      icon={<IconGraph size={14} />}
                      component={Link}
                      to={RoutePath.manageCourse.lessonsStat(data.slug).route}
                      rightSection={<IconChevronRight size={12} stroke={1.5} />}
                    >
                      {t('lesson_stats')}
                    </Menu.Item>
                    <Menu.Item
                      icon={<IconUsers size={14} />}
                      component={Link}
                      to={RoutePath.manageCourse.student(data.slug).route}
                      rightSection={<IconChevronRight size={12} stroke={1.5} />}
                    >
                      {t('trainee')}
                    </Menu.Item>
                  </Menu.Dropdown>
                </Menu>
              )}
            </div>
          </Flex>
          {role === UserRole.Trainee ? (
            <div>
              <Text c="dimmed" fz="sm" mt="md">
                {t('progress')}
              </Text>
              <Progress value={data.percentage} mt={5} size="sm"></Progress>
            </div>
          ) : (
            <Group position="apart">
              <Avatar.Group spacing={'sm'}>
                {data.students.length > 0 ? (
                  data.students.slice(0, 3).map((x) => {
                    return <StudentAvatar data={x} key={x.id} />;
                  })
                ) : (
                  <Text size="xs">{t('no_user_enrolled')}</Text>
                )}
                {data.students.length > 3 && (
                  <Avatar radius="xl">+{data.students.length - 3}</Avatar>
                )}
              </Avatar.Group>
            </Group>
          )}
        </Box>
      </Flex>
    </Card>
  );
};

export default TrainingCards;
