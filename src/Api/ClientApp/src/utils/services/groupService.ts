import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import errorType from './axiosError';
import { ICourse } from './courseService';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IPaginated, IUser, IUserProfile } from './types';

export interface IGroup {
  id: string;
  slug: string;
  name: string;
  isActive: boolean;
  user: IUserProfile;
  courseCount: number;
  memberCount: number;
  attachmentCount: number;
}

const getGroupDetail = (id: string) =>
  httpClient.get<IGroup>(api.groups.details(id));
export const useGetGroupDetail = (id: string) =>
  useQuery([api.groups.details(id)], () => getGroupDetail(id), {
    retry: (count: number, error: AxiosError) => {
      if (error?.response?.status === 403) {
        return false;
      }
      if (count === 3) {
        return false;
      }
      return true;
    },
  });

const updateGroupDetail = (data: {
  id: string;
  name: string;
  isActive: boolean;
}) => httpClient.put(api.groups.details(data.id), data);
export const useUpdateGroup = (id: string) => {
  const queryClient = useQueryClient();

  return useMutation([api.groups.details(id)], updateGroupDetail, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.groups.details(id)]);
    },
  });
};

const getGroups = (query: string) =>
  httpClient.get<IPaginated<IGroup>>(api.groups.list + `?${query}`);

export const useGroups = (query: string) =>
  useQuery([api.groups.list, query], () => getGroups(query));

const addGroup = async (name: string) =>
  await httpClient.post<IGroup>(api.groups.list, {
    name,
  });

export const useAddGroup = () => {
  const queryClient = useQueryClient();
  return useMutation([api.groups.list], addGroup, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.groups.list]);
    },
    onError: (err) => {
      return errorType(err);
    },
  });
};

export interface IGroupMember {
  id: string;
  groupId: string;
  groupName: string;
  isActive: boolean;
  user: IUser;
}

const getGroupMember = (id: string, query: string) =>
  httpClient.get<IPaginated<IGroupMember>>(api.groups.member(id, query));

export const useGroupMember = (id: string, query: string) =>
  useQuery([api.groups.member(id, query)], () => getGroupMember(id, query), {
    select: (data) => data.data,
    retry: false,
  });

const addMember = ({ id, data }: { id: string; data: any }) => {
  return httpClient.post(api.groups.addMember(id), data);
};
export const useAddGroupMember = (id: string, query: string) => {
  const queryClient = useQueryClient();
  return useMutation([api.groups.addMember(id)], addMember, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.groups.member(id, query)]);
    },
    onError: (err) => {
      return errorType(err);
    },
  });
};

const removeGroupMember = ({
  id,
  memberId,
}: {
  id: string;
  memberId: string;
}) => httpClient.delete(api.groups.removeMember(id, memberId));

export const useRemoveGroupMember = (
  id: string,
  query: string,
  memberId: string
) => {
  const queryClient = useQueryClient();
  return useMutation(
    [api.groups.removeMember(id, memberId)],
    removeGroupMember,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.groups.member(id, query)]);
      },
      onError: (err) => {
        return errorType(err);
      },
    }
  );
};

const deleteGroup = ({ id }: { id: string }) =>
  httpClient.delete(api.groups.details(id));
export const useDeleteGroup = (id: string, query: string) => {
  const queryClient = useQueryClient();
  return useMutation(['delete', api.groups.details(id)], deleteGroup, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.groups.list, query]);
    },
  });
};

const getGroupCourse = (id: string, search: string) => {
  return httpClient.get<IPaginated<ICourse>>(
    api.groups.course(id) + `?${search}`
  );
};

export const useGroupCourse = (id: string, search: string) => {
  return useQuery(
    [api.groups.course(id) + `?${search}`],
    () => getGroupCourse(id, search),
    {
      select: (data) => data.data,
      retry: false,
    }
  );
};

//get group attachment
export interface IGroupAttachmentItems {
  id: string;
  groupId: string;
  name: string;
  mimeType: string;
  url: string;
  createdOn: string;
  updatedOn: string;
}
export interface IGroupAttachment {
  items: IGroupAttachmentItems[];
}
const getGroupAttachment = (groupId: string, search: string) => {
  return httpClient.get<IPaginated<IGroupAttachment>>(
    api.groups.attachment + `?GroupIdentity=${groupId}` + `&${search}`
  );
};

export const useGroupAttachment = (groupId: string, search: string) => {
  return useQuery(
    [api.groups.attachment + search],
    () => getGroupAttachment(groupId, search),
    {
      select: (data) => data.data,
      retry: false,
    }
  );
};
interface IGroupType {
  id: string;
  groupId: string;
  name: string;
  mimeType: string;
  url: string;
  createdOn: string;
  updatedOn: string;
}

//post group attachment
const addGroupAttachement = ({
  groupIdentity,
  file,
}: {
  groupIdentity: string;
  file: File;
}) => {
  return httpClient.post<IGroupType>(
    api.groups.addAttachment,
    {
      groupIdentity,
      file,
    },
    {
      headers: {
        'content-type': 'multipart/form-data',
      },
    }
  );
};
export const useAddGroupAttachment = (search: string) => {
  const queryClient = useQueryClient();
  return useMutation([api.groups.addAttachment], addGroupAttachement, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.groups.attachment + search]);
    },
    onError: (err) => {
      return errorType(err);
    },
  });
};
// remove group attachment
const removeGroupAttachment = ({
  id,
  fileId,
}: {
  id: string;
  fileId: string;
}) => httpClient.delete(api.groups.removeAttachment(id, fileId));

export const useRemoveGroupAttachment = (
  id: string,
  fileId: string,
  search: string
) => {
  const queryClient = useQueryClient();
  return useMutation(
    [api.groups.removeAttachment(id, fileId)],
    removeGroupAttachment,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.groups.attachment + search]);
      },
      onError: (err) => {
        return errorType(err);
      },
    }
  );
};

//group notmember list
export interface INotMember {
  id: string;
  fullName: string;
  imageUrl: string;
  email: string;
  mobileNumber: string;
  label: string;
  value: string;
}
const getGroupNotMember = (
  id: string,
  query: string,
  departmentId: string | undefined
) =>
  httpClient.get<IPaginated<INotMember>>(
    api.groups.notMembers(id, query, departmentId)
  );

export const useGroupNotMember = (
  id: string,
  query: string,
  departmentId: string | undefined
) =>
  useQuery(
    [api.groups.notMembers(id, query, departmentId)],
    () => getGroupNotMember(id, query, departmentId),
    {
      select: (data) => data.data,
    }
  );
