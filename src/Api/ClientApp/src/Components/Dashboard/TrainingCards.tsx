import useAuth from '@hooks/useAuth';
import {
  Card,
  Text,
  Avatar,
  Menu,
  Progress,
  Group,
  createStyles,
  rem,
  ActionIcon,
  Image,
  Flex,
  Box,
} from '@mantine/core';
import {
  IconChevronRight,
  IconDotsVertical,
  IconGraph,
  IconSettings,
} from '@tabler/icons';
import { UserRole } from '@utils/enums';
import getCourseOgImageUrl from '@utils/getCourseOGImage';
import { getInitials } from '@utils/getInitialName';
import RoutePath from '@utils/routeConstants';
import { DashboardCourses } from '@utils/services/dashboardService';
import { IUser } from '@utils/services/types';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import UserShortProfile from '@components/UserShortProfile';

const useStyle = createStyles((theme) => ({
  card: {
    transition: 'transform 150ms ease, box-shadow 150ms ease',
    '&:hover': {
      transform: 'scale(1.01)',
      boxShadow: theme.shadows.md,
    },
  },
  footer: {
    padding: `${theme.spacing.xs} ${theme.spacing.lg}`,
    marginTop: theme.spacing.sm,
    borderTop: `${rem(1)} solid ${
      theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[2]
    }`,
  },
}));

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
  const { classes } = useStyle();
  return (
    <Card
      withBorder
      radius={'md'}
      p={'sm'}
      component={Link}
      to={RoutePath.courses.description(data.slug).route}
      className={classes.card}
    >
      <Flex sx={{ justifyContent: 'space-between' }}>
        <Box w="100">
          <Image
            src={getCourseOgImageUrl(data.user, data.name, data.thumbnailUrl)}
            radius="sm"
            height={100}
            width={'100%'}
            fit="contain"
          />
        </Box>
        <div>
          {(role === UserRole.Admin || role === UserRole.SuperAdmin) && (
            <Menu
              shadow="md"
              position="left"
              trigger="hover"
              withArrow
              withinPortal
              width={200}
            >
              <Menu.Target>
                <ActionIcon>
                  <IconDotsVertical size="1rem" />
                </ActionIcon>
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
              </Menu.Dropdown>
            </Menu>
          )}
        </div>
      </Flex>
      <Text lineClamp={1} size="lg" mt="md" weight="bold">
        {data.name}
      </Text>
      {role === UserRole.Trainee && (
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
        <Group position="apart" mt="xs">
          <UserShortProfile user={data.user} size="xs" />
          {role !== UserRole.Trainee && (
            <Avatar.Group spacing={'sm'}>
              {data.students.length > 0 ? (
                data.students.slice(0, 3).map((x) => {
                  return <StudentAvatar data={x} key={x.id} />;
                })
              ) : (
                <Text size="xs">{t('no_user_enrolled')}</Text>
              )}
              {data.students.length > 3 && (
                <Avatar
                  color="cyan"
                  radius="xl"
                  component={Link}
                  to={RoutePath.manageCourse.student(data.slug).route}
                >
                  +{data.students.length - 3}
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
