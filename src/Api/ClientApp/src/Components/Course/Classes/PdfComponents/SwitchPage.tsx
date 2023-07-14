import { ActionIcon, Group, Text } from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import { ToolbarSlot } from '@react-pdf-viewer/toolbar';
import { IconArrowUp, IconArrowDown } from '@tabler/icons';

const SwitchPage = ({ toolbarSlot }: { toolbarSlot: ToolbarSlot }) => {
  const matchesSmallScreen = useMediaQuery('(min-width: 450px');
  return (
    <Group>
      <toolbarSlot.GoToPreviousPage
        children={(props) => (
          <ActionIcon
            size={matchesSmallScreen ? 'md' : 'sm'}
            disabled={props.isDisabled}
            onClick={props.onClick}
          >
            <IconArrowUp />
          </ActionIcon>
        )}
      />
      <toolbarSlot.CurrentPageLabel
        children={(props) => (
          <Text size={matchesSmallScreen ? 'md' : 'sm'} color={'dimmed'}>
            {props.currentPage + 1}
          </Text>
        )}
      />
      <Text color={'dimmed'} size={matchesSmallScreen ? 'md' : 'sm'}>
        /
      </Text>
      <toolbarSlot.NumberOfPages
        children={(props) => (
          <Text size={matchesSmallScreen ? 'md' : 'sm'} color={'dimmed'}>
            {props.numberOfPages}
          </Text>
        )}
      />
      <toolbarSlot.GoToNextPage
        children={(props) => (
          <ActionIcon
            size={matchesSmallScreen ? 'md' : 'sm'}
            disabled={props.isDisabled}
            onClick={props.onClick}
          >
            <IconArrowDown />
          </ActionIcon>
        )}
      />
    </Group>
  );
};

export default SwitchPage;
