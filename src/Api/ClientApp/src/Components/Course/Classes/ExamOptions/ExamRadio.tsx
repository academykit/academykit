import { Box, Card, createStyles, Group, Title } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import RichTextEditor from '@mantine/rte';
import { ILessonStartQuestion, ILessonStartQuestionOption } from '@utils/services/examService';
import React from 'react'

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
    form: UseFormReturnType<ILessonStartQuestion<ILessonStartQuestionOption>[], (values: ILessonStartQuestion<ILessonStartQuestionOption>[]) => ILessonStartQuestion<any>[]>,
    options:ILessonStartQuestionOption[]
    currentIndex: number;
}

const ExamRadio = ({form, options, currentIndex}:Props) => {
  const { classes, cx } = useStyle();
  const changeFieldValue = (optionCurrentIndex: number) => {
    options.map((option, index) => {
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
        <Title size={"xs"} mb={5}>Options</Title>
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
                //@ts-ignore
                form.values[currentIndex].questionOptions[index]
                  .isCorrect,
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
  )
}

export default ExamRadio