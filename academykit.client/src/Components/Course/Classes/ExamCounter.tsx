import CountDownTimer from "@components/Ui/CountDownTimer";
import { Button, Group } from "@mantine/core";
import { useTranslation } from "react-i18next";
const ExamCounter = ({
  duration,
  isLoading,
  onSubmit,
  onClick,
}: {
  duration: number;
  isLoading: boolean;
  onSubmit: () => void;
  onClick: () => void;
}) => {
  const { t } = useTranslation();
  return (
    <Group>
      <CountDownTimer
        time={duration}
        startDateTime={new Date().toString()}
        cb={onSubmit}
      />
      <Button loading={isLoading} onClick={onClick}>
        {t("submit")}
      </Button>
    </Group>
  );
};

export default ExamCounter;
