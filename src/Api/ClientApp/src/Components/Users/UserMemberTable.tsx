import {
  Avatar,
  Badge,
  Loader,
  Modal,
  Paper,
  Table,
  Text,
} from "@mantine/core";
import { useEditUser } from "@utils/services/adminService";
import { UserRole } from "@utils/enums";

import { Suspense, useState } from "react";
import { Link } from "react-router-dom";
import { IconEdit } from "@tabler/icons";
import { IUserProfile } from "@utils/services/types";
import useAuth from "@hooks/useAuth";
import { IAuthContext } from "@context/AuthProvider";
import { getInitials } from "@utils/getInitialName";
import lazyWithRetry from "@utils/lazyImportWithReload";
import { useTranslation } from "react-i18next";
import { TFunction } from "i18next";

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

  return (
    <tr key={item?.id}>
      <td>
        <Modal
          size={800}
          opened={opened}
          onClose={() => setOpened(false)}
          title={`Editing User: ${item?.fullName}`}
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
        {item?.isActive ? (
          <Badge color={"green"}>{t("active")}</Badge>
        ) : (
          <Badge color={"red"}>{t("inactive")}</Badge>
        )}
      </td>

      <td>
        {item.role !== UserRole.SuperAdmin && auth?.auth?.id !== item.id && (
          <IconEdit
            onClick={() => setOpened(true)}
            style={{ cursor: "pointer" }}
          />
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
