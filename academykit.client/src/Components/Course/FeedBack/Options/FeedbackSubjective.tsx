import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import { UseFormReturnType } from '@mantine/form';
import { useDebouncedValue } from '@mantine/hooks';
import { IFeedbackQuestions } from '@utils/services/feedbackService';
import { useEffect, useState } from 'react';

type Props = {
  form: UseFormReturnType<
    IFeedbackQuestions[],
    (values: IFeedbackQuestions[]) => IFeedbackQuestions[]
  >;
  currentIndex: number;
};

const FeedbackSubjective = ({ form, currentIndex }: Props) => {
  const [value, setValue] = useState(form.values[currentIndex].answer);
  const [debounced] = useDebouncedValue(value, 200);
  useEffect(() => {
    form.setFieldValue(`${currentIndex}.answer`, debounced);
  }, [debounced]);
  return <RichTextEditor value={value} onChange={setValue} />;
};

export default FeedbackSubjective;
