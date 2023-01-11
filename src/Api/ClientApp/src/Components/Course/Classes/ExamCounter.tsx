import CountDownTimer from "@components/Ui/CountDownTimer";
import { Button, Group } from "@mantine/core";
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
  return (
    <Group>
      <CountDownTimer
        time={duration}
        startDateTime={new Date().toString()}
        cb={onSubmit}
      />
      <Button loading={isLoading} onClick={onClick}>
        Submit
      </Button>
    </Group>
  );
};

export default ExamCounter;
