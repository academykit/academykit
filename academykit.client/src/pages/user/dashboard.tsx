import Admin from '@components/Dashboard/Admin';
import EventCard from '@components/Dashboard/EventCard';
import Trainers from '@components/Dashboard/Trainers';
import { User } from '@components/Dashboard/User';
import useAuth from '@hooks/useAuth';
import { Card, Container, Grid, ScrollArea, Text } from '@mantine/core';
import { UserRole } from '@utils/enums';
import {
  useDashboard,
  useDashboardCourse,
  useUpcomingDashboardDetail,
} from '@utils/services/dashboardService';
import { useTranslation } from 'react-i18next';

const Dashboard = () => {
  const dashboard = useDashboard();
  const dashboardCourses = useDashboardCourse();
  const upcomingEvents = useUpcomingDashboardDetail();
  const hasUpcomingEvents = ((upcomingEvents.data?.length as number) ?? 0) > 0;

  const { t } = useTranslation();

  const auth = useAuth();
  const role = auth?.auth?.role;

  return (
    <Container fluid>
      <Text size="lg" fw="bolder" mb={'sm'}>
        {t('overview')}
      </Text>
      <Grid>
        <Grid.Col span={{ sm: 3 }} order={{ sm: 2 }}>
          <Card padding={10}>
            <Text size="lg" fw="bolder" mb={'sm'}>
              Upcoming Events
            </Text>
            <ScrollArea.Autosize mah={570} offsetScrollbars>
              {!hasUpcomingEvents ? (
                <Text size="sm">No upcoming events</Text>
              ) : (
                <>
                  {upcomingEvents.data?.map((event) => (
                    <EventCard
                      key={event.lessonSlug}
                      detail={event}
                    ></EventCard>
                  ))}
                </>
              )}
            </ScrollArea.Autosize>
          </Card>
        </Grid.Col>
        <Grid.Col span={{ sm: 9, xs: 12 }}>
          {dashboard.isSuccess && dashboardCourses.isSuccess && (
            <>
              {Number(role) === UserRole.Admin ||
              Number(role) === UserRole.SuperAdmin ? (
                <Admin
                  dashboard={dashboard.data}
                  dashboardCourses={dashboardCourses.data?.items}
                />
              ) : Number(role) === UserRole.Trainer ? (
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
        </Grid.Col>
      </Grid>
    </Container>
  );
};

export default Dashboard;
