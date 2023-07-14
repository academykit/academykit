/* eslint-disable */
import { Rating } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import { IFeedbackQuestions } from '@utils/services/feedbackService';
import { useEffect, useState } from 'react';

type Props = {
  form: UseFormReturnType<
    IFeedbackQuestions[],
    (values: IFeedbackQuestions[]) => IFeedbackQuestions[]
  >;
  currentIndex: number;
  readOnly: boolean;
};

const FeedbackRating = ({ form, currentIndex, readOnly }: Props) => {
  const [value, setValue] = useState(form.values[currentIndex].rating);

  useEffect(() => {
    form.setFieldValue(`${currentIndex}.rating`, value);
  }, [value]);

  return (
    <Rating
      value={value}
      onChange={setValue}
      size={'xl'}
      mt={10}
      readOnly={readOnly}
    />
  );
};

export default FeedbackRating;
