import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import errorType from "./axiosError";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IPaginated, IUser } from "./types";

export interface ILevel {
  id: string;
  slug: string;
  name: string;
  user: IUser;
}

const getLevels = async () => await httpClient.get<ILevel[]>(api.levels.list);

export const useLevels = () =>
  useQuery([api.levels.list], () => getLevels(), {
    select: (data) => data.data,
  });

const addLevel = async (tagName: string) =>
  await httpClient.post(api.levels.list, { name: tagName });

export const useAddLevel = () => {
  const queryClient = useQueryClient();
  return useMutation([api.levels.list], addLevel, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.levels.list]);
    },
    onError: (err) => {
      return errorType(err);
    },
  });
};
