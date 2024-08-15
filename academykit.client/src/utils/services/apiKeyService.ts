import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IApiKey, IPaginated } from "./types";

export const useApiKeys = (search: string) => {
  return useQuery({
    queryKey: [api.apiKey.list, search],
    queryFn: () => {
      return httpClient.get<IPaginated<IApiKey>>(
        [api.apiKey.list, search].join("?")
      );
    },
    select: (data) => data.data,
  });
};

const createApiKey = async () => {
  return await httpClient.post<IApiKey>(api.apiKey.add, {});
};

export const useCreateApiKey = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createApiKey,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.apiKey.list],
      });
    },
  });
};

const deleteApiKey = async (id: string) => {
  return await httpClient.delete<IApiKey>(api.apiKey.delete(id), {});
};

export const useDeleteApiKey = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: deleteApiKey,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.apiKey.list],
      });
    },
  });
};
