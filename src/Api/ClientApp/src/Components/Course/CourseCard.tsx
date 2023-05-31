import {
  Card,
  Image,
  Text,
  Group,
  Badge,
  Button,
  createStyles,
  Flex,
} from "@mantine/core";
import RichTextEditor from "@mantine/rte";
import { CourseLanguage } from "@utils/enums";
import getCourseOgImageUrl from "@utils/getCourseOGImage";
import RoutePath from "@utils/routeConstants";
import { ICourse } from "@utils/services/courseService";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const useStyles = createStyles((theme) => ({
  card: {
    [theme.fn.largerThan(theme.breakpoints.md)]: {
      width: "400px",
      minHeight: "300px",
    },
    [theme.fn.smallerThan(theme.breakpoints.sm)]: {
      width: "100%",
    },
    display: "flex",
    flexDirection: "column",
    justifyContent: "space-between",
  },
}));

const CourseCard = ({ course }: { course: ICourse }) => {
  const { classes, cx } = useStyles();
  const { t } = useTranslation();
  return (
    <Card
      className={classes.card}
      shadow="sm"
      p="lg"
      radius="md"
      withBorder
      ml={15}
      mb={15}
      h={380}
    >
      <Card.Section
        component={Link}
        to={RoutePath.courses.description(course.slug).route}
      >
        <Image
          src={getCourseOgImageUrl(
            course?.user,
            course?.name,
            course?.thumbnailUrl
          )}
          height={160}
          alt={course.name}
        />
      </Card.Section>

      <Group position="left" mt="md" mb="xs">
        <Text weight={600} lineClamp={2}>
          {course?.name}
        </Text>
        {course.language && (
          <Badge color="pink" variant="light">
            {CourseLanguage[course.language]}
          </Badge>
        )}
        {course.levelName && (
          <Badge color="blue" variant="light">
            {course.levelName}
          </Badge>
        )}
      </Group>

      <Text size="sm" color="dimmed" lineClamp={2} sx={{ height: "60px" }}>
        <RichTextEditor
          styles={{
            root: {
              border: "none",
            },
          }}
          value={course.description}
          readOnly
        />
      </Text>
      <Link
        style={{ textDecoration: "none" }}
        to={RoutePath.courses.description(course.slug).route}
      >
        <Button variant="light" color="blue" fullWidth mt="md" radius="md">
          {t("watch")}
        </Button>
      </Link>
    </Card>
  );
};
export default CourseCard;
