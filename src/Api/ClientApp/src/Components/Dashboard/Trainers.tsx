import { Box, Flex, Group, Paper, SimpleGrid, Text } from "@mantine/core";
import { IconActivity, IconBook, IconFileCheck } from "@tabler/icons";
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
];

const TrainerCardDual = ({ dashboard }: { dashboard: DashboardStats }) => {
  return (
    <Paper withBorder p="md" radius={"md"}>
      <Group position="left" noWrap>
        <IconBook size={26} stroke={1.5} />
        <Text size="md">My Trainings</Text>
      </Group>
      <Group position="apart" noWrap mt={10}>
        <Flex>
          <IconActivity size={26} stroke={1.5} />
          <Text ml={5} size="md">
            Active
          </Text>
        </Flex>
        <Flex>
          <IconFileCheck size={26} stroke={1.5} />
          <Text ml={5} size="md">
            Completed
          </Text>
        </Flex>
      </Group>
      <Group position="apart" w={"80%"} m="auto">
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
        <TrainerCardDual dashboard={dashboard} />
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
