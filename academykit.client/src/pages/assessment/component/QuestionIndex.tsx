import { Card, Grid, Group } from "@mantine/core";
import { IAssessmentExam } from "@utils/services/assessmentService";
import cx from "clsx";
import { Dispatch, SetStateAction } from "react";
import { useFormContext } from "react-hook-form";
import classes from "../styles/assessmentExam.module.css";

const QuestionIndex = ({
  setVisited,
  setCurrentIndex,
  currentIndex,
  visited,
}: {
  setVisited: Dispatch<SetStateAction<number[]>>;
  setCurrentIndex: Dispatch<SetStateAction<number>>;
  currentIndex: number;
  visited: number[];
}) => {
  const form = useFormContext<{ questions: IAssessmentExam[] }>();
  const questions = form.watch("questions");

  const handleSelection = (index: number) => {
    setVisited((visited) => [...visited, currentIndex]);
    setCurrentIndex(index);
  };

  return (
    <Grid.Col span={3} m={0} className={classes.optionsGridCol}>
      <Group p={10} className={classes.navigateWrapper}>
        {questions?.map((x, i) => (
          <div
            role="button"
            tabIndex={0}
            key={x.questionId}
            onClick={() => handleSelection(i)}
            onKeyDown={(e) => {
              if (e.key === "Enter" || e.key === " ") {
                handleSelection(i);
              }
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
  );
};

export default QuestionIndex;
