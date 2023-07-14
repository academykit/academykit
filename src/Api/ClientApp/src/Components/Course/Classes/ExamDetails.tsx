/* eslint-disable prettier/prettier */
import useAuth from '@hooks/useAuth';
import {
  Box,
  Button,
  Group,
  MantineProvider,
  Text,
  Title,
} from '@mantine/core';
import UserResults from '@pages/course/exam/Components/UserResults';
import { useQueryClient } from '@tanstack/react-query';
import { DATE_FORMAT } from '@utils/constants';
import RoutePath from '@utils/routeConstants';
import { useGetCourseLesson } from '@utils/services/courseService';
import { api } from '@utils/services/service-api';
import moment from 'moment';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useSearchParams } from 'react-router-dom';

const ExamDetails = ({
  id,
  lessonId,
}: {
  id: string;
  lessonId: string | undefined;
}) => {
  const auth = useAuth();
  const queryClient = useQueryClient();
  const { t } = useTranslation();
  const [searchParams] = useSearchParams();
  const userId = auth?.auth?.id ?? '';
  const { data } = useGetCourseLesson(
    id as string,
    lessonId === '1' ? undefined : lessonId
  );
  const invalidate = searchParams.get('invalidate');

  const exam = data?.questionSet;

  useEffect(() => {
    if (invalidate) {
      queryClient.invalidateQueries([
        api.lesson.courseLesson(
          id as string,
          lessonId === '1' ? undefined : lessonId
        ),
      ]);

      window.history.pushState(
        { fromJs: true },
        '',
        `${window.location.pathname}`
      );
    }
  }, [invalidate]);

  moment.updateLocale("en", {
    relativeTime: {
      future: `${t('in')} %s`,
      past: `%s`,
      s: `%d ${t('few_seconds_ago')}`,
      ss: `%d ${t('seconds_ago')}`,
      m: `%d ${t('minute_ago')}`,
      mm: `%d ${t('minutes_ago')}`,
      h: `%d ${t('hour_ago')}`,
      hh: `%d ${t('hours_ago')}`,
      d: `%d ${t('day_ago')}`,
      dd: `%d ${t('days_ago')}`,
      M: `%d ${t('month_ago')}`,
      MM: `%d ${t('months_ago')}`,
      y: `%d ${t('year_ago')}`,
      yy: `%d ${t('years_ago')}`,
    },
  });

  return (
    <Group
      p={10}
      sx={{
        height: '70vh',
        justifyContent: 'center',
        alignItems: 'center',
      }}
    >
      {exam && (
        <Group sx={{ justifyContent: 'space-around', width: '100%' }}>
          <Box>
            <Title lineClamp={3} align="justify">
              {exam?.name}
            </Title>
            {exam?.startTime && (
              <Text>
                {t('start_date')}: {moment(exam?.startTime).format(DATE_FORMAT)}{' '}
              </Text>
            )}
            {exam?.duration ? (
              <Text>
                {t('duration')}: {exam?.duration / 60} minute(s){' '}
              </Text>
            ) : (
              ''
            )}
            <Text>
              {t('total_retake')}: {exam?.allowedRetake}
            </Text>
            <Text>
              {t('remaining_retakes')}: {data?.remainingAttempt}
            </Text>
            {exam?.negativeMarking ? (
              <Text>
                {t('negative_marking')} {exam?.negativeMarking}
              </Text>
            ) : (
              ''
            )}
          </Box>
          <div>
            <Box sx={{ overflow: 'auto', maxHeight: '60vh' }} px={10}>
              {data.hasResult && (
                <MantineProvider
                  theme={{
                    colorScheme: 'dark',
                  }}
                >
                  <UserResults lessonId={exam?.slug} studentId={userId} />
                </MantineProvider>
              )}
            </Box>
            {moment().isBetween(exam?.startTime + 'z', exam?.endTime + 'z') ? (
              <>
                {data.remainingAttempt > 0 ? (
                  <Button
                    mt={10}
                    component={Link}
                    to={RoutePath.exam?.details(exam?.slug).route}
                    state={window.location.pathname}
                  >
                    {t('start_exam')}
                  </Button>
                ) : (
                  <Text mt={15}>{t('attempt_exceeded')}</Text>
                )}
              </>
            ) : (
              <Box mt={10}>
                {moment.utc().isBefore(exam?.startTime + 'Z')
                  ? `${t('starts')} ${moment(exam?.startTime + 'Z')
                      .utc()
                      .fromNow()}`
                  : `${t('ended')} ${moment(exam?.endTime + 'Z')
                      .utc()
                      .fromNow()}`}
              </Box>
            )}
          </div>
        </Group>
      )}
    </Group>
  );
};

export default ExamDetails;
