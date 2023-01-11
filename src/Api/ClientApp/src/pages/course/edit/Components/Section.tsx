import { ISection } from "@utils/services/courseService";

import SectionItem from "./Section/SectionItem";

// const dragReducer = produce((draft, action) => {
//   switch (action.type) {
//     case "MOVE": {
//       draft[action.from] = draft[action.from] || [];
//       draft[action.to] = draft[action.to] || [];
//       const [removed] = draft[action.from].splice(action.fromIndex, 1);
//       draft[action.to].splice(action.toIndex, 0, removed);
//     }
//   }
// });

const CourseSection = ({ data, slug }: { data: ISection[]; slug: string }) => {
  // const b = {};
  // data.map((e) => {
  //   //@ts-ignore
  //   return (b[e.id] = e.lessons);
  // });

  // const [state, dispatch] = useReducer(dragReducer, b);

  // const onDragEnd = useCallback((result: any) => {
  //   if (result.reason === "DROP") {
  //     if (!result.destination) {
  //       return;
  //     }
  //     dispatch({
  //       type: "MOVE",
  //       from: result.source.droppableId,
  //       to: result.destination.droppableId,
  //       fromIndex: result.source.index,
  //       toIndex: result.destination.index,
  //     });
  //   }
  // }, []);

  return (
    <div>
      {/* <DragDropContext onDragEnd={onDragEnd}> */}
      {data?.map((item) => (
        <SectionItem key={item.id} item={item} slug={slug} />
      ))}
      {/* </DragDropContext> */}
    </div>
  );
};

export default CourseSection;
