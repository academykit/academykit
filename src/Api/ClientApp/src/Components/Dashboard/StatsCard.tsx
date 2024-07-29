import { Group, Paper, Text } from '@mantine/core';
import {
  IconBook2,
  IconBooks,
  IconBrandZoom,
  IconCertificate,
  IconFile,
  IconFileCheck,
  IconPencil,
  IconPlayerPlay,
  IconSchool,
  IconUserCheck,
  IconUserCircle,
  IconUsers,
} from '@tabler/icons-react';
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
      <Group wrap="nowrap" color={data.color}>
        <Icon size={26} stroke={1.5} color={data.color} />
        <Text size="md" c={data.color}>
          {data.label}
        </Text>
      </Group>
      <Group mt={20}>
        <Text size="lg" fw={'bold'} c={data.color}>
          {dashboard && dashboard[data.key as keyof DashboardStats]}
        </Text>
        <Text c={data.color}>{backLabel}</Text>
      </Group>
    </Paper>
  );
};
