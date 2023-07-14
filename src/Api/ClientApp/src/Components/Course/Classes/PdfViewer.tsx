import * as React from 'react';
import { Viewer, Worker } from '@react-pdf-viewer/core';
// @ts-ignore
import { toolbarPlugin } from '@react-pdf-viewer/toolbar';
import { useMediaQuery } from '@mantine/hooks';

import '@react-pdf-viewer/core/lib/styles/index.css';
import '@react-pdf-viewer/toolbar/lib/styles/index.css';
import {
  ActionIcon,
  Badge,
  Button,
  Container,
  Divider,
  Group,
  Menu,
  Popover,
  useMantineColorScheme,
} from '@mantine/core';
import { ICourseLesson } from '@utils/services/courseService';
import { useWatchHistory } from '@utils/services/watchHistory';
import { showNotification } from '@mantine/notifications';
import { defaultLayoutPlugin } from '@react-pdf-viewer/default-layout';
import '@react-pdf-viewer/default-layout/lib/styles/index.css';
import Download from './PdfComponents/Download';
import FullScreen from './PdfComponents/FullScreen';
import Zoom from './PdfComponents/Zoom';
import SwitchPage from './PdfComponents/SwitchPage';
import { useTranslation } from 'react-i18next';
import useAuth from '@hooks/useAuth';
import { UserRole } from '@utils/enums';

interface PdfViewerProps {
  lesson: ICourseLesson;
  onEnded: () => void;
}

const PdfViewer: React.FC<PdfViewerProps> = ({ lesson, onEnded }) => {
  const watchHistory = useWatchHistory(lesson.courseId, lesson.id);
  const matches = useMediaQuery('(min-width: 991px');
  const matchesSmallScreen = useMediaQuery('(min-width: 550px');
  const auth = useAuth();
  const userRole = auth?.auth?.role;

  const theme = useMantineColorScheme();
  const { t } = useTranslation();
  const onMarkComplete = () => {
    onEnded();
    showNotification({
      title: t('success'),
      message: t('mark_pdf_complete'),
    });
  };

  const defaultLayoutPluginInstance = defaultLayoutPlugin({
    sidebarTabs: (_) => [],
    renderToolbar(Toolbar) {
      return (
        <Toolbar
          children={(toolbarSlot) => (
            <Container w="100%" fluid>
              <Group position="apart">
                <Group>
                  {matchesSmallScreen && (
                    <>
                      <Zoom toolbarSlot={toolbarSlot} />
                      <Divider orientation="vertical" />
                    </>
                  )}
                  <SwitchPage toolbarSlot={toolbarSlot} />
                </Group>

                <Group>
                  {/* Mark as completed button hidden for:
                    - Org Admin
                    - Org Superadmin
                    - Training's trainer
                  */}
                  {!lesson.isCompleted ? (
                    userRole != UserRole.SuperAdmin &&
                    userRole != UserRole.Admin &&
                    userRole != lesson.user.role && (
                      <Button
                        onClick={onMarkComplete}
                        loading={watchHistory.isLoading}
                      >
                        {t('mark_complete')}
                      </Button>
                    )
                  ) : (
                    <Badge>{t('Completed')}</Badge>
                  )}
                  <FullScreen toolbarSlot={toolbarSlot} />
                  {/* <Download toolbarSlot={toolbarSlot} /> */}
                  {/* <Menu
                    width={300}
                    withArrow
                    shadow="md"
                    position="bottom-end"
                    styles={{
                      dropdown: {
                        // position: "fixed",
                      },
                    }}
                  >
                    <Menu.Target>
                      <IconDotsVertical />
                    </Menu.Target>
                    <Menu.Dropdown
                      sx={(theme) => ({
                        position: "absolute",
                        top: "200px",
                        background:
                          theme.colorScheme === "dark"
                            ? theme.colors.dark[7]
                            : theme.white,
                      })}
                    >
                      <Menu.Item>
                        <Zoom toolbarSlot={toolbarSlot} />
                      </Menu.Item>
                      <Menu.Item>
                        <SwitchPage toolbarSlot={toolbarSlot} />
                      </Menu.Item>
                    </Menu.Dropdown>
                  </Menu> */}
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
          border: '1px solid rgba(0, 0, 0, 0.3)',
          display: 'flex',
          flexDirection: 'column',
          height: matches ? '100%' : '405px',
        }}
      >
        <div
          style={{
            flex: 1,
            overflow: 'hidden',
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
