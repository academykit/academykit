import { Box, Card, createStyles, Group, Title } from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import RichTextEditor from "@mantine/rte";
import {
  ILessonStartQuestion,
  ILessonStartQuestionOption,
} from "@utils/services/examService";
import { useTranslation } from "react-i18next";

const useStyle = createStyles((theme) => ({
  option: {
    ">label": {
      cursor: "pointer",
    },
  },
  active: {
    outline: `2px solid ${theme.colors[theme.primaryColor][1]}`,
  },
}));

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
  const { classes, cx } = useStyle();
  const { t } = useTranslation();

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"} mb={5}>
          {t("options")}
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
                //@ts-ignore
                form.values[currentIndex].questionOptions[index].isCorrect,
            })}
          >
            <RichTextEditor
              styles={{
                root: {
                  border: "none",
                },
              }}
              readOnly
              value={option.option}
            ></RichTextEditor>
          </Card>
        </label>
      ))}
    </Box>
  );
};

export default ExamCheckBox;
