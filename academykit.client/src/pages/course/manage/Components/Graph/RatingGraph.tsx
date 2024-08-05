import { Title as Heading, Paper, Text } from "@mantine/core";
import {
  BarElement,
  CategoryScale,
  Chart as ChartJS,
  Legend,
  LinearScale,
  Title,
  Tooltip,
} from "chart.js";
import { Bar } from "react-chartjs-2";
import { useTranslation } from "react-i18next";

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

export const options = {
  maintainAspectRatio: false,
  responsive: true,
  plugins: {
    legend: {
      position: "top" as const,
      onClick: function () {},
    },
  },
};

const labels = ["One", "Two", "Three", "Four", "Five"];

interface IProps {
  name: string;
  stats: {
    fiveRating: number;
    fourRating: number;
    threeRating: number;
    twoRating: number;
    oneRating: number;
  };
  responseCount: number;
}

const RatingGraph = ({ name, stats, responseCount }: IProps) => {
  const { t } = useTranslation();

  const data = {
    labels,
    datasets: [
      {
        label: "Ratings",
        data: [
          stats.oneRating,
          stats.twoRating,
          stats.threeRating,
          stats.fourRating,
          stats.fiveRating,
        ],
        backgroundColor: "rgba(53, 162, 235, 0.5)",
      },
    ],
  };

  return (
    <>
      <Heading order={4}>{name}</Heading>
      <Text fz="sm" c="dimmed">
        {t("responses")}: {responseCount}
      </Text>

      <Paper mb={10} p="sm" h={300} withBorder style={{ position: "relative" }}>
        <Bar options={options} data={data} />
      </Paper>
    </>
  );
};

export default RatingGraph;
