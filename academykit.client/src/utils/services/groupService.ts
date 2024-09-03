import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AxiosError } from "axios";
import errorType from "./axiosError";
import { ICourse } from "./courseService";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IPaginated, IUser, IUserProfile } from "./types";

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
  useQuery({
    queryKey: [api.groups.details(id)],
    queryFn: () => getGroupDetail(id),

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

  return useMutation({
    mutationKey: [api.groups.details(id)],
    mutationFn: updateGroupDetail,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.groups.details(id)],
      });
    },
  });
};

const getGroups = (query: string) =>
  httpClient.get<IPaginated<IGroup>>(api.groups.list + `?${query}`);

export const useGroups = (query: string) =>
  useQuery({
    queryKey: [api.groups.list, query],
    queryFn: () => getGroups(query),
  });

const addGroup = async (name: string) =>
  await httpClient.post<IGroup>(api.groups.list, {
    name,
  });

export const useAddGroup = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.groups.list],
    mutationFn: addGroup,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.groups.list],
      });
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
  useQuery({
    queryKey: [api.groups.member(id, query)],
    queryFn: () => getGroupMember(id, query),
    select: (data) => data.data,
    retry: false,
  });

const addMember = ({ id, data }: { id: string; data: any }) => {
  return httpClient.post(api.groups.addMember(id), data);
};
export const useAddGroupMember = (id: string, query: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.groups.addMember(id)],
    mutationFn: addMember,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.groups.member(id, query)],
      });
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
  return useMutation({
    mutationKey: [api.groups.removeMember(id, memberId)],
    mutationFn: removeGroupMember,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.groups.member(id, query)],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

const deleteGroup = ({ id }: { id: string }) =>
  httpClient.delete(api.groups.details(id));
export const useDeleteGroup = (id: string, query: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["delete", api.groups.details(id)],
    mutationFn: deleteGroup,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.groups.list, query],
      });
    },
  });
};

const getGroupCourse = (id: string, search: string) => {
  return httpClient.get<IPaginated<ICourse>>(
    api.groups.course(id) + `?${search}`
  );
};

export const useGroupCourse = (id: string, search: string) => {
  return useQuery({
    queryKey: [api.groups.course(id) + `?${search}`],
    queryFn: () => getGroupCourse(id, search),
    select: (data) => data.data,
    retry: false,
  });
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
  return useQuery({
    queryKey: [api.groups.attachment + search],
    queryFn: () => getGroupAttachment(groupId, search),
    select: (data) => data.data,
    retry: false,
  });
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
const addGroupAttachment = ({
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
        "content-type": "multipart/form-data",
      },
    }
  );
};
export const useAddGroupAttachment = (search: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.groups.addAttachment],
    mutationFn: addGroupAttachment,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.groups.attachment + search],
      });
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
  return useMutation({
    mutationKey: [api.groups.removeAttachment(id, fileId)],
    mutationFn: removeGroupAttachment,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.groups.attachment + search],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

//group not member list
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
  useQuery({
    queryKey: [api.groups.notMembers(id, query, departmentId)],
    queryFn: () => getGroupNotMember(id, query, departmentId),
    select: (data) => data.data,
  });
