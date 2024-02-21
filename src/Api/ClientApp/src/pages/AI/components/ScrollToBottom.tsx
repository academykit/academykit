import { ActionIcon } from '@mantine/core';
import { IconArrowDown } from '@tabler/icons';

interface IProps {
  scrollToBottom: () => void;
}

const ScrollToBottom = ({ scrollToBottom }: IProps) => {
  return (
    <ActionIcon
      pos={'absolute'}
      bottom={50}
      left={'50%'}
      style={{ transform: 'translateX(-50%)' }}
      variant="light"
      color="gray"
      radius="xl"
      size={'lg'}
      onClick={scrollToBottom}
    >
      <IconArrowDown />
    </ActionIcon>
  );
};

export default ScrollToBottom;
