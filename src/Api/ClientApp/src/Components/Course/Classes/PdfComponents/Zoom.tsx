import { ActionIcon, Group, Text } from '@mantine/core';
import { ToolbarSlot } from '@react-pdf-viewer/toolbar';
import { IconZoomIn, IconZoomOut } from '@tabler/icons';

const Zoom = ({ toolbarSlot }: { toolbarSlot: ToolbarSlot }) => {
  return (
    <Group>
      <toolbarSlot.ZoomIn
        children={(props) => (
          <ActionIcon onClick={props.onClick}>
            <IconZoomIn />
          </ActionIcon>
        )}
      />

      <toolbarSlot.Zoom
        children={(props) => <Text color={'dimmed'}>{props.scale}</Text>}
      />

      <toolbarSlot.ZoomOut
        children={(props) => (
          <ActionIcon onClick={props.onClick}>
            <IconZoomOut />
          </ActionIcon>
        )}
      />
    </Group>
  );
};

export default Zoom;
