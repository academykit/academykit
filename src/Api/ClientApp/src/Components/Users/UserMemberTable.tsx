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
  useMantineColorScheme,
} from "@mantine/core";
import { useEditUser, useResendEmail } from "@utils/services/adminService";
import { UserRole, UserStatus } from "@utils/enums";

import { Suspense, useState } from "react";
import { Link } from "react-router-dom";
import { IconEdit, IconSend } from "@tabler/icons";
import { IUserProfile } from "@utils/services/types";
import useAuth from "@hooks/useAuth";
import { IAuthContext } from "@context/AuthProvider";
import { getInitials } from "@utils/getInitialName";
import lazyWithRetry from "@utils/lazyImportWithReload";
import { useTranslation } from "react-i18next";
import { TFunction } from "i18next";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";

const AddUpdateUserForm = lazyWithRetry(() => import("./AddUpdateUserForm"));

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

  const handleResendEmail = async () => {
    try {
      await resend.mutateAsync(item.id);
      showNotification({
        message: "Email sent successfully!",
        title: "Successful",
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        title: "Error!",
        color: "red",
      });
    }
  };

  return (
    <tr key={item?.id}>
      <td>
        <Modal
          size={800}
          opened={opened}
          onClose={() => setOpened(false)}
          title={`${t("edit_user")} ${item?.fullName}`}
          styles={{ title: { fontWeight: "bold" } }}
        >
          <Suspense fallback={<Loader />}>
            <AddUpdateUserForm
              setOpened={setOpened}
              opened={opened}
              isEditing={true}
              apiHooks={editUser}
              item={item}
            />
          </Suspense>
        </Modal>
        <div style={{ display: "flex", textDecoration: "none" }}>
          <Link
            to={`/userProfile/${item.id}`}
            style={{ textDecoration: "none" }}
          >
            <Avatar size={26} src={item?.imageUrl} radius={26}>
              {!item?.imageUrl && getInitials(item?.fullName ?? "")}
            </Avatar>
          </Link>

          <Text size="sm" weight={500} lineClamp={1} ml={5}>
            {item?.fullName}
          </Text>
        </div>
      </td>
      <td>{t(`${UserRole[item.role]}`)}</td>
      <td>{item?.email}</td>

      <td>{item?.mobileNumber}</td>
      <td>
        {item?.status === UserStatus.Active ? (
          <Badge color={"green"}>Active</Badge>
        ) : item?.status === UserStatus.InActive ? (
          <Badge color={"red"}>InActive</Badge>
        ) : (
          <Badge color="yellow">Pending</Badge>
        )}
      </td>

      <td style={{ display: "flex" }}>
        {item.role !== UserRole.SuperAdmin && auth?.auth?.id !== item.id && (
          <Tooltip label="Edit User Details">
            <ActionIcon
              style={{
                cursor: "pointer",
                color: colorScheme === "dark" ? "#F8F9FA" : "#25262B",
              }}
            >
              <IconEdit
                onClick={() => setOpened(true)}
                style={{ cursor: "pointer" }}
                size={20}
              />
            </ActionIcon>
          </Tooltip>
        )}

        {auth?.auth?.id !== item.id && item.status === UserStatus.Pending && (
          <Tooltip label="Resend Email" onClick={handleResendEmail}>
            <ActionIcon
              style={{
                cursor: "pointer",
                color: colorScheme === "dark" ? "#F8F9FA" : "#25262B",
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
  users,
  search,
}: {
  users: IUserProfile[];
  search: string;
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
      >
        <thead>
          <tr>
            <th>{t("user")}</th>
            <th>{t("role")}</th>
            <th>{t("email")}</th>
            <th>{t("phone_number")}</th>
            <th>{t("active_status")}</th>
            <th>{t("actions")}</th>
          </tr>
        </thead>
        <tbody>{Rows(auth)}</tbody>
      </Table>
    </Paper>
  );
};

export default UserMemberTable;
