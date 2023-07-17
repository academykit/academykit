import {
  ActionIcon,
  MantineNumberSize,
  useMantineColorScheme,
} from '@mantine/core';
import { IconSun, IconMoonStars } from '@tabler/icons';

type Props = {
  size: MantineNumberSize;
};

export function ColorSchemeToggle({ size }: Props) {
  const { colorScheme, toggleColorScheme } = useMantineColorScheme();

  return (
    <ActionIcon onClick={() => toggleColorScheme()} size={size} color="primary">
      {colorScheme === 'dark' ? (
        <IconSun size={20} stroke={1.5} />
      ) : (
        <IconMoonStars size={20} stroke={1.5} />
      )}
    </ActionIcon>
  );
}
