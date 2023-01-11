import axios, { AxiosError } from "axios";
export interface ServerError {
  message: string;
  success: boolean;
}

const errorType = (err: any) => {
  if (axios.isAxiosError(err)) {
    const error = err as AxiosError<ServerError>;
    return error.response?.data?.message || "Something went wrong.";
  }
  return "Something went wrong.";
};

// const newErrorType = (error) => {}

export default errorType;
