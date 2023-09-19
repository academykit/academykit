import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { EFileStorageType } from '@utils/enums';
import queryStringGenerator from '@utils/queryStringGenerator';
import axios from 'axios';
import errorType from './axiosError';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IAddUser, IPaginated, IUser, IUserProfile } from './types';

export interface IDepartmentSetting {
  id: string;
  slug: string;
  name: string;
  isActive: boolean;
  items: [];
  user: IUser;
}
export interface IZoomSetting {
  id: string;
  isRecordingEnabled: boolean;
  updatedOn: string;
  user: IUser;
  sdkKey: string;
  sdkSecret: string;
  webhookSecret: string;
  webhookVerificationKey: string;
  oAuthAccountId: string;
  oAuthClientId: string;
  oAuthClientSecret: string;
}

export interface ILevel {
  id: string;
  name: string;
  user: IUser;
}
export interface ISMTPSetting {
  id: string;
  mailServer: string;
  mailPort: number;
  senderName: string;
  senderEmail: string;
  userName: string;
  password: string;
  replyTo: string;
  useSSL: boolean;
  updatedOn: string;
  user: IUser;
}

export interface ISMTPSettingUpdate {
  senderEmail: string;
  password: string;
  mailPort: number;
  mailServer: string;
  senderName: string;
  userName: string;
  replyTo: string;
  useSSL?: true;
}

export interface IGeneralSetting {
  id: string;
  logoUrl: string;
  companyName: string;
  companyAddress: string;
  companyContactNumber: string;
  emailSignature: string;
  updatedOn: string;
  user: IUser;
}
export interface ICompanySetting {
  name: string;
  imageUrl: string;
  customConfiguration?: string;
  appVersion: string;
}

export interface IGeneralSettingUpdate {
  logoUrl: string;
  companyName: string;
  companyAddress: string;
  companyContactNumber: string;
  emailSignature: string;
}

export interface IZoomLicense<T> {
  id: string;
  licenseEmail: string;
  hostId: string;
  capacity: 0;
  isActive: true;
  user: T;
}

const getUsers = (search: string) => {
  return httpClient.get<IPaginated<IUserProfile>>(api.adminUser.users(search));
};

export const useUsers = (search: string) =>
  useQuery(
    ['user-list', search],
    () => {
      return getUsers(search);
    },
    {
      enabled: true,
      select: (data) => data.data,
    }
  );

const updateUser = ({ data, id }: { data: any; id: string }) =>
  httpClient.put(api.auth.getUser(id), data);
export const useUpdateUser = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation(['update' + api.auth.getUser], updateUser, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.auth.me]);

      queryClient.invalidateQueries([api.auth.getUser(id)]);
    },
  });
};

const updateUserStatus = async ({
  userId,
  status,
}: {
  userId: string;
  status: boolean;
}) => await httpClient.patch(api.adminUser.updateUserStatus(userId, status));

export const useUpdateUserStatus = (search: string) => {
  const queryClient = useQueryClient();
  return useMutation(['user-list', search], updateUserStatus, {
    onSuccess: () => {
      queryClient.invalidateQueries(['user-list', search]);
    },
  });
};

const addUser = (data: IAddUser) =>
  httpClient.post(api.adminUser.addUsers, data);

export const useAddUser = (search: string) => {
  const queryClient = useQueryClient();
  return useMutation([api.adminUser.addUsers], addUser, {
    onSuccess: () => {
      queryClient.invalidateQueries(['user-list', search]);
    },
  });
};

const editUser = async ({ id, data }: { id: string; data: IAddUser }) =>
  await httpClient.put(api.adminUser.editUsers(id), data);

export const useEditUser = (id: string, search: string) => {
  const queryClient = useQueryClient();
  return useMutation(['update' + api.adminUser.editUsers(id)], editUser, {
    onSuccess: () => {
      queryClient.invalidateQueries(['user-list', search]);
    },
  });
};

