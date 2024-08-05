import {
  Badge,
  Box,
  Flex,
  Group,
  Paper,
  Popover,
  Text,
  Title,
  useMantineColorScheme,
  useMantineTheme,
} from "@mantine/core";
import { useHover, useMediaQuery } from "@mantine/hooks";
import { IconCheck } from "@tabler/icons-react";
import { LessonType } from "@utils/enums";
import formatDuration from "@utils/formatDuration";
import RoutePath from "@utils/routeConstants";
import { ILessons } from "@utils/services/courseService";
import { useTranslation } from "react-i18next";
import { Link, useParams } from "react-router-dom";
import classes from "../../styles/lesson.module.css";

const Lesson = ({
  lesson,
  courseSlug,
  index,
}: {
  lesson: ILessons;
  courseSlug: string;
  index: number;
}) => {
  const matches = useMediaQuery(`(min-width: 75em)`);
  const { hovered, ref } = useHover();
  const { lessonId } = useParams();
  const { t } = useTranslation();
  const { colorScheme } = useMantineColorScheme();
  const theme = useMantineTheme();

  return (
    <Popover
      width={200}
      position="top"
      withArrow
      shadow={"lg"}
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
          style={{
            backgroundColor:
              lessonId === lesson.slug
                ? colorScheme === "light"
                  ? theme.colors.dark[0]
                  : theme.colors.gray[7]
                : "",
          }}
          component={Link}
          replace={true}
          to={`${RoutePath.classes}/${courseSlug}/${lesson.slug}/description`}
        >
          <div ref={ref}>
            <Group>
              <Flex w={"100%"} p={15} direction={"row"}>
                <Box w={"100%"}>
                  <Title
                    c={colorScheme == "dark" ? "#C1C2C5" : "dark"}
                    size={matches ? 14 : 13}
                    mb={3}
                  >
                    {index + 1}. {lesson.name}
                  </Title>
                  <Badge color="blue" variant="light" ml={10}>
                    {t(`${LessonType[lesson.type]}`)}
                  </Badge>
                </Box>
                {lesson.isCompleted && (
                  <IconCheck
                    style={{
                      marginLeft: "auto",
                      marginTop: "auto",
                      marginBottom: "auto",
                    }}
                  />
                )}
              </Flex>
            </Group>
          </div>
        </Paper>
      </Popover.Target>
      <Popover.Dropdown style={{ pointerEvents: "none" }}>
        <Text fw={700} size="md" truncate>
          {lesson.name}
        </Text>

        {(lesson.type === LessonType.Exam ||
          lesson.type === LessonType.LiveClass ||
          lesson.type === LessonType.RecordedVideo ||
          lesson.type === LessonType.Video) && (
          <Text>
            {t("Duration")}
            {formatDuration(lesson.duration, false, t)}
          </Text>
        )}
      </Popover.Dropdown>
    </Popover>
  );
};

export default Lesson;
