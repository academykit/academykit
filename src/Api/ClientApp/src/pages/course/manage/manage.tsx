import { StatsCard } from "@components/Dashboard/StatsCard";
import { SimpleGrid } from "@mantine/core";
import { useGetCourseManageStatistics } from "@utils/services/manageCourseService";
import { useParams } from "react-router-dom";
const ManageCourse = () => {
  const params = useParams();
  const course_id = params.id as string;
  const getStat = useGetCourseManageStatistics(course_id);
  const incomingData = [
    {
      key: "totalEnrollments",
      label: "Total Enrollments",
      icon: "userEnrollment",
    },
    {
      key: "totalLessons",
      label: "Total Lessons",
      icon: "school",
    },
    {
      key: "totalTeachers",
      label: "Total Teachers",
      icon: "groups",
    },
    {
      key: "totalAssignments",
      label: "Total Assignments",
      icon: "book",
    },
    {
      key: "totalLectures",
      label: "Total Lectures",
      icon: "lecture",
    },
    {
      key: "totalExams",
      label: "Total Exams",
      icon: "trainings",
    },
    {
      key: "totalMeetings",
      label: "Total Meetings",
      icon: "meeting",
    },
    {
      key: "totalDocuments",
      label: "Total Documents",
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
