import { Box, Flex, Group, Paper, SimpleGrid, Text } from "@mantine/core";
import { IconActivity, IconBook, IconFileCheck } from "@tabler/icons";
import {
  DashboardCourses,
  DashboardStats,
} from "@utils/services/dashboardService";
import React from "react";
import { StatsCard } from "./StatsCard";
import TrainingCards from "./TrainingCards";
import { useTranslation } from "react-i18next";

const TrainerCardDual = ({ dashboard }: { dashboard: DashboardStats }) => {
  const { t } = useTranslation();
  return (
    <Paper withBorder p="md" radius={"md"}>
      <Group position="left" noWrap>
        <IconBook size={26} stroke={1.5} />
        <Text size="md">{t("my_trainings")}</Text>
      </Group>
      <Group position="apart" noWrap mt={10}>
        <Flex>
          <IconActivity size={26} stroke={1.5} />
          <Text ml={5} size="md">
            {t("active")}
          </Text>
        </Flex>
        <Flex>
          <IconFileCheck size={26} stroke={1.5} />
          <Text ml={5} size="md">
            {t("completed")}
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
  const { t } = useTranslation();
  const incomingData = [
    {
      key: "totalGroups",
      label: t("my_groups"),
      icon: "userEnrollment",
      signLabel: t("group"),
      pluLabel: t("groups"),
    },
    {
      key: "totalEnrolledCourses",
      label: "Total Enrollments",
      icon: "enrollment",
      signLabel: t("enrollment"),
      pluLabel: t("enrollments"),
    },
  ];
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
        {t("my_training")}
      </Text>
      {dashboardCourses.length > 0 ? (
        <Text mb={10} c="dimmed">
          {t("training_moderating")}
        </Text>
      ) : (
        <Text c="dimmed">{t("not_moderating_any_trainings")}</Text>
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
