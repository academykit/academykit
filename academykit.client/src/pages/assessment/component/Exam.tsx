import ExamCounter from "@components/Course/Classes/ExamCounter";
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
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { QuestionType, UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import {
  IAssessmentExam,
  IAssessmentExamDetail,
  IAssessmentExamSubmit,
  useSubmitAssessmentExam,
} from "@utils/services/assessmentService";
import errorType from "@utils/services/axiosError";
import { ILessonStartQuestionOption } from "@utils/services/examService";
import { t } from "i18next";
import { useEffect, useRef, useState } from "react";
import { FormProvider, UseFormReturn, useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import classes from "../styles/assessmentExam.module.css";
import AssessmentExamCheckBox from "./AssessmentExamCheckBox";
import AssessmentExamRadio from "./AssessmentExamRadio";
import QuestionIndex from "./QuestionIndex";

const Exam = ({
  data,
  assessmentId,
}: {
  data: IAssessmentExamDetail;
  assessmentId: string;
}) => {
  const params = useParams();
  const submitButtonRef = useRef<HTMLButtonElement | null>(null);
  const customLayout = useCustomLayout();
  const navigate = useNavigate();
  const auth = useAuth();
  const examSubmission = useSubmitAssessmentExam();
  const [currentIndex, setCurrentIndex] = useState(0);
  const [visited, setVisited] = useState<number[]>([]);
  const [submitClicked, setSubmitClicked] = useState(false);
  const [showConfirmation, setShowConfirmation] = useToggle();

  const questions = data.questions;

  const form = useForm({
    defaultValues: {
      questions: [...questions],
    },
  });
  const onQuestionVisit = (index: number) => {
    if (!visited.includes(index)) {
      setVisited((visited) => [...visited, index]);
    }
  };
  const handleCloseModal = () => {
    setSubmitClicked(false); // disallow user to multi click the button
    setShowConfirmation();
  };

  useEffect(() => {
    customLayout.setExamPage && customLayout.setExamPage(true);
    customLayout.setExamPageAction &&
      customLayout.setExamPageAction(
        auth?.auth && Number(auth?.auth?.role) >= UserRole.Trainer ? (
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
      customLayout.setExamPageTitle(<Title>{data.assessmentName}</Title>);
    return () => {
      customLayout.setExamPage && customLayout.setExamPage(false);
    };
  }, [customLayout.examPage]);

  const onSubmitHandler = async (d: { questions: IAssessmentExam[] }) => {
    try {
      const values = d.questions;
      const finalData: IAssessmentExamSubmit[] = [];
      values.forEach((x) => {
        const answers = x.assessmentQuestionOptions
          .filter((x) => x.isCorrect)
          .map((x) => x.optionId);

        finalData.push({
          assessmentQuestionId: x.questionId,
          answers: answers,
        });
      });

      await examSubmission.mutateAsync({
        assessmentId: assessmentId,
        data: finalData,
      });
      setShowConfirmation();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <>
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
          navigate(
            RoutePath.assessment.description(params.id as string).route,
            { replace: true }
          );
        }}
        title={t("submission_success")}
      >
        <Button
          onClick={() => {
            navigate(
              RoutePath.assessment.description(params.id as string).route,
              { replace: true }
            );
          }}
        >
          {t("close")}
        </Button>
      </Modal>
      <FormProvider {...form}>
        <form onSubmit={form.handleSubmit(onSubmitHandler)}>
          <Grid m={20} className={classes.parentGrid}>
            {/* exam display section */}
            <Grid.Col
              span={9}
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
                  <Title mb={20}>{questions[currentIndex]?.questionName}</Title>
                  {questions[currentIndex]?.description && (
                    <TextViewer
                      key={currentIndex}
                      content={questions[currentIndex]?.description}
                      styles={{ wordBreak: "break-all" }}
                    />
                  )}
                </Box>
                <Container className={classes.option}>
                  {questions[currentIndex]?.type ===
                    QuestionType.MultipleChoice &&
                    questions[currentIndex]?.assessmentQuestionOptions && (
                      <AssessmentExamCheckBox
                        currentIndex={currentIndex}
                        options={
                          questions[currentIndex]?.assessmentQuestionOptions
                        }
                      />
                    )}
                  {questions[currentIndex]?.type ===
                    QuestionType.SingleChoice &&
                    questions[currentIndex]?.assessmentQuestionOptions && (
                      <AssessmentExamRadio
                        currentIndex={currentIndex}
                        options={
                          questions[currentIndex]?.assessmentQuestionOptions
                        }
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
                <button
                  style={{ display: "none" }}
                  ref={submitButtonRef}
                ></button>
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

            <QuestionIndex
              currentIndex={currentIndex}
              setCurrentIndex={setCurrentIndex}
              setVisited={setVisited}
              visited={visited}
              form={
                form as UseFormReturn<{
                  questions: IAssessmentExam[] | ILessonStartQuestionOption[];
                }>
              }
            />
          </Grid>
        </form>
      </FormProvider>
    </>
  );
};

export default Exam;
