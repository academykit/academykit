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
import {
  IGroupAttachmentItems,
  useGroupAttachment,
  useRemoveGroupAttachment,
} from "@utils/services/groupService";
import { useState } from "react";
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
    const removeAttachment = useRemoveGroupAttachment(
      id as string,
      item.id,
      searchParams
    );
    const handleDelete = async () => {
      try {
        await removeAttachment.mutateAsync({
          id: id as string,
          fileId: item.id,
        });
        showNotification({
          message: "Attachment deleted successfully.",
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
      window.open(item.url);
    };

    return (
      <tr key={item.id}>
        <DeleteModal
          title={`Are you sure you want to delete attachment?`}
          open={item.id === deleteAttachment}
          onClose={setDeleteAttachment}
          onConfirm={handleDelete}
        />

        <td>{item.createdOn}</td>
        <td>{item.name}</td>
        <td>{item.mimeType}</td>
        <td style={{ maxWidth: "500px" }}>
          <TextInput
            size="md"
            value={item?.url}
            styles={{
              input: {
                cursor: "pointer",
                opacity: "0.4",
              },
            }}
          />
        </td>
        <td>
          <Flex>
            <CopyButton value={item?.url} timeout={2000}>
              {({ copied, copy }) => (
                <Tooltip
                  label={copied ? "Copied" : "Copy"}
                  withArrow
                  position="right"
                >
                  <ActionIcon color={copied ? "teal" : "gray"} onClick={copy}>
                    {copied ? <IconCheck size={16} /> : <IconCopy size={16} />}
                  </ActionIcon>
                </Tooltip>
              )}
            </CopyButton>
            <Tooltip label={"Download attachment"}>
              <ActionIcon onClick={handleDownload}>
                <IconDownload />
              </ActionIcon>
            </Tooltip>
          </Flex>
        </td>
        <td>
          <Flex>
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
                    <th>Url</th>
                    <th></th>
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
