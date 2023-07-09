import Admin from "@components/Dashboard/Admin";
import Trainers from "@components/Dashboard/Trainers";
import { User } from "@components/Dashboard/User";
import useAuth from "@hooks/useAuth";
import { Container, Text, Select } from "@mantine/core";
import { UserRole } from "@utils/enums";
import {
  useDashboard,
  useDashboardCourse,
} from "@utils/services/dashboardService";
import { useTranslation } from "react-i18next";

const Dashboard = () => {
  const dashboard = useDashboard();
  const dashboardCourses = useDashboardCourse();
  const { t } = useTranslation();

  const auth = useAuth();
  const role = auth?.auth?.role;

  return (
    <Container fluid>
      <Text size="lg" weight="bolder" mb={"sm"}>
        {t("overview")}
      </Text>
      {dashboard.isSuccess && dashboardCourses.isSuccess && (
        <>
          {role === UserRole.Admin || role === UserRole.SuperAdmin ? (
            <Admin
              dashboard={dashboard.data}
              dashboardCourses={dashboardCourses.data?.items}
            />
          ) : role === UserRole.Trainer ? (
            <Trainers
              dashboard={dashboard.data}
              dashboardCourses={dashboardCourses.data?.items}
            />
          ) : (
            <User
              dashboard={dashboard.data}
              dashboardCourses={dashboardCourses.data?.items}
            />
          )}
        </>
      )}
    </Container>
  );
};

export default Dashboard;
