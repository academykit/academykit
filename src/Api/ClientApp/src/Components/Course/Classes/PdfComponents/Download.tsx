/* eslint-disable react/no-children-prop */
import { ActionIcon } from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import { ToolbarSlot } from '@react-pdf-viewer/toolbar';
import { IconDownload } from '@tabler/icons';

const Download = ({ toolbarSlot }: { toolbarSlot: ToolbarSlot }) => {
  const matchesSmallScreen = useMediaQuery('(min-width: 450px');
  return (
    <toolbarSlot.Download
      children={(props) => (
        <ActionIcon
          size={matchesSmallScreen ? 'md' : 'sm'}
          color={'dimmed'}
          // eslint-disable-next-line react/prop-types
          onClick={props.onClick}
        >
          <IconDownload />
        </ActionIcon>
      )}
    />
  );
};

export default Download;
