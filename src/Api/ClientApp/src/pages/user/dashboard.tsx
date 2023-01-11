import Admin from "@components/Dashboard/Admin";
import { ChartTest } from "@components/Dashboard/ChartTest";
import { StatsRing } from "@components/Dashboard/StartRing";
import Trainers from "@components/Dashboard/Trainers";
import { StatsGrid, icons, User } from "@components/Dashboard/User";
import useAuth from "@hooks/useAuth";
import { Box, Container, Group, Text } from "@mantine/core";
import { UserRole } from "@utils/enums";
import {
  useDashboard,
  useDashboardCourse,
} from "@utils/services/dashboardService";
const data: {
  title: string;
  icon: keyof typeof icons;
  value: string;
  diff: number;
}[] = [
  {
    title: "New User",
    icon: "user",
    value: "10",
    diff: 100,
  },
  {
    title: "Total Revenue",
    icon: "coin",
    value: "10",
    diff: 100,
  },
  {
    title: "COUPONS USAGE",
    icon: "discount",
    value: "10",
    diff: 100,
  },
];

const asdasd = [
  { quarter: 1, earnings: 13000 },
  { quarter: 2, earnings: 16500 },
  { quarter: 3, earnings: 14250 },
  { quarter: 4, earnings: 19000 },
];

const ringData = [
  {
    label: "PAGE VIEWS",
    stats: "This month",
    progress: 50,
    color: "red",
    icon: "up",
  },
  {
    label: "NEW USERS",
    stats: "This month",
    progress: 30,
    color: "green",
    icon: "up",
  },
  {
    label: "Orders",
    stats: "This month",
    progress: 90,
    color: "yellow",
    icon: "up",
  },
];
const Dashboard = () => {
  const dashboard = useDashboard();
  const dashboardCourses = useDashboardCourse();

  const auth = useAuth();
  const role = auth?.auth?.role;

  return (
    <Container fluid>
      <Text size="lg" weight="bolder" mb={"sm"}>
        Overview
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
