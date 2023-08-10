import { useQuery } from '@tanstack/react-query';
import { httpClient } from './service-axios';

export enum FileAccess {
  Private = 1,
  Public = 2,
}

export const uploadFile = (file: File, type: number) => {
  return httpClient.post<string>(
    '/api/media/file',
    { file, type },
    {
      headers: {
        'content-type': 'multipart/form-data',
      },
    }
  );
};
export const uploadVideo = (file: File, type: number) => {
  return httpClient.post<string>(
    '/api/media/file',
    { file, type },
    {
      headers: {
        'content-type': 'multipart/form-data',
      },
    }
  );
};

export const getFileUrl = (key: string, enabled: boolean) => {
  return useQuery(
    ['/api/media/file/' + key],
    () => {
      return httpClient.get<string>('/api/media/file?key=' + key);
    },
    { select: (data) => data.data, retry: false, enabled }
  );
};

export const uploadUserCsv = (file: File | null) => {
  return httpClient.post(
    '/api/user/bulkuser',
    { file },
    {
      headers: {
        'content-type': 'multipart/form-data',
      },
    }
  );
};

export const downloadCSVFile = async (path: string) => {
  try {
    const response = await httpClient.get(path);
    const link = document.createElement('a');

    if (response.statusText == 'OK') {
      const objRef = window.URL.createObjectURL(
        new Blob([response.data as Blob], { type: 'text/csv;charset=utf-8' })
      );
      link.href = objRef;
      link.setAttribute('download', 'sample.csv');
      document.body.appendChild(link);
      link.click();
      window.URL.revokeObjectURL(objRef);
    }
  } catch (error) {
    console.log(error);
  }
};
