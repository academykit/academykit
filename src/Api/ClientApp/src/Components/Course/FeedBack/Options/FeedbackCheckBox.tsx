import TextViewer from '@components/Ui/RichTextViewer';
import { Box, Card, createStyles, Group, Title } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';

import {
  IFeedbackOptions,
  IFeedbackQuestions,
} from '@utils/services/feedbackService';
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
    IFeedbackQuestions[],
    (values: IFeedbackQuestions[]) => IFeedbackQuestions[]
  >;
  options: IFeedbackOptions[];
  currentIndex: number;
};

const FeedbackCheckBoxType = ({ options, form, currentIndex }: Props) => {
  const { classes, cx } = useStyle();
  const { t } = useTranslation();

  return (
    <Box mt={10} px={20} className={classes.option}>
      <Group>
        <Title size={'xs'}>
          {t('options')} ({t('multiple_choice')})
        </Title>
      </Group>
      {options.map((option, index) => (
        <label key={option.id} htmlFor={option.id}>
          <input
            type={'checkbox'}
            id={option.id}
            style={{ display: 'none' }}
            {...form.getInputProps(
              `${currentIndex}.feedbackQuestionOptions.${index}.isSelected`
            )}
          ></input>
          <Card
            shadow={'md'}
            my={10}
            p={10}
            className={cx({
              [classes.active]:
                form.values[currentIndex].feedbackQuestionOptions![index]
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
        </label>
      ))}
    </Box>
  );
};

export default FeedbackCheckBoxType;
