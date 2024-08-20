import { Badge, Button } from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { ICourseLesson } from "@utils/services/courseService";
import { getIframelyOembed } from "@utils/services/iframelyService";
import { useWatchHistory } from "@utils/services/watchHistory";
import { t } from "i18next";
import { useEffect, useState } from "react";

type Props = {
  url: string;
  lesson: ICourseLesson;
  onEnded: () => void;
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

  const [disable, setDisable] = useState(false);
  const watchHistory = useWatchHistory(props.lesson.courseId, props.lesson.id);

  const onMarkComplete = () => {
    setDisable(true);
    props.onEnded();
    showNotification({
      title: t("success"),
      message: t("mark_pdf_complete"),
    });
  };

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
    return (
      <div>
        <div dangerouslySetInnerHTML={html} />
        {!props.lesson.isCompleted ? (
          props.lesson.isTrainee && (
            <Button
              onClick={onMarkComplete}
              loading={watchHistory.isPending}
              disabled={disable}
            >
              {t("mark_complete")}
            </Button>
          )
        ) : (
          <Badge>{t("Completed")}</Badge>
        )}
      </div>
    );
  }
}
