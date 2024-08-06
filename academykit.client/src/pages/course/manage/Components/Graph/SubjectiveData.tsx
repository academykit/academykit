import { Box, Title as Heading, ScrollArea, Text } from "@mantine/core";
import { useTranslation } from "react-i18next";
import SubjectiveAnswer from "./SubjectiveAnswer";

interface IProps {
  name: string;
  answers: {
    answer: string;
  }[];
  responseCount: number;
}

const SubjectiveData = ({ name, answers, responseCount }: IProps) => {
  const { t } = useTranslation();

  return (
    <>
      <Box mb={15}>
        <Heading order={4}>{name}</Heading>
        <Text fz="sm" c="dimmed">
          {t("responses")}: {responseCount}
        </Text>

        <ScrollArea.Autosize mah={250} placeholder="">
          {answers.map((item, i) => (
            <SubjectiveAnswer key={i}>{item.answer}</SubjectiveAnswer>
          ))}
        </ScrollArea.Autosize>
      </Box>
    </>
  );
};

export default SubjectiveData;
