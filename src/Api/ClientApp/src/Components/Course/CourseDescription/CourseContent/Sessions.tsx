import useAuth from '@hooks/useAuth';
import { Box, Text, Title } from '@mantine/core';
import { CourseUserStatus, UserRole } from '@utils/enums';
import { ISection } from '@utils/services/courseService';
import Lesson from './Lesson';
import { useTranslation } from 'react-i18next';

const Sessions = ({
  section,
  courseSlug,
  enrollmentStatus,
}: {
  section: ISection;
  courseSlug: string;
  enrollmentStatus: number;
}) => {
  // const totalDuration = section.lessons?.reduce((a, b) => b.duration + a, 0);
  const auth = useAuth();
  const { t } = useTranslation();

  const canClickLessons =
    enrollmentStatus === CourseUserStatus.NotEnrolled &&
    auth?.auth &&
    auth.auth?.role > UserRole.Admin;

  return (
    <Box>
      <Title size={'h6'}>{section?.name}</Title>
      <Text size={10} color={'dimmed'}>
        {section.lessons?.length} {t('Lesson')}
      </Text>
      <Box
        my={20}
        mx={10}
        sx={{
          pointerEvents: canClickLessons ? 'none' : 'auto',
        }}
      >
        {section.lessons?.map((x, i) => (
          <Lesson key={x.id} lesson={x} index={i} courseSlug={courseSlug} />
        ))}
      </Box>
    </Box>
  );
};

export default Sessions;
