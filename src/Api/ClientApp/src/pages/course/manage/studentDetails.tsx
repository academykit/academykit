import { Center, Paper, Table, Box, Loader } from '@mantine/core';
import { useGetStudentStatisticsDetails } from '@utils/services/manageCourseService';
import { useParams } from 'react-router-dom';
import CourseStudentLessons from './Components/CourseStudentLessons';
import { useTranslation } from 'react-i18next';

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
      <Table striped withBorder>
        <thead>
          <tr>
            <th>{t('Lesson')}</th>
            <th>
              <Center>{t('lesson_status')}</Center>
            </th>
            <th>{t('lesson_type')}</th>
            <th>{t('action')}</th>
          </tr>
        </thead>
        <tbody>
          {studentDetails.data?.map((x, i) => (
            <CourseStudentLessons
              element={x}
              key={x.lessonId + i}
              studentId={studentId ?? ''}
              courseId={id as string}
            />
          ))}
        </tbody>
      </Table>
    </Paper>
  );
};

export default StudentDetails;
