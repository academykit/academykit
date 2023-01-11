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
import RichTextEditor from "@mantine/rte";
import RadioType from "@pages/course/assignment/Component/RadioType";
import { QuestionType } from "@utils/enums";
import {
  ILessonExamStart,
  ILessonExamSubmit,
  ILessonStartQuestion,
  ILessonStartQuestionOption,
  useSubmitExam,
} from "@utils/services/examService";
import { RefObject, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import ExamCounter from "./ExamCounter";
import ExamCheckBox from "./ExamOptions/ExamCheckBox";
import ExamRadio from "./ExamOptions/ExamRadio";

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
  },
  navigate: {
    display: "flex",
    height: "50px",
    width: "50px",
    justifyContent: "center",
    alignItems: "center",
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

  const form = useForm({
    initialValues: questions,
  });

  const submitButtonRef = useRef<RefObject<HTMLButtonElement>>();
  const [showConfirmation, setShowConfirmation] = useToggle();

  useEffect(() => {
    customLayout.setExamPage && customLayout.setExamPage(true);
    customLayout.setExamPageAction &&
      customLayout.setExamPageAction(
        <ExamCounter
          duration={data.duration}
          // @ts-ignore
          onSubmit={() => submitButtonRef.current.click()}
          isLoading={examSubmission.isLoading}
          onClick={() => setShowConfirmation()}
        />
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
  const navigate = useNavigate();

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
        title="Are you sure you want to submit this exam?"
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
            Submit
          </Button>
          <Button variant="outline" onClick={() => setShowConfirmation()}>
            Cancel
          </Button>
        </Group>
      </Modal>
      <Modal
        opened={examSubmission.isSuccess}
        onClose={() => navigate(-1)}
        title="Exam is submitted successfully. Please  be patient for your result"
      >
        <Button onClick={() => navigate(-1)}>Close</Button>
      </Modal>
      <Grid m={20}>
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
                <RichTextEditor
                  readOnly
                  value={questions[currentIndex]?.description}
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
                onClick={() => {
                  onQuestionVisit(currentIndex);
                  setCurrentIndex(currentIndex - 1);
                }}
              >
                Previous
              </Button>
            ) : (
              <div></div>
            )}
            {/* @ts-ignore */}
            <button style={{ display: "none" }} ref={submitButtonRef}></button>
            <Text>
              {currentIndex + 1}/{questions.length}
            </Text>

            {currentIndex < questions.length - 1 ? (
              <Button
                onClick={() => {
                  onQuestionVisit(currentIndex);
                  setCurrentIndex((currentIndex) => currentIndex + 1);
                }}
              >
                Next
              </Button>
            ) : (
              <div></div>
            )}
          </Card>
        </Grid.Col>
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
