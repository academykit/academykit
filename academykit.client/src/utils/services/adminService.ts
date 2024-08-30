import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { EFileStorageType, MailType } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import errorType from "./axiosError";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IAddUser, IPaginated, IUser, IUserProfile } from "./types";

export interface IMailNotification {
  id: string;
  mailName: string;
  mailSubject: string;
  mailMessage: string;
  mailType: MailType;
  isActive: boolean;
}

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
  isSetupCompleted?: boolean;
}

export interface IGeneralSettingUpdate {
  logoUrl: string;
  companyName: string;
  companyAddress: string;
  companyContactNumber: string;
  emailSignature: string;
}

export interface ISetupInitial {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  companyName: string;
  companyAddress: string;
  logoUrl: string;
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
  useQuery({
    queryKey: ["user-list", search],

    queryFn: () => {
      return getUsers(search);
    },

    enabled: true,
    select: (data) => data.data,
  });

const updateUser = ({ data, id }: { data: any; id: string }) =>
  httpClient.put(api.auth.getUser(id), data);
export const useUpdateUser = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.auth.getUser],
    mutationFn: updateUser,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.auth.me],
      });

      queryClient.invalidateQueries({
        queryKey: [api.auth.getUser(id)],
      });
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
  return useMutation({
    mutationKey: ["user-list", search],
    mutationFn: updateUserStatus,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["user-list", search],
      });
    },
  });
};

const addUser = (data: IAddUser) =>
  httpClient.post(api.adminUser.addUsers, data);

export const useAddUser = (search: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.adminUser.addUsers],
    mutationFn: addUser,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["user-list", search],
      });
    },
  });
};

const editUser = async ({ id, data }: { id: string; data: IAddUser }) =>
  await httpClient.put(api.adminUser.editUsers(id), data);

export const useEditUser = (id: string, search: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.editUsers(id)],
    mutationFn: editUser,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["user-list", search],
      });
    },
  });
};

export const useLevelSetting = () => {
  return useQuery({
    queryKey: [api.adminUser.getLevelSetting],

    queryFn: () => {
      return httpClient.get<ILevel[]>(api.adminUser.getLevelSetting);
    },

    staleTime: Infinity,
    cacheTime: Infinity,
    select: (data) => data.data,
  });
};
export const usePostLevelSetting = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["post" + api.adminUser.postLevelSetting],

    mutationFn: (data: { name: string }) => {
      return httpClient.post<ILevel>(api.adminUser.postLevelSetting, data);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getLevelSetting],
      });
    },
  });
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

export const useUpdateLevelSetting = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.getLevelSetting],
    mutationFn: updateLevelSetting,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getLevelSetting],
      });
    },
  });
};

const deleteLevelSetting = async (id: string) => {
  return await httpClient.delete(api.adminUser.deleteLevelSetting(id));
};
export const useDeleteLevelSetting = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.adminUser.deleteLevelSetting],
    mutationFn: deleteLevelSetting,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getLevelSetting],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

export const updateDepartmentStatus = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.updateDepartmentSetting(id)],

    mutationFn: (data: { name: string }) => {
      return httpClient.put<ILevel>(
        api.adminUser.updateDepartmentSetting(id),
        data
      );
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getLevelSetting],
      });
    },
  });
};

const getDepartment = async (search: string) =>
  await httpClient.get<IPaginated<IDepartmentSetting>>(
    api.adminUser.getDepartmentSettings + `?${search}`
  );

export const useDepartmentSetting = (search: string) => {
  return useQuery({
    queryKey: [api.adminUser.getDepartmentSettings, search],
    queryFn: () => getDepartment(search),
    staleTime: Infinity,
    cacheTime: Infinity,
    select: (data) => data.data,
  });
};
export const usePostDepartmentSetting = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["post" + api.adminUser.getDepartmentSettings],

    mutationFn: (data: { name: string; isActive: boolean }) => {
      return httpClient.post<IDepartmentSetting>(
        api.adminUser.getDepartmentSettings,
        data
      );
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getDepartmentSettings],
      });
    },
  });
};

const deleteDepartmentSetting = async (id: string) => {
  return await httpClient.delete(api.adminUser.deleteDepartmentSetting(id));
};

