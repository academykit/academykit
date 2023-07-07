import TextViewer from "@components/Ui/RichTextViewer";
import {
  Button,
  Card,
  Container,
  createStyles,
  Group,
  Loader,
  Rating,
  Title,
} from "@mantine/core";
import { FeedbackType } from "@utils/enums";
import { useReAuth } from "@utils/services/authService";

import { useGetUserFeedback } from "@utils/services/feedbackService";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

const useStyle = createStyles((theme) => ({
  option: {
    ">label": {
      cursor: "pointer",
    },
  },
  active: {
    outline: `2px solid ${theme.colors[theme.primaryColor][1]}`,
  },
}));

const FeedbackResult = () => {
  const { classes, cx } = useStyle();
  const { id, studentId } = useParams();
  const navigate = useNavigate();
  const auth = useReAuth();
  const { t } = useTranslation();
  const getUserFeedback = useGetUserFeedback(
    id as string,
    studentId ? (studentId as string) : auth.data?.id ?? ""
  );

  if (getUserFeedback.isError) {
    throw getUserFeedback.error;
  }
  if (getUserFeedback.isLoading) {
    return <Loader />;
  }

  return (
    <Container>
      {getUserFeedback.data?.map((x, currentIndex) => (
        <Card key={x.id} shadow="sm" my={10} withBorder>
          <Title>{x.name}</Title>

          {x.type === FeedbackType.Subjective ? (
            <TextViewer content={x.answer ?? ""} />
          ) : x.type === FeedbackType.Rating ? (
            <Rating value={x.rating} size={"xl"} mt={10} readOnly={true} />
          ) : (
            x.feedbackQuestionOptions &&
            x.feedbackQuestionOptions.map((option) => (
              <Card
                shadow={"md"}
                my={10}
                p={10}
                className={cx({
                  [classes.active]: option.isSelected,
                })}
              >
                <TextViewer
                  styles={{
                    root: {
                      border: "none",
                    },
                  }}
                  content={option.option}
                ></TextViewer>
              </Card>
            ))
          )}
        </Card>
      ))}
      <Group mt={20}>
        <Button type="reset" variant="outline" onClick={() => navigate(-1)}>
          {t("go_back_button")}
        </Button>
      </Group>
    </Container>
  );
};

export default FeedbackResult;
