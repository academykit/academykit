import { Button, Card, Group, Title } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import RichTextEditor from "@mantine/rte";
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

  const form = useForm({
    initialValues: item,
  });

  const handleSubmit = async (values: IAssignmentQuestion[]) => {
    const finalData: IAssignmentSubmission[] = [];
    values.forEach((x) => {
      var data: any = {};
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
        message: `Successfully submitted assignment`,
        title: "Success",
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
          <RichTextEditor
            styles={{
              root: {
                border: "none",
              },
            }}
            my={10}
            value={x.description}
            readOnly
          ></RichTextEditor>
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
      <Group mt={20}>
        <Button loading={submitAssignment.isLoading} type="submit">
          Submit
        </Button>
        <Button type="reset" variant="outline" onClick={() => navigation(-1)}>
          Cancel
        </Button>
      </Group>
    </form>
  );
};

export default AssignmentForm;