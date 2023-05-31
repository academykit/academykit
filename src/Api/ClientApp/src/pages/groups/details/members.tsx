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
  Group,
  Loader,
  Paper,
  Table,
  Text,
  Title,
  Transition,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconTrash } from "@tabler/icons";
import errorType from "@utils/services/axiosError";
import {
  IGroupMember,
  useGroupMember,
  useRemoveGroupMember,
} from "@utils/services/groupService";
import { Link, useParams } from "react-router-dom";
import AddMember from "../Components/AddMember";
import { UserRole } from "@utils/enums";
import { useTranslation } from "react-i18next";

const a = {
  in: { opacity: 1 },
  out: { opacity: 0 },
  common: { transformOrigin: "top" },
  transitionProperty: "transform, opacity",
};
const GroupMember = ({
  pagination,
  searchComponent,
  searchParams,
}: IWithSearchPagination) => {
  const [showAddMember, setShowAddMember] = useToggle();

  const { id } = useParams();
  const groupMember = useGroupMember(id as string, searchParams);

  if (groupMember.error) {
    throw groupMember.error;
  }

  const auth = useAuth();
  return (
    <Container fluid>
      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mt={20}
      >
        <Box>
          <Title> Group Members</Title>
          <Text>Here you can see all members of the group.</Text>
        </Box>
        {auth?.auth && auth?.auth?.role <= UserRole.Trainer && (
          <Transition mounted={!showAddMember} transition={a} duration={400}>
            {(styles) => (
              <>
                <Button onClick={() => setShowAddMember()}>
                  Add Group Member
                </Button>
              </>
            )}
          </Transition>
        )}
      </Group>
      <Box my={10}>
        <Transition mounted={showAddMember} transition={a} duration={400}>
          {(styles) => (
            <>
              <Box pb={20}>
                <AddMember
                  onCancel={setShowAddMember}
                  searchParams={searchParams}
                />
              </Box>
            </>
          )}
        </Transition>
      </Box>
      <Box mt={10}>{searchComponent("Search for group members")}</Box>
      {groupMember.isError && <>Unable to fetch data please try again</>}

      {groupMember.isLoading && <Loader />}
      {groupMember.data && groupMember.data?.totalCount < 1 && (
        <Box mt={10}>No Members Found!</Box>
      )}
      {groupMember.isSuccess && groupMember.data.totalCount !== 0 && (
        <Paper mt={10}>
          <Table striped>
            <thead>
              <tr>
                <th>User Name</th>
                <th>Email | Phone Number</th>
                {auth?.auth && auth?.auth?.role <= UserRole.Trainer && (
                  <th>Actions</th>
                )}
              </tr>
            </thead>
            <tbody>
              {groupMember.data?.items.map((d) => (
                <GroupMemberRow
                  data={d}
                  key={d.user.id}
                  search={searchParams}
                  auth={auth}
                />
              ))}
            </tbody>
          </Table>
        </Paper>
      )}
      {pagination(groupMember.data?.totalPage ?? 0)}
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
    <tr>
      <DeleteModal
        title={`Are you sure you want to remove "${data.user.fullName}" ?`}
        open={deleteDialog}
        onClose={setDeleteDialog}
        onConfirm={deleteMember}
      />

      <td>
        <Group spacing="sm">
          <Link to={`/userProfile/${data.user.id}`}>
            <Avatar size={26} src={data?.user?.imageUrl || ""} radius={26} />
          </Link>
          <Text size="sm" weight={500}>
            {data?.user?.fullName}
          </Text>
        </Group>
      </td>
      <td>
        {data?.user?.email} {data?.user?.mobileNumber && `| `}
        {data.user?.mobileNumber}
      </td>
      <td>
        {auth?.auth?.id !== data?.user?.id &&
          auth?.auth &&
          auth?.auth?.role <= UserRole.Trainer && (
            <Button
              onClick={() => setDeleteDialog()}
              variant="subtle"
              color={"red"}
            >
              <IconTrash />
            </Button>
          )}
      </td>
    </tr>
  );
};

export default withSearchPagination(GroupMember);
