import ContextField from "@components/Ui/ContextField";
import Copy from "@components/Ui/Copy";
import DeleteModal from "@components/Ui/DeleteModal";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  ActionIcon,
  Box,
  Button,
  Group,
  Loader,
  Modal,
  Paper,
  ScrollArea,
  Stack,
  Table,
  TextInput,
  Title,
  Tooltip,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconTrash } from "@tabler/icons-react";
import { DATE_FORMAT } from "@utils/constants";
import {
  useApiKeys,
  useCreateApiKey,
  useDeleteApiKey,
} from "@utils/services/apiKeyService";
import errorType from "@utils/services/axiosError";
import moment from "moment";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import * as Yup from "yup";

interface IFormValues {
  name: string;
}

const [FormProvider, useFormContext, useForm] =
  createFormContext<IFormValues>();

const ApiKeys = ({ searchParams, pagination }: IWithSearchPagination) => {
  const { t } = useTranslation();

  const [openedCreating, setOpenedCreating] = useState(false);
  const [openedCreated, setOpenedCreated] = useState<string>("");
  const [openedDelete, setOpenedDelete] = useState<string>("");

  const { data, isPending, isError } = useApiKeys(searchParams);
  const { mutateAsync: createApiKey, isPending: isCreatingApiKey } =
    useCreateApiKey();
  const { mutateAsync: deleteApiKey, isPending: isDeletingApiKey } =
    useDeleteApiKey();

  const closeCreateModal = () => {
    setOpenedCreating(false);
    setOpenedCreated("");
  };

  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(
      Yup.object().shape({
        name: Yup.string().required(t("is_mandatory") as string),
      })
    ),
  });
  useFormErrorHooks(form);
  const handleSubmit = async (values: IFormValues) => {
    try {
      const apiKey = await createApiKey(values.name);
      setOpenedCreated(apiKey.data.key);
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: t("error"),
        message: err,
        color: "red",
      });
    }
  };

  return (
    <>
      <Modal
        styles={{ header: { alignItems: "start" } }}
        title={openedCreated ? t("add_api_key_successfully") : t("add_api_key")}
        opened={!!openedCreating || !!openedCreated}
        onClose={closeCreateModal}
        zIndex={201}
      >
        <FormProvider form={form}>
          <form onSubmit={form.onSubmit(handleSubmit)}>
            <Stack>
              <ContextField<IFormValues> useFormContext={useFormContext}>
                {(form) => (
                  <TextInput
                    withAsterisk
                    type="text"
                    label={t("name")}
                    placeholder={t("enter_name_for_the_api_key") as string}
                    readOnly={!!openedCreated}
                    {...form.getInputProps("name")}
                  />
                )}
              </ContextField>
              {!!openedCreated && (
                <TextInput
                  type="text"
                  label={t("api_key")}
                  value={openedCreated}
                  readOnly
                  rightSection={<Copy value={openedCreated} />}
                />
              )}
              <Group>
                {openedCreated ? (
                  <>
                    <Button key="done" type="button" onClick={closeCreateModal}>
                      {t("done")}
                    </Button>
                  </>
                ) : (
                  <>
                    <Button
                      key="confirm"
                      type="submit"
                      loading={isCreatingApiKey}
                    >
                      {t("confirm")}
                    </Button>
                    <Button variant="outline" onClick={closeCreateModal}>
                      {t("cancel")}
                    </Button>
                  </>
                )}
              </Group>
            </Stack>
          </form>
        </FormProvider>
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
            onClick={() => {
              setOpenedCreating(true);
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
                <Table.Th>{t("name")}</Table.Th>
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
                    <Table.Td>{x.name}</Table.Td>
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
