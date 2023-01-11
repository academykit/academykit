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

const AddUpdateUserForm = lazyWithRetry(() => import("./AddUpdateUserForm"));

const UserRow = ({
  item,
  search,
  auth,
}: {
  item: IUserProfile;
  search: string;
  auth: IAuthContext | null;
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
      <td>{UserRole[item.role]}</td>
      <td>{item?.email}</td>
      <td>{item?.mobileNumber}</td>
      <td>
        {item?.isActive ? (
          <Badge color={"green"}>Active</Badge>
        ) : (
          <Badge color={"red"}>InActive</Badge>
        )}
      </td>

      <td>
        {auth?.auth?.id !== item.id && (
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

  const Rows = (auth: IAuthContext | null) =>
    users.map((item: any) => {
      return <UserRow item={item} search={search} key={item.id} auth={auth} />;
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
            <th>User</th>
            <th>Role</th>
            <th>Email</th>
            <th>Phone Number</th>
            <th>Active Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>{Rows(auth)}</tbody>
      </Table>
    </Paper>
  );
};

export default UserMemberTable;
