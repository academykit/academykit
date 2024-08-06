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
  Loader,
  Paper,
  ScrollArea,
  Table,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconTrash } from "@tabler/icons-react";
import { QuestionType } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  IQuestion,
  useDeleteQuestion,
  useQuestion,
} from "@utils/services/questionService";
import { TFunction } from "i18next";
import { useTranslation } from "react-i18next";
import { Link, useNavigate, useParams } from "react-router-dom";

const MCQQuestions = ({
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const { data, isLoading, isError } = useQuestion(id as string, searchParams);
  const navigate = useNavigate();
  const { t } = useTranslation();

  return (
    <Container fluid>
      <Flex justify={"end"}>
        {searchComponent(t("search_for_questions") as string)}
        <Button component={Link} ml={5} to="create" w={"12%"}>
          {t("add_question")}
        </Button>
      </Flex>
      <ScrollArea>
        {data && data.items && data.totalCount > 0 && (
          <Paper mt={10}>
            <Table striped withTableBorder withColumnBorders highlightOnHover>
              <Table.Thead>
                <Table.Tr>
                  <Table.Th>{t("name")}</Table.Th>
                  <Table.Th>{t("tags")}</Table.Th>
                  <Table.Th>{t("type")}</Table.Th>
                  <Table.Th>
                    <Center>{t("actions")}</Center>
                  </Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
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
              </Table.Tbody>
            </Table>
          </Paper>
        )}
        {isLoading && <Loader />}

        {data && pagination(data.totalPage, data.items.length)}
        {isError && <Box>{t("something_wrong")}</Box>}
        {data && data?.totalCount < 1 && (
          <Box mt={10}>{t("no_question_found")}</Box>
        )}
      </ScrollArea>
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
  navigate: (path: string) => void;
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
    <Table.Tr>
      <DeleteModal
        title={t(`question_delete_confirmation`)}
        open={showDelete}
        onClose={setShowDelete}
        onConfirm={confirmDelete}
      />

      <Table.Td>{data.name}</Table.Td>
      <Table.Td>
        {data.tags.map((x) => (
          <Badge key={x.id} color={"green"} mx={2}>
            {" "}
            {x.tagName}
          </Badge>
        ))}
      </Table.Td>

      <Table.Td>{t(`${QuestionType[data.type]}`)}</Table.Td>
      <Table.Td>
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
      </Table.Td>
    </Table.Tr>
  );
};

export default withSearchPagination(MCQQuestions);
