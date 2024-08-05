import { Title as Heading, Paper, Text } from "@mantine/core";
import {
  BarElement,
  CategoryScale,
  Chart as ChartJS,
  CoreScaleOptions,
  Legend,
  LinearScale,
  Scale,
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
  indexAxis: "y" as const,
  elements: {
    bar: {
      borderWidth: 2,
    },
  },
  maintainAspectRatio: false,
  responsive: true,
  plugins: {
    legend: {
      position: "top" as const,
      onClick: function () {},
    },
  },
  scales: {
    y: {
      ticks: {
        callback: function (
          this: Scale<CoreScaleOptions>,
          tickValue: string | number
        ): string | number {
          const truncatedValue =
            this.getLabelForValue(tickValue as number).toString().length > 6
              ? this.getLabelForValue(tickValue as number)
                  .toString()
                  .substring(0, 6) + "..."
              : this.getLabelForValue(tickValue as number);

          return truncatedValue;
        },
      },
    },
  },
};

interface IProps {
  name: string;
  feedbackOptions: {
    id: string;
    feedbackId: string;
    feedbackName: string;
    option: string;
    isSelected: boolean;
    order: number;
    selectedCount: number;
  }[];
  responseCount: number;
  type: "SingleChoice" | "MultipleChoice";
}

const HorizontalBarGraph = ({
  name,
  feedbackOptions,
  responseCount,
  type,
}: IProps) => {
  const { t } = useTranslation();

  const removeTags = (value: string) => {
    return value.replace(/<[^>]*>/g, "");
  };

  const labels = feedbackOptions.map((option) => removeTags(option.option));

  const data = {
    labels,
    datasets: [
      {
        label: "Options",
        data: feedbackOptions.map((option) => option.selectedCount),
        borderColor: "rgb(255, 99, 132)",
        backgroundColor: "rgba(255, 99, 132, 0.5)",
      },
    ],
  };

  return (
    <>
      <Heading order={4}>{name}</Heading>
      <Text fz="sm" c="dimmed">
        {t("responses")}: {responseCount} ({t(`${type}`)})
      </Text>
      <Paper mb={10} p="sm" h={300} withBorder style={{ position: "relative" }}>
        <Bar options={options} data={data} />
      </Paper>
    </>
  );
};

export default HorizontalBarGraph;