export const useDeleteDepartmentSetting = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.adminUser.deleteDepartmentSetting],
    mutationFn: deleteDepartmentSetting,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getDepartmentSettings],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

export const useUpdateDepartmentSettingStatus = (
  id: string,
  status: boolean
) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["patch" + api.adminUser.updateDepartmentStatus(id, status)],

    mutationFn: () => {
      return httpClient.patch(api.adminUser.updateDepartmentStatus(id, status));
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getDepartmentSettings],
      });
    },
  });
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

export const useUpdateDepartmentSetting = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.getDepartmentSettings],
    mutationFn: updateDepartmentSetting,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getDepartmentSettings],
      });
    },
  });
};
//end

export const useZoomSetting = () => {
  return useQuery({
    queryKey: [api.adminUser.getZoomSettings],

    queryFn: () => {
      return httpClient.get<IZoomSetting>(api.adminUser.getZoomSettings);
    },

    staleTime: Infinity,
    cacheTime: Infinity,
  });
};

export const useUpdateZoomSetting = (id: string | undefined) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.getZoomSettings],

    mutationFn: (data: {
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

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getZoomSettings],
      });
    },
  });
};

export const useSMTPSetting = () => {
  return useQuery({
    queryKey: [api.adminUser.getSMTPSettings],

    queryFn: () => {
      return httpClient.get<ISMTPSetting>(api.adminUser.getSMTPSettings);
    },

    staleTime: Infinity,
    cacheTime: Infinity,
  });
};

export const useUpdateSMTPSetting = (id: string | undefined) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.getSMTPSettings],

    mutationFn: (data: ISMTPSettingUpdate) => {
      return httpClient.put<ISMTPSetting>(
        api.adminUser.updateSMTPSettings(id),
        data
      );
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getZoomSettings],
      });
    },
  });
};

// General Settings
export const useGeneralSetting = () => {
  return useQuery({
    queryKey: [api.adminUser.getGeneralSettings],

    queryFn: () => {
      return httpClient.get<IGeneralSetting>(api.adminUser.getGeneralSettings);
    },

    staleTime: Infinity,
    cacheTime: Infinity,
  });
};
// company settings
export const useCompanySetting = () => {
  return useQuery({
    queryKey: [api.adminUser.getCompanySettings],

    queryFn: () => {
      return httpClient.get<ICompanySetting>(api.adminUser.getCompanySettings);
    },
  });
};

export const useUpdateGeneralSetting = (id: string | undefined) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.getGeneralSettings],

    mutationFn: (data: IGeneralSettingUpdate) => {
      return httpClient.put(api.adminUser.updateGeneralSettings(id), data);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getGeneralSettings],
      });
    },
  });
};

//Zoom License
export const useZoomLicense = () => {
  return useQuery({
    queryKey: [api.adminUser.getZoomLicense],

    queryFn: () => {
      return httpClient.get<IPaginated<IZoomLicense<IUser>>>(
        api.adminUser.getZoomLicense
      );
    },

    staleTime: Infinity,
    cacheTime: Infinity,
  });
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
  return useQuery({
    queryKey: [
      "active" + api.adminUser.getZoomLicense,
      startDateTime,
      duration,
    ],
    queryFn: () =>
      getActiveZoomLicense(startDateTime, duration, lessonIdentity),
    select: (data) => data,
    enabled: !!startDateTime && !!duration,
  });
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
  return useMutation({
    mutationKey: [api.adminUser.deleteZoomLicense],
    mutationFn: deleteZoomLicense,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getZoomLicense],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

const updateZoomLicense = ({ id, data }: { id: string; data: any }) =>
  httpClient.put(api.adminUser.deleteZoomLicense(id), data);

export const useUpdateZoomLicense = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.getZoomLicense],
    mutationFn: updateZoomLicense,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getZoomLicense],
      });
    },
  });
};

const addZoomLicense = (data: {
  licenseEmail: string;
  hostId: string;
  capacity: number;
}) => httpClient.post(api.adminUser.addZoomLicense, data);
export const useAddZoomLicense = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.adminUser.addZoomLicense],
    mutationFn: addZoomLicense,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getZoomLicense],
      });
    },

    onError: () => {},
  });
};

