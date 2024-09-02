import TextViewer from "@components/Ui/RichTextViewer";
import { Box, Card, Group, Title } from "@mantine/core";
import { IAssessmentExam } from "@utils/services/assessmentService";
import { t } from "i18next";
import { useFieldArray, useFormContext } from "react-hook-form";
import classes from "../styles/assessmentQuestion.module.css";

type Props = {
  
  currentIndex: number;
};

const AssessmentExamCheckBox = ({ currentIndex }: Props) => {
  const form = useFormContext<{ questions: IAssessmentExam[] }>();
  const { fields } = useFieldArray({
    name: `questions.${currentIndex}.assessmentQuestionOptions`,
    control: form.control,
  });
  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"} mb={5}>
          {t("options")} ({t("multiple_choice")})
        </Title>
      </Group>
      {fields.map((option, index) => (
        <label
          htmlFor={
            `questions.${currentIndex}.assessmentQuestionOptions.${index}.isCorrect`
          }
          key={option.optionId}
        >
          <input
             className={classes.checkbox}
            type={"checkbox"}
            id={
              `questions.${currentIndex}.assessmentQuestionOptions.${index}.isCorrect`
            }
            style={{ display: "none" }}
            {...form.register(
              `questions.${currentIndex}.assessmentQuestionOptions.${index}.isCorrect` as keyof {questions:IAssessmentExam[]},
            )}
           
          ></input>
          <Card
            shadow={"md"}
            my={10}
            p={10}
            className={classes.card}
          >
            <TextViewer
              styles={{
                root: {
                  border: "none",
                },
              }}
              content={option.option}
            ></TextViewer>
          </Card>
        </label>
      ))}
    </Box>
  );
};

export default AssessmentExamCheckBox;
