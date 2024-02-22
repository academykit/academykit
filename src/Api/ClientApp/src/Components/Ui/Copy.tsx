import {
  ActionIcon,
  ActionIconProps,
  CopyButton,
  Tooltip,
  rem,
} from '@mantine/core';
import { IconCheck, IconCopy } from '@tabler/icons';

interface IProps extends ActionIconProps {
  value: string;
}

const Copy = ({ value, disabled }: IProps) => {
  return (
    <>
      <CopyButton value={value} timeout={2000}>
        {({ copied, copy }) => (
          <Tooltip
            label={copied ? 'Copied' : 'Copy'}
            withArrow
            position="right"
          >
            <ActionIcon
              color={copied ? 'teal' : 'gray'}
              variant="subtle"
              onClick={copy}
              disabled={disabled}
            >
              {copied ? (
                <IconCheck style={{ width: rem(16) }} />
              ) : (
                <IconCopy style={{ width: rem(16) }} />
              )}
            </ActionIcon>
          </Tooltip>
        )}
      </CopyButton>
    </>
  );
};

export default Copy;
