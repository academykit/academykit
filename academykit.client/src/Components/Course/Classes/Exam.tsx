import TextViewer from "@components/Ui/RichTextViewer";
import useCustomLayout from "@context/LayoutProvider";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Card,
  Container,
  Grid,
  Group,
  Modal,
  Text,
  Title,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useMediaQuery, useToggle } from "@mantine/hooks";
import { CourseUserStatus, QuestionType, UserRole } from "@utils/enums";
import {
  ILessonExamStart,
  ILessonExamSubmit,
  ILessonStartQuestion,
  ILessonStartQuestionOption,
  useSubmitExam,
} from "@utils/services/examService";
import cx from "clsx";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router-dom";
import ExamCounter from "./ExamCounter";
import ExamCheckBox from "./ExamOptions/ExamCheckBox";
import ExamRadio from "./ExamOptions/ExamRadio";
import classes from "./style/exam.module.css";

const Exam = ({
  data,
  lessonId,
}: {
  data: ILessonExamStart;
  lessonId: string;
}) => {
  const [currentIndex, setCurrentIndex] = useState(0);
  const customLayout = useCustomLayout();
  const questions = data.questions;
  const examSubmission = useSubmitExam();
  const auth = useAuth();
  const location = useLocation();
  const { t } = useTranslation();
  const navigate = useNavigate();

  const form = useForm({
    initialValues: questions,
  });

  const submitButtonRef = useRef<HTMLButtonElement | null>(null);
  const [submitClicked, setSubmitClicked] = useState(false);
  const [showConfirmation, setShowConfirmation] = useToggle();

  const handleCloseModal = () => {
    setSubmitClicked(false); // disallow user to multi click the button
    setShowConfirmation();
  };

  useEffect(() => {
    const isAuthorOrTeacher =
      data.role === CourseUserStatus.Author ||
      data.role === CourseUserStatus.Teacher;
    customLayout.setExamPage && customLayout.setExamPage(true);
    customLayout.setExamPageAction &&
      customLayout.setExamPageAction(
        auth?.auth &&
          Number(auth?.auth?.role) >= UserRole.Trainer &&
          !isAuthorOrTeacher ? (
          <ExamCounter
            duration={data.duration}
            onSubmit={() => submitButtonRef.current?.click()}
            isLoading={examSubmission.isPending}
            onClick={() => setShowConfirmation()}
          />
        ) : (
          <Button onClick={() => navigate(-1)}>{t("close")}</Button>
        )
      );
    customLayout.setExamPageTitle &&
      customLayout.setExamPageTitle(<Title>{data.name}</Title>);
    return () => {
      customLayout.setExamPage && customLayout.setExamPage(false);
    };
  }, [customLayout.examPage]);

  const matches = useMediaQuery("(min-width: 56.25em)");
  const [visited, setVisited] = useState<number[]>([]);

  const onQuestionVisit = (index: number) => {
    if (!visited.includes(index)) {
      setVisited((visited) => [...visited, index]);
    }
  };

  const onSubmitHandler = async (
    values: ILessonStartQuestion<ILessonStartQuestionOption>[]
  ) => {
    const finalData: ILessonExamSubmit[] = [];
    values.forEach((x) => {
      const answers = x.questionOptions
        .filter((x) => x.isCorrect)
        .map((x) => x.id);

      finalData.push({
        questionSetQuestionId: x.questionSetQuestionId,
        answers: answers,
      });
    });
    try {
      await examSubmission.mutateAsync({
        data: finalData,
        lessonId: lessonId,
        questionSetSubmissionId: data.id,
      });
      setShowConfirmation();
    } catch (err) {
      console.log(err);
    }
  };

  return (
    <form onSubmit={form.onSubmit(onSubmitHandler)}>
      {/* confirmation pop-up Modal */}
      <Modal
        title={t("submit_exam_confirmation")}
        opened={showConfirmation}
        onClose={handleCloseModal}
      >
        <Group>
          <Button
            disabled={submitClicked}
            onClick={() => {
              setSubmitClicked(true);
              submitButtonRef && submitButtonRef.current?.click();
            }}
          >
            {t("submit")}
          </Button>
          <Button
            variant="outline"
            onClick={() => {
              setSubmitClicked(false);

              setShowConfirmation();
            }}
          >
            {t("cancel")}
          </Button>
        </Group>
      </Modal>

      {/* Notice pop-up Modal */}
      <Modal
        opened={examSubmission.isSuccess}
        onClose={() => {
          navigate(location.state + "?invalidate=true" ?? "/");
        }}
        title={t("submission_success")}
      >
        <Button
          onClick={() => {
            navigate(location.state + "?invalidate=true" ?? "/");
          }}
        >
          {t("close")}
        </Button>
      </Modal>

      <Grid m={20} className={classes.parentGrid}>
        {/* exam display section */}
        {/* <Grid.Col span={matches ? 9 : 12}> */}
        <Grid.Col
          span={matches ? 9 : 9}
          style={{ maxWidth: "100%" }}
          className={classes.questionGridCol}
        >
          <Box
            style={{
              flexDirection: "column",
              overflow: "auto",
            }}
          >
            <Box
              p={10}
              pb={20}
              style={{
                flexDirection: "column",
                width: "100%",
                justifyContent: "start",
                alignContent: "start",
              }}
            >
              <Title mb={20}>{questions[currentIndex]?.name}</Title>
              {questions[currentIndex]?.description && (
                <TextViewer
                  key={currentIndex}
                  content={questions[currentIndex]?.description}
                  styles={{ wordBreak: "break-all" }}
                />
              )}
            </Box>
            <Container className={classes.option}>
              {questions[currentIndex]?.type === QuestionType.MultipleChoice &&
                questions[currentIndex]?.questionOptions && (
                  <ExamCheckBox
                    currentIndex={currentIndex}
                    form={form}
                    options={questions[currentIndex]?.questionOptions}
                  />
                )}
              {questions[currentIndex]?.type === QuestionType.SingleChoice &&
                questions[currentIndex]?.questionOptions && (
                  <ExamRadio
                    currentIndex={currentIndex}
                    form={form}
                    options={questions[currentIndex]?.questionOptions}
                  />
                )}
            </Container>
          </Box>
          <Card p={20} className={classes.buttonNav}>
            {currentIndex !== 0 ? (
              <Button
                my={5}
                onClick={() => {
                  onQuestionVisit(currentIndex);
                  setCurrentIndex(currentIndex - 1);
                }}
                w={100}
              >
                {t("previous")}
              </Button>
            ) : (
              <div></div>
            )}
            <button style={{ display: "none" }} ref={submitButtonRef}></button>
            <Text my={5}>
              {currentIndex + 1}/{questions.length}
            </Text>

            {currentIndex < questions.length - 1 ? (
              <Button
                my={5}
                onClick={() => {
                  onQuestionVisit(currentIndex);
                  setCurrentIndex((currentIndex) => currentIndex + 1);
                }}
                w={100}
              >
                {t("next")}
              </Button>
            ) : (
              <div></div>
            )}
          </Card>
        </Grid.Col>

        {/* question counter section */}
        {/* <Grid.Col span={matches ? 3 : 12} m={0}> */}
        <Grid.Col span={3} m={0} className={classes.optionsGridCol}>
          <Group p={10} className={classes.navigateWrapper}>
            {form.values.map((x, i) => (
              <div
                key={i}
                onClick={() => {
                  setVisited((visited) => [...visited, currentIndex]);
                  setCurrentIndex(i);
                }}
                style={{
                  outline: "none",
                  border: "none",
                  backgroundColor: "none",
                }}
              >
                <Card
                  className={cx(classes.navigate, {
                    [classes.visited]:
                      visited.includes(i) &&
                      x.questionOptions.filter((x) => x.isCorrect).length <= 0,
                    [classes.answered]:
                      x.questionOptions.filter((x) => x.isCorrect).length > 0,
                    [classes.active]: currentIndex === i,
                  })}
                  radius={10000}
                >
                  {i + 1}
                </Card>
              </div>
            ))}
          </Group>
        </Grid.Col>
      </Grid>
    </form>
  );
};

export default Exam;
