import { Box, Group, SimpleGrid, Text } from "@mantine/core";
import {
  DashboardCourses,
  DashboardStats,
} from "@utils/services/dashboardService";

import { StatsCard } from "./StatsCard";
import TrainingCards from "./TrainingCards";

const incomingData = [
  {
    key: "totalUsers",
    label: "Total Users",
    icon: "userEnrollment",
  },
  {
    key: "totalActiveUsers",
    label: "Active Users",
    icon: "active",
  },
  {
    key: "totalGroups",
    label: "Total Groups",
    icon: "groups",
  },
  {
    key: "totalTrainers",
    label: "Total Trainers",
    icon: "trainers",
  },
  {
    key: "totalTrainings",
    label: "Total Trainings",
    icon: "trainings",
  },
];

const Admin = ({
  dashboard,
  dashboardCourses,
}: {
  dashboard: DashboardStats;
  dashboardCourses: DashboardCourses[];
}) => {
  return (
    <div>
      <SimpleGrid
        mb={20}
        cols={4}
        breakpoints={[
          { maxWidth: "md", cols: 2 },
          { maxWidth: "xs", cols: 1 },
        ]}
      >
        {dashboard &&
          incomingData.map((x, idx) => (
            //@ts-ignore
            <StatsCard key={idx} data={x} dashboard={dashboard} />
          ))}
      </SimpleGrid>
      <Text size={"xl"} weight="bold">
        My Trainings
      </Text>

      {dashboardCourses.length > 0 ? (
        <Text c="dimmed" mb={10}>
          Trainings you're moderating/operating on:
        </Text>
      ) : (
        <Text c="dimmed">You are not moderating on any Trainings.</Text>
      )}

      <Group sx={{ justifyContent: "space-evenly" }}>
        {dashboardCourses.length > 0 &&
          dashboardCourses.map((x, idx) => <TrainingCards data={x} />)}
      </Group>
    </div>
  );
};

export default Admin;
