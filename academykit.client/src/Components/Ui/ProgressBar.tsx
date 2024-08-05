import { Group, Progress, Text } from '@mantine/core';
import classes from './styles/progressBar.module.css';

const ProgressBar = ({
  total,
  positive,
}: {
  total: number;
  positive: number;
}) => {
  const positiveNumber = total > 0 ? (positive / total) * 100 : 0;

  return (
    <>
      <Group justify="space-between">
        <Text size="xs" c="teal" fw={700}>
          {positiveNumber.toFixed(0)}%
        </Text>
      </Group>
      <Progress
        classNames={{ root: classes.progressBar, section: classes.section }}
        value={positiveNumber}
      />
    </>
  );
};

export default ProgressBar;
