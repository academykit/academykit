import { Table, Text } from "@mantine/core";
import { useTranslation } from "react-i18next";

const EmptyRow = ({
  colspan,
  message,
}: {
  colspan: number;
  message: string;
}) => {
  const { t } = useTranslation();
  return (
    <Table.Tr>
      <Table.Td colSpan={colspan}>
        <Text ta="center">{t(`${message}`)}</Text>
      </Table.Td>
    </Table.Tr>
  );
};

export default EmptyRow;
