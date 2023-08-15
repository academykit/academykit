/* eslint-disable @typescript-eslint/no-unused-vars */
import { useState, useEffect } from 'react';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import { Box, Button, Container } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import {
  IAssignmentQuestion,
  useAssignmentQuestion,
} from '@utils/services/assignmentService';
import AssignmentItem from './Component/AssignmentItem';
import EditAssignment from './Component/EditAssignment';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Draggable,
  Droppable,
  DragDropContext,
  DropResult,
} from 'react-beautiful-dnd';
import { useQuestionReorder } from '@utils/services/courseService';
import { LessonType } from '@utils/enums';

interface Props {
  lessonId?: string;
}

const CreateAssignment = ({
  searchParams,
  lessonId,
}: Props & IWithSearchPagination) => {
  const [addQuestion, setAddQuestion] = useToggle();
  const { id, lessonId: lId } = useParams();
  const assignmentReorder = useQuestionReorder(lId as string);
  const questionList = useAssignmentQuestion(lId as string, searchParams);
  const { t } = useTranslation();
  const [lessonData, setLessonData] = useState<IAssignmentQuestion[]>();
  const navigate = useNavigate();

  useEffect(() => {
    setLessonData(questionList.data as IAssignmentQuestion[]);
  }, [questionList.isSuccess]);

  const items = lessonData?.map((x, index) => (
    <Draggable key={x.id} draggableId={x.id} index={index}>
      {(provided) => (
        <div
          ref={provided.innerRef}
          {...provided.draggableProps}
          {...provided.dragHandleProps}
        >
          <AssignmentItem
            key={x.id}
            data={x}
            search={searchParams}
            lessonId={lId as string}
          />
        </div>
      )}
    </Draggable>
  ));

  const onDragEnd = (result: DropResult) => {
    const { destination, source } = result;
    if (!destination) return;
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    //@ts-ignore
    const newList = [...lessonData];
    const temp = newList[source.index];
    newList.splice(source.index, 1);
    newList.splice(destination.index, 0, temp);
    assignmentReorder.mutate({
      id: id as string,
      lessonIdentity: lId as string,
      lessonType: LessonType.Assignment,
      data: newList.map((x) => x.id),
    });
    setLessonData(newList);
  };

  return (
    <>
      {questionList.isSuccess && (
        <>
          {questionList.data.length > 0 ? (
            <DragDropContext onDragEnd={onDragEnd}>
              <Droppable
                droppableId={lId as string}
                direction="vertical"
                type="assignment"
              >
                {(provided) => (
                  <div {...provided.droppableProps} ref={provided.innerRef}>
                    {items}
                    {provided.placeholder}
                  </div>
                )}
              </Droppable>
            </DragDropContext>
          ) : (
            <Box mt={10}>{t('no_assignment_questions')}</Box>
          )}
          {addQuestion && (
            <EditAssignment
              onCancel={() => setAddQuestion()}
              lessonId={lId as string}
              search={searchParams}
            />
          )}

          {!addQuestion && (
            <>
              <Button mt={10} onClick={() => setAddQuestion()}>
                {t('add_question')}
              </Button>
              <Button
                ml={10}
                variant="outline"
                onClick={() => navigate(-1)}
                mt={10}
              >
                {t('go_back_button')}
              </Button>
            </>
          )}
        </>
      )}
    </>
  );
};

export default withSearchPagination(CreateAssignment);

// <Box>
//   {questionList.data.map((x) => (
//     <AssignmentItem
//       key={x.id}
//       data={x}
//       search={searchParams}
//       lessonId={id as string}
//     />
//   ))}
// </Box>
