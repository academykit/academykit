import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import errorType from './axiosError';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IPaginated, IUser } from './types';

export interface ISkill {
  id: string;
  skillName: string;
  isActive: boolean;
  description: string;
  userModel: IUser[];
}

const getSkills = async (search: string) =>
  await httpClient.get<IPaginated<ISkill>>(api.skill.list + `?${search}`);

export const useSkills = (search: string) => {
  return useQuery({
    queryKey: [api.skill.list, search],
    queryFn: () => getSkills(search),
    staleTime: Infinity,
    cacheTime: Infinity,
    select: (data) => data.data,
  });
};

type IPostSkill = Omit<ISkill, 'id' | 'userModel'>;

export const usePostDepartmentSetting = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['post' + api.skill.list],

    mutationFn: (data: {
      skillName: string;
      isActive: boolean;
      description: string;
    }) => {
      return httpClient.post<IPostSkill>(api.skill.list, data);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.skill.list],
      });
    },
  });
};

const updateSkillDetail = ({ id, data }: { id: string; data: IPostSkill }) =>
  httpClient.put(api.skill.update(id), data);

export const useUpdateSkill = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['update' + api.skill.list],
    mutationFn: updateSkillDetail,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.skill.list],
      });
    },
  });
};

const deleteSkill = async (id: string) =>
  httpClient.delete(api.skill.update(id));

export const useDeleteSkill = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.skill.list],
    mutationFn: deleteSkill,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.skill.list],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};
