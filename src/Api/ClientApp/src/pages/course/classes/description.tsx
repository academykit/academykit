import { RichTextEditor } from "@mantine/rte";
import { Box } from "@mantine/core";
import { useParams } from "react-router-dom";
import { useGetCourseLesson } from "@utils/services/courseService";

const CourseDescriptionSection = () => {
  const { id, lessonId } = useParams();

  const courseLesson = useGetCourseLesson(id as string, lessonId);

  return (
    <Box m={5}>
      <RichTextEditor
        readOnly
        value={courseLesson.data?.description}
        id="rte"
      />
    </Box>
  );
};

export default CourseDescriptionSection;
