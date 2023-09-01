import {
  ActionIcon,
  Avatar,
  Badge,
  Loader,
  Modal,
  Paper,
  Table,
  Text,
  Tooltip,
  createStyles,
  useMantineColorScheme,
} from '@mantine/core';
import { useEditUser, useResendEmail } from '@utils/services/adminService';
import { UserRole, UserStatus } from '@utils/enums';

import { Suspense, useState } from 'react';
import { Link } from 'react-router-dom';
import { IconEdit, IconSend } from '@tabler/icons';
import { IUserProfile } from '@utils/services/types';
import useAuth from '@hooks/useAuth';
import { IAuthContext } from '@context/AuthProvider';
import { getInitials } from '@utils/getInitialName';
import lazyWithRetry from '@utils/lazyImportWithReload';
import { useTranslation } from 'react-i18next';
import { TFunction } from 'i18next';
import { showNotification } from '@mantine/notifications';
import errorType from '@utils/services/axiosError';
import { IWithSearchPagination } from '@hoc/useSearchPagination';

const AddUpdateUserForm = lazyWithRetry(() => import('./AddUpdateUserForm'));

const useStyles = createStyles(() => ({
  nameCotainer: {
    maxWidth: '210px',
    minWidth: '210px',
    width: '210px',
  },
  emailContainer: {
    maxWidth: '230px',
    minWidth: '230px',
    width: '230px',
    overflow: 'hidden',
    textOverflow: 'ellipsis',
  },
  roleContainer: {
    maxWidth: '120px',
    minWidth: '120px',
    width: '120px',
  },
  phoneContainer: {
    maxWidth: '120px',
    minWidth: '120px',
    width: '120px',
  },
}));

const UserRow = ({
  item,
  search,
  auth,
  t,
}: {
  item: IUserProfile;
  search: string;
  auth: IAuthContext | null;
  t: TFunction;
}) => {
  const [opened, setOpened] = useState(false);
  const editUser = useEditUser(item?.id, search);
  const { colorScheme } = useMantineColorScheme();
  const resend = useResendEmail(item?.id);
  const { classes } = useStyles();

  const handleResendEmail = async () => {
    try {
      await resend.mutateAsync(item.id);
      showNotification({
        message: t('Email sent successfully!'),
        title: t('successful'),
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        title: 'Error!',
        color: 'red',
      });
    }
  };

  return (
    <tr key={item?.id}>
      <td
        style={{
          width: '122px',
          maxWidth: '122px',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
        }}
      >
        {item.memberId}
      </td>
      <td>
        <Modal
          size={800}
          opened={opened}
          onClose={() => setOpened(false)}
          title={`${t('edit_user')} ${item?.fullName}`}
          styles={{ title: { fontWeight: 'bold' } }}
        >
          {opened && (
            <Suspense fallback={<Loader />}>
              <AddUpdateUserForm
                setOpened={setOpened}
                opened={opened}
                isEditing={true}
                apiHooks={editUser}
                item={item}
              />
            </Suspense>
          )}
        </Modal>
        <div style={{ display: 'flex', textDecoration: 'none' }}>
          <Link
            to={`/userProfile/${item.id}/certificate`}
            style={{ textDecoration: 'none' }}
          >
            <Avatar size={26} src={item?.imageUrl} radius={26}>
              {!item?.imageUrl && getInitials(item?.fullName ?? '')}
            </Avatar>
          </Link>

          <Text
            size="sm"
            weight={500}
            lineClamp={1}
            ml={5}
            className={classes.nameCotainer}
          >
            {item?.fullName}
          </Text>
        </div>
      </td>
      <td className={classes.roleContainer}>{t(`${UserRole[item.role]}`)}</td>
      <td className={classes.emailContainer}>{item?.email.toLowerCase()}</td>

      <td className={classes.phoneContainer}>{item?.mobileNumber}</td>
      <td>
        {item?.status === UserStatus.Active ? (
          <Badge color={'green'}>{t('active')}</Badge>
        ) : item?.status === UserStatus.InActive ? (
          <Badge color={'red'}>{t('inactive')}</Badge>
        ) : (
          <Badge color="yellow">{t('pending')}</Badge>
        )}
      </td>

      <td style={{ display: 'flex' }}>
        {item.role !== UserRole.SuperAdmin && auth?.auth?.id !== item.id && (
          <Tooltip label={t('edit_user_detail')}>
            <ActionIcon
              style={{
                cursor: 'pointer',
                color: colorScheme === 'dark' ? '#F8F9FA' : '#25262B',
              }}
            >
              <IconEdit
                onClick={() => setOpened(true)}
                style={{ cursor: 'pointer' }}
                size={20}
              />
            </ActionIcon>
          </Tooltip>
        )}

        {auth?.auth?.id !== item.id && item.status === UserStatus.Pending && (
          <Tooltip label={t('resend_email')} onClick={handleResendEmail}>
            <ActionIcon
              style={{
                cursor: 'pointer',
                color: colorScheme === 'dark' ? '#F8F9FA' : '#25262B',
              }}
            >
              {resend.isLoading ? (
                <Loader variant="oval" />
              ) : (
                <IconSend size={20} />
              )}
            </ActionIcon>
          </Tooltip>
        )}
      </td>
    </tr>
  );
};

const UserMemberTable = ({
  sortComponent,
  users,
  search,
}: {
  users: IUserProfile[];
  search: string;
  sortComponent: Pick<IWithSearchPagination, 'sortComponent'>['sortComponent'];
}) => {
  const auth = useAuth();
  const { t } = useTranslation();

  const Rows = (auth: IAuthContext | null) =>
    users.map((item: any) => {
      return (
        <UserRow item={item} search={search} key={item.id} auth={auth} t={t} />
      );
    });

  return (
    <Paper>
      <Table
        sx={{ minWidth: 800 }}
        verticalSpacing="sm"
        striped
        highlightOnHover
        withBorder
        withColumnBorders
      >
        <thead>
          <tr>
            <th>{t('userid')}</th>
            <th>
              {sortComponent({ sortKey: 'firstName', title: t('username') })}
            </th>
            <th>{t('role')}</th>
            <th>{sortComponent({ sortKey: 'email', title: t('email') })}</th>
            <th>{t('phone_number')}</th>
            <th>{t('active_status')}</th>
            <th>{t('actions')}</th>
          </tr>
        </thead>
        <tbody>{Rows(auth)}</tbody>
      </Table>
    </Paper>
  );
};

export default UserMemberTable;
