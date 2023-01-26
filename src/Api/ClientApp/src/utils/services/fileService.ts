import { useQuery } from "@tanstack/react-query";
import { httpClient } from "./service-axios";

export enum FileAccess {
  Private = 1,
  Public = 2
}

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


export const getFileUrl =  (key: string, enabled: boolean) => {
  return useQuery(['/api/media/file/'+key], () => 
  {
    return  httpClient.get<string>("/api/media/file?key="+key)},
  {select: data => data.data, retry: false, enabled}
  )
}


export const uploadUserCsv = (file: File | null) => {
  return httpClient.post(
    '/api/user/bulkuser',
    {file},
    {
      headers: {
        'content-type': 'multipart/form-data'
      }
    }
  )
}