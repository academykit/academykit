import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import { Box, Button, Group, Paper, Table } from '@mantine/core';
import { IconTableExport } from '@tabler/icons';
import { useGetExamSubmission } from '@utils/services/examService';
import { downloadCSVFile } from '@utils/services/fileService';
import { t } from 'i18next';
import moment from 'moment';
import { useParams } from 'react-router-dom';

const ExamSubmission = ({
  searchComponent,
  sortComponent,
  courseIdentity,
  lessonId,
}: IWithSearchPagination & { courseIdentity: string; lessonId: string }) => {
  const { id } = useParams();

  const { data, isLoading, isError } = useGetExamSubmission(
    courseIdentity,
    lessonId
  );

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (isError || !data) {
    return <div>Error fetching data</div>;
  }

  const exportUserCSVSubmission = `/api/course/${id}/lessonStatistics/${lessonId}/export`;

  return (
    <>
      <Group justify="flex-end" my="md">
        <Button
          rightSection={<IconTableExport size={18} />}
          variant="outline"
          onClick={() =>
            downloadCSVFile(exportUserCSVSubmission, 'lessonStatsSubmission')
          }
        >
          {t('export')}
        </Button>
      </Group>
      <Paper>
        <Box mb={'sm'}>{searchComponent('Search Student')}</Box>
        <Table striped withTableBorder withColumnBorders highlightOnHover>
          <Table.Thead>
            <Table.Tr>
              <Table.Th>{t('s_no')}</Table.Th>
              <Table.Th>
                {sortComponent({
                  sortKey: 'fullName',
                  title: t('trainee_name'),
                })}
              </Table.Th>
              <Table.Th>{t('submitted_date')}</Table.Th>
              <Table.Th>{t('obtained')}</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {data.map((submission, index) => (
              <Table.Tr key={index}>
                <Table.Td>{index + 1}</Table.Td>
                <Table.Td>
                  {submission.student?.fullName || 'Full Name Not Available'}
                </Table.Td>
                <Table.Td>
                  {moment(submission.submissionDate).format(
                    'MMMM D, YYYY h:mm:ss A'
                  )}
                </Table.Td>
                <Table.Td>
                  {submission.totalMarks !== undefined
                    ? submission.totalMarks.toFixed(2)
                    : 'N/A'}
                </Table.Td>
              </Table.Tr>
            ))}
          </Table.Tbody>
        </Table>
      </Paper>
    </>
  );
};

export default withSearchPagination(ExamSubmission);
