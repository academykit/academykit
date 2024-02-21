import TextViewer from '@components/Ui/RichTextViewer';
import { Box, Card, Group, Title } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import { IAssessmentExam } from '@utils/services/assessmentService';
import cx from 'clsx';
import { t } from 'i18next';
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

const AssessmentExamCheckBox = ({ form, options, currentIndex }: Props) => {
  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={'xs'} mb={5}>
          {t('options')} ({t('multiple_choice')})
        </Title>
      </Group>
      {options.map((option, index) => (
        <label key={option.optionId} htmlFor={option.optionId}>
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
            <TextViewer
              styles={{
                root: {
                  border: 'none',
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

export default AssessmentExamCheckBox;
