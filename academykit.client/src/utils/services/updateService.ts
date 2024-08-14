import { useQuery } from "@tanstack/react-query";
import { api } from "./service-api";
import { httpClient } from "./service-axios";

export interface IImageVersions {
  current: string;
  latest: string;
  available: boolean;
  releaseNotesUrl: string;
}

export const useImageVersions = () => {
  return useQuery({
    queryKey: [api.update.checkVersions],
    queryFn: () => {
      return httpClient.get<IImageVersions>(api.update.checkVersions);
    },
    enabled: false,
    refetchInterval: false,
    refetchOnMount: false,
    refetchOnReconnect: false,
    refetchOnWindowFocus: false,
    select: (data) => data.data,
  });
};
