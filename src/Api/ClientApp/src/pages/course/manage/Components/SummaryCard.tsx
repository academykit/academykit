import { Paper, Text, Title } from '@mantine/core';
import { useTranslation } from 'react-i18next';

interface IProps {
  title: string;
  count: number;
}

const SummaryCard = ({ title, count }: IProps) => {
  const { t } = useTranslation();

  return (
    <>
      <Paper p={'md'} withBorder miw={'100%'}>
        <Title order={4}>{t(`${title}`)}</Title>
        <Text>{count}</Text>
      </Paper>
    </>
  );
};

export default SummaryCard;
