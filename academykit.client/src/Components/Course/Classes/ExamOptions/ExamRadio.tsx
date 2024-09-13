import TextViewer from "@components/Ui/RichTextViewer";
import { Box, Card, Group, Title } from "@mantine/core";
import {
  ILessonStartQuestion,
  ILessonStartQuestionOption,
} from "@utils/services/examService";
import { useFormContext } from "react-hook-form";
import { useTranslation } from "react-i18next";
import classes from "../style/class.module.css";

type Props = {
  options: ILessonStartQuestionOption[];
  currentIndex: number;
};

const ExamRadio = ({ options, currentIndex }: Props) => {
  const { t } = useTranslation();
  const form = useFormContext<{
    questions: ILessonStartQuestion<ILessonStartQuestionOption>[];
  }>();

  const changeFieldValue = (optionCurrentIndex: number) => {
    const updatedOptions = options.map((option, index) => ({
      ...option,
      isCorrect: index === optionCurrentIndex,
    }));

    updatedOptions.forEach((option, index) =>
      form.setValue(
        `questions.${currentIndex}.questionOptions[${index}]`,
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
      {options.map((option, index) => (
        <div
          style={{ cursor: "pointer" }}
          key={option.id}
          onClick={() => changeFieldValue(index)}
        >
          <input
            type="checkbox"
            className={classes.checkbox}
            style={{ display: "none" }}
            {...form.register(
              `questions.${currentIndex}.questionOptions.${index}.isCorrect` as keyof {
                questions: ILessonStartQuestion<ILessonStartQuestionOption[]>;
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

export default ExamRadio;
