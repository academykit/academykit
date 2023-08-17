import { Flex, Group, Paper, SimpleGrid, Text } from '@mantine/core';
import { IconActivity, IconCertificate, IconFileCheck } from '@tabler/icons';
import {
  DashboardCourses,
  DashboardStats,
} from '@utils/services/dashboardService';
import { StatsCard } from './StatsCard';
import TrainingCards from './TrainingCards';
import { useTranslation } from 'react-i18next';

const TrainerCardDual = ({ dashboard }: { dashboard: DashboardStats }) => {
  const { t } = useTranslation();
  return (
    <Paper withBorder p="md" radius={'md'}>
      <Group position="left" noWrap>
        <IconCertificate size={26} stroke={1.5} />
        <Text size="md">{t('my_trainings')}</Text>
      </Group>
      <Group position="apart" noWrap mt={10}>
        <Flex>
          <IconActivity size={26} stroke={1.5} />
          <Text ml={5} size="md">
            {t('active')}
          </Text>
        </Flex>
        <Flex>
          <IconFileCheck size={26} stroke={1.5} />
          <Text ml={5} size="md">
            {t('completed')}
          </Text>
        </Flex>
      </Group>
      <Group position="apart" w={'80%'} m="auto">
        <Text>{dashboard.totalActiveTrainings}</Text>
        <Text>{dashboard.totalCompletedTrainings}</Text>
      </Group>
    </Paper>
  );
};

const Trainers = ({
  dashboard,
  dashboardCourses,
}: {
  dashboard: DashboardStats;
  dashboardCourses: DashboardCourses[];
}) => {
  const { t } = useTranslation();
  const incomingData = [
    {
      key: 'totalGroups',
      label: t('my_groups'),
      icon: 'groups',
      signLabel: t('group'),
      pluLabel: t('groups'),
      color: '#C5F6FA',
    },
    {
      key: 'totalEnrolledCourses',
      label: 'Total Enrollments',
      icon: 'enrollment',
      signLabel: t('enrollment'),
      pluLabel: t('enrollments'),
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
          incomingData.map((x) => (
            <StatsCard key={x.key} data={x} dashboard={dashboard} />
          ))}
        <TrainerCardDual dashboard={dashboard} />
      </SimpleGrid>
      <Text size={'xl'} weight="bold">
        {t('my_training')}
      </Text>
      {dashboardCourses.length > 0 ? (
        <Text mb={10} c="dimmed">
          {t('training_moderating')}
        </Text>
      ) : (
        <Text c="dimmed">{t('not_moderating_any_trainings')}</Text>
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

export default Trainers;
