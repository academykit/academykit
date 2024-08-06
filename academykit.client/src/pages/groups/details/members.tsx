import DeleteModal from "@components/Ui/DeleteModal";
import { IAuthContext } from "@context/AuthProvider";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  Avatar,
  Box,
  Button,
  Container,
  Drawer,
  Group,
  Loader,
  Paper,
  Table,
  Text,
  Title,
} from "@mantine/core";
import { useDisclosure, useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconPlus, IconTrash } from "@tabler/icons-react";
import { UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  IGroupMember,
  useGroupMember,
  useRemoveGroupMember,
} from "@utils/services/groupService";
import { useTranslation } from "react-i18next";
import { Link, useParams } from "react-router-dom";
import AddMember from "../Components/AddMember";

const GroupMember = ({
  pagination,
  searchComponent,
  searchParams,
}: IWithSearchPagination) => {
  const [opened, { open, close }] = useDisclosure(false);
  const { t } = useTranslation();
  const { id } = useParams();
  const groupMember = useGroupMember(id as string, searchParams);

  if (groupMember.error) {
    throw groupMember.error;
  }

  const auth = useAuth();
  return (
    <Container fluid>
      <Group
        style={{ justifyContent: "space-between", alignItems: "center" }}
        mt={20}
      >
        <Box>
          <Title>{t("group_members")}</Title>
          <Text>{t("group_members_description")}</Text>
        </Box>

        {auth?.auth && Number(auth?.auth?.role) <= UserRole.Trainer && (
          <Button leftSection={<IconPlus size={15} />} onClick={open}>
            {t("add_group_member")}
          </Button>
        )}
      </Group>
      <Drawer
        opened={opened}
        onClose={close}
        title={t("add_group_member")}
        overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
      >
        <AddMember onCancel={close} searchParams={searchParams} />
      </Drawer>
      <Box mt={10}>{searchComponent(t("search_group_members") as string)}</Box>
      {groupMember.isError && <>{t("unable_to_fetch")}</>}

      {groupMember.isLoading && <Loader />}
      {groupMember.data && groupMember.data?.totalCount < 1 && (
        <Box mt={10}>{t("no_members_found")}</Box>
      )}
      {groupMember.isSuccess && groupMember.data.totalCount !== 0 && (
        <Paper mt={10}>
          <Table striped withTableBorder withColumnBorders highlightOnHover>
            <Table.Thead>
              <Table.Tr>
                <Table.Th>{t("user_name")}</Table.Th>
                <Table.Th>{t("email_or_phone")}</Table.Th>
                {auth?.auth && Number(auth?.auth?.role) <= UserRole.Trainer && (
                  <Table.Th>{t("actions")}</Table.Th>
                )}
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {groupMember.data?.items.map((d) => (
                <GroupMemberRow
                  data={d}
                  key={d.user.id}
                  search={searchParams}
                  auth={auth}
                />
              ))}
            </Table.Tbody>
          </Table>
        </Paper>
      )}
      {pagination(
        groupMember.data?.totalPage ?? 0,
        groupMember.data?.items.length ?? 1
      )}
    </Container>
  );
};

const GroupMemberRow = ({
  data,
  search,
  auth,
}: {
  auth: IAuthContext | null;
  data: IGroupMember;
  search: string;
}) => {
  const [deleteDialog, setDeleteDialog] = useToggle();
  const { id } = useParams();
  const removeGroupMember = useRemoveGroupMember(
    id as string,
    search,
    data.user.id
  );
  const { t } = useTranslation();
  const deleteMember = async () => {
    removeGroupMember.mutate({ id: id as string, memberId: data.id });
  };
  if (removeGroupMember.isError) {
    const error = errorType(removeGroupMember.error);
    showNotification({
      message: error,
      color: "red",
    });
    removeGroupMember.reset();
    setDeleteDialog();
  }
  if (removeGroupMember.isSuccess) {
    showNotification({
      message: `${t("remove_success")} ${data.user.fullName}`,
    });
    removeGroupMember.reset();
    setDeleteDialog();
  }
  return (
    <Table.Tr>
      <DeleteModal
        title={`${t("sure_want_to_remove")} "${
          data.user.fullName?.split(" ")[0]
        }" ?`}
        open={deleteDialog}
        onClose={setDeleteDialog}
        onConfirm={deleteMember}
      />

      <Table.Td>
        <Group gap="sm">
          <Link to={`/userProfile/${data.user.id}/certificate`}>
            <Avatar size={26} src={data?.user?.imageUrl || ""} radius={26} />
          </Link>
          <Text size="sm" fw={500}>
            {data?.user?.fullName}
          </Text>
        </Group>
      </Table.Td>
      <Table.Td>
        {data?.user?.email} {data?.user?.mobileNumber && `| `}
        {data.user?.mobileNumber}
      </Table.Td>
      <Table.Td>
        {auth?.auth?.id !== data?.user?.id &&
          auth?.auth &&
          Number(auth?.auth?.role) <= UserRole.Trainer && (
            <Button
              onClick={() => setDeleteDialog()}
              variant="subtle"
              c={"red"}
            >
              <IconTrash />
            </Button>
          )}
      </Table.Td>
    </Table.Tr>
  );
};

export default withSearchPagination(GroupMember);