//file storage
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
  return useQuery({
    queryKey: [api.fileStorage.getFileStorageSetting],
    queryFn: () => getFileStorageSetting(),

    select: (data) => {
      return data.data;
    },

    // retry: 0,
    // refetchOnMount: false,
    // refetchOnWindowFocus: false,
  });
};

//put file storage

const updateFileStorage = async (data: IFileStorage[]) =>
  await httpClient.put<IFileStorage>(
    api.fileStorage.updateFileStorageSetting,
    data.filter((x) => x.isActive)[0]
  );

export const useUpdateFileStorage = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.fileStorage.updateFileStorageSetting],
    mutationFn: updateFileStorage,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.fileStorage.getFileStorageSetting],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

// ------------Resend Email-------------------
const resendEmail = async (id: string) =>
  await httpClient.patch(api.auth.resendEmail(id));
export const useResendEmail = (id: string) =>
  useMutation({
    mutationKey: [api.auth.resendEmail(id)],
    mutationFn: resendEmail,
  });

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
  useQuery({
    queryKey: [api.adminUser.getTrainer(search, lessonType, id)],
    queryFn: () => getTrainers(search, lessonType, id),
    enabled: true,
    select: (data) => data.data,
  });

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
  return useQuery({
    queryKey: ["logs" + query],
    queryFn: () => getLogs(query),

    //enabled: !!startDateTime && !!duration,
    select: (data) => data.data,
  });
};

const getSingleLog = async (id: string) => {
  return httpClient.get<IServerLogs>(api.adminUser.getSingleLog(id));
};

export const useGetSingleLog = (id: string) => {
  return useQuery({
    queryKey: ["log" + id],
    queryFn: () => getSingleLog(id),
    select: (data) => data.data,
  });
};

// -----------------Mail Notification-------------------

const getMailNotification = async (search: string) =>
  httpClient.get<IPaginated<IMailNotification>>(
    api.adminUser.getMailNotification + `?${search}`
  );

export const useMailNotification = (search: string) => {
  return useQuery({
    queryKey: [api.adminUser.getMailNotification, search],
    queryFn: () => getMailNotification(search),
    staleTime: Infinity,
    cacheTime: Infinity,
    select: (data) => data.data,
  });
};

type IPostMailNotification = Omit<IMailNotification, "id">;
const updateMailNotificationDetail = ({
  id,
  data,
}: {
  id: string;
  data: IPostMailNotification;
}) => httpClient.put(api.adminUser.updateMailNotification(id), data);

export const useUpdateMailNotification = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.adminUser.getMailNotification],
    mutationFn: updateMailNotificationDetail,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getMailNotification],
      });
    },
  });
};

export const usePostMailNotification = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["post" + api.adminUser.getMailNotification],

    mutationFn: (data: IPostMailNotification) => {
      return httpClient.post<IMailNotification>(
        api.adminUser.getMailNotification,
        data
      );
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getMailNotification],
      });
    },
  });
};

const getMailPreview = async (id: string) =>
  httpClient.get<string>(api.adminUser.updateMailNotification(id));

export const useMailPreview = (id: string) => {
  return useQuery({
    queryKey: [api.adminUser.updateMailNotification(id)],
    queryFn: () => getMailPreview(id),
    staleTime: Infinity,
    cacheTime: Infinity,
    select: (data) => data.data,
    enabled: false,
  });
};

// interface ITestEmail {
//   emailAddress: string;
// }
const testEmail = async ({ id, data }: { id: string; data: any }) =>
  httpClient.patch(api.adminUser.testEmail(id), data);

export const useTestEmail = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["test" + api.adminUser.testEmail],
    mutationFn: testEmail,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.testEmail],
      });
    },
  });
};

const deleteMailNotification = async (id: string) =>
  httpClient.delete(api.adminUser.updateMailNotification(id));

export const useDeleteMailNotification = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.adminUser.getMailNotification],
    mutationFn: deleteMailNotification,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getMailNotification],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

export const useInitialSetup = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.adminUser.initialSetup],

    mutationFn: (data: ISetupInitial) => {
      return httpClient.post(api.adminUser.initialSetup, data);
    },
    onSuccess: () => {
      queryClient.refetchQueries({
        queryKey: [api.adminUser.getCompanySettings],
      });
    },
  });
};
