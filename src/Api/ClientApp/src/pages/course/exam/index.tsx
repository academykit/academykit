import Exam from "@components/Course/Classes/Exam";
import { Container, Loader } from "@mantine/core";
import { useStartExam } from "@utils/services/examService";
import { useEffect } from "react";
import { useParams } from "react-router-dom";

const LessonExam = () => {
  const { id } = useParams();
  const exam = useStartExam(id as string);
  useEffect(() => {
    exam.mutate();
  }, []);
  if (exam.isLoading) {
    return <Loader />;
  }
  if (exam.isError) {
    throw exam.error;
  }
  return exam.data ? (
    <Exam data={exam.data?.data} lessonId={id as string} />
  ) : (
    <></>
  );
};

export default LessonExam;
