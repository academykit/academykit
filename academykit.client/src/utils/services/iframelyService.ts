import { api } from "./service-api";
import { httpClient } from "./service-axios";

type IFramely = {
  html: string;
  error: number;
  message: string;
};

export const getIframelyOEmbed = async (url: string) => {
  return await httpClient.get<IFramely>(api.iframely.oEmbed(url));
};
