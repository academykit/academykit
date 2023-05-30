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
  Flex,
} from "@mantine/core";
import UserMemberTable from "@components/Users/UserMemberTable";
import { useAddUser, useUsers } from "@utils/services/adminService";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import errorType from "@utils/services/axiosError";
import lazyWithRetry from "@utils/lazyImportWithReload";
import { uploadUserCsv } from "@utils/services/fileService";
import { showNotification } from "@mantine/notifications";
const AddUpdateUserForm = lazyWithRetry(
  () => import("../../Components/Users/AddUpdateUserForm")
);
import * as Yup from "yup";
import { useForm, yupResolver } from "@mantine/form";
import { useTranslation } from "react-i18next";

const sortByObject = [
  { value: "firstName:Ascending", label: "Name (A-Z)" },
  { value: "firstName:Descending", label: "Name (Z-A)" },
  { value: "email:Ascending", label: "Email (A-Z)" },
  { value: "email:Descending", label: "Email (Z-A)" },
];

const schema = Yup.object().shape({
  fileUpload: Yup.mixed().required("CSV file is required!"),
});

const UsersList = ({
  searchParams,
  pagination,
  searchComponent,
  sortComponent,
}: IWithSearchPagination) => {
  const [opened, setOpened] = useState(false);
  const { data, isLoading: loading, isError: error } = useUsers(searchParams);
  const addUser = useAddUser(searchParams);
  const [currentTab, setCurrentTab] = useState<string | null>("user");
  const [csvLoad, setCsvLoad] = useState<boolean>(false);
  const { t } = useTranslation();

  const form = useForm<{ fileUpload: File | null }>({
    initialValues: {
      fileUpload: null,
    },
    validate: yupResolver(schema),
  });

  const onSubmit = async (values: { fileUpload: File | null }) => {
    setCsvLoad(true);
    try {
      await uploadUserCsv(values.fileUpload);
      showNotification({
        message: t("user_imported_success"),
        title: t("successful"),
      });
      setOpened(false);
      form.reset();
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
        title: "Error",
      });
    }
    setCsvLoad(false);
  };

  return (
    <>
      <Modal
        size={800}
        opened={opened}
        onClose={() => setOpened(false)}
        title={t("add_more_user")}
        styles={{ title: { fontWeight: "bold" } }}
      >
        <Suspense fallback={<Loader />}>
          <Tabs value={currentTab} onTabChange={setCurrentTab}>
            <Tabs.List>
              <Tabs.Tab value="user">{t("add_user")}</Tabs.Tab>
              <Tabs.Tab value="import">{t("import_users")}</Tabs.Tab>
            </Tabs.List>
            <Tabs.Panel value="user">
              <Box mt={10}>
                <AddUpdateUserForm
                  setOpened={() => setOpened(false)}
                  opened={opened}
                  apiHooks={addUser}
                  isEditing={false}
                />
              </Box>
            </Tabs.Panel>
            <Tabs.Panel value="import">
              <Text my={10} size="sm">
                {t("csv_format")} {t("please")}
                <Anchor
                  href="https://vurilo-desktop-app.s3.ap-south-1.amazonaws.com/bulkimportsample.csv"
                  style={{
                    textDecoration: "underline",
                  }}
                  mx={5}
                >
                  {t("click_here")}
                </Anchor>
                {t("to_download_csv")}
              </Text>
              <form onSubmit={form.onSubmit(onSubmit)}>
                <FileInput
                  label={t("upload_csv")}
                  name="fileUpload"
                  withAsterisk
                  // value={file}
                  // onChange={setFile}
                  placeholder={t("your_csv") as string}
                  mt={10}
                  clearable
                  description={t("accepts_csv")}
                  accept="text/csv,
          application/vnd.openxmlformats-officedocument.presentationml.presentation,
          application/vnd.ms-excel,
          application/csv"
                  {...form.getInputProps("fileUpload")}
                />
                <Button loading={csvLoad} mt={10} type="submit">
                  {t("submit")}
                </Button>
              </form>
            </Tabs.Panel>
          </Tabs>
        </Suspense>
      </Modal>

      <Group
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>{t("users")}</Title>
        <div>
          <Button onClick={() => setOpened(true)}>{t("add_user")}</Button>
        </div>
      </Group>
      <Flex mb={10}>
        {searchComponent(t("search_users") as string)}
        <Flex>{sortComponent(sortByObject, t("sort_by"))}</Flex>
      </Flex>
      {loading && <Loader />}
      {error && <Box>{errorType(error)}</Box>}

      <ScrollArea>
        {data &&
          data?.items &&
          (data.items.length < 1 ? (
            <Box>{t("no_users")}</Box>
          ) : (
            <UserMemberTable users={data?.items} search={searchParams} />
          ))}
      </ScrollArea>
      {data && pagination(data.totalPage)}
    </>
  );
};
export default withSearchPagination(UsersList);
