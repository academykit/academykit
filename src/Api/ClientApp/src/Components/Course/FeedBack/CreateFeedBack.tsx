import Breadcrumb from "@components/Ui/BreadCrumb";

import { Box, Button, Container } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { useFeedbackQuestion } from "@utils/services/feedbackService";
import EditFeedback from "./EditFeedBack";
import FeedbackItem from "./FeedbackList";

interface Props {
  lessonId: string;
}

const CreateFeedback = ({ lessonId }: Props) => {
  const [addQuestion, setAddQuestion] = useToggle();

  const feedbackList = useFeedbackQuestion(lessonId, "");
  return (
    <Container>
      <Breadcrumb hide={3} />

      {feedbackList.isSuccess && (
        <>
          {feedbackList.data.length > 0 ? (
            <Box>
              {feedbackList.data.map((x) => (
                <FeedbackItem
                  key={x.id}
                  data={x}
                  search={""}
                  lessonId={lessonId}
                />
              ))}
            </Box>
          ) : (
            <Box mb={10}>No Questions Found!</Box>
          )}
        </>
      )}
      {addQuestion && (
        <EditFeedback
          onCancel={() => setAddQuestion()}
          lessonId={lessonId}
          search={""}
        />
      )}
      <Button onClick={() => setAddQuestion()} mt={10}>
        Add Feedback
      </Button>
    </Container>
  );
};

export default CreateFeedback;
