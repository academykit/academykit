import UnAuthorize from "@pages/401";
import Forbidden from "@pages/403";
import NotFound from "@pages/404";
import ServerError from "@pages/500";
import { isDevelopment, isProduction } from "@utils/env";
import axios, { AxiosError } from "axios";
import { Component, ErrorInfo, ReactNode } from "react";

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
  } else if (statusCode === 403) {
    return <Forbidden />;
  } else if (statusCode === 404) {
    return <NotFound />;
  } else if ((statusCode as number) >= 500) {
    return <ServerError />;
  } else {
    return <></>;
  }
};

class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
  };

  public static getDerivedStateFromError(_: Error): State {
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
            <h2>Something went wrong.</h2>
            <details style={{ whiteSpace: "pre-wrap" }}>
              {this.state.error && this.state.error.toString()}
              <br />
              {this.state.errorInfo?.componentStack}
            </details>
          </div>
          <RenderErrorComponent statusCode={this.state.errorCode as number} />
        </>
      );
    } else if (this.state.hasError && isProduction) {
      return (
        <RenderErrorComponent statusCode={this.state.errorCode as number} />
      );
    }
    return this.props.children;
  }
}

export default ErrorBoundary;
