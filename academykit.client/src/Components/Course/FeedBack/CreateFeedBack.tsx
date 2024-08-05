import { useState, useEffect } from 'react';
import { Box, Button } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import {
  IFeedbackQuestions,
  useFeedbackQuestion,
} from '@utils/services/feedbackService';
import EditFeedback from './EditFeedBack';
import FeedbackItem from './FeedbackList';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuestionReorder } from '@utils/services/courseService';
import {
  Draggable,
  Droppable,
  DragDropContext,
  DropResult,
} from 'react-beautiful-dnd';
import { LessonType } from '@utils/enums';

const CreateFeedback = () => {
  const { id, lessonId } = useParams();
  const [isEditing, setIsEditing] = useState(false);
  const [feedbackData, setFeedbackData] = useState<IFeedbackQuestions[]>();
  const feedbackReorder = useQuestionReorder(lessonId as string);
  const [addQuestion, setAddQuestion] = useToggle();
  const { t } = useTranslation();
  const feedbackList = useFeedbackQuestion(lessonId as string, '');
  const navigate = useNavigate();

  useEffect(() => {
    setFeedbackData(feedbackList.data as IFeedbackQuestions[]);
  }, [feedbackList.isSuccess, feedbackList.isRefetching]);

  const handleEditStateChange = () => {
    setIsEditing((prev) => !prev);
  };

  const items = feedbackData?.map((x, index) => (
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
          <FeedbackItem
            key={x.id}
            data={x}
            search={''}
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
    const newList = [...feedbackData];
    const temp = newList[source.index];
    newList.splice(source.index, 1);
    newList.splice(destination.index, 0, temp);
    feedbackReorder.mutate({
      id: id as string,
      lessonIdentity: lessonId as string,
      lessonType: LessonType.Feedback,
      data: newList.map((x) => x.id),
    });
    setFeedbackData(newList);
  };

  return (
    <>
      {feedbackList.isSuccess && (
        <>
          {feedbackList.data.length > 0 ? (
            <DragDropContext onDragEnd={onDragEnd}>
              <Droppable
                droppableId={lessonId as string}
                direction="vertical"
                type="feedback"
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
            <Box mb={10}>{t('no_feedback_questions')}</Box>
          )}
        </>
      )}
      {addQuestion && (
        <EditFeedback
          onCancel={() => setAddQuestion()}
          lessonId={lessonId as string}
          search={''}
        />
      )}
      <Button onClick={() => setAddQuestion()} mt={10}>
        {t('add_feedback')}
      </Button>
      <Button ml={10} variant="outline" onClick={() => navigate(-1)} mt={10}>
        {t('go_back_button')}
      </Button>
    </>
  );
};

export default CreateFeedback;
