import {
  Flex,
  Group,
  Paper,
  SimpleGrid,
  Text,
  useMantineColorScheme,
} from '@mantine/core';
import {
  IconActivity,
  IconCertificate,
  IconFileCheck,
} from '@tabler/icons-react';
import {
  DashboardCourses,
  DashboardStats,
} from '@utils/services/dashboardService';
import { useTranslation } from 'react-i18next';
import { StatsCard } from './StatsCard';
import TrainingCards from './TrainingCards';

const TrainerCardDual = ({ dashboard }: { dashboard: DashboardStats }) => {
  const { t } = useTranslation();
  const { colorScheme } = useMantineColorScheme();

  return (
    <Paper
      withBorder
      p="md"
      radius={'md'}
      style={{ color: colorScheme == 'dark' ? 'white' : 'black' }}
    >
      <Group wrap="nowrap">
        <IconCertificate size={26} stroke={1.5} />
        <Text size="md">{t('my_trainings')}</Text>
      </Group>
      <Group justify="space-between" wrap="nowrap" mt={10}>
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
      <Group justify="space-between" w={'80%'} m="auto">
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
  const { colorScheme } = useMantineColorScheme();

  const incomingData = [
    {
      key: 'totalGroups',
      label: t('my_groups'),
      icon: 'groups',
      signLabel: t('group'),
      pluLabel: t('groups'),
      color: colorScheme == 'dark' ? 'white' : 'black',
    },
    {
      key: 'totalEnrolledCourses',
      label: 'Total Enrollments',
      icon: 'enrollment',
      signLabel: t('enrollment'),
      pluLabel: t('enrollments'),
      color: colorScheme == 'dark' ? 'white' : 'black',
    },
  ];
  return (
    <div>
      <SimpleGrid mb={20} cols={{ xs: 1, md: 2, lg: 3 }}>
        {dashboard &&
          incomingData.map((x) => (
            <StatsCard key={x.key} data={x} dashboard={dashboard} />
          ))}
        <TrainerCardDual dashboard={dashboard} />
      </SimpleGrid>
      <Text size={'xl'} fw="bold">
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
        spacing={10}
        cols={{ base: 1, sm: 2, md: 3, 1280: 3, 1780: 4 }}
      >
        {dashboardCourses.length > 0 &&
          dashboardCourses.map((x) => <TrainingCards key={x.id} data={x} />)}
      </SimpleGrid>
    </div>
  );
};

export default Trainers;
