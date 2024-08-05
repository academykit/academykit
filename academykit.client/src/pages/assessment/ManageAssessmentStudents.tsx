import EmptyRow from '@components/Ui/EmptyRow';
import UserShortProfile from '@components/UserShortProfile';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
import {
  Box,
  Button,
  Center,
  Flex,
  Group,
  Modal,
  Paper,
  ScrollArea,
  Table,
  Tooltip,
} from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { IconEye, IconTableExport } from '@tabler/icons-react';
import {
  IAssessmentResult,
  useGetAllResults,
} from '@utils/services/assessmentService';
import { downloadCSVFile } from '@utils/services/fileService';
import { t } from 'i18next';
import { useParams } from 'react-router-dom';
import ResultTable from './component/ResultTable';

const Row = ({ data }: { data: IAssessmentResult }) => {
  const [resultModal, toggleResultModal] = useToggle();

  return (
    <>
      <Modal
        opened={resultModal}
        onClose={() => toggleResultModal()}
        size={'lg'}
      >
        <ResultTable assessmentId={data.assessmentId} userId={data.user.id} />
      </Modal>

      <Table.Tr>
        <Table.Td>
          <UserShortProfile user={data.user} size="sm" />
        </Table.Td>
        <Table.Td>{data.obtainedMarks}</Table.Td>
        <Table.Td>
          <Center>
            <Tooltip
              style={{
                maxWidth: '400px',
                textOverflow: 'ellipsis',
                overflow: 'hidden',
              }}
              label={t('view_result')}
            >
              <Button variant="subtle" onClick={() => toggleResultModal()}>
                <IconEye />
              </Button>
            </Tooltip>
          </Center>
        </Table.Td>
      </Table.Tr>
    </>
  );
};

const ManageAssessmentStudents = ({
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  const params = useParams();
  const user = useAuth();
  const getStudentResults = useGetAllResults(params.id as string);

  const exportUserCSVSubmission = `/api/assessmentExam/${params.id}/GetStudentResults/${user?.auth?.id}/Export`;
  return (
    <>
      <Flex>
        <Box mx={3} style={{ width: '100%' }}>
          {searchComponent(t('search_trainees') as string)}
        </Box>
      </Flex>
      <Group justify="flex-end" my="md">
        <Button
          rightSection={<IconTableExport size={18} />}
          variant="outline"
          onClick={() =>
            downloadCSVFile(exportUserCSVSubmission, 'AssessmentStats')
          }
        >
          {t('export')}
        </Button>
      </Group>
      <Paper mt={10}>
        <ScrollArea>
          <Table
            style={{ minWidth: 800 }}
            verticalSpacing="xs"
            striped
            highlightOnHover
            withTableBorder
            withColumnBorders
          >
            <Table.Thead>
              <Table.Tr>
                <Table.Th>{t('name')}</Table.Th>
                <Table.Th>{t('obtained_marks')}</Table.Th>
                <Table.Th style={{ textAlign: 'center' }}>
                  {t('actions')}
                </Table.Th>
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {getStudentResults.data &&
              getStudentResults.data.totalCount > 0 ? (
                getStudentResults.data?.items.map((item) => (
                  <Row key={item.id} data={item} />
                ))
              ) : (
                <EmptyRow colspan={3} message="no_trainee" />
              )}
            </Table.Tbody>
          </Table>
        </ScrollArea>

        {getStudentResults.data &&
          pagination(
            getStudentResults.data?.totalPage,
            getStudentResults.data?.items.length
          )}
      </Paper>
    </>
  );
};

export default withSearchPagination(ManageAssessmentStudents);
