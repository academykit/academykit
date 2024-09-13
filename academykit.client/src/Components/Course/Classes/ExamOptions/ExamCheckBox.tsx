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

const ExamCheckBox = ({ options, currentIndex }: Props) => {
  const form = useFormContext<{ questions: ILessonStartQuestionOption[] }>();
  const { t } = useTranslation();

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"} mb={5}>
          {t("options")} ({t("multiple_choice")})
        </Title>
      </Group>
      {options.map((option, index) => (
        <label
          htmlFor={`questions.${currentIndex}.questionOptions.${option.id}`}
          key={option.id}
        >
          <input
            className={classes.checkbox}
            type={"checkbox"}
            id={`questions.${currentIndex}.questionOptions.${option.id}`}
            style={{ display: "none" }}
            {...form.register(
              `questions.${currentIndex}.questionOptions.${index}.isCorrect` as keyof {
                questions: ILessonStartQuestion<ILessonStartQuestionOption[]>;
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

export default ExamCheckBox;
