import Admin from '@components/Dashboard/Admin';
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
  Text,
} from '@mantine/core';
import { LessonType, UserRole } from '@utils/enums';
import {
  useDashboard,
  useDashboardCourse,
} from '@utils/services/dashboardService';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const Dashboard = () => {
  const dashboard = useDashboard();
  const dashboardCourses = useDashboardCourse();
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
          <Card>
            <Text size="lg" weight="bolder" mb={'sm'}>
              Upcoming Events
            </Text>

            <Text size="sm">No upcoming events</Text>

            <Indicator size={15} processing color="green" position="top-start">
              <Paper
                mt={10}
                p={10}
                radius={'md'}
                component={Link}
                to={'/'}
                bg={'#E9FAC8'}
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
            </Indicator>
            <Indicator
              size={15}
              processing
              disabled
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
                  Assignement Name
                </Text>
                <Text size="sm" lineClamp={2}>
                  Training Name
                </Text>
                <Group mt={'sm'}>
                  <Badge color="red" variant="outline">
                    {t(`${LessonType[LessonType.Assignment]}`)}
                  </Badge>
                  <Text size="sm">6 Aug 2023</Text>
                </Group>
              </Paper>
            </Indicator>
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
