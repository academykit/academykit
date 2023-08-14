import useAuth from '@hooks/useAuth';
import { ActionIcon, Box, Loader, Table, Title } from '@mantine/core';
import { IconEye } from '@tabler/icons';
import { DATE_FORMAT } from '@utils/constants';
import { UserRole } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import { useMyResult } from '@utils/services/examService';
import axios from 'axios';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const UserResults = ({
  lessonId,
  studentId,
}: {
  lessonId: string;
  studentId: string;
}) => {
  const user = useAuth();
  const { t } = useTranslation();
  const result = useMyResult(lessonId, studentId);

  const hasExceededAttempt = result.data?.hasExceededAttempt;
  const endDate = result.data?.endDate;
  const exam_endDate = moment.utc(endDate, 'YYYY-MM-DD[T]HH:mm[Z]');
  const current_time = moment.utc(moment().toDate(), 'YYYY-MM-DD[T]HH:mm[Z]');

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
        sx={(theme) => ({
          ...theme.defaultGradient,
        })}
        w={'100%'}
        striped
        withBorder
        withColumnBorders
        highlightOnHover
      >
        <thead>
          <tr>
            <th>{t('obtained')}</th>
            <th>{t('submission_date')}</th>
            <th>{t('completed_duration')}</th>
            <th>{t('actions')}</th>
          </tr>
        </thead>
        <tbody>
          {result.data?.questionSetSubmissions?.map((r) => (
            <tr key={r.questionSetSubmissionId}>
              <td>{r.obtainedMarks}</td>
              <td>{moment(r.submissionDate).format(DATE_FORMAT)}</td>
              <td>{r.completeDuration}</td>
              <td>
                {(hasExceededAttempt ||
                  current_time.isAfter(exam_endDate) ||
                  user?.auth?.role == UserRole.Admin ||
                  user?.auth?.role == UserRole.SuperAdmin ||
                  user?.auth?.role == UserRole.Trainer) && (
                  <ActionIcon
                    component={Link}
                    to={
                      RoutePath.exam.resultOne(
                        lessonId,
                        r.questionSetSubmissionId
                      ).route
                    }
                  >
                    <IconEye />
                  </ActionIcon>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </>
  );
};

export default UserResults;
