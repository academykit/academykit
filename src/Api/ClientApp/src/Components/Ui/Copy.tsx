import { ActionIcon, CopyButton, Tooltip, rem } from '@mantine/core';
import { IconCheck, IconCopy } from '@tabler/icons';

const Copy = ({ value }: { value: string }) => {
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
