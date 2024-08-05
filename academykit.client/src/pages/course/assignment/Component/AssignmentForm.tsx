import { Button, Card, Group, Title } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { QuestionType } from "@utils/enums";
import {
  IAssignmentQuestion,
  IAssignmentSubmission,
  useSubmitAssignment,
} from "@utils/services/assignmentService";
import errorType from "@utils/services/axiosError";
import { useNavigate, useParams } from "react-router-dom";
import CheckboxType from "./CheckboxType";
import RadioType from "./RadioType";
import SubjectiveType from "./SubjectiveType";
import { useTranslation } from "react-i18next";
import TextViewer from "@components/Ui/RichTextViewer";

const AssignmentForm = ({
  item,
  lessonId,
}: {
  item: IAssignmentQuestion[];
  lessonId: string;
}) => {
  const submitAssignment = useSubmitAssignment({ lessonId });
  const { id } = useParams();
  const navigation = useNavigate();
  const { t } = useTranslation();

  const form = useForm({
    initialValues: item,
  });

  const handleSubmit = async (values: IAssignmentQuestion[]) => {
    const finalData: IAssignmentSubmission[] = [];
    values.forEach((x) => {
      const data: any = {};
      if (x.assignmentSubmissionId) data["id"] = x.assignmentSubmissionId;
      data["assignmentId"] = x.id;
      data["answer"] = x.answer;
      if (x.assignmentQuestionOptions) {
        data["selectedOption"] = x.assignmentQuestionOptions
          .filter((y) => y.isSelected)
          .map((y) => y.id);
      }
      finalData.push(data);
    });
    try {
      await submitAssignment.mutateAsync({
        lessonId: id as string,
        data: finalData,
      });
      showNotification({
        message: t("submit_assignment_success"),
        title: t("success"),
      });
      navigation(-1);
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        title: t("error"),
        color: "red",
      });
    }
  };

  return (
    <form onSubmit={form.onSubmit(handleSubmit)}>
      {item.map((x, currentIndex) => (
        <Card key={x.id} shadow="sm" my={10} withBorder>
          <Title>{x.name}</Title>
          <TextViewer
            styles={{
              root: {
                border: "none",
              },
            }}
            content={x.description}
          ></TextViewer>
          {x.type === QuestionType.MultipleChoice &&
            x?.assignmentQuestionOptions && (
              <CheckboxType
                options={x?.assignmentQuestionOptions}
                currentIndex={currentIndex}
                form={form}
              />
            )}
          {x.type === QuestionType.SingleChoice &&
            x?.assignmentQuestionOptions && (
              <RadioType
                options={x?.assignmentQuestionOptions}
                currentIndex={currentIndex}
                form={form}
              />
            )}
          {x.type == QuestionType.Subjective && (
            <SubjectiveType form={form} currentIndex={currentIndex} />
          )}
        </Card>
      ))}
      {item[0].isTrainee && (
        <Group mt={20}>
          <Button loading={submitAssignment.isLoading} type="submit">
            {t("submit")}
          </Button>
          <Button type="reset" variant="outline" onClick={() => navigation(-1)}>
            {t("cancel")}
          </Button>
        </Group>
      )}
      {!item[0].isTrainee && (
        <Group mt={20}>
          <Button type="reset" variant="outline" onClick={() => navigation(-1)}>
            {t("close")}
          </Button>
        </Group>
      )}
    </form>
  );
};

export default AssignmentForm;
