import { Box, Grid, Paper, SimpleGrid } from '@mantine/core';
import { useGetAssignmentSummary } from '@utils/services/assignmentService';
import { useTranslation } from 'react-i18next';
import Ranking from '../Ranking';
import SummaryCard from '../SummaryCard';
import TopThreePerformers from './TopThreePerformers';

interface AssignmentSummaryProps {
  courseIdentity: string;
  lessonId: string;
}

const AssignmentSummary = ({
  courseIdentity,
  lessonId,
}: AssignmentSummaryProps) => {
  const { t } = useTranslation();
  const { data, isLoading, isError } = useGetAssignmentSummary(
    courseIdentity,
    lessonId
  );

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (isError) {
    return <div>Error fetching data</div>;
  }

  const { weekStudents, topStudents, assignmentStatus, mostWrongAnsQues } =
    data;

  const [firstStudent, secondStudent, thirdStudent, ...otherStudents] =
    topStudents;

  return (
    <>
      <Box>
        <SimpleGrid
          cols={{ base: 1, sm: 2, lg: 5 }}
          spacing={{ base: 5, sm: 'xl' }}
          verticalSpacing={{ base: 'md', sm: 'xl' }}
          mb={15}
        >
          <SummaryCard
            title={t('total_attendees')}
            count={assignmentStatus.totalAttend}
          />
          <SummaryCard
            title={t('total_passed')}
            count={assignmentStatus.totalPass}
          />
          <SummaryCard
            title={t('total_failed')}
            count={assignmentStatus.totalFail}
          />
          <SummaryCard
            title={t('average')}
            count={Number(assignmentStatus.averageMarks.toPrecision(3))}
          />
        </SimpleGrid>

        <Grid>
          <Grid.Col
            span={6}
            order={{ base: 2, sm: 2, lg: 1 }}
            miw={{ base: '100%', sm: '100%', md: '50%', lg: '50%' }}
          >
            <Box h={'100%'}>
              <Ranking
                title="most_unanswered_questions"
                data={mostWrongAnsQues}
              />
              <Ranking title="weak_students" data={weekStudents.reverse()} />
            </Box>
          </Grid.Col>

          <Grid.Col
            span={6}
            order={{ base: 1, sm: 1, lg: 2 }}
            miw={{ base: '100%', sm: '100%', md: '50%', lg: '50%' }}
          >
            <TopThreePerformers
              students={[firstStudent, secondStudent, thirdStudent]}
            />

            <Paper p={'md'} mb={10}>
              <ol start={4} style={{ paddingLeft: '18px', margin: 0 }}>
                {otherStudents.map((student) => (
                  <li key={student.id}>{student.fullName}</li>
                ))}
              </ol>
            </Paper>
          </Grid.Col>
        </Grid>
      </Box>
    </>
  );
};

export default AssignmentSummary;
