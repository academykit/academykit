import LicenseForm from "@components/LicenseForm";
import { Modal } from "@mantine/core";
import { FC, createContext, useState } from "react";

interface ILicenseContext {
  isValid: boolean;
  setValid: () => void;
}

export const LicenseContext = createContext<ILicenseContext | null>(null);

const LicenseProvider: FC<React.PropsWithChildren> = ({ children }) => {
  const [isValid, setIsValid] = useState<boolean>(false);
  const setValid = () => {
    setIsValid((isValid) => !isValid);
  };
  return (
    <LicenseContext.Provider value={{ isValid, setValid }}>
      <Modal onClose={() => {}} opened={!isValid} withCloseButton={false}>
        <LicenseForm />
      </Modal>
      {children}
    </LicenseContext.Provider>
  );
};

export default LicenseProvider;
