import { SimpleGrid, Text } from '@mantine/core';
import {
  DashboardCourses,
  DashboardStats,
} from '@utils/services/dashboardService';

import { StatsCard } from './StatsCard';
import TrainingCards from './TrainingCards';
import { useTranslation } from 'react-i18next';

const Admin = ({
  dashboard,
  dashboardCourses,
}: {
  dashboard: DashboardStats;
  dashboardCourses: DashboardCourses[];
}) => {
  const { t } = useTranslation();

  const incomingData = [
    {
      key: 'totalUsers',
      label: t('total_users'),
      icon: 'userEnrollment',
      signLabel: t('user'),
      pluLabel: t('users'),
      color: '#E5DBFF',
    },
    {
      key: 'totalActiveUsers',
      label: t('active_users'),
      icon: 'active',
      signLabel: t('user'),
      pluLabel: t('users'),
      color: '#63E6BE',
    },
    {
      key: 'totalGroups',
      label: t('total_groups'),
      icon: 'groups',
      signLabel: t('group'),
      pluLabel: t('groups'),
      color: '#C5F6FA',
    },
    {
      key: 'totalTrainers',
      label: t('total_trainers'),
      icon: 'trainers',
      signLabel: t('trainer'),
      pluLabel: t('trainers'),
      color: '#FFE8CC',
    },
    {
      key: 'totalTrainings',
      label: t('total_trainings'),
      icon: 'trainings',
      signLabel: t('training'),
      pluLabel: t('trainings'),
      color: '#E9FAC8',
    },
  ];

  return (
    <div>
      <SimpleGrid
        mb={20}
        cols={4}
        breakpoints={[
          { maxWidth: 'md', cols: 2 },
          { maxWidth: 'xs', cols: 1 },
        ]}
      >
        {dashboard &&
          incomingData.map((x, idx) => (
            <StatsCard key={idx} data={x} dashboard={dashboard} />
          ))}
      </SimpleGrid>
      <Text size={'xl'} weight="bold">
        {t('my_trainings')}
      </Text>

      {dashboardCourses.length > 0 ? (
        <Text c="dimmed" mb={10}>
          {t('training_on_operations')}
        </Text>
      ) : (
        <Text c="dimmed">{t('no_trainings')}</Text>
      )}

      <SimpleGrid
        cols={1}
        spacing={10}
        breakpoints={[
          { minWidth: 'sx', cols: 1 },
          { minWidth: 'sm', cols: 2 },
          { minWidth: 'md', cols: 3 },
          { minWidth: 1280, cols: 3 },
          { minWidth: 1780, cols: 4 },
        ]}
      >
        {dashboardCourses.length > 0 &&
          dashboardCourses.map((x) => <TrainingCards key={x.id} data={x} />)}
      </SimpleGrid>
    </div>
  );
};

export default Admin;
