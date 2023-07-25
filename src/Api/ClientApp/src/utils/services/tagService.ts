import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import errorType from './axiosError';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IPaginated } from './types';

export interface ITag {
  id: string;
  slug: string;
  name: string;
  isActive: boolean;
  tagName?: string;
}

const getTags = async (search: string) =>
  await httpClient.get<IPaginated<ITag>>(api.tags.list + `?${search}`);

export const useTags = (search: string) =>
  useQuery([api.tags.list, search], () => getTags(search), {
    select: (data) => data.data,
    keepPreviousData: true,
  });

const addTag = async (tagName: string) =>
  await httpClient.post<ITag>(api.tags.list, { name: tagName });

export const useAddTag = () => {
  const queryClient = useQueryClient();
  return useMutation([api.tags.list], addTag, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.tags.list]);
    },
    onError: (err) => {
      return errorType(err);
    },
  });
};
