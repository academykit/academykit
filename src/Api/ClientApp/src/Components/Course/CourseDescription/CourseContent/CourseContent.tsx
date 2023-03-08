import UserShortProfile from "@components/UserShortProfile";
import { Box, Button, Group, Text, Title } from "@mantine/core";
import { UserRole } from "@utils/enums";
import formatDuration from "@utils/formatDuration";
import { ISection } from "@utils/services/courseService";
import { IUser } from "@utils/services/types";
import Sessions from "./Sessions";

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
  return (
    <Box my={20}>
      <Group my={4} position="apart">
        {user && <UserShortProfile user={user} size={"md"} />}
      </Group>
      <Title size={"h5"}>Course Content of {courseName}</Title>
      <Text size={10} color={"dimmed"}>
        {formatDuration(duration)} {sections.length} Section(s){" "}
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
