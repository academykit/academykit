import UserShortProfile from '@components/UserShortProfile';
import { Box, Group, Text, Title } from '@mantine/core';
import formatDuration from '@utils/formatDuration';
import { ISection } from '@utils/services/courseService';
import { IUser } from '@utils/services/types';
import { useTranslation } from 'react-i18next';
import Sessions from './Sessions';

const CourseContent = ({
  sections,
  duration,
  courseSlug,
  enrollmentStatus,
  courseName,
  user,
}: {
  sections: ISection[];
  duration: number;
  courseSlug: string;
  enrollmentStatus: number;
  courseName?: string;
  user?: IUser;
}) => {
  const { t } = useTranslation();
  return (
    <Box my={20}>
      <Group my={4} justify="space-between">
        {user && <UserShortProfile user={user} size={'md'} />}
      </Group>
      <Title
        order={5}
        style={{
          whiteSpace: 'nowrap',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
        }}
      >
        {t('content_of')} {courseName}
      </Title>
      <Text size={'md'} c={'dimmed'}>
        {formatDuration(duration, false, t)} {sections.length} {t('section/s')}{' '}
      </Text>
      <Box m={4} mx={10}>
        {sections.map((x) => (
          <Sessions
            key={x.id}
            section={x}
            courseSlug={courseSlug}
            enrollmentStatus={enrollmentStatus}
          />
        ))}
      </Box>
    </Box>
  );
};

export default CourseContent;
