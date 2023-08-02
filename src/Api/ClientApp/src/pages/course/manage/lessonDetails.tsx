import {
  Button,
  Center,
  Group,
  Paper,
  Table,
  Box,
  Loader,
} from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import { LessonType } from '@utils/enums';
import errorType from '@utils/services/axiosError';
import { exportFeedback } from '@utils/services/feedbackService';
import { useGetLessonStatisticsDetails } from '@utils/services/manageCourseService';
import { useState } from 'react';
import { useParams } from 'react-router-dom';
import CourseLessonDetails from './Components/CourseLessonDetails';
import { useTranslation } from 'react-i18next';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';

const LessonDetails = ({
  searchComponent,
  pagination,
  searchParams,
}: IWithSearchPagination) => {
  const { id, lessonId } = useParams();
  const { t } = useTranslation();

  const lessonDetails = useGetLessonStatisticsDetails(
    id as string,
    lessonId as string,
    searchParams
  );
  const [loading, setLoading] = useState(false);

  if (lessonDetails.isLoading) return <Loader />;

  if (lessonDetails.error) throw lessonDetails.error;

  const handleExport = async () => {
    setLoading(true);
    try {
      const res = await exportFeedback(lessonId as string);

      const element = document.createElement('a');
      setLoading(false);

      element.setAttribute(
        'href',
        'data:text/plain;charset=utf-8,' +
          encodeURIComponent(res.data as string)
      );
      element.setAttribute('download', 'Feedback-' + lessonId + '.csv');
      document.body.appendChild(element);
      element.click();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        title: t('error'),
        color: 'red',
      });
    }
    setLoading(false);
  };

  return (
    <>
      {lessonDetails.data?.items[0]?.lessonType === LessonType.Feedback && (
        <Group position="right" my="md">
          <Button onClick={handleExport} loading={loading}>
            {t('export')}
          </Button>
        </Group>
      )}
      <Paper>
        <Box mb={'sm'}>{searchComponent('Search Student')}</Box>
        {lessonDetails.data && lessonDetails.data?.items.length > 0 ? (
          <Table striped withBorder>
            <thead>
              <tr>
                <th>{t('trainees')}</th>
                <th>
                  <Center>{t('status')}</Center>
                </th>

                <th>{t('actions')}</th>
              </tr>
            </thead>
            <tbody>
              {lessonDetails.data?.items.map((x) => (
                <CourseLessonDetails
                  element={x}
                  key={x.lessonId}
                  courseId={id as string}
                />
              ))}
            </tbody>
          </Table>
        ) : (
          <Box>{t('no_enrolled_student_found')}</Box>
        )}
        {lessonDetails.data &&
          pagination(
            lessonDetails.data.totalPage,
            lessonDetails.data?.items.length
          )}
      </Paper>
    </>
  );
};

export default withSearchPagination(LessonDetails);
