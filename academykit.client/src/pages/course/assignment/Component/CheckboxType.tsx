import TextViewer from "@components/Ui/RichTextViewer";
import { Box, Card, Group, Title } from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import {
  IAssignmentOptions,
  IAssignmentQuestion,
} from "@utils/services/assignmentService";
import cx from "clsx";
import { useTranslation } from "react-i18next";
import classes from "../../styles/radioType.module.css";

type Props = {
  form: UseFormReturnType<
    IAssignmentQuestion[],
    (values: IAssignmentQuestion[]) => IAssignmentQuestion[]
  >;
  options: IAssignmentOptions[];
  currentIndex: number;
};

const CheckBoxType = ({ options, form, currentIndex }: Props) => {
  const { t } = useTranslation();
  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={"xs"}>
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
              `${currentIndex}.assignmentQuestionOptions.${index}.isSelected`
            )}
          ></input>
          <Card
            shadow={"md"}
            my={10}
            p={10}
            className={cx({
              [classes.active]:
                form.values[currentIndex].assignmentQuestionOptions![index]
                  .isSelected,
            })}
            style={{ cursor: "pointer" }}
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

export default CheckBoxType;
