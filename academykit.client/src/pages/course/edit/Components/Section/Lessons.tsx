import AddLesson from "@components/Ui/AddLessons";
import { CourseStatus } from "@utils/enums";
import { ILessons } from "@utils/services/courseService";
import { Draggable, Droppable } from "react-beautiful-dnd";
import Lesson from "./Lesson";

const Lessons = ({
  lessons,
  sectionId,
  courseStatus,
}: {
  lessons: ILessons[];
  sectionId: string;
  courseStatus: CourseStatus;
}) => {
  const items = lessons.map((x, index) => (
    <Draggable key={x.slug} draggableId={x.slug} index={index}>
      {(provided) => (
        <div
          ref={provided.innerRef}
          {...provided.draggableProps}
          {...provided.dragHandleProps}
        >
          <Lesson lesson={x} sectionId={sectionId} />
        </div>
      )}
    </Draggable>
  ));
  return (
    <>
      <Droppable droppableId={sectionId} direction="vertical" type="lesson">
        {(provided) => (
          <div {...provided.droppableProps} ref={provided.innerRef}>
            {items}
            {provided.placeholder}
          </div>
        )}
      </Droppable>
      {courseStatus !== CourseStatus.Completed && (
        <AddLesson sectionId={sectionId} />
      )}
    </>
  );
};

export default Lessons;
