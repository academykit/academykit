import { CourseStatus } from '@utils/enums';
import {
  ILessons,
  ISection,
  useLessonReorder,
  useSectionReorder,
} from '@utils/services/courseService';

import { useMemo, useState } from 'react';
import {
  DragDropContext,
  Draggable,
  DropResult,
  Droppable,
} from 'react-beautiful-dnd';

import SectionItem from './Section/SectionItem';

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
    const { destination, source, type } = result;
    if (!destination) return;
    const newList = [...data];
    switch (type) {
      case 'section':
        // eslint-disable-next-line no-case-declarations
        const temp = newList[source.index];
        newList.splice(source.index, 1);
        newList.splice(destination.index, 0, temp);
        sectionReorder.mutate({ id: slug, data: newList.map((x) => x.id) });
        setLessonData(newList);
        break;
      default:
        // eslint-disable-next-line no-case-declarations
        const draggableIndex = newList.findIndex(
          (x) => x.slug == source.droppableId
        );
        // eslint-disable-next-line no-case-declarations
        const lessons = newList[draggableIndex].lessons && [
          ...(newList[draggableIndex].lessons as ILessons[]),
        ];
        // eslint-disable-next-line no-case-declarations
        const tempLesson = lessons?.splice(source.index, 1);
        newList[draggableIndex].lessons = lessons;
        // eslint-disable-next-line no-case-declarations
        const dropableIndex = newList.findIndex(
          (x) => x.slug == destination.droppableId
        );
        tempLesson &&
          newList[dropableIndex].lessons?.splice(
            destination.index,
            0,
            tempLesson[0]
          );
        setLessonData(newList);
        // eslint-disable-next-line no-case-declarations
        const requestData = {
          sectionIdentity: newList[dropableIndex].id,
          ids: newList[dropableIndex].lessons?.map((x) => x.id),
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
        {(provided) => (
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
