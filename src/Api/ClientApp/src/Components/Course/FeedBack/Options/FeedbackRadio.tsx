import {
  Box,
  Button,
  Card,
  createStyles,
  Group,
  Text,
  Title,
} from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import RichTextEditor from "@mantine/rte";

import {
  IFeedbackOptions,
  IFeedbackQuestions,
} from "@utils/services/feedbackService";

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
    IFeedbackQuestions[],
    (values: IFeedbackQuestions[]) => IFeedbackQuestions[]
  >;
  options: IFeedbackOptions[];
  currentIndex: number;
};

const FeedbackRadio = ({ options, form, currentIndex }: Props) => {
  const { classes, cx } = useStyle();
  const changeFieldValue = (optionCurrentIndex: number) => {
    options.map((option, index) => {
      if (index !== optionCurrentIndex) {
        form.setFieldValue(
          `${currentIndex}.feedbackQuestionOptions.${index}.isSelected`,
          false
        );
      } else {
        form.setFieldValue(
          `${currentIndex}.feedbackQuestionOptions.${optionCurrentIndex}.isSelected`,
          true
        );
      }
    });
  };

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"}>Options</Title>
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
              `${currentIndex}.feedbackQuestionOptions.${index}.isSelected`
            )}
          ></input>
          <Card
            shadow={"md"}
            my={10}
            p={10}
            className={cx({
              [classes.active]:
                //@ts-ignore
                form.values[currentIndex].feedbackQuestionOptions[index]
                  .isSelected,
            })}
          >
            <input type={"checkbox"} style={{ display: "none" }} />
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
        </div>
      ))}
    </Box>
  );
};

export default FeedbackRadio;