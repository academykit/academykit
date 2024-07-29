import {
  Group,
  Paper,
  SimpleGrid,
  Text,
  useMantineColorScheme,
} from '@mantine/core';
import {
  IconArrowDownRight,
  IconArrowUpRight,
  IconCoin,
  IconDiscount2,
  IconReceipt2,
  IconUserPlus,
} from '@tabler/icons-react';
import {
  DashboardCourses,
  DashboardStats,
} from '@utils/services/dashboardService';
import { useTranslation } from 'react-i18next';
import { StatsCard } from './StatsCard';
import TrainingCards from './TrainingCards';
import classes from './styles/user.module.css';

export const icons = {
  user: IconUserPlus,
  discount: IconDiscount2,
  receipt: IconReceipt2,
  coin: IconCoin,
};

export interface StatsGridProps {
  data: {
    title: string;
    icon: keyof typeof icons;
    value: string;
    diff: number;
  }[];
}

export const User = ({
  dashboard,
  dashboardCourses,
}: {
  dashboard: DashboardStats;
  dashboardCourses: DashboardCourses[];
}) => {
  const { t } = useTranslation();
  const { colorScheme } = useMantineColorScheme();

  const incomingData = [
    {
      key: 'totalEnrolledCourses',
      label: 'Total Enrollments',
      icon: 'enrollment',
      signLabel: t('enrollment'),
      pluLabel: t('enrollments'),
      color: colorScheme == 'dark' ? 'white' : 'black',
    },
    {
      key: 'totalInProgressCourses',
      label: 'Active Trainings',
      icon: 'trainings',
      signLabel: t('training'),
      pluLabel: t('trainings'),
      color: colorScheme == 'dark' ? 'white' : 'black',
    },
    {
      key: 'totalCompletedCourses',
      label: 'My Completed Trainings',
      icon: 'completed',
      signLabel: t('training'),
      pluLabel: t('trainings'),
      color: colorScheme == 'dark' ? 'white' : 'black',
    },
  ];
  return (
    <div>
      <SimpleGrid mb={20} cols={{ base: 4, md: 2, xs: 1 }}>
        {dashboard &&
          incomingData.map((x) => (
            <StatsCard key={x.key} data={x} dashboard={dashboard} />
          ))}
      </SimpleGrid>
      <Text size={'xl'} fw={700}>
        {t('my_trainings')}
      </Text>
      {dashboardCourses.length > 0 ? (
        <Text c="dimmed" mb={10}>
          {t('my_progress')}
        </Text>
      ) : (
        <Text c="dimmed">{t('not_enrolled_training')}</Text>
      )}
      <SimpleGrid
        cols={{ base: 1, sm: 2, md: 3, '1280': 3, '1780': 4 }}
        spacing={10}
      >
        {dashboardCourses.length > 0 &&
          dashboardCourses.map((x) => <TrainingCards key={x.id} data={x} />)}
      </SimpleGrid>
    </div>
  );
};

export function StatsGrid({ data }: StatsGridProps) {
  const { t } = useTranslation();

  const stats = data.map((stat) => {
    const Icon = icons[stat.icon];
    const DiffIcon = stat.diff > 0 ? IconArrowUpRight : IconArrowDownRight;

    return (
      <Paper withBorder p="md" radius="md" key={stat.title}>
        <Group justify="space-between">
          <Text size="xs" c="dimmed" className={classes.title}>
            {stat.title}
          </Text>
          <Icon className={classes.icon} size={22} stroke={1.5} />
        </Group>

        <Group justify="flex-end" gap="xs" mt={25}>
          <Text className={classes.value}>{stat.value}</Text>
          <Text
            c={stat.diff > 0 ? 'teal' : 'red'}
            size="sm"
            className={classes.diff}
          >
            <span>{stat.diff}%</span>
            <DiffIcon size={16} stroke={1.5} />
          </Text>
        </Group>

        <Text size="xs" c="dimmed" mt={7}>
          {t('previous_month')}
        </Text>
      </Paper>
    );
  });
  return (
    <div className={classes.root}>
      <SimpleGrid cols={{ base: 4, md: 2, xs: 1 }}>{stats}</SimpleGrid>
    </div>
  );
}
