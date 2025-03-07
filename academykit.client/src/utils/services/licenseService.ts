import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { LICENSE_KEY, TOKEN_STORAGE } from "@utils/constants";
import { api } from "./service-api";
import { httpClient } from "./service-axios";

export interface ILicense {
  activated: string;
  valid: string;
  licenseKey: string;
  checkoutUrl: string;
  activatedOn: string;
  expiredOn: string;
  variantName: string;
  variantId: number;
}

const licenseValidation = () => httpClient.get<ILicense>(api.license.validate);

export const useValidateLicense = () =>
  useQuery({
    queryKey: [api.license.validate],
    queryFn: () => licenseValidation(),
    select: (data) => data.data,
    enabled: !!localStorage.getItem(TOKEN_STORAGE),
  });

const licenseCheckout = () => httpClient.get<ILicense>(api.license.checkout);

export const useCheckoutLicense = () => {
  return useMutation({
    mutationKey: [api.license.checkout],

    mutationFn: licenseCheckout,

    onSuccess: (data) => {
      window.open(data.data.checkoutUrl, "_blank");
    },
  });
};

const getLicense = () => httpClient.get<ILicense>(api.license.list);

export const getLicenses = (forceEnabled?: boolean) =>
  useQuery({
    queryKey: [api.license.list],
    queryFn: () => getLicense(),
    select: (data) => data.data,
    enabled:
      forceEnabled ??
      !!(
        localStorage.getItem(TOKEN_STORAGE) &&
        !localStorage.getItem(LICENSE_KEY)
      ),
  });

export const useActivateLicense = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.license.activate],

    mutationFn: ({ licenseKey }: { licenseKey: string }) => {
      return httpClient.post<ILicense>(api.license.activate, { licenseKey });
    },

    onSuccess: (data) => {
      queryClient.refetchQueries({ queryKey: [api.license.list] });
      localStorage.setItem(LICENSE_KEY, data?.data?.licenseKey);
    },
  });
};

export const useUpdateLicense = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.license.update],

    mutationFn: ({ licenseKey }: { licenseKey: string }) => {
      return httpClient.put<ILicense>(api.license.update, { licenseKey });
    },

    onSuccess: (data) => {
      queryClient.refetchQueries({ queryKey: [api.license.list] });
      localStorage.setItem(LICENSE_KEY, data?.data?.licenseKey);
    },
  });
};
