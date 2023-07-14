import { Box } from '@mantine/core';
import { useParams } from 'react-router-dom';
import { useGetCourseLesson } from '@utils/services/courseService';
import { LessonType } from '@utils/enums';
import TextViewer from '@components/Ui/RichTextViewer';

const CourseDescriptionSection = () => {
  const { id, lessonId } = useParams();

  const courseLesson = useGetCourseLesson(id as string, lessonId);
  const isExam = courseLesson.data?.type === LessonType.Exam;

  return (
    <Box m={5}>
      <TextViewer
        content={
          isExam
            ? courseLesson.data?.questionSet.description ?? ''
            : courseLesson.data?.description ?? ''
        }
        // id="rte"
      />
    </Box>
  );
};

export default CourseDescriptionSection;
