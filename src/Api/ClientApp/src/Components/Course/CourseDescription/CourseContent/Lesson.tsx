/* eslint-disable @typescript-eslint/no-unused-vars */
import {
  Badge,
  Box,
  createStyles,
  Flex,
  Group,
  Paper,
  Popover,
  Text,
  Title,
} from '@mantine/core';
import { useHover, useMediaQuery } from '@mantine/hooks';
import { LessonType } from '@utils/enums';
import formatDuration from '@utils/formatDuration';
import RoutePath from '@utils/routeConstants';
import { ILessons } from '@utils/services/courseService';
import { Link, useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { IconCheck } from '@tabler/icons';

const useStyle = createStyles((theme) => {
  return {
    paper: {
      '&:hover': {
        backgroundColor:
          theme.colorScheme === 'light'
            ? theme.colors.dark[2]
            : theme.colors.gray[7],
        transform: 'scale(1.02)',
      },
    },
  };
});
const Lesson = ({
  lesson,
  courseSlug,
  index,
}: {
  lesson: ILessons;
  courseSlug: string;
  index: number;
}) => {
  const { classes, theme } = useStyle();
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.lg}px)`);
  const { hovered, ref } = useHover();
  const { lessonId } = useParams();
  const { t } = useTranslation();

  return (
    <Popover
      width={200}
      position="top"
      withArrow
      shadow={'lg'}
      opened={hovered}
    >
      <Popover.Target>
        <Paper
          my={15}
          radius={10}
          w={'100%'}
          shadow={'md'}
          className={classes.paper}
          withBorder
          sx={{
            backgroundColor:
              lessonId === lesson.slug
                ? theme.colorScheme === 'light'
                  ? theme.colors.dark[0]
                  : theme.colors.gray[7]
                : '',
          }}
          component={Link}
          replace={true}
          to={`${RoutePath.classes}/${courseSlug}/${lesson.slug}/description`}
        >
          <div ref={ref}>
            <Group>
              <Flex w={'100%'} p={15} direction={'row'}>
                <div>
                  <Title size={matches ? 14 : 13} lineClamp={2} mb={3}>
                    {index + 1}. {lesson.name}
                  </Title>
                  <Badge color="blue" variant="light" ml={10}>
                    {t(`${LessonType[lesson.type]}`)}
                  </Badge>
                </div>
                {lesson.isCompleted && (
                  <IconCheck
                    style={{
                      marginLeft: 'auto',
                      marginTop: 'auto',
                      marginBottom: 'auto',
                    }}
                  />
                )}
              </Flex>
            </Group>
          </div>
        </Paper>
      </Popover.Target>
      <Popover.Dropdown sx={{ pointerEvents: 'none' }}>
        <Text fw={700} size="md">
          {lesson.name}
        </Text>

        {(lesson.type === LessonType.Exam ||
          lesson.type === LessonType.LiveClass ||
          lesson.type === LessonType.RecordedVideo ||
          lesson.type === LessonType.Video) && (
          <Text>
            {t('Duration')}
            {formatDuration(lesson.duration, false, t)}
          </Text>
        )}
      </Popover.Dropdown>
    </Popover>
  );
};

export default Lesson;
