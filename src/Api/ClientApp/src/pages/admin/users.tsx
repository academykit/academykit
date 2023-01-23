import React, { Suspense, useState } from "react";
import {
  ScrollArea,
  Group,
  Button,
  Modal,
  Loader,
  Box,
  Title,
  Text,
  Anchor,
} from "@mantine/core";
import UserMemberTable from "@components/Users/UserMemberTable";
import { useAddUser, useUsers } from "@utils/services/adminService";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import errorType from "@utils/services/axiosError";
import lazyWithRetry from "@utils/lazyImportWithReload";
import CSVUpload from "@components/Ui/CSVUpload";
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
  const [importModal, setImportModal] = useState(false);
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
      <Modal
        opened={importModal}
        onClose={() => setImportModal(false)}
        title="Bulk Import Users"
        styles={{ title: { fontWeight: "bold" } }}
      >
        <Text mb={10} size="sm">
          CSV file format should be similar to sample CSV. Please
          <Anchor
            href="https://google.com"
            style={{
              textDecoration: "underline",
            }}
            mx={5}
          >
            click here
          </Anchor>
          to download sample CSV.
        </Text>

        <CSVUpload />
      </Modal>
      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>Users</Title>
        <div>
          <Button onClick={() => setOpened(true)}>Add User</Button>
          {/* <Button
            ml={10}
            variant="outline"
            onClick={() => setImportModal(true)}
          >
            Import Users
          </Button> */}
        </div>
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
