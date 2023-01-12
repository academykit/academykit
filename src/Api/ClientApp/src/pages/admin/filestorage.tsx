import FileStorageUI from "@components/Admin/filestorage";
import { Loader, Paper } from "@mantine/core";
import { useGetFileStorageSetting } from "@utils/services/adminService";

const FileStorage = () => {
  const files = useGetFileStorageSetting();
  if (files.isLoading) {
    return <Loader />;
  }
  if (files.isError) {
    throw files.error;
  }
  return <FileStorageUI data={files.data} />;
};

export default FileStorage;