import TextViewer from '@components/Ui/RichTextViewer';
import {
  Box,
  Card,
  createStyles,
  Group,
  Title,
  UnstyledButton,
} from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import {
  IAssignmentOptions,
  IAssignmentQuestion,
} from '@utils/services/assignmentService';
import { useTranslation } from 'react-i18next';

const useStyle = createStyles((theme) => ({
  option: {
    '>label': {
      cursor: 'pointer',
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

const RadioType = ({ options, form, currentIndex }: Props) => {
  const { classes, cx } = useStyle();
  const changeFieldValue = (optionCurrentIndex: number) => {
    options.map((_option, index) => {
      if (index !== optionCurrentIndex) {
        form.setFieldValue(
          `${currentIndex}.assignmentQuestionOptions.${index}.isSelected`,
          false
        );
      } else {
        form.setFieldValue(
          `${currentIndex}.assignmentQuestionOptions.${optionCurrentIndex}.isSelected`,
          true
        );
      }
    });
  };
  const { t } = useTranslation();
  const onChangeRadioType = (index: number) => {
    options.forEach((x, i) => {
      if (i === index) {
        return form.setFieldValue(
          `${currentIndex}.assignmentQuestionOptions.${index}.isSelected`,
          true
        );
      }
      return form.setFieldValue(
        `${currentIndex}.assignmentQuestionOptions.${index}.isSelected`,
        false
      );
    });
  };

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={'xs'}>{t('options')}</Title>
      </Group>
      {options.map((option, index) => (
        <UnstyledButton
          style={{ cursor: 'pointer' }}
          key={option.id}
          onClick={() => changeFieldValue(index)}
        >
          <input
            type={'radio'}
            id={option.id}
            style={{ display: 'none' }}
            onChange={() => onChangeRadioType(index)}
            checked={
              form.values[currentIndex].assignmentQuestionOptions![index]
                .isSelected
            }
          ></input>
          <Card
            shadow={'md'}
            my={10}
            p={10}
            className={cx({
              [classes.active]:
                form.values[currentIndex].assignmentQuestionOptions![index]
                  .isSelected,
            })}
          >
            <input type={'checkbox'} style={{ display: 'none' }} />
            <TextViewer
              styles={{
                root: {
                  border: 'none',
                },
              }}
              content={option.option}
            ></TextViewer>
          </Card>
        </UnstyledButton>
      ))}
    </Box>
  );
};

export default RadioType;
