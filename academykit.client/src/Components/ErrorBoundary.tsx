import UnAuthorize from "@pages/401";
import Forbidden from "@pages/403";
import NotFound from "@pages/404";
import ServerError from "@pages/500";
import { isDevelopment, isProduction } from "@utils/env";
import axios, { type AxiosError } from "axios";
import i18next from "i18next";
import { Component, type ErrorInfo, type ReactNode } from "react";

interface Props {
  children?: ReactNode;
}

interface State {
  hasError: boolean;
  error?: Error;
  errorInfo?: ErrorInfo;
  errorCode?: number | undefined;
}

const RenderErrorComponent = ({ statusCode }: { statusCode: number }) => {
  if (statusCode === 401) {
    return <UnAuthorize />;
  }
  if (statusCode === 403) {
    return <Forbidden />;
  }
  if (statusCode === 404) {
    return <NotFound />;
  }
  if ((statusCode as number) >= 500) {
    return <ServerError />;
  }
  return <></>;
};

class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
  };

  public static getDerivedStateFromError(): State {
    // Update state so the next render will show the fallback UI.
    return { hasError: true };
  }

  public componentDidCatch(error: Error | AxiosError, errorInfo: ErrorInfo) {
    if (axios.isAxiosError(error)) {
      this.setState({
        errorCode: error.response?.status,
      });
    }
    this.setState({
      error: error,
      hasError: true,
      errorInfo,
    });
  }

  public render() {
    if (this.state.hasError && isDevelopment) {
      return (
        <>
          <div>
            <h2>{i18next.t("something_wrong")}</h2>
            <details style={{ whiteSpace: "pre-wrap" }}>
              {this.state.error?.toString()}
              <br />
              {this.state.errorInfo?.componentStack}
            </details>
          </div>
          <RenderErrorComponent statusCode={this.state.errorCode as number} />
        </>
      );
    }
    if (this.state.hasError && isProduction) {
      return (
        <RenderErrorComponent statusCode={this.state.errorCode as number} />
      );
    }
    return this.props.children;
  }
}

export default ErrorBoundary;
