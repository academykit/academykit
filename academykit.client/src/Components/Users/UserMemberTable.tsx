import { IAuthContext } from "@context/AuthProvider";
import { IWithSearchPagination } from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
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
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconSend } from "@tabler/icons-react";
import { UserRole, UserStatus } from "@utils/enums";
import { getInitials } from "@utils/getInitialName";
import lazyWithRetry from "@utils/lazyImportWithReload";
import { useEditUser, useResendEmail } from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { IUserProfile } from "@utils/services/types";
import { TFunction } from "i18next";
import { Suspense, useState } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import classes from "./styles/memberTable.module.css";

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
        message: t("Email sent successfully!"),
        title: t("successful"),
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
    <Table.Tr key={item?.id}>
      <Table.Td
        style={{
          width: "122px",
          maxWidth: "122px",
          overflow: "hidden",
          textOverflow: "ellipsis",
        }}
      >
        {item.memberId}
      </Table.Td>
      <Table.Td>
        <Modal
          size={800}
          opened={opened}
          onClose={() => setOpened(false)}
          title={`${t("edit_user")} ${item?.fullName}`}
          styles={{ title: { fontWeight: "bold" } }}
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
        <div style={{ display: "flex", textDecoration: "none" }}>
          <Link
            to={`/userProfile/${item.id}/certificate`}
            style={{ textDecoration: "none" }}
          >
            <Avatar size={26} src={item?.imageUrl} radius={26}>
              {!item?.imageUrl && getInitials(item?.fullName ?? "")}
            </Avatar>
          </Link>

          <Text
            size="sm"
            fw={500}
            lineClamp={1}
            ml={5}
            className={classes.nameCotainer}
          >
            {item?.fullName}
          </Text>
        </div>
      </Table.Td>
      <Table.Td className={classes.roleContainer}>
        {t(`${UserRole[item.role]}`)}
      </Table.Td>
      <Table.Td className={classes.emailContainer}>
        {item?.email.toLowerCase()}
      </Table.Td>

      <Table.Td className={classes.phoneContainer}>
        {item?.mobileNumber}
      </Table.Td>
      <Table.Td>
        {item?.status === UserStatus.Active ? (
          <Badge variant="light" color={"green"}>
            {t("active")}
          </Badge>
        ) : item?.status === UserStatus.InActive ? (
          <Badge variant="light" color={"red"}>
            {t("inactive")}
          </Badge>
        ) : (
          <Badge variant="light" color="yellow">
            {t("pending")}
          </Badge>
        )}
      </Table.Td>

      <Table.Td style={{ display: "flex" }}>
        {item.role !== UserRole.SuperAdmin && auth?.auth?.id !== item.id && (
          <Tooltip label={t("edit_user_detail")}>
            <ActionIcon
              variant="transparent"
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
          <Tooltip label={t("resend_email")} onClick={handleResendEmail}>
            <ActionIcon
              variant="transparent"
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
      </Table.Td>
    </Table.Tr>
  );
};

const UserMemberTable = ({
  sortComponent,
  users,
  search,
}: {
  users: IUserProfile[];
  search: string;
  sortComponent: Pick<IWithSearchPagination, "sortComponent">["sortComponent"];
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
        style={{ minWidth: 800 }}
        verticalSpacing="sm"
        striped
        highlightOnHover
        withTableBorder
        withColumnBorders
      >
        <Table.Thead>
          <Table.Tr>
            <Table.Th>{t("userid")}</Table.Th>
            <Table.Th>
              {sortComponent({ sortKey: "firstName", title: t("username") })}
            </Table.Th>
            <Table.Th>{t("role")}</Table.Th>
            <Table.Th>
              {sortComponent({ sortKey: "email", title: t("email") })}
            </Table.Th>
            <Table.Th>{t("phone_number")}</Table.Th>
            <Table.Th>{t("active_status")}</Table.Th>
            <Table.Th>{t("actions")}</Table.Th>
          </Table.Tr>
        </Table.Thead>
        <Table.Tbody>{Rows(auth)}</Table.Tbody>
      </Table>
    </Paper>
  );
};

export default UserMemberTable;
