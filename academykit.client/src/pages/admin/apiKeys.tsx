import Copy from "@components/Ui/Copy";
import DeleteModal from "@components/Ui/DeleteModal";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  ActionIcon,
  Box,
  Button,
  Group,
  Loader,
  Modal,
  Paper,
  ScrollArea,
  Table,
  TextInput,
  Title,
  Tooltip,
} from "@mantine/core";
import { IconTrash } from "@tabler/icons-react";
import { DATE_FORMAT } from "@utils/constants";
import {
  useApiKeys,
  useCreateApiKey,
  useDeleteApiKey,
} from "@utils/services/apiKeyService";
import moment from "moment";
import { useState } from "react";
import { useTranslation } from "react-i18next";

const ApiKeys = ({ searchParams, pagination }: IWithSearchPagination) => {
  const { t } = useTranslation();

  const [openedCreated, setOpenedCreated] = useState<string>("");
  const [openedDelete, setOpenedDelete] = useState<string>("");

  const { data, isPending, isError } = useApiKeys(searchParams);
  const { mutateAsync: createApiKey, isPending: isCreatingApiKey } =
    useCreateApiKey();
  const { mutateAsync: deleteApiKey, isPending: isDeletingApiKey } =
    useDeleteApiKey();

  return (
    <>
      <Modal
        styles={{ header: { alignItems: "start" } }}
        title={t("add_api_key_successfully")}
        opened={!!openedCreated}
        onClose={() => setOpenedCreated("")}
        zIndex={201}
      >
        <TextInput
          type="text"
          value={openedCreated}
          readOnly
          rightSection={<Copy value={openedCreated} />}
        />
      </Modal>
      <DeleteModal
        title={`${t("sure_to_delete_api_key")}`}
        open={!!openedDelete}
        loading={isDeletingApiKey}
        onClose={() => setOpenedDelete("")}
        onConfirm={async () => {
          if (!isDeletingApiKey) {
            await deleteApiKey(openedDelete);
            setOpenedDelete("");
          }
        }}
      />
      <Group
        style={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title mt={20} size={30} mb={10}>
          {t("api_keys")}
        </Title>
        <div>
          <Button
            type="button"
            loading={isCreatingApiKey}
            onClick={async () => {
              const apiKey = await createApiKey();
              setOpenedCreated(apiKey.data.key);
            }}
          >
            {t("add_api_key")}
          </Button>
        </div>
      </Group>
      <ScrollArea>
        <Paper>
          <Table striped withTableBorder withColumnBorders highlightOnHover>
            <Table.Thead>
              <Table.Tr>
                <Table.Th>{t("api_key")}</Table.Th>
                <Table.Th>{t("created_date")}</Table.Th>
                <Table.Th>{t("action")}</Table.Th>
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {!!data &&
                data.totalCount > 0 &&
                data.items.map((x) => (
                  <Table.Tr key={x.id}>
                    <Table.Td>{x.key}</Table.Td>
                    <Table.Td>
                      {moment(x.createdOn).format(DATE_FORMAT)}
                    </Table.Td>
                    <Table.Td>
                      <Group>
                        <Copy value={x.key} />
                        <Tooltip label={t("delete")}>
                          <ActionIcon
                            color="red"
                            variant="subtle"
                            onClick={() => setOpenedDelete(x.id)}
                          >
                            <IconTrash />
                          </ActionIcon>
                        </Tooltip>
                      </Group>
                    </Table.Td>
                  </Table.Tr>
                ))}
            </Table.Tbody>
          </Table>
        </Paper>
      </ScrollArea>
      {data && data.totalCount === 0 && (
        <Box mt={5}>{t("no_api_keys_found")}</Box>
      )}
      {isPending && <Loader />}
      {isError && <Box>{t("something_went_wrong")}</Box>}
      {data && pagination(data.totalPage, data.items.length)}
    </>
  );
};

export default withSearchPagination(ApiKeys, false);
