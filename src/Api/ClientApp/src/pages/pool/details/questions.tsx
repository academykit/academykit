import DeleteModal from "@components/Ui/DeleteModal";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  Badge,
  Box,
  Button,
  Center,
  Container,
  Group,
  Loader,
  Modal,
  Paper,
  Table,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconTrash } from "@tabler/icons";
import { QuestionType } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  IQuestion,
  useDeleteQuestion,
  useQuestion,
} from "@utils/services/questionService";
import { Link, useParams, useNavigate } from "react-router-dom";

const MCQQuestions = ({
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const questions = useQuestion(id as string, searchParams);
  const navigate = useNavigate();

  return (
    <Container fluid>
      {questions.isLoading && <Loader />}
      {questions.isSuccess && (
        <>
          <div style={{ display: "flex" }}>
            {searchComponent("Search for questions")}
            <Button component={Link} ml={5} to="create">
              Add Question
            </Button>
          </div>

          {questions.data && questions.data.totalCount > 0 ? (
            <Paper mt={10}>
              <Table striped>
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Tags</th>
                    <th>Type</th>
                    <th>
                      <Center>Actions</Center>
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {questions.data?.items?.map((x) => (
                    <QuestionRow
                      data={x}
                      search={searchParams}
                      poolId={id as string}
                      key={x.id}
                      navigate={navigate}
                    />
                  ))}
                </tbody>
              </Table>
            </Paper>
          ) : (
            <Box mt={10}>No Questions found!</Box>
          )}

          {pagination(questions.data?.totalPage)}
        </>
      )}
      {questions.isError && (
        <Box>Something went wrong! Please try again later</Box>
      )}
    </Container>
  );
};

const QuestionRow = ({
  data,
  search,
  poolId,
  navigate,
}: {
  data: IQuestion;
  search: string;
  poolId: string;
  navigate: Function;
}) => {
  const [showDelete, setShowDelete] = useToggle();
  const deleteService = useDeleteQuestion(poolId, search);
  const confirmDelete = async () => {
    try {
      await deleteService.mutateAsync({ poolId: poolId, questionId: data.id });
      showNotification({
        message: `Successfully Deleted Question ${data.name}`,
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
    setShowDelete();
  };

  return (
    <tr>
      <DeleteModal
        title={`Are you sure you want to delete question?`}
        open={showDelete}
        onClose={setShowDelete}
        onConfirm={confirmDelete}
      />

      <td>{data.name}</td>
      <td style={{ width: "400px" }}>
        {data.tags.map((x) => (
          <Badge color={"green"} mx={2}>
            {" "}
            {x.tagName}
          </Badge>
        ))}
      </td>

      <td>{QuestionType[data.type]}</td>
      <td>
        <Center>
          <Button
            variant="subtle"
            onClick={() => {
              navigate("edit/" + data.id);
            }}
          >
            <IconEdit />
          </Button>
          <Button
            onClick={() => setShowDelete()}
            variant="subtle"
            color={"red"}
          >
            <IconTrash />
          </Button>
        </Center>
      </td>
    </tr>
  );
};

export default withSearchPagination(MCQQuestions);
