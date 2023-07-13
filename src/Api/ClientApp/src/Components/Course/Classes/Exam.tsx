import useCustomLayout from "@context/LayoutProvider";
import {
  Box,
  Button,
  Card,
  Container,
  createStyles,
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
import { useTranslation } from "react-i18next";
import { RefObject, useEffect, useRef, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import ExamCounter from "./ExamCounter";
import ExamCheckBox from "./ExamOptions/ExamCheckBox";
import ExamRadio from "./ExamOptions/ExamRadio";
import useAuth from "@hooks/useAuth";
import TextViewer from "@components/Ui/RichTextViewer";

const useStyle = createStyles((theme) => ({
  option: {
    padding: 20,
    width: "100%",
    justifyContent: "start",
    alignItems: "start",
    borderRadius: "5px",
    border: "1px solid gray",
    ">label": {
      cursor: "pointer",
    },
    marginBottom: "15px",
  },
  navigate: {
    display: "flex",
    height: "50px",
    width: "50px",
    justifyContent: "center",
    alignItems: "center",
    cursor: "pointer",
  },
  navigateWrapper: {
    border: "1px solid grey",
    borderRadius: "5px",

    maxHeight: "80vh",
    height: "100%",
    overflowY: "auto",
    alignContent: "start",
    justifyContent: "start",
  },
  buttonNav: {
    display: "flex",
    justifyContent: "space-between",
    position: "fixed",
    bottom: "0",
    right: "0",
    width: "100%",
    zIndex: 100,
  },
  active: {
    border: `3px solid ${theme.colors[theme.primaryColor][1]}`,
  },
  visited: {
    border: `2px solid ${theme.colors.yellow[4]}`,
  },
  answered: {
    backgroundColor: theme.colors[theme.primaryColor][1],
  },
}));

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

  const submitButtonRef = useRef<RefObject<HTMLButtonElement>>();
  const [showConfirmation, setShowConfirmation] = useToggle();

  useEffect(() => {
    const isAuthorOrTeacher =
      data.role === CourseUserStatus.Author ||
      data.role === CourseUserStatus.Teacher;
    customLayout.setExamPage && customLayout.setExamPage(true);
    customLayout.setExamPageAction &&
      customLayout.setExamPageAction(
        auth?.auth &&
          auth?.auth?.role >= UserRole.Trainer &&
          !isAuthorOrTeacher ? (
          <ExamCounter
            duration={data.duration}
            // @ts-ignore
            onSubmit={() => submitButtonRef.current.click()}
            isLoading={examSubmission.isLoading}
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
  const { classes, theme, cx } = useStyle();
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.md}px)`);
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
    } catch (err) {}
  };

  return (
    <form onSubmit={form.onSubmit(onSubmitHandler)}>
      <Modal
        title={t("submit_exam_confirmation")}
        opened={showConfirmation}
        onClose={setShowConfirmation}
      >
        <Group>
          <Button
            onClick={() => {
              // @ts-ignore
              submitButtonRef && submitButtonRef.current?.click();
            }}
          >
            {t("submit")}
          </Button>
          <Button variant="outline" onClick={() => setShowConfirmation()}>
            {t("cancel")}
          </Button>
        </Group>
      </Modal>
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
      <Grid m={20}>
        {/* exam display section */}
        <Grid.Col span={matches ? 9 : 12}>
          <Box
            sx={{
              flexDirection: "column",
              overflow: "auto",
            }}
          >
            <Box
              p={10}
              pb={20}
              sx={{
                flexDirection: "column",
                width: "100%",
                justifyContent: "start",
                alignContent: "start",
              }}
            >
              <Title mb={20}>{questions[currentIndex]?.name}</Title>
              {questions[currentIndex]?.description && (
                <TextViewer
                  content={questions[currentIndex]?.description}
                  sx={{ wordBreak: "break-all" }}
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
          <Card p={4} px={20} className={classes.buttonNav}>
            {currentIndex !== 0 ? (
              <Button
                my={5}
                onClick={() => {
                  onQuestionVisit(currentIndex);
                  setCurrentIndex(currentIndex - 1);
                }}
              >
                {t("previous")}
              </Button>
            ) : (
              <div></div>
            )}
            {/* @ts-ignore */}
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
              >
                {t("next")}
              </Button>
            ) : (
              <div></div>
            )}
          </Card>
        </Grid.Col>

        {/* question counter section */}
        <Grid.Col span={matches ? 3 : 12} m={0}>
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
