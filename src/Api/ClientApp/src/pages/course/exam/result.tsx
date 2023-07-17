import { Loader } from '@mantine/core';
import { useGetOneExamResult } from '@utils/services/examService';
import { useParams } from 'react-router-dom';
import SubmittedResultDetails from './Components/SubmittedResultDetails';

const ExamResult = () => {
  const { id, submissionId } = useParams();
  const userResult = useGetOneExamResult(id as string, submissionId as string);
  if (userResult.isLoading) {
    return <Loader />;
  }
  if (userResult.isError) {
    throw userResult.error;
  }
  if (userResult.isSuccess) {
    return (
      <SubmittedResultDetails
        user={userResult.data.user}
        submissionDate={userResult.data.submissionDate}
        questions={userResult.data.results}
        duration={userResult.data.completeDuration}
        marks={userResult.data.obtainedMarks}
        name={userResult.data.name}
        totalMarks={userResult.data.totalMarks}
        totalDuration={userResult.data.duration}
      />
    );
  }
  return <></>;
};

export default ExamResult;
