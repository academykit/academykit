import Breadcrumb from "@components/Ui/BreadCrumb";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import { Box, Button, Container } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { useAssignmentQuestion } from "@utils/services/assignmentService";
import { useParams } from "react-router-dom";
import AssignmentItem from "./Component/AssignmentItem";
import EditAssignment from "./Component/EditAssignment";

interface Props {
  lessonId: string;
}

const CreateAssignment = ({
  searchParams,
  pagination,
  searchComponent,
  lessonId,
}: Props & IWithSearchPagination) => {
  const [addQuestion, setAddQuestion] = useToggle();

  const questionList = useAssignmentQuestion(lessonId, searchParams);
  return (
    <Container>
      {questionList.isSuccess && (
        <>
          {questionList.data.length > 0 ? (
            <Box>
              {questionList.data.map((x) => (
                <AssignmentItem
                  key={x.id}
                  data={x}
                  search={searchParams}
                  lessonId={lessonId}
                />
              ))}
            </Box>
          ) : (
            <Box mt={10}>No assignment questions found!</Box>
          )}
          {addQuestion && (
            <EditAssignment
              onCancel={() => setAddQuestion()}
              lessonId={lessonId}
              search={searchParams}
            />
          )}

          {!addQuestion && (
            <Button mt={10} onClick={() => setAddQuestion()}>
              Add Question
            </Button>
          )}
        </>
      )}
    </Container>
  );
};

export default withSearchPagination(CreateAssignment);
