import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import errorType from './axiosError';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IPaginated, IUser } from './types';

export interface IPoolTeacher {
  id: string;
  questionPoolId: string;
  questionPoolName: string;
  role: number;
  user: IUser;
}
const getPoolsTeacher = (q_id: string) => {
  return httpClient.get<IPaginated<IPoolTeacher>>(api.poolTeacher.get(q_id));
};
export const usePoolsTeacher = (q_id: string) => {
  return useQuery({
    queryKey: [api.poolTeacher.get(q_id)],
    queryFn: () => getPoolsTeacher(q_id),
    select: (data) => data.data,
  });
};

export interface ICreateCourseTeacher {
  courseIdentity: string;
  email: string;
  id: string;
  user?: IUser;
  courseName: string;
}
const createTeacherPool = async (data: {
  questionPoolIdentity: string;
  email: string;
}) => {
  await httpClient.post(api.poolTeacher.list, {
    email: data.email,
    questionPoolIdentity: data.questionPoolIdentity,
  });
};
export const useCreateTeacherPool = (id: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ['post' + api.poolTeacher.list],
    mutationFn: createTeacherPool,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.poolTeacher.get(id)],
      });
    },
  });
};

const deletePoolTeacher = async (id: string) => {
  return await httpClient.delete(api.poolTeacher.detail(id));
};
export const useDeletePoolTeacher = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['delete' + api.poolTeacher.detail],
    mutationFn: deletePoolTeacher,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.poolTeacher.get(id)],
      });
    },
  });
};

export interface IPool {
  id: string;
  slug: string;
  name: string;
  isDeleted: boolean;
  questionCount: 0;
  user: IUser;
}

const getPools = (search: string) =>
  httpClient.get<IPaginated<IPool>>(api.pool.list + `?${search}`);

export const usePools = (search: string) =>
  useQuery({
    queryKey: [api.pool.list, search],
    queryFn: () => getPools(search),
    select: (data) => data.data,
  });

const addPool = async (name: string) =>
  await httpClient.post<IPool>(api.pool.list, { name });

export const useAddPool = (searchParams: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.pool.list, searchParams],
    mutationFn: addPool,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.pool.list, searchParams],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

const getOnePool = (poolId: string) =>
  httpClient.get<IPool>(api.pool.getOne(poolId));

export const useOnePool = (poolId: string) =>
  useQuery({
    queryKey: [api.pool.getOne(poolId)],
    queryFn: () => getOnePool(poolId),
    select: (data) => data.data,
  });

const addOnePool = async ({ name, poolId }: { name: string; poolId: string }) =>
  await httpClient.put<IPool>(api.pool.getOne(poolId), { name });

export const useAddOnePool = (poolId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.pool.getOne(poolId)],
    mutationFn: addOnePool,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.pool.getOne(poolId)],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

const deleteQuestionPool = (identity: string) => {
  return httpClient.delete(api.pool.getOne(identity));
};
export const useDeleteQuestionPool = (
  identity: string,
  searchParams: string
) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['delete' + api.pool.getOne(identity)],
    mutationFn: deleteQuestionPool,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.pool.list, searchParams],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

// export interface IPoolMember {
//   id: string;
//   groupId: string;
//   groupName: string;
//   isActive: boolean;
//   user: IUser;
// }

// const getGroupMember = (id: string, search: string) =>
//   httpClient.get<IPaginated<IGroupMember>>(api.groups.member(id));

// export const useGroupMember = (id: string, search: string) =>
//   useQuery([api.groups.member(id), search], () => getGroupMember(id, search), {
//     select: (data) => data.data,
//   });

// const addMember = ({ id, data }: { id: string; data: string[] }) =>
//   httpClient.post(api.groups.addMember(id), data);

// export const useAddGroupMember = (id: string, search: string) => {
//   const queryClient = useQueryClient();
//   return useMutation([api.groups.addMember(id)], addMember, {
//     onSuccess: (data) => {
//       queryClient.invalidateQueries([api.groups.member(id), search]);
//     },
//     onError: (err) => {
//       return errorType(err);
//     },
//   });
// };

// const removeGroupMember = ({
//   id,
//   memberId,
// }: {
//   id: string;
//   memberId: string;
// }) => httpClient.delete(api.groups.removeMember(id, memberId));

// export const useRemoveGroupMember = (
//   id: string,
//   search: string,
//   memberId: string
// ) => {
//   const queryClient = useQueryClient();
//   return useMutation(
//     [api.groups.removeMember(id, memberId)],
//     removeGroupMember,
//     {
//       onSuccess: (data) => {
//         queryClient.invalidateQueries([api.groups.member(id), search]);
//       },
//       onError: (err) => {
//         return errorType(err);
//       },
//     }
//   );
// };
