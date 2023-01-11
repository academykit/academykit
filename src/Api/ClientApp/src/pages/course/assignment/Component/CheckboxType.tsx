import { Box, Card, createStyles, Group, Text, Title } from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import RichTextEditor from "@mantine/rte";
import {
  IAssignmentOptions,
  IAssignmentQuestion,
} from "@utils/services/assignmentService";

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
    IAssignmentQuestion[],
    (values: IAssignmentQuestion[]) => IAssignmentQuestion[]
  >;
  options: IAssignmentOptions[];
  currentIndex: number;
};

const CheckBoxType = ({ options, form, currentIndex }: Props) => {
  const { classes, cx } = useStyle();

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"}>Options</Title>
      </Group>
      {options.map((option, index) => (
        <label key={option.id} htmlFor={option.id}>
          <input
            type={"checkbox"}
            id={option.id}
            style={{ display: "none" }}
            {...form.getInputProps(
              `${currentIndex}.assignmentQuestionOptions.${index}.isSelected`
            )}
          ></input>
          <Card
            shadow={"md"}
            my={10}
            p={10}
            className={cx({
              [classes.active]:
                //@ts-ignore
                form.values[currentIndex].assignmentQuestionOptions[index]
                  .isSelected,
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

export default CheckBoxType;
