import TextViewer from "@components/Ui/RichTextViewer";
import { Box, Card, Group, Title } from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import {
  ILessonStartQuestion,
  ILessonStartQuestionOption,
} from "@utils/services/examService";
import cx from "clsx";
import { useTranslation } from "react-i18next";
import classes from "../style/class.module.css";

type Props = {
  form: UseFormReturnType<
    ILessonStartQuestion<ILessonStartQuestionOption>[],
    (
      values: ILessonStartQuestion<ILessonStartQuestionOption>[]
    ) => ILessonStartQuestion<any>[]
  >;
  options: ILessonStartQuestionOption[];
  currentIndex: number;
};

const ExamRadio = ({ form, options, currentIndex }: Props) => {
  const { t } = useTranslation();
  const changeFieldValue = (optionCurrentIndex: number) => {
    options.map((_option, index) => {
      if (index !== optionCurrentIndex) {
        form.setFieldValue(
          `${currentIndex}.questionOptions.${index}.isCorrect`,
          false
        );
      } else {
        form.setFieldValue(
          `${currentIndex}.questionOptions.${optionCurrentIndex}.isCorrect`,
          true
        );
      }
    });
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
            type={"checkbox"}
            id={option.id}
            style={{ display: "none" }}
            {...form.getInputProps(
              `${currentIndex}.questionOptions.${index}.isCorrect`
            )}
          ></input>
          <Card
            shadow={"md"}
            my={10}
            p={10}
            className={cx({
              [classes.active]:
                form.values[currentIndex].questionOptions[index].isCorrect,
            })}
          >
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
