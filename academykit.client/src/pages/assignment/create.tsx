import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import { Box, Button } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { LessonType } from '@utils/enums';
import {
  IAssignmentQuestion,
  useAssignmentQuestion,
} from '@utils/services/assignmentService';
import { useQuestionReorder } from '@utils/services/courseService';
import { useEffect, useState } from 'react';
import {
  DragDropContext,
  Draggable,
  DropResult,
  Droppable,
} from 'react-beautiful-dnd';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import AssignmentItem from './Component/AssignmentItem';
import EditAssignment from './Component/EditAssignment';

const CreateAssignment = ({ searchParams }: IWithSearchPagination) => {
  const [isEditing, setIsEditing] = useState(false);
  const [addQuestion, setAddQuestion] = useToggle();
  const { id, lessonId } = useParams();
  const assignmentReorder = useQuestionReorder(lessonId as string);
  const questionList = useAssignmentQuestion(lessonId as string, searchParams);
  const { t } = useTranslation();
  const [lessonData, setLessonData] = useState<IAssignmentQuestion[]>();
  const navigate = useNavigate();

  useEffect(() => {
    setLessonData(questionList.data as IAssignmentQuestion[]);
  }, [questionList.isSuccess, questionList.isRefetching]);

  const handleEditStateChange = () => {
    setIsEditing((prev) => !prev);
  };

  const items = lessonData?.map((x, index) => (
    <Draggable
      key={x.id}
      draggableId={x.id}
      index={index}
      isDragDisabled={isEditing}
    >
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
            lessonId={lessonId as string}
            onEditChange={handleEditStateChange}
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
      lessonIdentity: lessonId as string,
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
                droppableId={lessonId as string}
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
              lessonId={lessonId as string}
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