export const useLevelSetting = () => {
  return useQuery(
    [api.adminUser.getLevelSetting],
    () => {
      return httpClient.get<ILevel[]>(api.adminUser.getLevelSetting);
    },

    { staleTime: Infinity, cacheTime: Infinity, select: (data) => data.data }
  );
};
export const usePostLevelSetting = () => {
  const queryClient = useQueryClient();
  return useMutation(
    ['post' + api.adminUser.postLevelSetting],
    (data: { name: string }) => {
      return httpClient.post<ILevel>(api.adminUser.postLevelSetting, data);
    },
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getLevelSetting]);
      },
    }
  );
};

const updateLevelSetting = async ({
  id,
  name,
}: {
  id: string;
  name: string;
}) => {
  return await httpClient.put(`/api/level/${id}`, { name });
};

export const useUpdateLevelSetting = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation(
    ['update' + api.adminUser.updateLevelSetting(id)],
    updateLevelSetting,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getLevelSetting]);
      },
    }
  );
};

const deleteLevelSetting = async (id: string) => {
  return await httpClient.delete(api.adminUser.deleteLevelSetting(id));
};
export const useDeleteLevelSetting = () => {
  const queryClient = useQueryClient();
  return useMutation(
    [api.adminUser.deleteLevelSetting],
    deleteLevelSetting,

    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getLevelSetting]);
      },
      onError: (err) => {
        return errorType(err);
      },
    }
  );
};

export const updateDepartmentStatus = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation(
    ['update' + api.adminUser.updateDepartmentSetting(id)],
    (data: { name: string }) => {
      return httpClient.put<ILevel>(
        api.adminUser.updateDepartmentSetting(id),
        data
      );
    },
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getLevelSetting]);
      },
    }
  );
};

const getDepartment = async (search: string) =>
  await httpClient.get<IPaginated<IDepartmentSetting>>(
    api.adminUser.getDepartmentSettings + `?${search}`
  );

export const useDepartmentSetting = (search: string) => {
  return useQuery(
    [api.adminUser.getDepartmentSettings, search],
    () => getDepartment(search),
    {
      staleTime: Infinity,
      cacheTime: Infinity,
      select: (data) => data.data,
    }
  );
};
export const usePostDepartmentSetting = () => {
  const queryClient = useQueryClient();
  return useMutation(
    ['post' + api.adminUser.getDepartmentSettings],
    (data: { name: string; isActive: boolean }) => {
      return httpClient.post<IDepartmentSetting>(
        api.adminUser.getDepartmentSettings,
        data
      );
    },
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getDepartmentSettings]);
      },
    }
  );
};

const deleteDepartmentSetting = async (id: string) => {
  return await httpClient.delete(api.adminUser.deleteDepartmentSetting(id));
};

export const useDeleteDepartmentSetting = () => {
  const queryClient = useQueryClient();
  return useMutation(
    [api.adminUser.deleteDepartmentSetting],
    deleteDepartmentSetting,

    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getDepartmentSettings]);
      },
      onError: (err) => {
        return errorType(err);
      },
    }
  );
};

export const useUpdateDepartmentSettingStatus = (
  id: string,
  status: boolean
) => {
  const queryClient = useQueryClient();
  return useMutation(
    ['patch' + api.adminUser.updateDepartmentStatus(id, status)],
    () => {
      return httpClient.patch(api.adminUser.updateDepartmentStatus(id, status));
    },
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getDepartmentSettings]);
      },
    }
  );
};
//start

const updateDepartmentSetting = async ({
  id,
  name,
  isActive,
}: {
  id: string;
  name: string;
  isActive: boolean;
}) => {
  return await httpClient.put(`/api/department/${id}`, { name, isActive });
};

export const useUpdateDepartmentSetting = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation(
    ['update' + api.adminUser.updateDepartmentSetting(id)],
    updateDepartmentSetting,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getDepartmentSettings]);
      },
    }
  );
};
//end

