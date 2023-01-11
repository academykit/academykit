import { Button, Card, Group, Rating, Title } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import RichTextEditor from "@mantine/rte";
import { FeedbackType } from "@utils/enums";

import errorType from "@utils/services/axiosError";
import {
  IFeedbackQuestions,
  IFeedbackSubmission,
  useFeedbackSubmission,
} from "@utils/services/feedbackService";
import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import FeedbackCheckBoxType from "./Options/FeedbackCheckBox";
import FeedbackRadio from "./Options/FeedbackRadio";
import FeedbackRating from "./Options/FeedbackRating";
import FeedbackSubjective from "./Options/FeedbackSubjective";

const FeedbackForm = ({
  item,
  lessonId,
}: {
  item: IFeedbackQuestions[];
  lessonId: string;
}) => {
  const submitFeedback = useFeedbackSubmission({ lessonId });
  const { id } = useParams();
  const navigation = useNavigate();

  const form = useForm({
    initialValues: item,
  });

  const handleSubmit = async (values: IFeedbackQuestions[]) => {
    const finalData: IFeedbackSubmission[] = [];
    values.forEach((x) => {
      var data: any = {};
      data["feedbackId"] = x.id;
      data["answer"] = x.answer;
      data["rating"] = x.rating;
      if (x.feedbackQuestionOptions) {
        data["selectedOption"] = x.feedbackQuestionOptions
          .filter((y) => y.isSelected)
          .map((y) => y.id);
      }
      finalData.push(data);
    });

    try {
      await submitFeedback.mutateAsync({
        lessonId: id as string,
        data: finalData,
      });
      showNotification({
        message: `Thank you for your feedback.`,
      });
      navigation(-1);
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        title: "Error",
        color: "red",
      });
    }
  };

  return (
    <form onSubmit={form.onSubmit(handleSubmit)}>
      {item.map((x, currentIndex) => (
        <Card key={x.id} shadow="sm" my={10} withBorder>
          <Title>{x.name}</Title>

          {x.type === FeedbackType.MultipleChoice &&
            x?.feedbackQuestionOptions && (
              <FeedbackCheckBoxType
                options={x?.feedbackQuestionOptions}
                currentIndex={currentIndex}
                form={form}
              />
            )}
          {x.type === FeedbackType.SingleChoice &&
            x?.feedbackQuestionOptions && (
              <FeedbackRadio
                options={x?.feedbackQuestionOptions}
                currentIndex={currentIndex}
                form={form}
              />
            )}
          {x.type === FeedbackType.Subjective && (
            <FeedbackSubjective form={form} currentIndex={currentIndex} />
          )}
          {x.type === FeedbackType.Rating && (
            <FeedbackRating
              form={form}
              currentIndex={currentIndex}
              readOnly={false}
            />
          )}
        </Card>
      ))}
      <Group mt={20}>
        <Button loading={submitFeedback.isLoading} type="submit">
          Submit
        </Button>
        <Button type="reset" variant="outline" onClick={() => navigation(-1)}>
          Cancel
        </Button>
      </Group>
    </form>
  );
};

export default FeedbackForm;
