import UserShortProfile from '@components/UserShortProfile';
import useAuth from '@hooks/useAuth';
import {
    ActionIcon,
    Anchor,
    Avatar,
    Box,
    Card,
    Flex,
    Group,
    Image,
    Menu,
    Progress,
    Text,
} from '@mantine/core';
import {
    IconChevronRight,
    IconDotsVertical,
    IconGraph,
    IconSettings,
} from '@tabler/icons-react';
import { UserRole } from '@utils/enums';
import getCourseOgImageUrl from '@utils/getCourseOGImage';
import { getInitials } from '@utils/getInitialName';
import RoutePath from '@utils/routeConstants';
import { useGeneralSetting } from '@utils/services/adminService';
import { DashboardCourses } from '@utils/services/dashboardService';
import { IUser } from '@utils/services/types';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import classes from './styles/trainingCard.module.css';

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
  const generalSettings = useGeneralSetting();
  const companyName = generalSettings.data?.data.companyName;
  const companyLogo = generalSettings.data?.data.logoUrl;

  return (
    <Card withBorder radius={'md'} p={'sm'} className={classes.card}>
      <Flex style={{ justifyContent: 'space-between' }}>
        <Box w="180">
          <Image
            src={getCourseOgImageUrl({
              author: data.user,
              title: data.name,
              thumbnailUrl: data.thumbnailUrl,
              companyName: companyName,
              companyLogo: companyLogo,
            })}
            radius="sm"
            height={100}
            width={'100%'}
            fit="contain"
          />
        </Box>
        <div>
          {(Number(role) === UserRole.Admin ||
            Number(role) === UserRole.SuperAdmin) && (
            <Menu
              shadow="md"
              position="left"
              trigger="hover"
              withArrow
              withinPortal
              width={200}
            >
              <Menu.Target>
                <ActionIcon variant="subtle">
                  <IconDotsVertical size="1rem" />
                </ActionIcon>
              </Menu.Target>
              <Menu.Dropdown>
                <Menu.Label>{t('manage')}</Menu.Label>
                <Menu.Item
                  leftSection={<IconSettings size={14} />}
                  component={Link}
                  to={RoutePath.manageCourse.manage(data.slug).route}
                  rightSection={<IconChevronRight size={12} stroke={1.5} />}
                >
                  {t('statistics')}
                </Menu.Item>
                <Menu.Item
                  leftSection={<IconGraph size={14} />}
                  component={Link}
                  to={RoutePath.manageCourse.lessonsStat(data.slug).route}
                  rightSection={<IconChevronRight size={12} stroke={1.5} />}
                >
                  {t('lesson_stats')}
                </Menu.Item>
              </Menu.Dropdown>
            </Menu>
          )}
        </div>
      </Flex>
      <Anchor
        component={Link}
        to={RoutePath.courses.description(data.slug).route}
        size="lg"
        mt="md"
        truncate="end"
        maw={'100%'}
      >
        {data.name}
      </Anchor>
      {Number(role) === UserRole.Trainee && (
        <div>
          <Text c="dimmed" fz="sm" mt="md">
            {t('progress')}
          </Text>
          <Progress
            value={data.percentage}
            aria-label={t('progress') as string}
            mt={5}
            size="md"
          ></Progress>
        </div>
      )}

      <Card.Section className={classes.footer}>
        <Group justify="space-between" mt="xs">
          <UserShortProfile user={data.user} size="xs" />
          {Number(role) !== UserRole.Trainee && (
            <Avatar.Group spacing={'sm'}>
              {data.students?.length > 0 ? (
                data.students.slice(0, 3).map((x) => {
                  return <StudentAvatar data={x} key={x.id} />;
                })
              ) : (
                <Text size="xs">{t('no_user_enrolled')}</Text>
              )}
              {data.students?.length > 3 && (
                <Avatar
                  c="cyan"
                  radius="xl"
                  component={Link}
                  to={RoutePath.manageCourse.student(data.slug).route}
                >
                  +{data.students?.length - 3}
                </Avatar>
              )}
            </Avatar.Group>
          )}
        </Group>
      </Card.Section>
    </Card>
  );
};

export default TrainingCards;
