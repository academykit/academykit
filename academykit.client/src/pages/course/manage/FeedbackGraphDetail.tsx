import { Box } from '@mantine/core';
import { UseQueryResult } from '@tanstack/react-query';
import { FeedbackType } from '@utils/enums';
import { IFeedbackChart } from '@utils/services/feedbackService';
import HorizontalBarGraph from './Components/Graph/HorizontalBarGraph';
import RatingGraph from './Components/Graph/RatingGraph';
import SubjectiveData from './Components/Graph/SubjectiveData';

const FeedbackGraphDetail = ({
  chartData,
}: {
  chartData: UseQueryResult<IFeedbackChart[], unknown>;
}) => {
  return (
    <>
      <Box mb={25}>
        {chartData.data?.map((chart, index) => {
          if (chart.type == FeedbackType.Rating) {
            return (
              <RatingGraph
                key={index}
                name={chart.feedbackName}
                stats={chart.rating}
                responseCount={chart.ratingCount}
              />
            );
          } else if (chart.type == FeedbackType.SingleChoice) {
            return (
              <HorizontalBarGraph
                key={index}
                name={chart.feedbackName}
                feedbackOptions={chart.feedbackQuestionOptions}
                responseCount={chart.singleChoiceCount}
                type="SingleChoice"
              />
            );
          } else if (chart.type == FeedbackType.MultipleChoice) {
            return (
              <HorizontalBarGraph
                key={index}
                name={chart.feedbackName}
                feedbackOptions={chart.feedbackQuestionOptions}
                responseCount={chart.multipleChoiceCount}
                type="MultipleChoice"
              />
            );
          } else if (chart.type == FeedbackType.Subjective) {
            return (
              <SubjectiveData
                key={index}
                name={chart.feedbackName}
                answers={chart.subjectiveAnswer}
                responseCount={chart.answerCount}
              />
            );
          }
        })}
      </Box>
    </>
  );
};

export default FeedbackGraphDetail;
