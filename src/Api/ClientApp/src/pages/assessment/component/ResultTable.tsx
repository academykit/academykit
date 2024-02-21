import { ActionIcon, Table, Title } from '@mantine/core';
import { IconEye } from '@tabler/icons';
import { DATE_FORMAT } from '@utils/constants';
import RoutePath from '@utils/routeConstants';
import { useGetStudentResult } from '@utils/services/assessmentService';
import { t } from 'i18next';
import moment from 'moment';
import { Link } from 'react-router-dom';

const Row = ({
  obtainedMark,
  submissionDate,
  completedDuration,
  questionSetSubmissionId,
  assessmentId,
}: {
  obtainedMark: string;
  submissionDate: string;
  completedDuration: string;
  questionSetSubmissionId: string;
  assessmentId: string;
}) => {
  return (
    <Table.Tr>
      <Table.Td>{obtainedMark}</Table.Td>
      <Table.Td>{moment(submissionDate).format(DATE_FORMAT)}</Table.Td>
      <Table.Td>{completedDuration}</Table.Td>
      <Table.Td>
        <ActionIcon
          variant="light"
          component={Link}
          to={
            RoutePath.assessmentExam.resultOne(
              assessmentId,
              questionSetSubmissionId
            ).route
          }
        >
          <IconEye />
        </ActionIcon>
      </Table.Td>
    </Table.Tr>
  );
};

const ResultTable = ({
  assessmentId,
  userId,
}: {
  assessmentId: string;
  userId: string;
}) => {
  const studentResult = useGetStudentResult(assessmentId, userId);

  return (
    <>
      <Title order={4} mb={10}>
        {t('previous_result')}
      </Title>

      <Table withTableBorder withColumnBorders striped>
        <Table.Thead>
          <Table.Tr>
            <Table.Th>{t('obtained')}</Table.Th>
            <Table.Th>{t('submission_date')}</Table.Th>
            <Table.Th>{t('completed_duration')}</Table.Th>
            <Table.Th>{t('action')}</Table.Th>
          </Table.Tr>
        </Table.Thead>

        <Table.Tbody>
          {studentResult.data &&
            studentResult.data.assessmentSetResultDetails &&
            studentResult.data.assessmentSetResultDetails.map((result) => (
              <Row
                key={result.questionSetSubmissionId}
                assessmentId={assessmentId}
                obtainedMark={result.obtainedMarks}
                completedDuration={result.completeDuration}
                submissionDate={result.submissionDate.toString()}
                questionSetSubmissionId={result.questionSetSubmissionId}
              />
            ))}
        </Table.Tbody>
      </Table>
    </>
  );
};

export default ResultTable;
