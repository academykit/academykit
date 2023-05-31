import {
  Box,
  Button,
  Group,
  NumberInput,
  Switch,
  Textarea,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import {
  IAddAssignmentReview,
  IAssignmentReview,
  useAddReview,
  useEditReview,
} from "@utils/services/assignmentService";
import errorType from "@utils/services/axiosError";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

const AssignmentReviewForm = ({
  closeModal,
  edit,
  reviewId,
  editData,
}: {
  closeModal: () => void;
  edit: boolean;
  reviewId?: string;
  editData?: IAssignmentReview;
}) => {
  const { studentId, id } = useParams();
  const addReview = useAddReview(id as string, studentId as string);
  const editReview = useEditReview(id as string, studentId as string);

  const [isPass, setIsPass] = useState(
    editData?.assignmentReview?.isPassed ?? false
  );
  const{t}= useTranslation();

  const submitHandler = async (data: IAddAssignmentReview) => {
    try {
      if (edit) {
        await editReview.mutateAsync({
          data,
          lessonId: id as string,
          id: reviewId as string,
        });
      } else {
        await addReview.mutateAsync({ data, lessonId: id as string });
      }
      showNotification({
        title: t("successful"),
        message: `${t("Successfully")} ${
          edit ? t("edited") : t("added")
        } ${t("review_on_assignment")}`,
      });
      closeModal();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  const form = useForm<IAddAssignmentReview>({
    initialValues: {
      isPassed: editData?.assignmentReview?.isPassed ?? false,
      marks: Number(editData?.assignmentReview?.mark) || 0,
      review: editData?.assignmentReview?.review ?? "",
      userId: studentId as string,
    },
  });
  return (
    <Box>
      <form onSubmit={form.onSubmit(submitHandler)}>
        <Textarea label="Comment" {...form.getInputProps("review")} />
        <NumberInput {...form.getInputProps("marks")} label="Marks" />
        <Switch
          checked={isPass}
          {...form.getInputProps("isPassed")}
          onChange={(event) => {
            setIsPass(!isPass);
            form.setFieldValue("isPassed", !isPass);
          }}
          label="Pass"
          mt={10}
        />
        <Group mt={20}>
          <Button type="submit">Submit</Button>
          <Button onClick={closeModal} variant="outline">
            Cancel
          </Button>
        </Group>
      </form>
    </Box>
  );
};

export default AssignmentReviewForm;
