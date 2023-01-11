import AddLesson from "@components/Ui/AddLessons";
import { ILessons } from "@utils/services/courseService";
import Lesson from "./Lesson";

const Lessons = ({
  lessons,
  sectionId,
}: {
  lessons: ILessons[];
  sectionId: string;
}) => {
  return (
    <>
      {lessons.map((x) => (
        <Lesson lesson={x} key={x.id} sectionId={sectionId} />
      ))}
      <AddLesson sectionId={sectionId} />
    </>
  );
};

export default Lessons;
