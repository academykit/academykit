import LicenseForm from "@components/LicenseForm";
import useAuth from "@hooks/useAuth";
import { Modal } from "@mantine/core";
import { LICENSE_KEY } from "@utils/constants";
import {
  getLicenses,
  useValidateLicense,
} from "@utils/services/licenseService";
import type { FC, PropsWithChildren } from "react";
import { createContext, useEffect, useState } from "react";

interface ILicenseContext {
  isValid: boolean;
  setValid: (value: boolean) => void;
}

export const LicenseContext = createContext<ILicenseContext | null>(null);

const LicenseProvider: FC<PropsWithChildren> = ({ children }) => {
  const [isValid, setIsValid] = useState<boolean>(true);
  const auth = useAuth();
  const setValid = (value: boolean) => {
    setIsValid(value);
  };
  const licenseKeys = getLicenses();

  const licenseToken = localStorage.getItem(LICENSE_KEY);

  const license = useValidateLicense(licenseToken ?? "");

  useEffect(() => {
    if (licenseKeys.isSuccess) {
      if (licenseKeys?.data?.licenseKey) {
        localStorage.setItem(LICENSE_KEY, licenseKeys.data.licenseKey);
      } else {
        setIsValid(false);
      }
    }
  }, [licenseToken, licenseKeys.isSuccess]);

  useEffect(() => {
    if (license.isSuccess) {
      if (license?.data?.valid) {
        setIsValid(true);
      } else {
        setIsValid(false);
      }
    } else if (license.isError) {
      setIsValid(false);
      localStorage.removeItem(LICENSE_KEY);
    }
  }, [auth?.loggedIn, license]);

  return (
    <LicenseContext.Provider value={{ isValid, setValid }}>
      {auth?.loggedIn && (
        <Modal onClose={() => {}} opened={!isValid} withCloseButton={false}>
          <LicenseForm />
        </Modal>
      )}
      {children}
    </LicenseContext.Provider>
  );
};

export default LicenseProvider;
