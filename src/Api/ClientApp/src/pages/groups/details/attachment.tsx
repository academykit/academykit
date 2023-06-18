import AddAssignment from "@components/Group/AddAttachment";
import DeleteModal from "@components/Ui/DeleteModal";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  ActionIcon,
  Button,
  Container,
  Flex,
  Modal,
  Paper,
  ScrollArea,
  Table,
  Text,
  Title,
  Tooltip,
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { IconDownload, IconTrash } from "@tabler/icons";
import { UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import { getFileUrl } from "@utils/services/fileService";
import {
  IGroupAttachmentItems,
  useGroupAttachment,
  useRemoveGroupAttachment,
} from "@utils/services/groupService";
import moment from "moment";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

const GroupAttachment = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const getGroupAttachment = useGroupAttachment(id as string, searchParams);
  const [deleteAttachment, setDeleteAttachment] = useState("");
  const [opened, setOpened] = useState(false);
  const authUser = useAuth();

  if (getGroupAttachment.error) {
    throw getGroupAttachment.error;
  }
  const { t } = useTranslation();

  const Rows = ({ item }: { item: IGroupAttachmentItems }) => {
    const [enabled, setEnabled] = useState(false);
    const auth = useAuth();
    const removeAttachment = useRemoveGroupAttachment(
      id as string,
      item.id,
      searchParams
    );
    const fileUrl = getFileUrl(item.url, enabled);

    const handleDelete = async () => {
      try {
        await removeAttachment.mutateAsync({
          id: id as string,
          fileId: item.id,
        });
        showNotification({
          message: t("delete_attachment_success"),
        });
        setOpened(false);
      } catch (err) {
        const error = errorType(err);
        showNotification({
          message: error,
          color: "red",
        });
      }
      setDeleteAttachment("");
    };
    const handleDownload = () => {
      setEnabled(true);
      if (fileUrl.isSuccess) {
        window.open(fileUrl.data);
      }
    };

    return (
      <tr key={item.id}>
        <DeleteModal
          title={t("sure_to_delete_attachment")}
          open={item.id === deleteAttachment}
          onClose={setDeleteAttachment}
          onConfirm={handleDelete}
        />

        <td>{moment(item.createdOn + "Z").format("YYYY-MM-DD HH:mm:ss")}</td>
        <td>{item.name}</td>
        <td>{item.mimeType}</td>

        <td>
          <Flex>
            <Tooltip label={t("download_attachment")}>
              <ActionIcon onClick={() => handleDownload()}>
                <IconDownload />
              </ActionIcon>
            </Tooltip>

            {auth?.auth && auth?.auth?.role !== UserRole.Trainee && (
              <Tooltip label={t("delete_attachment")}>
                <ActionIcon onClick={() => setDeleteAttachment(item.id)}>
                  <IconTrash color="red" />
                </ActionIcon>
              </Tooltip>
            )}
          </Flex>
        </td>
      </tr>
    );
  };
  return (
    <>
      <Modal
        size={500}
        opened={opened}
        onClose={() => setOpened(false)}
        styles={{ title: { fontWeight: "bold" } }}
        title={t("upload_attachments")}
      >
        {opened && (
          <AddAssignment close={() => setOpened(false)} search={searchParams} />
        )}
      </Modal>
      <Container fluid>
        <Flex
          my={10}
          wrap={"wrap"}
          sx={{ justifyContent: "space-between", alignItems: "center" }}
        >
          <Title sx={{ flexGrow: 2 }}>{t("attachments")}</Title>
          <Flex
            sx={{
              justifyContent: "end",
              alignItems: "center",
            }}
          >
            {authUser?.auth && authUser?.auth?.role <= 3 && (
              <Button onClick={() => setOpened(true)} my={10} ml={5}>
                {t("add_new_attachment")}
              </Button>
            )}
          </Flex>
        </Flex>
        {searchComponent(t("search_attachments") as string)}

        {getGroupAttachment.data &&
        getGroupAttachment.data?.items.length >= 1 ? (
          <ScrollArea>
            <Paper mt={10}>
              <Table
                sx={{ minWidth: 800, overflow: "auto" }}
                verticalSpacing="xs"
                striped
                highlightOnHover
                withBorder
              >
                <thead>
                  <tr>
                    <th>{t("uploaded_date")}</th>
                    <th>{t("name")}</th>
                    <th>{t("type")}</th>
                    <th>{t("action")}</th>
                  </tr>
                </thead>
                <tbody>
                  {getGroupAttachment.data?.items?.map((item: any) => (
                    <Rows item={item} key={item?.id} />
                  ))}
                </tbody>
              </Table>
              {getGroupAttachment.data &&
                getGroupAttachment.data.totalPage > 1 &&
                pagination(
                  getGroupAttachment.data?.totalPage,
                  getGroupAttachment.data.items.length
                )}
            </Paper>
          </ScrollArea>
        ) : (
          <Text mt={10}>{t("no_attachments_found")}</Text>
        )}
      </Container>
    </>
  );
};

export default withSearchPagination(GroupAttachment);
