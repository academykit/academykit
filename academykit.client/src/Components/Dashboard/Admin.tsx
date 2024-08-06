import { SimpleGrid, Text, useMantineColorScheme } from "@mantine/core";
import {
  DashboardCourses,
  DashboardStats,
} from "@utils/services/dashboardService";

import { useTranslation } from "react-i18next";
import { StatsCard } from "./StatsCard";
import TrainingCards from "./TrainingCards";

const Admin = ({
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
      key: "totalUsers",
      label: t("total_users"),
      icon: "userEnrollment",
      signLabel: t("user"),
      pluLabel: t("users"),
      color: colorScheme == "dark" ? "white" : "black",
    },
    {
      key: "totalActiveUsers",
      label: t("active_users"),
      icon: "active",
      signLabel: t("user"),
      pluLabel: t("users"),
      color: colorScheme == "dark" ? "white" : "black",
    },
    {
      key: "totalGroups",
      label: t("total_groups"),
      icon: "groups",
      signLabel: t("group"),
      pluLabel: t("groups"),
      color: colorScheme == "dark" ? "white" : "black",
    },
    {
      key: "totalTrainers",
      label: t("total_trainers"),
      icon: "trainers",
      signLabel: t("trainer"),
      pluLabel: t("trainers"),
      color: colorScheme == "dark" ? "white" : "black",
    },
    {
      key: "totalTrainings",
      label: t("total_trainings"),
      icon: "trainings",
      signLabel: t("training"),
      pluLabel: t("trainings"),
      color: colorScheme == "dark" ? "white" : "black",
    },
  ];

  return (
    <div>
      <SimpleGrid mb={20} cols={{ base: 1, md: 2, lg: 4 }}>
        {dashboard &&
          incomingData.map((x, idx) => (
            <StatsCard key={idx} data={x} dashboard={dashboard} />
          ))}
      </SimpleGrid>
      <Text size={"xl"} fw="bold">
        {t("my_trainings")}
      </Text>

      {dashboardCourses.length > 0 ? (
        <Text c="dimmed" mb={10}>
          {t("training_on_operations")}
        </Text>
      ) : (
        <Text c="dimmed">{t("no_trainings")}</Text>
      )}

      <SimpleGrid spacing={10} cols={{ base: 1, sm: 2, md: 3, clg: 3, cxl: 4 }}>
        {dashboardCourses.length > 0 &&
          dashboardCourses.map((x) => <TrainingCards key={x.id} data={x} />)}
      </SimpleGrid>
    </div>
  );
};

export default Admin;
