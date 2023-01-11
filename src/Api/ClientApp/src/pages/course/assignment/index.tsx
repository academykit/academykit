import { Container, Divider } from "@mantine/core";

import { useAssignmentQuestion } from "@utils/services/assignmentService";
import { useParams } from "react-router-dom";
import AssignmentForm from "./Component/AssignmentForm";

const AssignmentPage = () => {
  const { id } = useParams();

  const assignment = useAssignmentQuestion(id as string, "");

  return (
    <Container my={50}>
      <Divider />
      {assignment.isSuccess && (
        <AssignmentForm item={assignment.data} lessonId={id as string} />
      )}
    </Container>
  );
};

export default AssignmentPage;
