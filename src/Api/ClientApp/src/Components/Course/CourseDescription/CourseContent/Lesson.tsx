import {
  Badge,
  Box,
  createStyles,
  Group,
  Paper,
  Popover,
  Text,
  Title,
} from "@mantine/core";
import { useHover, useMediaQuery } from "@mantine/hooks";
import { LessonType, ReadableEnum } from "@utils/enums";
import formatDuration from "@utils/formatDuration";
import RoutePath from "@utils/routeConstants";
import { ILessons } from "@utils/services/courseService";
import { Link, useParams } from "react-router-dom";

const useStyle = createStyles((theme) => {
  return {
    paper: {
      "&:hover": {
        backgroundColor:
          theme.colorScheme === "light"
            ? theme.colors.dark[2]
            : theme.colors.gray[7],
        transform: "scale(1.02)",
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

  return (
    <Popover
      width={200}
      position="top"
      withArrow
      shadow={"md"}
      opened={hovered}
    >
      <Popover.Target>
        <Paper
          my={15}
          radius={10}
          w={"100%"}
          shadow={"md"}
          className={classes.paper}
          withBorder
          sx={{
            backgroundColor:
              lessonId === lesson.slug
                ? theme.colorScheme === "light"
                  ? theme.colors.dark[0]
                  : theme.colors.gray[7]
                : "",
          }}
          //@ts-ignore
          ref={ref}
          component={Link}
          replace={true}
          to={`${RoutePath.classes}/${courseSlug}/${lesson.slug}${
            lesson.type === LessonType.LiveClass ? "/comments" : "/description"
          }`}
        >
          <Group>
            <Box w={"100%"} p={15}>
              <Title size={matches ? 20 : 13} lineClamp={2}>
                {index + 1}. {lesson.name}
              </Title>
              <Badge color="blue" variant="light" ml={10}>
                {ReadableEnum[
                  LessonType[lesson.type] as keyof typeof ReadableEnum
                ] ?? LessonType[lesson.type]}
              </Badge>
            </Box>
          </Group>
        </Paper>
      </Popover.Target>
      <Popover.Dropdown sx={{ pointerEvents: "none" }}>
        <Text fw={700} size="lg">
          {lesson.name}
        </Text>

        {(lesson.type === LessonType.Exam ||
          lesson.type === LessonType.LiveClass ||
          lesson.type === LessonType.RecordedVideo ||
          lesson.type === LessonType.Video) && (
          <Text>Duration: {formatDuration(lesson.duration)}</Text>
        )}
      </Popover.Dropdown>
    </Popover>
  );
};

export default Lesson;
