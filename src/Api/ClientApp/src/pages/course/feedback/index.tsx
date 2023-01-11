import FeedbackForm from "@components/Course/FeedBack/ViewFeedback";
import { Container, Divider } from "@mantine/core";

import { useFeedbackQuestion } from "@utils/services/feedbackService";
import { useParams } from "react-router-dom";

const FeedbackPage = () => {
  const { id } = useParams();

  const feedback = useFeedbackQuestion(id as string, "");

  return (
    <Container my={50}>
      {feedback.isSuccess && (
        <FeedbackForm item={feedback.data} lessonId={id as string} />
      )}
    </Container>
  );
};

export default FeedbackPage;
