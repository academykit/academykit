import TextViewer from "@components/Ui/RichTextViewer";
import { Badge, Button, Card, Group, Image, Text } from "@mantine/core";
import { CourseLanguage } from "@utils/enums";
import getCourseOgImageUrl from "@utils/getCourseOGImage";
import RoutePath from "@utils/routeConstants";
import { useGeneralSetting } from "@utils/services/adminService";
import { ICourse } from "@utils/services/courseService";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import classes from "./styles/coursecard.module.css";

const CourseCard = ({ course }: { course: ICourse }) => {
  const { t } = useTranslation();
  const generalSettings = useGeneralSetting();
  const companyName = generalSettings.data?.data.companyName;
  const companyLogo = generalSettings.data?.data.logoUrl;
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
          src={getCourseOgImageUrl({
            author: course?.user,
            title: course?.name,
            thumbnailUrl: course?.thumbnailUrl,
            companyName: companyName,
            companyLogo: companyLogo,
          })}
          height={160}
          alt={course.name}
        />
      </Card.Section>

      <Group mt="md" mb="xs">
        <Text style={{ fontWeight: "600px" }} lineClamp={2}>
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

      <Text size="sm" c="dimmed" lineClamp={2} style={{ height: "60px" }}>
        <TextViewer
          styles={{
            root: {
              border: "none",
            },
          }}
          content={course.description}
        />
      </Text>
      <Link
        style={{ textDecoration: "none" }}
        to={RoutePath.courses.description(course.slug).route}
      >
        <Button variant="light" c="blue" fullWidth mt="md" radius="md">
          {t("watch")}
        </Button>
      </Link>
    </Card>
  );
};
export default CourseCard;
