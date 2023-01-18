import { Box, SimpleGrid, Text } from "@mantine/core";
import {
  DashboardCourses,
  DashboardStats,
} from "@utils/services/dashboardService";
import React from "react";
import { StatsCard } from "./StatsCard";
import TrainingCards from "./TrainingCards";

const incomingData = [
  {
    key: "totalGroups",
    label: "My Groups",
    icon: "userEnrollment",
  },
  {
    key: "totalActiveTrainings",
    label: "My Active Trainings",
    icon: "trainings",
  },
  {
    key: "totalCompletedTrainings",
    label: "My Completed Trainings",
    icon: "active",
  },
];

const Trainers = ({
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
            <StatsCard key={x.key} data={x} dashboard={dashboard} />
          ))}
      </SimpleGrid>
      <Text size={"xl"} weight="bold">
        My Training
      </Text>
      {dashboardCourses.length > 0 ? (
        <Text mb={10} c="dimmed">
          Trainings you are moderating on:
        </Text>
      ) : (
        <Text c="dimmed">You are not Moderating on any Trainings.</Text>
      )}

      <SimpleGrid
        cols={3}
        breakpoints={[
          { maxWidth: "md", cols: 2 },
          { maxWidth: "xs", cols: 1 },
        ]}
      >
        {dashboardCourses.length > 0 &&
          dashboardCourses.map((x, idx) => (
            <TrainingCards key={x.id} data={x} />
          ))}
      </SimpleGrid>
    </div>
  );
};

export default Trainers;
