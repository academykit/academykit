import { LicenseContext } from "@context/LicenseContext";
import { useContext } from "react";

const useLicenseValidation = () => {
  return useContext(LicenseContext);
};

export default useLicenseValidation;
