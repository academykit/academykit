import TextViewer from "@components/Ui/RichTextViewer";
import { Box, Card, Group, Title } from "@mantine/core";
import { IAssessmentExam } from "@utils/services/assessmentService";
import { t } from "i18next";
import { useFormContext } from "react-hook-form";
import classes from "../styles/assessmentQuestion.module.css";
import { OptionProps } from "./AssessmentExamRadio";

const AssessmentExamCheckBox = ({ currentIndex, options }: OptionProps) => {
  const form = useFormContext<{ questions: IAssessmentExam[] }>();

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"} mb={5}>
          {t("options")} ({t("multiple_choice")})
        </Title>
      </Group>
      {options.map((option, index) => (
        <label
          htmlFor={`questions.${currentIndex}.assessmentQuestionOptions.${index}.isCorrect`}
          key={option.optionId}
        >
          <input
            className={classes.checkbox}
            type={"checkbox"}
            id={`questions.${currentIndex}.assessmentQuestionOptions.${index}.isCorrect`}
            style={{ display: "none" }}
            {...form.register(
              `questions.${currentIndex}.assessmentQuestionOptions.${index}.isCorrect` as keyof {
                questions: IAssessmentExam[];
              }
            )}
          ></input>
          <Card shadow={"md"} my={10} p={10} className={classes.card}>
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
