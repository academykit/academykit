/* eslint-disable @typescript-eslint/no-unused-vars */
import TextEditor from '@components/Ui/TextEditor';
import { UseFormReturnType } from '@mantine/form';
import { useDebouncedValue } from '@mantine/hooks';
import { IAssignmentQuestion } from '@utils/services/assignmentService';
import { useEffect, useState } from 'react';

type Props = {
  form: UseFormReturnType<
    IAssignmentQuestion[],
    (values: IAssignmentQuestion[]) => IAssignmentQuestion[]
  >;
  currentIndex: number;
};

const SubjectiveType = ({ form, currentIndex }: Props) => {
  const [value, setValue] = useState(form.values[currentIndex]?.answer || '');
  const [debounced] = useDebouncedValue(value, 100, { leading: true });
  useEffect(() => {
    form.setFieldValue(`${currentIndex}.answer`, debounced);
  }, [debounced, currentIndex]);
  return <TextEditor value={value} onChange={setValue} />;
};

export default SubjectiveType;
