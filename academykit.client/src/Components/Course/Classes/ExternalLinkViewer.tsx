import { getIframelyOembed } from "@utils/services/iframelyService";
import { useEffect, useState } from "react";

type Props = {
  url: string;
};

type Error = {
  code: number;
  message: string;
};

export default function ExternalLinkViewer(props: Props) {
  const [error, setError] = useState<Error | null>(null);
  const [isLoaded, setIsLoaded] = useState(false);
  const [html, setHtml] = useState({
    __html: "<div />",
  });

  useEffect(() => {
    if (props && props.url) {
      getIframelyOembed(props.url).then(
        (response) => {
          const res = response.data;
          console.log(res);
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

  if (error) {
    return <div>Error: {error.message}</div>;
  } else if (!isLoaded) {
    return <div>Loading…</div>;
  } else {
    return <div dangerouslySetInnerHTML={html} />;
  }
}
