import { Button, Divider, Group, Title, useMantineTheme } from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

const SubmitResultHeader = ({
  marks,
  duration,
  totalMarks,
}: {
  marks: number;
  duration: string;
  totalMarks: number;
}) => {
  const theme = useMantineTheme();
  const smallScreen = useMediaQuery(`(min-width: ${theme.breakpoints.sm}px)`);
  const textSize = smallScreen ? 3 : 5;
  const navigate = useNavigate();
  const { t } = useTranslation();
  return (
    <Group>
      <Title order={textSize}>
        {marks}/{totalMarks}
      </Title>
      <Divider orientation="vertical" />
      {duration && <Title order={textSize}>{duration}</Title>}

      <Button onClick={() => navigate(-1)} size={smallScreen ? "md" : "xs"}>
        {t("close")}
      </Button>
    </Group>
  );
};

export default SubmitResultHeader;
