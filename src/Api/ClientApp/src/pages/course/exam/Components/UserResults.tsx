import useAuth from '@hooks/useAuth';
import {
  ActionIcon,
  Box,
  Loader,
  Table,
  Title,
  useMantineTheme,
} from '@mantine/core';
import { IconEye } from '@tabler/icons';
import { DATE_FORMAT } from '@utils/constants';
import { UserRole } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import { useMyResult } from '@utils/services/examService';
import axios from 'axios';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { Link, useLocation } from 'react-router-dom';

const UserResults = ({
  lessonId,
  studentId,
  isTrainee,
}: {
  lessonId: string;
  studentId: string;
  isTrainee: boolean;
}) => {
  const user = useAuth();
  const { t } = useTranslation();
  const result = useMyResult(lessonId, studentId);
  const theme = useMantineTheme();

  const hasExceededAttempt = result.data?.hasExceededAttempt;
  const endDate = result.data?.endDate;
  const exam_endDate = moment.utc(endDate, 'YYYY-MM-DD[T]HH:mm[Z]');
  const current_time = moment.utc(moment().toDate(), 'YYYY-MM-DD[T]HH:mm[Z]');
  const location = useLocation();

  if (result.isLoading) {
    return <Loader />;
  }
  if (result.isError) {
    if (axios.isAxiosError(result.error)) {
      if (result.error.response?.data) {
        return <Box>{(result.error.response?.data as any)?.message}</Box>;
      } else if (result.error.response?.status === 404) {
        return <Box> {t('no_result')}</Box>;
      } else {
        return <div>{t('something_wrong')}</div>;
      }
    }
  }
  if (result.data?.attemptCount === 0) {
    return (
      <Title my={10} size={'sm'}>
        {t('no_previous_attempt')}
      </Title>
    );
  }
  return (
    <>
      <Title mt={20}> {t('previous_result')}</Title>
      <Table
        styles={{
          td: {
            backgroundColor: location.pathname.includes('lessons-stat')
              ? ''
              : theme.colors.gray[9],
          },
        }}
        w={'100%'}
        striped
        withTableBorder
        withColumnBorders
        highlightOnHover
      >
        <Table.Thead>
          <Table.Tr>
            <Table.Th>{t('obtained')}</Table.Th>
            <Table.Th>{t('submission_date')}</Table.Th>
            <Table.Th>{t('completed_duration')}</Table.Th>
            <Table.Th>{t('actions')}</Table.Th>
          </Table.Tr>
        </Table.Thead>
        <Table.Tbody>
          {result.data?.questionSetSubmissions?.map((r) => (
            <Table.Tr key={r.questionSetSubmissionId}>
              <Table.Td>{r.obtainedMarks}</Table.Td>
              <Table.Td>
                {moment(r.submissionDate).format(DATE_FORMAT)}
              </Table.Td>
              <Table.Td>{r.completeDuration}</Table.Td>
              <Table.Td>
                {(hasExceededAttempt ||
                  current_time.isAfter(exam_endDate) ||
                  Number(user?.auth?.role) == UserRole.Admin ||
                  Number(user?.auth?.role) == UserRole.SuperAdmin ||
                  // trainer who is not trainee
                  (Number(user?.auth?.role) == UserRole.Trainer &&
                    !isTrainee)) && (
                  <ActionIcon
                    component={Link}
                    to={
                      RoutePath.exam.resultOne(
                        lessonId,
                        r.questionSetSubmissionId
                      ).route
                    }
                    variant="light"
                  >
                    <IconEye />
                  </ActionIcon>
                )}
              </Table.Td>
            </Table.Tr>
          ))}
        </Table.Tbody>
      </Table>
    </>
  );
};

export default UserResults;
