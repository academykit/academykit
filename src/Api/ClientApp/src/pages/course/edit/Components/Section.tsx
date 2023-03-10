import { CourseStatus } from "@utils/enums";
import { ILessons } from "@utils/services/courseService";

import {
  ISection,
  useLessonReorder,
  useSectionReorder,
} from "@utils/services/courseService";
import { useMemo, useState } from "react";
import {
  DragDropContext,
  Draggable,
  Droppable,
  DropResult,
} from "react-beautiful-dnd";

import SectionItem from "./Section/SectionItem";

const CourseSection = ({
  data,
  slug,
  status,
}: {
  data: ISection[];
  slug: string;
  status: CourseStatus;
}) => {
  const [lessonData, setLessonData] = useState<ISection[]>(data);
  useMemo(() => setLessonData(data), [data]);

  const lessonReorder = useLessonReorder(slug as string);
  const sectionReorder = useSectionReorder(slug as string);

  const onDragEnd = (result: DropResult) => {
    const { destination, source, draggableId, type } = result;
    if (!destination) return;

    switch (type) {
      case "section":
        let newList = [...data];
        const temp = newList[source.index];
        newList.splice(source.index, 1);
        newList.splice(destination.index, 0, temp);
        sectionReorder.mutate({ id: slug, data: newList.map((x) => x.id) });
        setLessonData(newList);
        break;
      default:
        let tempList = [...data];
        const draggableIndex = tempList.findIndex(
          (x) => x.slug == source.droppableId
        );
        const lessons = tempList[draggableIndex].lessons && [
          ...(tempList[draggableIndex].lessons as ILessons[]),
        ];
        const tempLesson = lessons?.splice(source.index, 1);
        tempList[draggableIndex].lessons = lessons;
        const dropableIndex = tempList.findIndex(
          (x) => x.slug == destination.droppableId
        );
        tempLesson &&
          tempList[dropableIndex].lessons?.splice(
            destination.index,
            0,
            tempLesson[0]
          );
        setLessonData(tempList);
        const requestData = {
          sectionIdentity: tempList[dropableIndex].id,
          ids: tempList[dropableIndex].lessons?.map((x) => x.id),
        };
        lessonReorder.mutateAsync({ id: slug, data: requestData });
    }
  };

  const items = lessonData.map((item, index) => (
    <Draggable key={item.id} index={index} draggableId={item.slug}>
      {(provided, snapshot) => (
        <div {...provided.draggableProps} ref={provided.innerRef}>
          <SectionItem
            status={status}
            dragHandleProps={provided.dragHandleProps}
            snapshot={snapshot}
            key={item.id}
            item={item}
            slug={slug}
          />
        </div>
      )}
    </Draggable>
  ));

  return (
    <DragDropContext onDragEnd={onDragEnd}>
      <Droppable droppableId="dnd-list" direction="vertical" type="section">
        {(provided, snapshot) => (
          <div {...provided.droppableProps} ref={provided.innerRef}>
            {items}
            {provided.placeholder}
          </div>
        )}
      </Droppable>
    </DragDropContext>
  );
};

export default CourseSection;
