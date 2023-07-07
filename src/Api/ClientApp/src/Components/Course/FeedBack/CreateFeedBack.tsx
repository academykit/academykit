import { Box, Button, Container } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { useFeedbackQuestion } from "@utils/services/feedbackService";
import EditFeedback from "./EditFeedBack";
import FeedbackItem from "./FeedbackList";
import { useTranslation } from "react-i18next";

interface Props {
  lessonId: string;
}

const CreateFeedback = ({ lessonId }: Props) => {
  const [addQuestion, setAddQuestion] = useToggle();
  const { t } = useTranslation();
  const feedbackList = useFeedbackQuestion(lessonId, "");
  return (
    <Container>
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
            <Box mb={10}>{t("no_feedback_questions")}</Box>
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
        {t("add_feedback")}
      </Button>
    </Container>
  );
};

export default CreateFeedback;
