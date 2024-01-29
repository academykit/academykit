import { Box, Center, Loader, Paper, Table } from '@mantine/core';
import { useGetStudentStatisticsDetails } from '@utils/services/manageCourseService';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';
import CourseStudentLessons from './Components/CourseStudentLessons';

const StudentDetails = () => {
  const { t } = useTranslation();
  const { id, studentId } = useParams();
  const studentDetails = useGetStudentStatisticsDetails(
    id as string,
    studentId as string
  );

  if (studentDetails.isError) {
    throw studentDetails.error;
  }

  if (studentDetails.data?.length === 0)
    return <Box>{t('trainee_not_started_lesson')}</Box>;

  if (studentDetails.isLoading) return <Loader />;

  return (
    <Paper>
      <Table striped withTableBorder withColumnBorders highlightOnHover>
        <Table.Thead>
          <Table.Tr>
            <Table.Th>{t('Lesson')}</Table.Th>
            <Table.Th>
              <Center>{t('lesson_status')}</Center>
            </Table.Th>
            <Table.Th>{t('lesson_type')}</Table.Th>
            <Table.Th>{t('action')}</Table.Th>
          </Table.Tr>
        </Table.Thead>
        <Table.Tbody>
          {studentDetails.data?.map((x, i) => (
            <CourseStudentLessons
              element={x}
              key={x.lessonId + i}
              studentId={studentId ?? ''}
              courseId={id as string}
            />
          ))}
        </Table.Tbody>
      </Table>
    </Paper>
  );
};

export default StudentDetails;
