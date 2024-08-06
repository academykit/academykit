import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import errorType from "./axiosError";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IPaginated } from "./types";

export interface ITag {
  id: string;
  slug: string;
  name: string;
  isActive: boolean;
  tagName?: string;
}

const getTags = async (
  search: string,
  trainingType?: number,
  identity?: string
) =>
  await httpClient.get<IPaginated<ITag>>(
    api.tags.list +
      `?${search}${identity ? `Idenitiy=${identity}` : ""}${
        trainingType ? `&TrainingType=${trainingType}` : ""
      }`
  );

export const useTags = (
  search: string,
  identity?: string,
  trainingType?: number
) =>
  useQuery({
    queryKey: [api.tags.list, search],
    queryFn: () => getTags(search, trainingType, identity),

    // keepPreviousData: true,
    select: (data) => data.data,
  });

const addTag = async (tagName: string) =>
  await httpClient.post<ITag>(api.tags.list, { name: tagName });

export const useAddTag = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.tags.list],
    mutationFn: addTag,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.tags.list],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};
