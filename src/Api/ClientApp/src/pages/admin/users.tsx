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
  Tabs,
  FileInput,
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
  const [currentTab, setCurrentTab] = useState<string | null>("user");
  const [file, setFile] = useState<File | null>(null);

  const onSubmit = () => {};

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
          <Tabs value={currentTab} onTabChange={setCurrentTab}>
            <Tabs.List>
              <Tabs.Tab value="user">Add User</Tabs.Tab>
              <Tabs.Tab value="import">Import Users</Tabs.Tab>
            </Tabs.List>
            <Tabs.Panel value="user">
              <Box mt={10}>
                <AddUpdateUserForm
                  setOpened={setOpened}
                  opened={opened}
                  apiHooks={addUser}
                  isEditing={false}
                />
              </Box>
            </Tabs.Panel>
            <Tabs.Panel value="import">
              <Text my={10} size="sm">
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
              <FileInput
                label="Upload your CSV file"
                value={file}
                onChange={setFile}
                mt={10}
                accept="text/csv,
          application/vnd.openxmlformats-officedocument.presentationml.presentation,
          application/vnd.ms-excel,
          application/csv"
              />
              <Button mt={10}>Submit</Button>
              {/* <CSVUpload /> */}
            </Tabs.Panel>
          </Tabs>
        </Suspense>
      </Modal>

      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>Users</Title>
        <div>
          <Button onClick={() => setOpened(true)}>Add User</Button>
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
