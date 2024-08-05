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

const ExamCheckBox = ({ form, options, currentIndex }: Props) => {
  const { t } = useTranslation();

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"} mb={5}>
          {t("options")} ({t("multiple_choice")})
        </Title>
      </Group>
      {options.map((option, index) => (
        <label key={option.id} htmlFor={option.id}>
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
