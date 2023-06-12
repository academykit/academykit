import { Box, Group, SimpleGrid, Text } from "@mantine/core";
import {
  DashboardCourses,
  DashboardStats,
} from "@utils/services/dashboardService";

import { StatsCard } from "./StatsCard";
import TrainingCards from "./TrainingCards";
import { useTranslation } from "react-i18next";

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
      key: "totalUsers",
      label: t("total_users"),
      icon: "userEnrollment",
    },
    {
      key: "totalActiveUsers",
      label: t("active_users"),
      icon: "active",
    },
    {
      key: "totalGroups",
      label: t("total_groups"),
      icon: "groups",
    },
    {
      key: "totalTrainers",
      label: t("total_trainers"),
      icon: "trainers",
    },
    {
      key: "totalTrainings",
      label: t("total_trainings"),
      icon: "trainings",
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
            <StatsCard key={idx} data={x} dashboard={dashboard} />
          ))}
      </SimpleGrid>
      <Text size={"xl"} weight="bold">
        {t("my_trainings")}
      </Text>

      {dashboardCourses.length > 0 ? (
        <Text c="dimmed" mb={10}>
          {t("training_on_operations")}
        </Text>
      ) : (
        <Text c="dimmed">{t("no_trainings")}</Text>
      )}

      <Group
        sx={{
          "@media (max-width: 1345px)": {
            justifyContent: "space-evenly",
          },
          justifyContent: "start",
        }}
      >
        {dashboardCourses.length > 0 &&
          dashboardCourses.map((x, idx) => <TrainingCards data={x} />)}
      </Group>
    </div>
  );
};

export default Admin;
