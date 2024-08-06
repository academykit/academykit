import CourseCard from "@components/Course/CourseCard";
import CourseCardHorizontal from "@components/Course/CourseCardHorizontal";
import { Group } from "@mantine/core";
import { UserRole } from "@utils/enums";
import { ICourse } from "@utils/services/courseService";

const CourseList = ({
  role,
  courses,
  search,
}: {
  role: UserRole;
  courses: ICourse[];
  search: string;
}) => {
  if (role <= UserRole.Trainer) {
    return (
      <>
        {courses.map((d) => (
          <CourseCardHorizontal key={d.id} course={d} search={search} />
        ))}
      </>
    );
  }
  return (
    <Group
      style={{
        justifyContent: "space-around",
        marginTop: "0",
      }}
    >
      {courses.map((d) => (
        <CourseCard key={d.id} course={d} />
      ))}
    </Group>
  );
};

export default CourseList;
