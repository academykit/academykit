import { showNotification } from '@mantine/notifications';
import { useQuery } from '@tanstack/react-query';
import errorType from './axiosError';
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
  return useQuery({
    queryKey: ['/api/media/file/' + key],

    queryFn: () => {
      return httpClient.get<string>('/api/media/file?key=' + key);
    },

    select: (data) => data.data,
    retry: false,
    enabled,
  });
};

export const uploadUserCsv = (file: File | null) => {
  return httpClient.post(
    '/api/user/bulkUser',
    { file },
    {
      headers: {
        'content-type': 'multipart/form-data',
      },
    }
  );
};

export const downloadCSVFile = async (path: string, fileName: string) => {
  try {
    const response = await httpClient.get(path);
    const link = document.createElement('a');

    const objRef = window.URL.createObjectURL(
      new Blob([response.data as Blob], { type: 'text/csv; charset=UTF-8' })
    );
    link.href = objRef;
    link.setAttribute('download', `${fileName}.csv`);
    document.body.appendChild(link);
    link.click();
    window.URL.revokeObjectURL(objRef);
  } catch (error) {
    const err = errorType(error);
    showNotification({
      message: err,
      color: 'red',
      title: 'Error',
    });
  }
};
