import { useEffect, useState } from "react";

const KEY = "318409658a86c3e837fda2c4e616e978";

declare global {
  interface Window {
    iframely: {
      load: () => void;
    };
  }
}

type Props = {
  url: string;
};

type ErrorCodes = {
  code: number;
  message: string;
};

export default function ExternalLinkViewer(props: Props) {
  const [error, setError] = useState<ErrorCodes | null>(null);
  const [isLoaded, setIsLoaded] = useState(false);
  const [html, setHtml] = useState({
    __html: "<div />",
  });

  useEffect(() => {
    if (props && props.url) {
      fetch(
        `https://cdn.iframe.ly/api/oembed?url=${encodeURIComponent(props.url)}&key=${KEY}&iframe=1&omit_script=1`
      )
        .then((res) => res.json())
        .then(
          (res) => {
            setIsLoaded(true);
            if (res.html) {
              setHtml({ __html: res.html });
            } else if (res.error) {
              setError({ code: res.error, message: res.message });
            }
          },
          (error) => {
            setIsLoaded(true);
            setError(error);
          }
        );
    } else {
      setError({ code: 400, message: "Provide url attribute for the element" });
    }
  }, []);

  useEffect(() => {
    window?.iframely && window.iframely.load();
  }, [isLoaded]);

  if (error) {
    return (
      <div>
        Error: {error.code} - {error.message}
      </div>
    );
  } else if (!isLoaded) {
    return <div>Loading…</div>;
  } else {
    return <div dangerouslySetInnerHTML={html} />;
  }
}
