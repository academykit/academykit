import { Group, Paper, Text } from '@mantine/core';
import {
  IconSchool,
  IconUserCheck,
  IconUsers,
  IconFileCheck,
  IconCertificate,
  IconBooks,
  IconBook2,
  IconBrandZoom,
  IconFile,
  IconUserCircle,
  IconPencil,
  IconPlayerPlay,
} from '@tabler/icons';
import {
  DashboardStats,
  DashboardStatsData,
} from '@utils/services/dashboardService';

const icons = {
  userEnrollment: IconUserCircle,
  active: IconUserCheck,
  groups: IconUsers,
  trainers: IconSchool,
  trainings: IconCertificate,
  totalGroups: IconUsers,
  completed: IconFileCheck,
  enrollment: IconCertificate,
  school: IconSchool,
  book: IconBooks,
  lecture: IconBook2,
  meeting: IconBrandZoom,
  document: IconFile,
  video: IconPlayerPlay,
  exam: IconPencil,
};

interface StatsGridProps {
  data: DashboardStatsData;
  dashboard: DashboardStats;
}

export const StatsCard = ({ data, dashboard }: StatsGridProps) => {
  const Icon = icons[data.icon as keyof typeof icons];
  const backLabel =
    dashboard && dashboard[data.key as keyof DashboardStats] > 1
      ? data.pluLabel
      : data.signLabel;

  return (
    <Paper withBorder p="md" color={data.color} radius="md">
      <Group position="left" noWrap color={data.color}>
        <Icon size={26} stroke={1.5} color={data.color} />
        <Text size="md" color={data.color}>
          {data.label}
        </Text>
      </Group>
      <Group mt={20}>
        <Text size="lg" weight={'bold'} color={data.color}>
          {dashboard && dashboard[data.key as keyof DashboardStats]}
        </Text>
        <Text color={data.color}>{backLabel}</Text>
      </Group>
    </Paper>
  );
};
