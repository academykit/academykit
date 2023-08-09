import { Button, Group, Title, Text } from '@mantine/core';
import { ICourseLesson } from '@utils/services/courseService';
import moment from 'moment';
import { useTranslation } from 'react-i18next';

const Meetings = ({ data }: { data: ICourseLesson }) => {
  const EndTime = moment
    .utc(data.meeting.startDate)
    .add(data.meeting.duration, 'seconds');
  const StartTime = moment.utc(data.meeting.startDate);
  const { t } = useTranslation();

  // render 'in [time] [s/min/hr/...]' if time left
  // render '[time] [s/min/hr/...] ago' if meeting ended
  moment.updateLocale('en', {
    relativeTime: {
      future: `${t('in')} %s`,
      past: `%s`,
      s: `%d ${t('in_second')}`,
      ss: `%d ${t('in_seconds')}`,
      m: moment().isBefore(StartTime)
        ? `%d ${t('in_minute')}`
        : `%d ${t('minute_ago')}`,
      mm: moment().isBefore(StartTime)
        ? `%d ${t('in_minutes')}`
        : `%d ${t('minutes_ago')}`,
      h: moment().isBefore(StartTime)
        ? `%d ${t('in_hour')}`
        : `%d ${t('hour_ago')}`,
      hh: moment().isBefore(StartTime)
        ? `%d ${t('in_hours')}`
        : `%d ${t('hours_ago')}`,
      d: moment().isBefore(StartTime)
        ? `%d ${t('in_day')}`
        : `%d ${t('day_ago')}`,
      dd: moment().isBefore(StartTime)
        ? `%d ${t('in_days')}`
        : `%d ${t('days_ago')}`,
      M: moment().isBefore(StartTime)
        ? `%d ${t('in_month')}`
        : `%d ${t('month_ago')}`,
      MM: moment().isBefore(StartTime)
        ? `%d ${t('in_months')}`
        : `%d ${t('months_ago')}`,
      y: moment().isBefore(StartTime)
        ? `%d ${t('in_year')}`
        : `%d ${t('year_ago')}`,
      yy: moment().isBefore(StartTime)
        ? `%d ${t('in_years')}`
        : `%d ${t('years_ago')}`,
    },
  });

  return (
    <Group px={40} sx={{ flexDirection: 'column' }}>
      <Title lineClamp={3} align="justify">
        {data.name}
      </Title>
      {t('class_duration')} : {Number(data?.meeting?.duration) / 60}
      {t('minutes')}
      {moment().isBetween(StartTime, EndTime) ? (
        <Button
          component={'a'}
          href={`/meet.html?l=${data.slug}&c=${data.courseId}`}
        >
          {t('join_meeting')}
        </Button>
      ) : (
        <div style={{ display: 'block' }}>
          {moment().isBefore(StartTime)
            ? `${t('starts')} ${moment(
                moment.utc(data.meeting.startDate).local()
              ).fromNow()}`
            : `${t('started')} ${moment(
                moment.utc(data.meeting.startDate).local()
              ).fromNow()}`}
        </div>
      )}
      <Text>Meeting Id: {data.zoomId}</Text>
      <Text>Password: {data.password ?? 'N/A'}</Text>
    </Group>
  );
};

export default Meetings;
