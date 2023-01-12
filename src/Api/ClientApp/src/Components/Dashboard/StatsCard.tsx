import { Group, Paper, Text } from "@mantine/core";
import {
  IconSchool,
  IconUserCheck,
  IconUsers,
  IconBook,
  IconFileCheck,
  IconCertificate,
  IconBooks,
  IconBook2,
  IconBrandZoom,
  IconFile,
  IconUserCircle,
} from "@tabler/icons";
import { DashboardStats } from "@utils/services/dashboardService";

const icons = {
  userEnrollment: IconUserCircle,
  active: IconUserCheck,
  groups: IconUsers,
  trainers: IconSchool,
  trainings: IconBook,
  totalGroups: IconUsers,
  completed: IconFileCheck,
  enrollment: IconCertificate,
  school: IconSchool,
  book: IconBooks,
  lecture: IconBook2,
  meeting: IconBrandZoom,
  document: IconFile,
};

interface StatsGridProps {
  data: {
    key: string;
    label: string;
    icon: keyof typeof icons;
  };
  dashboard: DashboardStats;
}

export const StatsCard = ({ data, dashboard }: StatsGridProps) => {
  const Icon = icons[data.icon];
  const splitString = data.label.split(" ");
  const backLabel = splitString[splitString.length > 2 ? 2 : 1];

  return (
    <Paper withBorder p="md" radius="md">
      <Group position="left" noWrap>
        <Icon size={26} stroke={1.5} />
        <Text size="md">{data.label}</Text>
      </Group>
      <Group mt={20}>
        <Text size="lg" weight={"bold"}>
          {dashboard && dashboard[data.key as keyof DashboardStats]}
        </Text>
        <Text>{backLabel}</Text>
      </Group>
    </Paper>
  );
};