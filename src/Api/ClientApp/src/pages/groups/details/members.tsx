import DeleteModal from '@components/Ui/DeleteModal';
import { IAuthContext } from '@context/AuthProvider';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
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
} from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconTrash } from '@tabler/icons';
import errorType from '@utils/services/axiosError';
import {
  IGroupMember,
  useGroupMember,
  useRemoveGroupMember,
} from '@utils/services/groupService';
import { Link, useParams } from 'react-router-dom';
import AddMember from '../Components/AddMember';
import { UserRole } from '@utils/enums';
import { useTranslation } from 'react-i18next';

const a = {
  in: { opacity: 1 },
  out: { opacity: 0 },
  common: { transformOrigin: 'top' },
  transitionProperty: 'transform, opacity',
};
const GroupMember = ({
  pagination,
  searchComponent,
  searchParams,
}: IWithSearchPagination) => {
  const [showAddMember, setShowAddMember] = useToggle();
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
        sx={{ justifyContent: 'space-between', alignItems: 'center' }}
        mt={20}
      >
        <Box>
          <Title>{t('group_members')}</Title>
          <Text>{t('group_members_description')}</Text>
        </Box>
        {auth?.auth && auth?.auth?.role <= UserRole.Trainer && (
          <Transition mounted={!showAddMember} transition={a} duration={400}>
            {() => (
              <>
                <Button onClick={() => setShowAddMember()}>
                  {t('add_group_member')}
                </Button>
              </>
            )}
          </Transition>
        )}
      </Group>
      <Box my={10}>
        <Transition mounted={showAddMember} transition={a} duration={400}>
          {() => (
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
      <Box mt={10}>{searchComponent(t('search_group_members') as string)}</Box>
      {groupMember.isError && <>{t('unable_to_fetch')}</>}

      {groupMember.isLoading && <Loader />}
      {groupMember.data && groupMember.data?.totalCount < 1 && (
        <Box mt={10}>{t('no_members_found')}</Box>
      )}
      {groupMember.isSuccess && groupMember.data.totalCount !== 0 && (
        <Paper mt={10}>
          <Table striped>
            <thead>
              <tr>
                <th>{t('user_name')}</th>
                <th>{t('email_or_phone')}</th>
                {auth?.auth && auth?.auth?.role <= UserRole.Trainer && (
                  <th>{t('actions')}</th>
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
      color: 'red',
    });
    removeGroupMember.reset();
    setDeleteDialog();
  }
  if (removeGroupMember.isSuccess) {
    showNotification({
      message: `${t('remove_success')} ${data.user.fullName}`,
    });
    removeGroupMember.reset();
    setDeleteDialog();
  }
  return (
    <tr>
      <DeleteModal
        title={`${t('sure_want_to_remove')} "${data.user.fullName?.split(
          ' '
        )[0]}" ?`}
        open={deleteDialog}
        onClose={setDeleteDialog}
        onConfirm={deleteMember}
      />

      <td>
        <Group spacing="sm">
          <Link to={`/userProfile/${data.user.id}/certificate`}>
            <Avatar size={26} src={data?.user?.imageUrl || ''} radius={26} />
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
              color={'red'}
            >
              <IconTrash />
            </Button>
          )}
      </td>
    </tr>
  );
};

export default withSearchPagination(GroupMember);
