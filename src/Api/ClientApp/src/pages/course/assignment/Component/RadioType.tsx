import TextViewer from '@components/Ui/RichTextViewer';
import {
  Box,
  Card,
  Group,
  Title,
  UnstyledButton,
  useMantineColorScheme,
} from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import {
  IAssignmentOptions,
  IAssignmentQuestion,
} from '@utils/services/assignmentService';
import cx from 'clsx';
import { useTranslation } from 'react-i18next';
import classes from '../../styles/radioType.module.css';

type Props = {
  form: UseFormReturnType<
    IAssignmentQuestion[],
    (values: IAssignmentQuestion[]) => IAssignmentQuestion[]
  >;
  options: IAssignmentOptions[];
  currentIndex: number;
};

const RadioType = ({ options, form, currentIndex }: Props) => {
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
    options.forEach((_x, i) => {
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
  const theme = useMantineColorScheme();

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={'xs'}>
          {t('options')} ({t('single_choice')})
        </Title>
      </Group>
      {options.map((option, index) => (
        <UnstyledButton
          style={{
            cursor: 'pointer',
            width: '100%',
            color: theme.colorScheme == 'dark' ? 'white' : 'black',
          }}
          key={option.id}
          onClick={() => changeFieldValue(index)}
        >
          <input
            type={'radio'}
            id={option.id}
            style={{
              display: 'none',
              color: theme.colorScheme == 'dark' ? 'white' : 'black',
            }}
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
            style={{
              color: theme.colorScheme == 'dark' ? 'white' : 'black',
            }}
          >
            <input type={'checkbox'} style={{ display: 'none' }} />
            <TextViewer
              styles={{
                root: {
                  border: 'none',
                  color: theme.colorScheme == 'dark' ? 'white' : 'black',
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
