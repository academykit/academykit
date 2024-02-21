import ExamCounter from '@components/Course/Classes/ExamCounter';
import TextViewer from '@components/Ui/RichTextViewer';
import useCustomLayout from '@context/LayoutProvider';
import useAuth from '@hooks/useAuth';
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
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { useMediaQuery, useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { QuestionType, UserRole } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import {
  IAssessmentExamDetail,
  IAssessmentExamSubmit,
  useSubmitAssessmentExam,
} from '@utils/services/assessmentService';
import errorType from '@utils/services/axiosError';
import cx from 'clsx';
import { t } from 'i18next';
import { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import classes from '../styles/assessmentExam.module.css';
import AssessmentExamCheckBox from './AssessmentExamCheckBox';
import AssessmentExamRadio from './AssessmentExamRadio';

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
  const matches = useMediaQuery('(min-width: 56.25em)');
  const [submitClicked, setSubmitClicked] = useState(false);
  const [showConfirmation, setShowConfirmation] = useToggle();

  const questions = data.questions;

  const form = useForm({
    initialValues: questions,
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
            isLoading={examSubmission.isLoading}
            onClick={() => setShowConfirmation()}
          />
        ) : (
          <Button onClick={() => navigate(-1)}>{t('close')}</Button>
        )
      );
    customLayout.setExamPageTitle &&
      customLayout.setExamPageTitle(<Title>{data.assessmentName}</Title>);
    return () => {
      customLayout.setExamPage && customLayout.setExamPage(false);
    };
  }, [customLayout.examPage]);

  const onSubmitHandler = async (values: typeof form.values) => {
    try {
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
        color: 'red',
      });
    }
  };

  return (
    <>
      <Modal
        title={t('submit_exam_confirmation')}
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
            {t('submit')}
          </Button>
          <Button
            variant="outline"
            onClick={() => {
              setSubmitClicked(false);
              setShowConfirmation();
            }}
          >
            {t('cancel')}
          </Button>
        </Group>
      </Modal>

      {/* Notice pop-up Modal */}
      <Modal
        opened={examSubmission.isSuccess}
        onClose={() => {
          navigate(RoutePath.assessment.description(params.id as string).route);
        }}
        title={t('submission_success')}
      >
        <Button
          onClick={() => {
            navigate(
              RoutePath.assessment.description(params.id as string).route
            );
          }}
        >
          {t('close')}
        </Button>
      </Modal>

      <form onSubmit={form.onSubmit(onSubmitHandler)}>
        <Grid m={20} className={classes.parentGrid}>
          {/* exam display section */}
          <Grid.Col
            span={matches ? 9 : 9}
            style={{ maxWidth: '100%' }}
            className={classes.questionGridCol}
          >
            <Box
              style={{
                flexDirection: 'column',
                overflow: 'auto',
              }}
            >
              <Box
                p={10}
                pb={20}
                style={{
                  flexDirection: 'column',
                  width: '100%',
                  justifyContent: 'start',
                  alignContent: 'start',
                }}
              >
                <Title mb={20}>{questions[currentIndex]?.questionName}</Title>
                {questions[currentIndex]?.description && (
                  <TextViewer
                    key={currentIndex}
                    content={questions[currentIndex]?.description}
                    styles={{ wordBreak: 'break-all' }}
                  />
                )}
              </Box>
              <Container className={classes.option}>
                {questions[currentIndex]?.type ===
                  QuestionType.MultipleChoice &&
                  questions[currentIndex]?.assessmentQuestionOptions && (
                    <AssessmentExamCheckBox
                      currentIndex={currentIndex}
                      form={form}
                      options={
                        questions[currentIndex]?.assessmentQuestionOptions
                      }
                    />
                  )}
                {questions[currentIndex]?.type === QuestionType.SingleChoice &&
                  questions[currentIndex]?.assessmentQuestionOptions && (
                    <AssessmentExamRadio
                      currentIndex={currentIndex}
                      form={form}
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
                  {t('previous')}
                </Button>
              ) : (
                <div></div>
              )}
              <button
                style={{ display: 'none' }}
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
                  {t('next')}
                </Button>
              ) : (
                <div></div>
              )}
            </Card>
          </Grid.Col>

          {/* question counter section */}
          <Grid.Col
            span={matches ? 3 : 3}
            m={0}
            className={classes.optionsGridCol}
          >
            <Group p={10} className={classes.navigateWrapper}>
              {form.values.map((x, i) => (
                <div
                  key={i}
                  onClick={() => {
                    setVisited((visited) => [...visited, currentIndex]);
                    setCurrentIndex(i);
                  }}
                  style={{
                    outline: 'none',
                    border: 'none',
                    backgroundColor: 'none',
                  }}
                >
                  <Card
                    className={cx(classes.navigate, {
                      [classes.visited]:
                        visited.includes(i) &&
                        x.assessmentQuestionOptions.filter((x) => x.isCorrect)
                          .length <= 0,
                      [classes.answered]:
                        x.assessmentQuestionOptions.filter((x) => x.isCorrect)
                          .length > 0,
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
    </>
  );
};

export default Exam;
