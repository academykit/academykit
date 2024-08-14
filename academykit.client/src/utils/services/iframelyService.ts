import { useQuery } from "@tanstack/react-query";
import { api } from "./service-api";
import { httpClient } from "./service-axios";

const getIframelyOembed = async (url: string) => {
  return await httpClient.get(api.iframely.oembed(url));
};
export const useGetIframelyOembed = (url: string) => {
  return useQuery({
    queryKey: [api.iframely.oembed(url), url],
    queryFn: () => getIframelyOembed(url),

    select: (data) => {
      return data.data;
    },
  });
};
