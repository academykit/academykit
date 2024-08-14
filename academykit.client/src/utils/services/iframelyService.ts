import { api } from "./service-api";
import { httpClient } from "./service-axios";

type IFramely = {
  html: string;
  error: number;
  message: string;
};

export const getIframelyOembed = async (url: string) => {
  return await httpClient.get<IFramely>(api.iframely.oembed(url));
};
