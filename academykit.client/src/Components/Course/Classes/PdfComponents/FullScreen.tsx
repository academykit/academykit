/* eslint-disable react/no-children-prop */
import { ActionIcon } from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import { ToolbarSlot } from '@react-pdf-viewer/toolbar';
import { IconArrowsMaximize } from '@tabler/icons-react';

const FullScreen = ({ toolbarSlot }: { toolbarSlot: ToolbarSlot }) => {
  const matchesSmallScreen = useMediaQuery('(min-width: 450px');
  return (
    <toolbarSlot.EnterFullScreen
      children={(props) => (
        <ActionIcon
          size={matchesSmallScreen ? 'md' : 'sm'}
          color={'dimmed'}
          // eslint-disable-next-line react/prop-types
          onClick={props.onClick}
        >
          <IconArrowsMaximize />
        </ActionIcon>
      )}
    />
  );
};

export default FullScreen;
