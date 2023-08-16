/* eslint-disable @typescript-eslint/no-unused-vars */
import Admin from '@components/Dashboard/Admin';
import EventCard from '@components/Dashboard/EventCard';
import Trainers from '@components/Dashboard/Trainers';
import { User } from '@components/Dashboard/User';
import useAuth from '@hooks/useAuth';
import {
  Badge,
  Card,
  Container,
  Grid,
  Group,
  Indicator,
  Paper,
  ScrollArea,
  Text,
} from '@mantine/core';
import { LessonType, UserRole } from '@utils/enums';
import {
  useDashboard,
  useDashboardCourse,
  useUpcomingDashboardDetail,
} from '@utils/services/dashboardService';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const Dashboard = () => {
  const dashboard = useDashboard();
  const dashboardCourses = useDashboardCourse();
  const upcomingEvents = useUpcomingDashboardDetail();
  const hasUpcomingEvents = ((upcomingEvents.data?.length as number) ?? 0) > 0;
  console.log(hasUpcomingEvents);
  const { t } = useTranslation();

  const auth = useAuth();
  const role = auth?.auth?.role;

  return (
    <Container fluid>
      <Text size="lg" weight="bolder" mb={'sm'}>
        {t('overview')}
      </Text>
      <Grid>
        <Grid.Col sm={4} orderSm={2}>
          <Card padding={10}>
            <Text size="lg" weight="bolder" mb={'sm'}>
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
                      lessonName={event.lessonName}
                      trainingName="Training Name"
                      lessonType={event.lessonType}
                      date={event.startDate}
                    ></EventCard>
                  ))}
                </>
              )}
              {/* <Indicator
                size={15}
                processing
                color="green"
                position="top-start"
              >
                <Paper
                  mt={10}
                  p={10}
                  radius={'md'}
                  component={Link}
                  to={'/'}
                  bg={'#C5F6FA'}
                >
                  <Text size="lg" weight="bolder" lineClamp={2}>
                    Lesson Name
                  </Text>
                  <Text size="sm" lineClamp={2}>
                    Training Name
                  </Text>
                  <Group mt={'sm'}>
                    <Badge color="blue" variant="outline">
                      {t(`${LessonType[LessonType.Video]}`)}
                    </Badge>
                    <Text size="sm">5 Aug 2023</Text>
                  </Group>
                </Paper>
              </Indicator> */}
            </ScrollArea.Autosize>
          </Card>
        </Grid.Col>
        <Grid.Col sm={8} xs={12}>
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
        </Grid.Col>
      </Grid>
    </Container>
  );
};

export default Dashboard;
