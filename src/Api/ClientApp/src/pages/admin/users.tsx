import React, { Suspense, useState } from "react";
import {
  ScrollArea,
  Group,
  Button,
  Modal,
  Loader,
  Box,
  Title,
} from "@mantine/core";
import UserMemberTable from "@components/Users/UserMemberTable";
import { useAddUser, useUsers } from "@utils/services/adminService";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import errorType from "@utils/services/axiosError";
import lazyWithRetry from "@utils/lazyImportWithReload";
const AddUpdateUserForm = lazyWithRetry(
  () => import("../../Components/Users/AddUpdateUserForm")
);

const sortByObject = [
  { value: "firstName:Ascending", label: "Name (A-Z)" },
  { value: "firstName:Descending", label: "Name (Z-A)" },
  { value: "email:Ascending", label: "Email (A-Z)" },
  { value: "email:Descending", label: "Email (Z-A)" },
];

const UsersList = ({
  searchParams,
  pagination,
  searchComponent,
  sortComponent,
}: IWithSearchPagination) => {
  const [opened, setOpened] = useState(false);
  const { data, isLoading: loading, isError: error } = useUsers(searchParams);
  const addUser = useAddUser(searchParams);

  return (
    <>
      <Modal
        size={800}
        opened={opened}
        onClose={() => setOpened(false)}
        title="Add More Users"
        styles={{ title: { fontWeight: "bold" } }}
      >
        <Suspense fallback={<Loader />}>
          <AddUpdateUserForm
            setOpened={setOpened}
            opened={opened}
            apiHooks={addUser}
            isEditing={false}
          />
        </Suspense>
      </Modal>
      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>Users</Title>

        <Button ml={5} onClick={() => setOpened(true)}>
          Add User
        </Button>
      </Group>
      <div style={{ display: "flex", marginBottom: "10px" }}>
        {searchComponent("Search for users")}
        <div style={{ display: "flex" }}>
          {sortComponent(sortByObject, "Sort BY")}
        </div>
      </div>
      {loading && <Loader />}
      {error && <Box>{errorType(error)}</Box>}

      <ScrollArea>
        {data &&
          data?.items &&
          (data.items.length < 1 ? (
            <Box>No Users Found!</Box>
          ) : (
            <UserMemberTable users={data?.items} search={searchParams} />
          ))}
      </ScrollArea>
      {data && pagination(data.totalPage)}
    </>
  );
};
export default withSearchPagination(UsersList);
