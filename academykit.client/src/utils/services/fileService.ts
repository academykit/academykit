import { showNotification } from "@mantine/notifications";
import { useQuery } from "@tanstack/react-query";
import errorType from "./axiosError";
import { httpClient } from "./service-axios";

export enum FileAccess {
  Private = 1,
  Public = 2,
}

export const uploadFile = (file: File, type: number) => {
  const formData = new FormData();
  formData.append("file", file, file.name);
  formData.append("type", type.toString());
  return httpClient.post<string>("/api/media/file", formData);
};
export const uploadVideo = (file: File, type: number) => {
  const formData = new FormData();
  formData.append("file", file, file.name);
  formData.append("type", type.toString());
  return httpClient.post<string>("/api/media/file", formData);
};

export const getFileUrl = (key: string, enabled: boolean) => {
  return useQuery({
    queryKey: ["/api/media/file/" + key],

    queryFn: () => {
      return httpClient.get<string>("/api/media/file?key=" + key);
    },

    select: (data) => data.data,
    retry: false,
    enabled,
  });
};

export const uploadUserCsv = (file: File | null) => {
  if (!file) return;
  const formData = new FormData();
  formData.append("file", file, file.name);

  return httpClient.post("/api/user/bulkUser", formData);
};

export const downloadCSVFile = async (path: string, fileName: string) => {
  try {
    const response = await httpClient.get(path);
    const link = document.createElement("a");

    const objRef = window.URL.createObjectURL(
      new Blob([response.data as Blob], { type: "text/csv; charset=UTF-8" })
    );
    link.href = objRef;
    link.setAttribute("download", `${fileName}.csv`);
    document.body.appendChild(link);
    link.click();
    window.URL.revokeObjectURL(objRef);
  } catch (error) {
    const err = errorType(error);
    showNotification({
      message: err,
      color: "red",
      title: "Error",
    });
  }
};
