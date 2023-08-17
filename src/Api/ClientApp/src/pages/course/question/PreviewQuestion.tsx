import Breadcrumb from '@components/Ui/BreadCrumb';
import { Title, Text, Box, Loader, Button, Flex } from '@mantine/core';
import { useTranslation } from 'react-i18next';
import {
  QuestionSetQuestions,
  useQuestionSetQuestions,
} from '@utils/services/questionService';
import { useNavigate, useParams } from 'react-router-dom';
import PreviewQuestionCard from './PreviewQuestionCard';
import {
  Draggable,
  Droppable,
  DragDropContext,
  DropResult,
} from 'react-beautiful-dnd';
import { useQuestionReorder } from '@utils/services/courseService';
import { LessonType } from '@utils/enums';
import { useEffect, useState } from 'react';
import { IconChevronLeft } from '@tabler/icons';

const PreviewQuestion = () => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const params = useParams();
  const [questionData, setQuestionData] = useState<QuestionSetQuestions[]>();
  const questions = useQuestionSetQuestions(params.lessonSlug as string);
  const questionReorder = useQuestionReorder(params.lessonSlug as string);

  useEffect(() => {
    setQuestionData(questions.data as QuestionSetQuestions[]);
  }, [questions.isSuccess, questions.isRefetching]);

  const items = questionData?.map((x, index) => (
    <Draggable key={x.id} draggableId={x.id} index={index}>
      {(provided) => (
        <div
          ref={provided.innerRef}
          {...provided.draggableProps}
          {...provided.dragHandleProps}
        >
          <PreviewQuestionCard key={x.id} question={x} />
        </div>
      )}
    </Draggable>
  ));

  const onDragEnd = (result: DropResult) => {
    const { destination, source } = result;
    if (!destination) return;
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    //@ts-ignore
    const newList = [...questionData];
    const temp = newList[source.index];
    newList.splice(source.index, 1);
    newList.splice(destination.index, 0, temp);
    questionReorder.mutate({
      id: params.id as string,
      lessonIdentity: params.lessonSlug as string,
      lessonType: LessonType.Exam,
      data: newList.map((x) => x.questionSetQuestionId),
    });
    setQuestionData(newList);
  };

  return (
    <div>
      <Breadcrumb hide={3} />
      <Button
        variant="subtle"
        leftIcon={<IconChevronLeft />}
        mx={4}
        onClick={() => navigate(-1)}
        mt={15}
      >
        {t('go_back_button')}
      </Button>
      <Flex mt={10} align={'center'} justify={'space-between'}>
        <Title truncate mb={20} ml={15}>
          {'Preview Questions'}
        </Title>
        <Button
          variant="subtle"
          mx={4}
          onClick={() => navigate('../lessons/questions/' + params?.lessonSlug)}
        >
          {t('edit_questions')}
        </Button>
      </Flex>

      {questions.isSuccess ? (
        <Box p={20}>
          {questions.data ? (
            <DragDropContext onDragEnd={onDragEnd}>
              <Droppable
                droppableId={params.lessonSlug as string}
                direction="vertical"
                type="question"
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
            <Text>{t('no_question_found')}</Text>
          )}
        </Box>
      ) : (
        <Loader />
      )}
    </div>
  );
};

export default PreviewQuestion;
