import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { ICertificateList } from './manageCourseService';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IPaginated, IUser } from './types';

export interface ExternalCertificatePost {
  name: string;
  startDate: string;
  endDate: string;
  imageUrl: string;
  location: string;
  institute: string;
  duration: number;
  status: CertificateStatus;
  cost: number;
}

export enum CertificateStatus {
  Draft = 1,
  Approved = 2,
  Rejected = 3,
}

export interface GetExternalCertificate extends ExternalCertificatePost {
  id: string;

  isVerified: boolean;
  user: IUser;
}

const getExternalCertificate = () => {
  return httpClient.get<GetExternalCertificate[]>(api.externalCertificate.add);
};

export const useGetExternalCertificate = (isEnabled: boolean) => {
  return useQuery(
    ['certificate', api.externalCertificate.add],

    () => getExternalCertificate(),
    {
      select: (data) => data.data,
      enabled: isEnabled,
    }
  );
};

const addCertificate = (data: ExternalCertificatePost) => {
  return httpClient.post(api.externalCertificate.add, data);
};

export const useAddCertificate = () => {
  const queryClient = useQueryClient();
  return useMutation(['post' + api.externalCertificate.add], addCertificate, {
    onSuccess: () => {
      queryClient.invalidateQueries([
        'certificate',
        api.externalCertificate.add,
      ]);
    },
  });
};

const updateCertificate = ({
  data,
  id,
}: {
  data: ExternalCertificatePost;
  id: string;
}) => {
  return httpClient.put(api.externalCertificate.update(id), data);
};

export const useUpdateCertificate = () => {
  const queryClient = useQueryClient();
  return useMutation(
    ['update' + api.externalCertificate.add],
    updateCertificate,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([
          'certificate',
          api.externalCertificate.add,
        ]);
      },
    }
  );
};

const getUserCertificate = (id?: string) => {
  return httpClient.get<GetExternalCertificate[]>(
    api.externalCertificate.user(id)
  );
};

export const useGetUserCertificate = (id?: string) => {
  return useQuery(
    [api.externalCertificate.user(id)],
    () => getUserCertificate(id),
    {
      select: (data) => data.data,
      enabled: !!id,
    }
  );
};

export interface ListCertificate extends ExternalCertificatePost {
  user: IUser;
  id: string;
}

const getListCertificate = (query: string) =>
  httpClient.get<IPaginated<ListCertificate>>(
    api.externalCertificate.list + `?${query}`
  );

export const useGetListCertificate = (query: string) =>
  useQuery([api.externalCertificate.list, query], () =>
    getListCertificate(query)
  );

const updateCertificateStatus = ({
  id,
  status,
}: {
  id: string;
  status: CertificateStatus;
}) =>
  httpClient.patch(
    api.externalCertificate.updateStatus(id) + '?status=' + status
  );
export const useUpdateCertificateStatus = (id: string, search: string) => {
  const queryClient = useQueryClient();
  return useMutation(
    [api.externalCertificate.updateStatus(id)],
    updateCertificateStatus,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.externalCertificate.list, search]);
      },
    }
  );
};

const getInternalCertificate = () => {
  return httpClient.get<ICertificateList[]>(api.externalCertificate.internal);
};

export const useGetInternalCertificate = () => {
  return useQuery([api.externalCertificate.internal], () =>
    getInternalCertificate()
  );
};
