import {
  ActionIcon,
  ActionIconProps,
  CopyButton,
  Tooltip,
  rem,
} from "@mantine/core";
import { IconCheck, IconCopy } from "@tabler/icons-react";
import { useTranslation } from "react-i18next";

interface IProps extends ActionIconProps {
  value: string;
}

const Copy = ({ value, disabled }: IProps) => {
  const { t } = useTranslation();

  return (
    <>
      <CopyButton value={value} timeout={2000}>
        {({ copied, copy }) => (
          <Tooltip
            label={copied ? t("copied") : t("copy")}
            withArrow
            position="right"
          >
            <ActionIcon
              color={copied ? "teal" : "gray"}
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
