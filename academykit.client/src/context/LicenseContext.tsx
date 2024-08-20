import LicenseForm from "@components/LicenseForm";
import useAuth from "@hooks/useAuth";
import { Modal } from "@mantine/core";
import { useValidateLicense } from "@utils/services/licenseService";
import { FC, createContext, useEffect, useState } from "react";

interface ILicenseContext {
  isValid: boolean;
  setValid: () => void;
}

export const LicenseContext = createContext<ILicenseContext | null>(null);

const LicenseProvider: FC<React.PropsWithChildren> = ({ children }) => {
  const [isValid, setIsValid] = useState<boolean>(true);
  const auth = useAuth();
  const setValid = () => {
    setIsValid((isValid) => !isValid);
  };
  const license = useValidateLicense("D0D13409-8BE7-4190-8FA9-1981D43738F");
  console.log("license", license.data);
  useEffect(() => {
    // if (localStorage.getItem("licenseKey")) {
    if (license.isSuccess) {
      if (license?.data?.valid) {
        setIsValid(true);
      } else {
        setIsValid(false);
      }
    }
    // } else {
    //   setIsValid(false);
    // }
  }, [auth?.loggedIn, license]);
  console.log("license error", license.error);
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
