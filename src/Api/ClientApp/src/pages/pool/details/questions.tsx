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
  Flex,
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
import { TFunction } from "i18next";
import { useTranslation } from "react-i18next";
import { Link, useParams, useNavigate } from "react-router-dom";

const MCQQuestions = ({
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const { data, isLoading, isSuccess, isError } = useQuestion(
    id as string,
    searchParams
  );
  const navigate = useNavigate();
  const { t } = useTranslation();

  return (
    <Container fluid>
      <Flex justify={"end"}>
        {searchComponent(t("search_for_questions") as string)}
        <Button component={Link} ml={5} to="create">
          {t("add_question")}
        </Button>
      </Flex>

      {data && data.items && data.totalCount > 0 && (
        <Paper mt={10}>
          <Table striped>
            <thead>
              <tr>
                <th>{t("name")}</th>
                <th>{t("tags")}</th>
                <th>{t("type")}</th>
                <th>
                  <Center>{t("actions")}</Center>
                </th>
              </tr>
            </thead>
            <tbody>
              {data?.items?.map((x) => (
                <QuestionRow
                  data={x}
                  search={searchParams}
                  poolId={id as string}
                  key={x.id}
                  navigate={navigate}
                  t={t}
                />
              ))}
            </tbody>
          </Table>
        </Paper>
      )}
      {isLoading && <Loader />}

      {data && pagination(data.totalPage, data.items.length)}
      {isError && <Box>{t("something_wrong")}</Box>}
      {data && data?.totalCount < 1 && (
        <Box mt={10}>{t("no_question_found")}</Box>
      )}
    </Container>
  );
};

const QuestionRow = ({
  data,
  search,
  poolId,
  navigate,
  t,
}: {
  data: IQuestion;
  search: string;
  poolId: string;
  navigate: Function;
  t: TFunction;
}) => {
  const [showDelete, setShowDelete] = useToggle();
  const deleteService = useDeleteQuestion(poolId, search);
  const confirmDelete = async () => {
    try {
      const { data: res } = (await deleteService.mutateAsync({
        poolId: poolId,
        questionId: data.id,
      })) as any;
      showNotification({
        message: `${res?.message}`,
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
        title={t(`question_delete_confirmation`)}
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

      <td>{t(`${QuestionType[data.type]}`)}</td>
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
