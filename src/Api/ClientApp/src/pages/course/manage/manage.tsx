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
    },
    {
      key: "totalLessons",
      label: t("total_lessons"),
      icon: "school",
    },
    {
      key: "totalTeachers",
      label: t("total_trainers"),
      icon: "groups",
    },
    {
      key: "totalAssignments",
      label: t("total_assignments"),
      icon: "book",
    },
    {
      key: "totalLectures",
      label: t("total_videos"),
      icon: "lecture",
    },
    {
      key: "totalExams",
      label: t("total_exams"),
      icon: "trainings",
    },
    {
      key: "totalMeetings",
      label: t("total_meetings"),
      icon: "meeting",
    },
    {
      key: "totalDocuments",
      label: t("total_documents"),
      icon: "document",
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
