import { StatsCard } from "@components/Dashboard/StatsCard";
import { SimpleGrid } from "@mantine/core";
import { useGetCourseManageStatistics } from "@utils/services/manageCourseService";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
const ManageCourse = () => {
  const params = useParams();
  const course_id = params.id as string;
  const getStat = useGetCourseManageStatistics(course_id);
  const { t } = useTranslation();
  const incomingData = [
    {
      key: "totalEnrollments",
      label: t("total_enrollments"),
      icon: "userEnrollment",
      pluLabel: t("Enrollments"),
      signLabel: t("Enrollment"),
    },
    {
      key: "totalLessons",
      label: t("total_lessons"),
      icon: "school",
      pluLabel: t("Lessons"),
      signLabel: t("Lesson"),
    },
    {
      key: "totalTeachers",
      label: t("total_trainers"),
      icon: "groups",
      pluLabel: t("trainers"),
      signLabel: t("trainer"),
    },
    {
      key: "totalAssignments",
      label: t("total_assignments"),
      icon: "book",
      pluLabel: t("Assignments"),
      signLabel: t("Assignment"),
    },
    {
      key: "totalLectures",
      label: t("total_videos"),
      icon: "lecture",
      pluLabel: t("Lectures"),
      signLabel: t("Lecture"),
    },
    {
      key: "totalExams",
      label: t("total_exams"),
      icon: "trainings",
      pluLabel: t("Exams"),
      signLabel: t("Exam"),
    },
    {
      key: "totalMeetings",
      label: t("total_meetings"),
      icon: "meeting",
      pluLabel: t("Meetings"),
      signLabel: t("Meeting"),
    },
    {
      key: "totalDocuments",
      label: t("total_documents"),
      icon: "document",
      pluLabel: t("Documents"),
      signLabel: t("Document"),
    },
  ];
  return (
    <div>
      <SimpleGrid
        mb={20}
        cols={4}
        breakpoints={[
          { maxWidth: "md", cols: 2 },
          { maxWidth: "xs", cols: 1 },
        ]}
      >
        {getStat.data &&
          incomingData.map((x, idx) => (
            //@ts-ignore
            <StatsCard key={idx} data={x} dashboard={getStat.data} />
          ))}
      </SimpleGrid>
    </div>
  );
};

export default ManageCourse;
