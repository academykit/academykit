import TextViewer from "@components/Ui/RichTextViewer";
import { Box, Card, Group, Title } from "@mantine/core";
import { IAssessmentExam } from "@utils/services/assessmentService";
import { useFieldArray, useFormContext } from "react-hook-form";
import { useTranslation } from "react-i18next";
import classes from "../styles/assessmentQuestion.module.css";
type Props = {
  options: [
    {
      optionId: string;
      option: string;
      order: number;
    },
  ];
  currentIndex: number;
};

const AssessmentExamRadio = ({ currentIndex }: Props) => {
  const form = useFormContext<{ questions: IAssessmentExam[] }>();

  const { t } = useTranslation();
  const { fields } = useFieldArray({
    name: `questions.${currentIndex}.assessmentQuestionOptions`,
    control: form.control,
  });
  const changeFieldValue = (optionCurrentIndex: number) => {
    const updatedOptions = fields.map((option, index) => ({
      ...option,
      isCorrect: index === optionCurrentIndex,
    }));

    updatedOptions.forEach((option, index) =>
      form.setValue(
        `questions.${currentIndex}.assessmentQuestionOptions[${index}]`,
        option
      )
    );
  };

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"} mb={5}>
          {t("options")}
        </Title>
      </Group>
      {fields.map((option, index) => (
        <div
          style={{ cursor: "pointer" }}
          key={option.optionId}
          onClick={() => changeFieldValue(index)}
        >
          <input
            type="checkbox"
            className={classes.checkbox}
            style={{ display: "none" }}
            {...form.register(
              `questions.${currentIndex}.assessmentQuestionOptions[${index}].isCorrect` as keyof {
                questions: IAssessmentExam[];
              }
            )}
          />
          <Card shadow={"md"} my={10} p={10} className={classes.card}>
            <input type={"checkbox"} style={{ display: "none" }} />
            <TextViewer
              styles={{
                root: {
                  border: "none",
                },
              }}
              content={option.option}
            ></TextViewer>
          </Card>
        </div>
      ))}
    </Box>
  );
};

export default AssessmentExamRadio;
