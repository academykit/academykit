import { useQuery } from "@tanstack/react-query";
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

const licenseActivate = (licenseKey: string) =>
  httpClient.get<ILicense>(api.license.validate(licenseKey));

export const useActivateLicense = (licenseKey: string) =>
  useQuery({
    queryKey: [api.license.activate(licenseKey)],
    queryFn: () => licenseActivate(licenseKey),
    select: (data) => {
      if (data.data.activated) {
        localStorage.setItem(License_Key, data?.data?.licenseKey);
      }
    },
  });
