import {
  Group,
  Switch,
  useMantineColorScheme,
  useMantineTheme,
} from '@mantine/core';
import { IconMoonStars, IconSun } from '@tabler/icons-react';

export function SwitchToggle() {
  const { colorScheme, toggleColorScheme } = useMantineColorScheme();
  const theme = useMantineTheme();

  return (
    <Group justify="center" my={30}>
      <Switch
        checked={colorScheme === 'dark'}
        onChange={() => toggleColorScheme()}
        size="lg"
        onLabel={<IconSun color={theme.white} size={20} stroke={1.5} />}
        offLabel={
          <IconMoonStars color={theme.colors.gray[6]} size={20} stroke={1.5} />
        }
      />
    </Group>
  );
}
