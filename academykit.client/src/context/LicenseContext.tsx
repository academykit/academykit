import LicenseForm from "@components/LicenseForm";
import useAuth from "@hooks/useAuth";
import { Modal } from "@mantine/core";
import { License_Key } from "@utils/constants";
import {
  getLicenses,
  useValidateLicense,
} from "@utils/services/licenseService";
import { FC, createContext, useEffect, useState } from "react";

interface ILicenseContext {
  isValid: boolean;
  setValid: (value: boolean) => void;
}

export const LicenseContext = createContext<ILicenseContext | null>(null);

const LicenseProvider: FC<React.PropsWithChildren> = ({ children }) => {
  const [isValid, setIsValid] = useState<boolean>(true);
  const [licenseKey, setLicenseKey] = useState("");
  const auth = useAuth();
  const setValid = (value: boolean) => {
    setIsValid(value);
  };
  const licenseKeys = getLicenses();

  const licenseToken = localStorage.getItem(License_Key);

  const license = useValidateLicense(licenseToken ?? "");

  useEffect(() => {
    if (licenseToken) {
      setLicenseKey(licenseKey);
    } else {
      if (licenseKeys.isSuccess) {
        if (licenseKeys.data?.length) {
          localStorage.setItem(License_Key, licenseKeys.data[0]?.licenseKey);
        }
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
