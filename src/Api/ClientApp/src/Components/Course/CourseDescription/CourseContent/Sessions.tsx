import useAuth from "@hooks/useAuth";
import { Box, Text, Title } from "@mantine/core";
import { CourseUserStatus, UserRole } from "@utils/enums";
import formatDuration from "@utils/formatDuration";
import { ISection } from "@utils/services/courseService";
import Lesson from "./Lesson";

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

  const canClickLessons =
    enrollmentStatus === CourseUserStatus.NotEnrolled &&
    auth?.auth &&
    auth.auth?.role > UserRole.Admin;

  return (
    <Box>
      <Title size={"h6"}>{section?.name}</Title>
      <Text size={10} color={"dimmed"}>
        {formatDuration(section.duration ?? 0)}. {section.lessons?.length}{" "}
        Lesson(s)
      </Text>
      <Box
        my={20}
        mx={10}
        sx={{
          pointerEvents: canClickLessons ? "none" : "auto",
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