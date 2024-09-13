import { Button, Card, Grid, Group } from "@mantine/core";
import { IAssessmentExam } from "@utils/services/assessmentService";
import { ILessonStartQuestionOption } from "@utils/services/examService";
import cx from "clsx";
import { Dispatch, SetStateAction } from "react";
import { UseFormReturn } from "react-hook-form";
import classes from "../styles/assessmentExam.module.css";

const QuestionIndex = ({
  setVisited,
  setCurrentIndex,
  currentIndex,
  visited,
  form,
}: {
  setVisited: Dispatch<SetStateAction<number[]>>;
  setCurrentIndex: Dispatch<SetStateAction<number>>;
  currentIndex: number;
  visited: number[];
  form: UseFormReturn<{
    questions: ILessonStartQuestionOption[] | IAssessmentExam[];
  }>;
}) => {
  const questions = form.watch("questions");

  const handleSelection = (index: number) => {
    setVisited((visited) => [...visited, currentIndex]);
    setCurrentIndex(index);
  };

  return (
    <Grid.Col span={3} m={0} className={classes.optionsGridCol}>
      <Group p={10} className={classes.navigateWrapper}>
        {questions?.map((x, i) => {
          const isAssessmentExam = "assessmentQuestionOptions" in x;

          return (
            <Button
              key={isAssessmentExam ? x.questionId : x.id}
              onClick={() => handleSelection(i)}
              variant="subtle"
              styles={() => ({
                root: {
                  background: "none",
                  border: "none",
                  padding: 0,
                  margin: 0,
                  color: "inherit",
                  font: "inherit",
                  textAlign: "inherit",
                  cursor: "pointer",
                  outline: "none",
                  boxShadow: "none",
                },
              })}
            >
              {isAssessmentExam ? (
                <Card
                  className={cx(classes.navigate, {
                    [classes.visited]:
                      visited.includes(i) &&
                      x.assessmentQuestionOptions.filter((opt) => opt.isCorrect)
                        .length <= 0,
                    [classes.answered]:
                      x.assessmentQuestionOptions.filter((opt) => opt.isCorrect)
                        .length > 0,
                    [classes.active]: currentIndex === i,
                  })}
                  radius={10000}
                >
                  {i + 1}
                </Card>
              ) : (
                <Card
                  className={cx(classes.navigate, {
                    [classes.visited]: visited.includes(i),
                    [classes.answered]: x.isCorrect,
                    [classes.active]: currentIndex === i,
                  })}
                  radius={10000}
                >
                  {i + 1}
                </Card>
              )}
            </Button>
          );
        })}
      </Group>
    </Grid.Col>
  );
};

export default QuestionIndex;
