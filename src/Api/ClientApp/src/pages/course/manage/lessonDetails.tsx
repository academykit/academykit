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
  Title,
} from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import { IconChevronLeft, IconTableExport } from '@tabler/icons';
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
import { useLocation, useNavigate, useParams } from 'react-router-dom';
import CourseLessonDetails from './Components/CourseLessonDetails';
import AssignmentSubmission from './Components/Submission/AssignmentSubmission';
import ExamSubmission from './Components/Submission/ExamSubmission';
import AssignmentSummary from './Components/Summary/AssignmentSummary';
import ExamSummary from './Components/Summary/ExamSummary';
import FeedbackGraphDetail from './FeedbackGraphDetail';

const Visibility = ({
  children,
  view,
  lessonType,
}: {
  children: JSX.Element;
  view: number;
  lessonType: LessonType;
}) => {
  if (view === lessonType) {
    return children;
  }
};

const getDefaultValue = (lessonType: LessonType) => {
  if (lessonType === LessonType.Exam) {
    return 'summary-exam';
  } else if (lessonType === LessonType.Feedback) {
    return 'list';
  } else if (lessonType === LessonType.Assignment) {
    return 'summary-assignment';
  } else {
    return 'list';
  }
};

const LessonDetails = ({
  searchComponent,
  pagination,
  searchParams,
}: IWithSearchPagination) => {
  const { id, lessonId } = useParams();
  const navigate = useNavigate();
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
  const exportIndividualCSV = `/api/assignment/${lessonId}/AssignmentIndividualExport`;

  return (
    <>
      <Button
        leftSection={<IconChevronLeft size={16} />}
        variant="outline"
        onClick={() => navigate(-1)}
      >
        Go back
      </Button>
      <Title ta={'center'}>{state?.lessonName}</Title>

      <Tabs
        defaultValue={getDefaultValue(state?.lessonType)}
        orientation="horizontal"
      >
        <Tabs.List>
          <Visibility view={state?.lessonType} lessonType={LessonType.Exam}>
            <Tabs.Tab value="summary-exam">{t('summary')}</Tabs.Tab>
          </Visibility>
          <Visibility
            view={state?.lessonType}
            lessonType={LessonType.Assignment}
          >
            <Tabs.Tab value="summary-assignment">{t('summary')}</Tabs.Tab>
          </Visibility>

          <Tabs.Tab value="list">{t('individual_tab')}</Tabs.Tab>

          <Visibility view={state?.lessonType} lessonType={LessonType.Feedback}>
            <Tabs.Tab value="graph" onClick={() => chartData.refetch()}>
              {t('summary')}
            </Tabs.Tab>
          </Visibility>

          <Visibility view={state?.lessonType} lessonType={LessonType.Exam}>
            <Tabs.Tab value="submission-exam">{t('submission')}</Tabs.Tab>
          </Visibility>

          <Visibility
            view={state?.lessonType}
            lessonType={LessonType.Assignment}
          >
            <Tabs.Tab value="submission-assignment">{t('submission')}</Tabs.Tab>
          </Visibility>
        </Tabs.List>

        <Tabs.Panel value="summary-exam" pt="xs">
          <ExamSummary
            courseIdentity={id as string}
            lessonId={lessonId as string}
          />
        </Tabs.Panel>

        <Tabs.Panel value="submission-exam" pt="xs">
          <ExamSubmission
            courseIdentity={id as string}
            lessonId={lessonId as string}
          />
        </Tabs.Panel>

        <Tabs.Panel value="summary-assignment" pt="xs">
          <AssignmentSummary
            courseIdentity={id as string}
            lessonId={lessonId as string}
          />
        </Tabs.Panel>

        <Tabs.Panel value="submission-assignment" pt="xs">
          <AssignmentSubmission />
        </Tabs.Panel>

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
          {lessonDetails.data?.items[0]?.lessonType ===
            LessonType.Assignment && (
            <Group justify="flex-end" my="md">
              <Button
                rightSection={<IconTableExport size={18} />}
                variant="outline"
                onClick={() =>
                  downloadCSVFile(
                    exportIndividualCSV,
                    'AssignmentIndividualStats'
                  )
                }
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
                  {lessonDetails.data?.items.map((x, index) => (
                    <CourseLessonDetails
                      element={x}
                      key={index}
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
