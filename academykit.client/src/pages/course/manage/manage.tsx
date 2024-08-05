import { StatsCard } from "@components/Dashboard/StatsCard";
import { SimpleGrid, useMantineColorScheme } from "@mantine/core";
import { DashboardStats } from "@utils/services/dashboardService";
import { useGetCourseManageStatistics } from "@utils/services/manageCourseService";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
const ManageCourse = () => {
  const params = useParams();
  const course_id = params.id as string;
  const getStat = useGetCourseManageStatistics(course_id);
  const { t } = useTranslation();
  const { colorScheme } = useMantineColorScheme();

  const incomingData = [
    {
      key: "totalEnrollments",
      label: t("total_enrollments"),
      icon: "userEnrollment",
      pluLabel: t("Enrollments"),
      signLabel: t("Enrollment"),
      color: colorScheme === "dark" ? "white" : "black",
    },
    {
      key: "totalLessons",
      label: t("total_lessons"),
      icon: "book",
      pluLabel: t("Lessons"),
      signLabel: t("Lesson"),
      color: colorScheme === "dark" ? "white" : "black",
    },
    {
      key: "totalTeachers",
      label: t("total_trainers"),
      icon: "groups",
      pluLabel: t("trainers"),
      signLabel: t("trainer"),
      color: colorScheme === "dark" ? "white" : "black",
    },
    {
      key: "totalAssignments",
      label: t("total_assignments"),
      icon: "lecture",
      pluLabel: t("Assignments"),
      signLabel: t("Assignment"),
      color: colorScheme === "dark" ? "white" : "black",
    },
    {
      key: "totalLectures",
      label: t("total_videos"),
      icon: "video",
      pluLabel: t("Videos"),
      signLabel: t("Video"),
      color: colorScheme === "dark" ? "white" : "black",
    },
    {
      key: "totalExams",
      label: t("total_exams"),
      icon: "exam",
      pluLabel: t("Exams"),
      signLabel: t("Exam"),
      color: colorScheme === "dark" ? "white" : "black",
    },
    {
      key: "totalMeetings",
      label: t("total_meetings"),
      icon: "meeting",
      pluLabel: t("Meetings"),
      signLabel: t("Meeting"),
      color: colorScheme === "dark" ? "white" : "black",
    },
    {
      key: "totalDocuments",
      label: t("total_documents"),
      icon: "document",
      pluLabel: t("Documents"),
      signLabel: t("Document"),
      color: colorScheme === "dark" ? "white" : "black",
    },
  ];
  return (
    <div>
      <SimpleGrid mb={20} cols={{ xs: 1, md: 2, lg: 4 }}>
        {getStat.data &&
          incomingData.map((x, idx) => (
            <StatsCard
              key={idx}
              data={x}
              dashboard={getStat.data as DashboardStats}
            />
          ))}
      </SimpleGrid>
    </div>
  );
};

export default ManageCourse;
