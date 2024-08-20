import { useMutation, useQuery } from "@tanstack/react-query";
import { License_Key } from "@utils/constants";
import { api } from "./service-api";
import { httpClient } from "./service-axios";

export interface ILicense {
  activated: string;
  valid: string;
  licenseKey: string;
}

const licenseValidation = (licenseKey: string) =>
  httpClient.get<ILicense>(api.license.validate(licenseKey));

export const useValidateLicense = (licenseKey: string) =>
  useQuery({
    queryKey: [api.license.validate(licenseKey)],
    queryFn: () => licenseValidation(licenseKey),
    select: (data) => data.data,
  });

const getLicense = () => httpClient.get<ILicense[]>(api.license.list);

export const getLicenses = () =>
  useQuery({
    queryKey: [api.license.list],
    queryFn: () => getLicense(),
    select: (data) => data.data,
  });

export const useActivatelicense = () => {
  return useMutation({
    mutationKey: [api.license.activate],

    mutationFn: ({ licenseKey }: { licenseKey: string }) => {
      return httpClient.post<ILicense>(api.license.activate, { licenseKey });
    },

    onSuccess: (data) => {
      localStorage.setItem(License_Key, data?.data?.licenseKey);
    },
  });
};
