import { createStyles, Group, Progress, Text } from '@mantine/core';

const useStyles = createStyles((theme) => ({
  progressBar: {
    '&:not(:first-of-type)': {
      borderLeft: `3px solid ${
        theme.colorScheme === 'dark' ? theme.colors.dark[7] : theme.white
      }`,
    },
  },
}));

const ProgressBar = ({
  total,
  positive,
}: {
  total: number;
  positive: number;
}) => {
  const positiveNumber = total > 0 ? (positive / total) * 100 : 0;
  const { classes, theme } = useStyles();

  return (
    <>
      <Group position="apart">
        <Text size="xs" color="teal" weight={700}>
          {positiveNumber.toFixed(0)}%
        </Text>
      </Group>
      <Progress
        classNames={{ bar: classes.progressBar }}
        sections={[
          {
            value: positiveNumber,
            color:
              theme.colorScheme === 'dark'
                ? theme.colors.teal[9]
                : theme.colors.teal[6],
          },
        ]}
      />
    </>
  );
};

export default ProgressBar;
