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
import {
  ActionIcon,
  Badge,
  Button,
  Container,
  Group,
  Text,
  useMantineColorScheme,
} from "@mantine/core";
import { ICourseLesson } from "@utils/services/courseService";
import { useWatchHistory } from "@utils/services/watchHistory";
import { showNotification } from "@mantine/notifications";
import { defaultLayoutPlugin } from "@react-pdf-viewer/default-layout";
import "@react-pdf-viewer/default-layout/lib/styles/index.css";
import {
  IconArrowsMaximize,
  IconDownload,
  IconZoomIn,
  IconZoomOut,
} from "@tabler/icons";

interface PdfViewerProps {
  lesson: ICourseLesson;
  onEnded: () => void;
}

const PdfViewer: React.FC<PdfViewerProps> = ({ lesson, onEnded }) => {
  const toolbarPluginInstance = toolbarPlugin({});
  // const { renderDefaultToolbar, Toolbar } = toolbarPluginInstance;
  const watchHistory = useWatchHistory(lesson.courseId, lesson.id);
  const matches = useMediaQuery("(min-width: 991px");
  const theme = useMantineColorScheme();
  const onMarkComplete = () => {
    onEnded();
    showNotification({
      title: "Success",
      message: "Pdf marked as completed.",
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

  const defaultLayoutPluginInstance = defaultLayoutPlugin({
    sidebarTabs: (_) => [],
    renderToolbar(Toolbar) {
      return (
        <Toolbar
          children={(toolbarSlot) => (
            <Container w="100%" fluid>
              <Group position="apart">
                <Group>
                  <toolbarSlot.ZoomIn
                    children={(props) => (
                      <ActionIcon onClick={props.onClick}>
                        <IconZoomIn />
                      </ActionIcon>
                    )}
                  />

                  <toolbarSlot.Zoom
                    children={(props) => (
                      <Text color={"dimmed"}>{props.scale}</Text>
                    )}
                  />

                  <toolbarSlot.ZoomOut
                    children={(props) => (
                      <ActionIcon onClick={props.onClick}>
                        <IconZoomOut />
                      </ActionIcon>
                    )}
                  />
                  <toolbarSlot.CurrentPageLabel
                    children={(props) => (
                      <Text color={"red"}>{props.pageLabel}</Text>
                    )}
                  />
                </Group>

                <Group>
                  {!lesson.isCompleted ? (
                    <Button
                      onClick={onMarkComplete}
                      loading={watchHistory.isLoading}
                    >
                      Mark Complete
                    </Button>
                  ) : (
                    <Badge>Completed</Badge>
                  )}
                  <toolbarSlot.EnterFullScreen
                    children={(props) => (
                      <ActionIcon onClick={props.onClick}>
                        <IconArrowsMaximize />
                      </ActionIcon>
                    )}
                  />
                  <toolbarSlot.Download
                    children={(props) => (
                      <ActionIcon onClick={props.onClick}>
                        <IconDownload />
                      </ActionIcon>
                    )}
                  />
                </Group>
              </Group>
            </Container>
          )}
        />
      );
    },
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
            flex: 1,
            overflow: "hidden",
          }}
        >
          <Viewer
            theme={theme.colorScheme}
            plugins={[defaultLayoutPluginInstance]}
            fileUrl={lesson.documentUrl}
          />
        </div>
      </div>
    </Worker>
  );
};

export default PdfViewer;
