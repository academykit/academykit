import TextViewer from '@components/Ui/RichTextViewer';
import { Box, Card, Group, Title } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import { IAssessmentExam } from '@utils/services/assessmentService';
import cx from 'clsx';
import { useTranslation } from 'react-i18next';
import classes from '../styles/assessmentQuestion.module.css';

type Props = {
  form: UseFormReturnType<
    IAssessmentExam[],
    (values: IAssessmentExam[]) => IAssessmentExam[]
  >;
  options: [
    {
      optionId: string;
      option: string;
      order: number;
    },
  ];
  currentIndex: number;
};

const AssessmentExamRadio = ({ form, options, currentIndex }: Props) => {
  const { t } = useTranslation();
  const changeFieldValue = (optionCurrentIndex: number) => {
    options.map((_option, index) => {
      if (index !== optionCurrentIndex) {
        form.setFieldValue(
          `${currentIndex}.assessmentQuestionOptions.${index}.isCorrect`,
          false
        );
      } else {
        form.setFieldValue(
          `${currentIndex}.assessmentQuestionOptions.${optionCurrentIndex}.isCorrect`,
          true
        );
      }
    });
  };

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={'xs'} mb={5}>
          {t('options')}
        </Title>
      </Group>
      {options.map((option, index) => (
        <div
          style={{ cursor: 'pointer' }}
          key={option.optionId}
          onClick={() => changeFieldValue(index)}
        >
          <input
            type={'checkbox'}
            id={option.optionId}
            style={{ display: 'none' }}
            {...form.getInputProps(
              `${currentIndex}.assessmentQuestionOptions.${index}.isCorrect`
            )}
          ></input>
          <Card
            shadow={'md'}
            my={10}
            p={10}
            className={cx({
              [classes.active]:
                form.values[currentIndex].assessmentQuestionOptions[index]
                  .isCorrect,
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
        </div>
      ))}
    </Box>
  );
};

export default AssessmentExamRadio;
