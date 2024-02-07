import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import { Box, Paper, Table } from '@mantine/core';
import { t } from 'i18next';

const AssignmentSubmission = ({
  searchComponent,
  sortComponent,
}: IWithSearchPagination) => {
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
          <Table.Tbody></Table.Tbody>
        </Table>
      </Paper>
    </>
  );
};

export default withSearchPagination(AssignmentSubmission);
