/* eslint-disable */
import { ActionIcon } from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import { ToolbarSlot } from '@react-pdf-viewer/toolbar';
import { IconArrowsMaximize } from '@tabler/icons';

const FullScreen = ({ toolbarSlot }: { toolbarSlot: ToolbarSlot }) => {
  const matchesSmallScreen = useMediaQuery('(min-width: 450px');
  return (
    <toolbarSlot.EnterFullScreen
      children={(props) => (
        <ActionIcon
          size={matchesSmallScreen ? 'md' : 'sm'}
          color={'dimmed'}
          onClick={props.onClick}
        >
          <IconArrowsMaximize />
        </ActionIcon>
      )}
    />
  );
};

export default FullScreen;
