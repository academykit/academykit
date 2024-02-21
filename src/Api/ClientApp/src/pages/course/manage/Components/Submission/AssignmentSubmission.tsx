import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import { Box, Paper, Table } from '@mantine/core';
import { useGetAssignmentSubmission } from '@utils/services/assignmentService';
import { t } from 'i18next';
import moment from 'moment';
import { useParams } from 'react-router-dom';

const AssignmentSubmission = ({
  searchComponent,
  sortComponent,
  searchParams,
  pagination,
}: IWithSearchPagination) => {
  const params = useParams();
  const { data, isLoading, isError } = useGetAssignmentSubmission(
    params.id as string,
    params.lessonId as string,
    searchParams
  );

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (isError || !data) {
    return <div>Error fetching data</div>;
  }

  return (
    <>
      <Paper>
        <Box mb={'sm'}>{searchComponent('Search Student')}</Box>
        <Table striped withTableBorder withColumnBorders highlightOnHover>
          <Table.Thead>
            <Table.Tr>
              <Table.Th>{t('s_no')}</Table.Th>
              <Table.Th>
                {sortComponent({
                  sortKey: 'firstName',
                  title: t('trainee_name'),
                })}
              </Table.Th>

              <Table.Th>{t('submitted_date')}</Table.Th>
              <Table.Th>{t('obtained')}</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {data.items.map((submission, index) => (
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
        {data && pagination(data?.totalPage, data?.items.length)}
      </Paper>
    </>
  );
};

export default withSearchPagination(AssignmentSubmission);
