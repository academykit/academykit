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
  CopyButton,
  Flex,
  Group,
  Input,
  Modal,
  Paper,
  ScrollArea,
  Table,
  Text,
  Textarea,
  TextInput,
  Title,
  Tooltip,
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { IconCheck, IconCopy, IconDownload, IconTrash } from "@tabler/icons";
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

  const Rows = ({ item }: { item: IGroupAttachmentItems }) => {
    const [enabled, setEnabled] = useState(false);
    const removeAttachment = useRemoveGroupAttachment(
      id as string,
      item.id,
      searchParams
    );
    const fileUrl = getFileUrl(item.url, enabled);

    const { t } = useTranslation();
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
          title={`Are you sure you want to delete attachment?`}
          open={item.id === deleteAttachment}
          onClose={setDeleteAttachment}
          onConfirm={handleDelete}
        />

        <td>{moment(item.createdOn + "Z").format("YYYY-MM-DD HH:mm:ss")}</td>
        <td>{item.name}</td>
        <td>{item.mimeType}</td>

        <td>
          <Flex>
            <Tooltip label={"Download attachment"}>
              <ActionIcon onClick={() => handleDownload()}>
                <IconDownload />
              </ActionIcon>
            </Tooltip>

            <Tooltip label={"Delete attachment"}>
              <ActionIcon onClick={() => setDeleteAttachment(item.id)}>
                <IconTrash color="red" />
              </ActionIcon>
            </Tooltip>
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
        title="Upload Attachments"
      >
        <AddAssignment close={() => setOpened(false)} search={searchParams} />
      </Modal>
      <Container fluid>
        <Flex
          my={10}
          wrap={"wrap"}
          sx={{ justifyContent: "space-between", alignItems: "center" }}
        >
          <Title sx={{ flexGrow: 2 }}>Attachments</Title>
          <Flex
            sx={{
              justifyContent: "end",
              alignItems: "center",
            }}
          >
            {authUser?.auth && authUser?.auth?.role <= 3 && (
              <Button onClick={() => setOpened(true)} my={10} ml={5}>
                Add New Attachment
              </Button>
            )}
          </Flex>
        </Flex>
        {searchComponent("Search Attachments")}

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
                    <th>Uploaded Date</th>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Action</th>
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
                pagination(getGroupAttachment.data?.totalPage)}
            </Paper>
          </ScrollArea>
        ) : (
          <Text mt={10}>No Attachments Found!</Text>
        )}
      </Container>
    </>
  );
};

export default withSearchPagination(GroupAttachment);
