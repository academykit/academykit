import * as React from "react";
import { Viewer, Worker } from "@react-pdf-viewer/core";
// @ts-ignore
import { toolbarPlugin } from "@react-pdf-viewer/toolbar";
import { useMediaQuery } from "@mantine/hooks";
import type {
  ToolbarSlot,
  TransformToolbarSlot,
  // @ts-ignore
} from "@react-pdf-viewer/toolbar";
import "@react-pdf-viewer/core/lib/styles/index.css";
import "@react-pdf-viewer/toolbar/lib/styles/index.css";
import { Badge, Button } from "@mantine/core";
import { ICourseLesson } from "@utils/services/courseService";
import { useWatchHistory } from "@utils/services/watchHistory";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";

interface PdfViewerProps {
  lesson: ICourseLesson;
  onEnded: () => void;
}

const PdfViewer: React.FC<PdfViewerProps> = ({ lesson, onEnded }) => {
  const toolbarPluginInstance = toolbarPlugin();
  const { renderDefaultToolbar, Toolbar } = toolbarPluginInstance;
  const watchHistory = useWatchHistory(lesson.courseId, lesson.id);
  const matches = useMediaQuery("(min-width: 991px");

  const onMarkComplete = () => {
      
      onEnded();
      showNotification({
        title: "Success",
        message: "Pdf marked as completed!",
      });
    
  };

  const transform: TransformToolbarSlot = (slot: ToolbarSlot) => ({
    ...slot,
    EnterFullScreenMenuItem: () => <></>,
    SwitchTheme: () =>
      !lesson.isCompleted ? (
        <Button onClick={onMarkComplete} loading={watchHistory.isLoading}>
          Mark Complete
        </Button>
      ) : (
        <Badge>Completed</Badge>
      ),
    Open: () => <></>,
  });

  return (
    <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.1.81/build/pdf.worker.min.js">
      <div
        className="js-viewer-container"
        style={{
          border: "1px solid rgba(0, 0, 0, 0.3)",
          display: "flex",
          flexDirection: "column",
          height: matches ? "100%" : "405px",
        }}
      >
        <div
          style={{
            alignItems: "center",
            backgroundColor: "#eeeeee",
            borderBottom: "1px solid rgba(0, 0, 0, 0.1)",
            display: "flex",
            padding: "0.25rem",
          }}
        >
          <Toolbar>{renderDefaultToolbar(transform)}</Toolbar>
        </div>
        <div
          style={{
            flex: 1,
            overflow: "hidden",
          }}
        >
          <Viewer fileUrl={lesson.documentUrl} />
        </div>
      </div>
    </Worker>
  );
};

export default PdfViewer;
