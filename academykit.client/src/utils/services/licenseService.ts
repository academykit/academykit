import { useMutation, useQuery } from "@tanstack/react-query";
import { LICENSE_KEY, TOKEN_STORAGE } from "@utils/constants";
import { api } from "./service-api";
import { httpClient } from "./service-axios";

export interface ILicense {
  activated: string;
  valid: string;
  licenseKey: string;
  checkoutUrl: string;
}

const licenseValidation = (licenseKey: string) =>
  httpClient.get<ILicense>(api.license.validate(licenseKey));

export const useValidateLicense = (licenseKey: string) =>
  useQuery({
    queryKey: [api.license.validate(licenseKey)],
    queryFn: () => licenseValidation(licenseKey),
    select: (data) => data.data,
    enabled: !!localStorage.getItem(TOKEN_STORAGE),
  });

const licenseCheckout = () => httpClient.get<ILicense>(api.license.checkout);

export const useCheckoutLicense = (isCheckout: boolean) =>
  useQuery({
    queryKey: [api.license.checkout],
    queryFn: () => licenseCheckout(),
    select: (data) => data.data,
    enabled: isCheckout,
  });

const getLicense = () => httpClient.get<ILicense[]>(api.license.list);

export const getLicenses = () =>
  useQuery({
    queryKey: [api.license.list],
    queryFn: () => getLicense(),
    select: (data) => data.data,
    enabled: !!(
      localStorage.getItem(TOKEN_STORAGE) && localStorage.getItem(LICENSE_KEY)
    ),
  });

export const useActivatelicense = () => {
  return useMutation({
    mutationKey: [api.license.activate],

    mutationFn: ({ licenseKey }: { licenseKey: string }) => {
      return httpClient.post<ILicense>(api.license.activate, { licenseKey });
    },

    onSuccess: (data) => {
      localStorage.setItem(LICENSE_KEY, data?.data?.licenseKey);
    },
  });
};
