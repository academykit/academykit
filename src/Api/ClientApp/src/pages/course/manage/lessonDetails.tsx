import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import {
  Box,
  Button,
  Center,
  Group,
  Loader,
  Paper,
  Table,
  Tabs,
} from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import { IconTableExport } from '@tabler/icons';
import { LessonType } from '@utils/enums';
import errorType from '@utils/services/axiosError';
import {
  exportFeedback,
  useGetFeedbackGraph,
} from '@utils/services/feedbackService';
import { downloadCSVFile } from '@utils/services/fileService';
import { useGetLessonStatisticsDetails } from '@utils/services/manageCourseService';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLocation, useParams } from 'react-router-dom';
import CourseLessonDetails from './Components/CourseLessonDetails';
import FeedbackGraphDetail from './FeedbackGraphDetail';

const LessonDetails = ({
  searchComponent,
  pagination,
  searchParams,
}: IWithSearchPagination) => {
  const { id, lessonId } = useParams();
  const { t } = useTranslation();
  const { state } = useLocation();
  const lessonDetails = useGetLessonStatisticsDetails(
    id as string,
    lessonId as string,
    searchParams
  );
  const [loading, setLoading] = useState(false);
  const chartData = useGetFeedbackGraph(lessonId as string);

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

  const exportUserCSV = `/api/course/${id}/lessonStatistics/${lessonId}/export`;

  return (
    <>
      <Tabs defaultValue="list" orientation="horizontal">
        <Tabs.List>
          <Tabs.Tab value="list">{t('list_tab')}</Tabs.Tab>
          {state?.lessonType == LessonType.Feedback && (
            <Tabs.Tab value="graph" onClick={() => chartData.refetch()}>
              {t('summary')}
            </Tabs.Tab>
          )}
        </Tabs.List>

        <Tabs.Panel value="list" pt="xs">
          {lessonDetails.data?.items[0]?.lessonType === LessonType.Feedback && (
            <Group justify="flex-end" my="md">
              <Button onClick={handleExport} loading={loading}>
                {t('export')}
              </Button>
            </Group>
          )}
          {lessonDetails.data?.items[0]?.lessonType === LessonType.Exam && (
            <Group justify="flex-end" my="md">
              <Button
                rightSection={<IconTableExport size={18} />}
                variant="outline"
                onClick={() => downloadCSVFile(exportUserCSV, 'lessonStats')}
              >
                {t('export')}
              </Button>
            </Group>
          )}
          <Paper>
            <Box mb={'sm'}>{searchComponent('Search Student')}</Box>
            {lessonDetails.data && lessonDetails.data?.items.length > 0 ? (
              <Table striped withTableBorder withColumnBorders highlightOnHover>
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>{t('trainees')}</Table.Th>
                    <Table.Th>
                      <Center>{t('status')}</Center>
                    </Table.Th>

                    <Table.Th>{t('actions')}</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {lessonDetails.data?.items.map((x) => (
                    <CourseLessonDetails
                      element={x}
                      key={x.lessonId}
                      courseId={id as string}
                    />
                  ))}
                </Table.Tbody>
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
        </Tabs.Panel>

        <Tabs.Panel value="graph" pt="xs">
          <FeedbackGraphDetail chartData={chartData} />
        </Tabs.Panel>
      </Tabs>
    </>
  );
};

export default withSearchPagination(LessonDetails);
