import { useGetIframelyOembed } from "@utils/services/iframelyService";

type Props = {
  url: string;
};

export default function ExternalLinkViewer(props: Props) {
  const { isLoading, isError, error, data } = useGetIframelyOembed(props.url);

  if (isError) {
    return <div>Error: {error.message}</div>;
  } else if (!isLoading) {
    return <div>Loading…</div>;
  } else {
    return <div dangerouslySetInnerHTML={data} />;
  }
}
