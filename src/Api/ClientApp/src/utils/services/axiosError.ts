import axios, { AxiosError } from "axios";
import i18next from "i18next";

export interface ServerError {
  message: string;
  success: boolean;
}

const errorType = (err: any) => {
  if (axios.isAxiosError(err)) {
    const error = err as AxiosError<ServerError>;
    return (
      error.response?.data?.message || (i18next.t("something_wrong") as string)
    );
  }
  return i18next.t("something_wrong");
};

// const newErrorType = (error) => {}

export default errorType;
