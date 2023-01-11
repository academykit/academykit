import { httpClient } from "./service-axios";

export const uploadFile = (file: File, type: number) => {
  return httpClient.post<string>(
    "/api/media/file",
    { file, type },
    {
      headers: {
        "content-type": "multipart/form-data",
      },
    }
  );
};
export const uploadVideo = (file: File, type: number) => {
  return httpClient.post<string>(
    "/api/media/file",
    { file, type },
    {
      headers: {
        "content-type": "multipart/form-data",
      },
    }
  );
};
