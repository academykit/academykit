/* eslint-disable @typescript-eslint/no-unused-vars */
import useAuth from "@hooks/useAuth";
import { Accordion, Box, Text, Title } from "@mantine/core";
import { CourseUserStatus, UserRole } from "@utils/enums";
import { ISection } from "@utils/services/courseService";
import { useTranslation } from "react-i18next";
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
  const { t } = useTranslation();

  const canClickLessons =
    enrollmentStatus === CourseUserStatus.NotEnrolled &&
    auth?.auth &&
    Number(auth.auth?.role) > UserRole.Admin;

  // automatic expand current section
  // const getCurrentSectionName = () => {
  //   const currentSection = section.lessons?.find(
  //     (x) => x.slug === params.lessonId
  //   );
  //   return currentSection?.sectionId ?? '';
  // };

  return (
    <Box>
      <Accordion>
        <Accordion.Item value={section?.id}>
          <Accordion.Control>
            <Title size={"h6"}>{section?.name}</Title>
            <Text size={"md"} c={"dimmed"}>
              {section.lessons?.length} {t("Lesson")}
            </Text>
          </Accordion.Control>
          <Accordion.Panel>
            <Box
              my={20}
              mx={10}
              style={{
                pointerEvents: canClickLessons ? "none" : "auto",
              }}
            >
              {section.lessons?.map((x, i) => (
                <Lesson
                  key={x.id}
                  lesson={x}
                  index={i}
                  courseSlug={courseSlug}
                />
              ))}
            </Box>
          </Accordion.Panel>
        </Accordion.Item>
      </Accordion>
    </Box>
  );
};

export default Sessions;
