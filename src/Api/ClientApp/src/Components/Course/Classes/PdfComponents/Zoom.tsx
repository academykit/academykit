import { ActionIcon, Group, Text } from '@mantine/core';
import { ToolbarSlot } from '@react-pdf-viewer/toolbar';
import { IconZoomIn, IconZoomOut } from '@tabler/icons';

const Zoom = ({ toolbarSlot }: { toolbarSlot: ToolbarSlot }) => {
  return (
    <Group>
      <toolbarSlot.ZoomIn
        // eslint-disable-next-line react/no-children-prop
        children={(props) => (
          // eslint-disable-next-line react/prop-types
          <ActionIcon onClick={props.onClick}>
            <IconZoomIn />
          </ActionIcon>
        )}
      />

      <toolbarSlot.Zoom
        // eslint-disable-next-line react/no-children-prop, react/prop-types
        children={(props) => <Text color={'dimmed'}>{props.scale}</Text>}
      />

      <toolbarSlot.ZoomOut
        // eslint-disable-next-line react/no-children-prop
        children={(props) => (
          // eslint-disable-next-line react/prop-types
          <ActionIcon onClick={props.onClick}>
            <IconZoomOut />
          </ActionIcon>
        )}
      />
    </Group>
  );
};

export default Zoom;
