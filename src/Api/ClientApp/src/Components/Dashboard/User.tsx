import {
  Box,
  createStyles,
  Group,
  Paper,
  SimpleGrid,
  Text,
} from "@mantine/core";
import {
  IconUserPlus,
  IconDiscount2,
  IconReceipt2,
  IconCoin,
  IconArrowUpRight,
  IconArrowDownRight,
} from "@tabler/icons";
import {
  DashboardCourses,
  DashboardStats,
} from "@utils/services/dashboardService";
import { StatsCard } from "./StatsCard";
import TrainingCards from "./TrainingCards";

const incomingData = [
  {
    key: "totalEnrolledCourses",
    label: "Total Enrollments",
    icon: "enrollment",
  },
  {
    key: "totalInProgressCourses",
    label: "Active Trainings",
    icon: "trainings",
  },
  {
    key: "totalCompletedCourses",
    label: "My Completed Trainings",
    icon: "completed",
  },
];

const useStyles = createStyles((theme) => ({
  root: {
    padding: theme.spacing.xl * 1.5,
  },

  value: {
    fontSize: 24,
    fontWeight: 700,
    lineHeight: 1,
  },

  diff: {
    lineHeight: 1,
    display: "flex",
    alignItems: "center",
  },

  icon: {
    color:
      theme.colorScheme === "dark"
        ? theme.colors.dark[3]
        : theme.colors.gray[4],
  },

  title: {
    fontWeight: 700,
    textTransform: "uppercase",
  },
}));

export const icons = {
  user: IconUserPlus,
  discount: IconDiscount2,
  receipt: IconReceipt2,
  coin: IconCoin,
};

export interface StatsGridProps {
  data: {
    title: string;
    icon: keyof typeof icons;
    value: string;
    diff: number;
  }[];
}

export const User = ({
  dashboard,
  dashboardCourses,
}: {
  dashboard: DashboardStats;
  dashboardCourses: DashboardCourses[];
}) => {
  return (
    <div>
      <SimpleGrid
        mb={20}
        cols={4}
        breakpoints={[
          { maxWidth: "md", cols: 2 },
          { maxWidth: "xs", cols: 1 },
        ]}
      >
        {dashboard &&
          incomingData.map((x, idx) => (
            //@ts-ignore
            <StatsCard key={x.key} data={x} dashboard={dashboard} />
          ))}
      </SimpleGrid>
      <Text size={"xl"} weight="bold">
        My Trainings
      </Text>
      {dashboardCourses.length > 0 ? (
        <Text c="dimmed" mb={10}>
          Your progress on learning materials:
        </Text>
      ) : (
        <Text c="dimmed">You are not enrolled to any Trainings.</Text>
      )}

      <SimpleGrid
        cols={3}
        breakpoints={[
          { maxWidth: "md", cols: 2 },
          { maxWidth: "xs", cols: 1 },
        ]}
      >
        {dashboardCourses.length > 0 &&
          dashboardCourses.map((x, idx) => <TrainingCards data={x} />)}
      </SimpleGrid>
    </div>
  );
};

export function StatsGrid({ data }: StatsGridProps) {
  const { classes } = useStyles();

  const stats = data.map((stat) => {
    const Icon = icons[stat.icon];
    const DiffIcon = stat.diff > 0 ? IconArrowUpRight : IconArrowDownRight;

    return (
      <Paper withBorder p="md" radius="md" key={stat.title}>
        <Group position="apart">
          <Text size="xs" color="dimmed" className={classes.title}>
            {stat.title}
          </Text>
          <Icon className={classes.icon} size={22} stroke={1.5} />
        </Group>

        <Group align="flex-end" spacing="xs" mt={25}>
          <Text className={classes.value}>{stat.value}</Text>
          <Text
            color={stat.diff > 0 ? "teal" : "red"}
            size="sm"
            weight={500}
            className={classes.diff}
          >
            <span>{stat.diff}%</span>
            <DiffIcon size={16} stroke={1.5} />
          </Text>
        </Group>

        <Text size="xs" color="dimmed" mt={7}>
          Compared to previous month
        </Text>
      </Paper>
    );
  });
  return (
    <div className={classes.root}>
      <SimpleGrid
        cols={4}
        breakpoints={[
          { maxWidth: "md", cols: 2 },
          { maxWidth: "xs", cols: 1 },
        ]}
      >
        {stats}
      </SimpleGrid>
    </div>
  );
}