import Exam from "@components/Course/Classes/Exam";
import { Button, Container, Loader, Title } from "@mantine/core";
import { useStartExam } from "@utils/services/examService";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import classes from "../styles/assignment.module.css";

const LessonExam = () => {
  const { id } = useParams();
  const exam = useStartExam(id as string);
  const { t } = useTranslation();
  const navigate = useNavigate();

  useEffect(() => {
    exam.mutate();
  }, []);

  if (exam.isLoading) {
    return <Loader />;
  }
  if (exam.isError) {
    throw exam.error;
  }
  if (exam.data?.data.questions.length === 0) {
    return (
      <Container className={classes.root}>
        <div>
          <Title className={classes.title}>{t("no_question_found")}</Title>

          <Button
            variant="outline"
            size="md"
            mt="xl"
            onClick={() => navigate(-1)}
            className={classes.control}
          >
            {t("back_course")}
          </Button>
        </div>
      </Container>
    );
  }
  return exam.data ? (
    <Exam data={exam.data?.data} lessonId={id as string} />
  ) : (
    <></>
  );
};

export default LessonExam;