export const useZoomSetting = () => {
  return useQuery(
    [api.adminUser.getZoomSettings],
    () => {
      return httpClient.get<IZoomSetting>(api.adminUser.getZoomSettings);
    },
    { staleTime: Infinity, cacheTime: Infinity }
  );
};

export const useUpdateZoomSetting = (id: string | undefined) => {
  const queryClient = useQueryClient();
  return useMutation(
    ['update' + api.adminUser.getZoomSettings],
    (data: {
      oAuthAccountId: string;
      oAuthClientId: string;
      oAuthClientSecret: string;
      sdkKey: string;
      sdkSecret: string;
      isRecordingEnabled: boolean;
    }) => {
      return httpClient.put<IZoomSetting>(
        api.adminUser.updateZoomSettings(id),
        data
      );
    },
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getZoomSettings]);
      },
    }
  );
};

export const useSMTPSetting = () => {
  return useQuery(
    [api.adminUser.getSMTPSettings],
    () => {
      return httpClient.get<ISMTPSetting>(api.adminUser.getSMTPSettings);
    },
    { staleTime: Infinity, cacheTime: Infinity }
  );
};

export const useUpdateSMTPSetting = (id: string | undefined) => {
  const queryClient = useQueryClient();
  return useMutation(
    ['update' + api.adminUser.getSMTPSettings],
    (data: ISMTPSettingUpdate) => {
      return httpClient.put<ISMTPSetting>(
        api.adminUser.updateSMTPSettings(id),
        data
      );
    },
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getZoomSettings]);
      },
    }
  );
};

// General Settings
export const useGeneralSetting = () => {
  return useQuery(
    [api.adminUser.getGeneralSettings],
    () => {
      return httpClient.get<IGeneralSetting>(api.adminUser.getGeneralSettings);
    },
    { staleTime: Infinity, cacheTime: Infinity }
  );
};
// company settings
export const useCompanySetting = () => {
  return useQuery([api.adminUser.getCompanySettings], () => {
    return httpClient.get<ICompanySetting>(api.adminUser.getCompanySettings);
  });
};

export const useUpdateGeneralSetting = (id: string | undefined) => {
  const queryClient = useQueryClient();
  return useMutation(
    ['update' + api.adminUser.getGeneralSettings],
    (data: IGeneralSettingUpdate) => {
      return httpClient.put(api.adminUser.updateGeneralSettings(id), data);
    },
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getGeneralSettings]);
      },
    }
  );
};

//Zoom License
export const useZoomLicense = () => {
  return useQuery(
    [api.adminUser.getZoomLicense],
    () => {
      return httpClient.get<IPaginated<IZoomLicense<IUser>>>(
        api.adminUser.getZoomLicense
      );
    },
    { staleTime: Infinity, cacheTime: Infinity }
  );
};

const getActiveZoomLicense = (
  startDateTime: string,
  duration: number,
  lessonIdentity?: string
) => {
  const query = queryStringGenerator({
    startDateTime,
    duration,
    lessonIdentity,
  });
  return httpClient.get<IZoomLicense<IUser>[]>(
    api.adminUser.getActiveZoomLicense(query)
  );
};
export const useActiveZoomLicense = (
  startDateTime: string,
  duration: number,
  lessonIdentity?: string
) => {
  return useQuery(
    ['active' + api.adminUser.getZoomLicense, startDateTime, duration],
    () => getActiveZoomLicense(startDateTime, duration, lessonIdentity),
    {
      select: (data) => data,
      enabled: !!startDateTime && !!duration,
    }
  );
};

export const updateZoomLicenseStatus = async ({
  id,
  status,
}: {
  id: string;
  status: boolean;
}) => {
  return await httpClient.patch(
    api.adminUser.updateZoomLicenseStatus(id, status)
  );
};

const deleteZoomLicense = async (id: string) => {
  return await httpClient.delete(api.adminUser.deleteZoomLicense(id));
};

export const useDeleteZoomLicense = () => {
  const queryClient = useQueryClient();
  return useMutation(
    [api.adminUser.deleteZoomLicense],
    deleteZoomLicense,

    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getZoomLicense]);
      },
      onError: (err) => {
        return errorType(err);
      },
    }
  );
};

