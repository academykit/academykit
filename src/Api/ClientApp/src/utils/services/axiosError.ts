import axios, { AxiosError } from "axios";
import { useTranslation } from "react-i18next";
export interface ServerError {
  message: string;
  success: boolean;
}

const errorType = (err: any) => {
  const { t } = useTranslation();
  if (axios.isAxiosError(err)) {
    const error = err as AxiosError<ServerError>;
    return error.response?.data?.message || t("something_wrong");
  }
  return t("something_wrong");
};

// const newErrorType = (error) => {}

export default errorType;