const updateZoomLicense = ({ id, data }: { id: string; data: any }) =>
  httpClient.put(api.adminUser.deleteZoomLicense(id), data);

export const useUpdateZoomLicense = () => {
  const queryClient = useQueryClient();
  return useMutation(
    ['update' + api.adminUser.getZoomLicense],
    updateZoomLicense,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.adminUser.getZoomLicense]);
      },
    }
  );
};

const addZoomLicense = (data: {
  licenseEmail: string;
  hostId: string;
  capacity: number;
}) => httpClient.post(api.adminUser.addZoomLicense, data);
export const useAddZoomLicense = () => {
  const queryClient = useQueryClient();
  return useMutation([api.adminUser.addZoomLicense], addZoomLicense, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.adminUser.getZoomLicense]);
    },
    onError: () => {},
  });
};

//filestorage
export interface IFileStorage {
  type: EFileStorageType;
  values: IFileStorageValues[];
  isActive: boolean;
}
export interface IFileStorageValues {
  key: string;
  value: string | null;
}
const getFileStorageSetting = async () => {
  return await httpClient.get<IFileStorage[]>(
    api.fileStorage.getFileStorageSetting
  );
};

export const useGetFileStorageSetting = () => {
  return useQuery(
    [api.fileStorage.getFileStorageSetting],
    () => getFileStorageSetting(),
    {
      select: (data) => {
        return data.data;
      },
      // retry: 0,
      onError: (err) => {
        if (axios.isAxiosError(err)) {
          if (err.response?.status === 403) {
            return null;
          }
        }
      },
      // refetchOnMount: false,
      // refetchOnWindowFocus: false,
    }
  );
};

//putfilestorage

const updateFileStorage = async (data: IFileStorage[]) =>
  await httpClient.put<IFileStorage>(
    api.fileStorage.updateFileStorageSetting,
    data.filter((x) => x.isActive)[0]
  );

export const useUpdateFileStorage = () => {
  const queryClient = useQueryClient();
  return useMutation(
    [api.fileStorage.updateFileStorageSetting],
    updateFileStorage,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.fileStorage.getFileStorageSetting]);
      },
      onError: (err) => {
        return errorType(err);
      },
    }
  );
};

// ------------Resend Email-------------------
const resendEmail = async (id: string) =>
  await httpClient.patch(api.auth.resendEmail(id));
export const useResendEmail = (id: string) =>
  useMutation([api.auth.resendEmail(id)], resendEmail);

//----------------------Get Trainers-------------------

interface ITrainerGet {
  userId: string;
  role: number;
  email: string;
}

const getTrainers = async (search: string, lessonType?: number, id?: string) =>
  await httpClient.get<ITrainerGet[]>(
    api.adminUser.getTrainer(search, lessonType, id)
  );
export const useGetTrainers = (
  search: string,
  lessonType?: number,
  id?: string
) =>
  useQuery(
    [api.adminUser.getTrainer(search, lessonType, id)],
    () => getTrainers(search, lessonType, id),
    {
      enabled: true,
      select: (data) => data.data,
    }
  );

//----------------------Get server logs-------------------

export interface IServerLogs {
  id: string;
  type: number;
  message: string;
  trackBy: string;
  timeStamp: Date;
}

const getLogs = async (query: string) => {
  return httpClient.get<IPaginated<IServerLogs>>(api.adminUser.getLogs(query));
};

export const useGetServerLogs = (query: string) => {
  return useQuery(['logs' + query], () => getLogs(query), {
    select: (data) => data.data,
    //enabled: !!startDateTime && !!duration,
  });
};

const getSingleLog = async (id: string) => {
  return httpClient.get<IServerLogs>(api.adminUser.getSingleLog(id));
};

export const useGetSingleLog = (id: string) => {
  return useQuery(['log' + id], () => getSingleLog(id), {
    select: (data) => data.data,
  });
};
