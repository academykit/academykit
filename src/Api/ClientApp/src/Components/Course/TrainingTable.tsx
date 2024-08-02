import { Paper, Table } from '@mantine/core';
import { DATE_FORMAT } from '@utils/constants';
import { CourseLanguage } from '@utils/enums';
import { ICourse } from '@utils/services/courseService';
import moment from 'moment';
import { useTranslation } from 'react-i18next';

const TrainingRow = ({ item }: { item: ICourse }) => {
  return (
    <Table.Tr key={item?.id}>
      <Table.Td
        style={{
          minWidth: '122px',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
        }}
      >
        {item?.name ?? ''}
      </Table.Td>
      <Table.Td>{item?.groupName ?? ''}</Table.Td>
      <Table.Td>{item?.language && CourseLanguage[item.language]}</Table.Td>
      <Table.Td>{item?.levelName ?? ''}</Table.Td>
      <Table.Td>
        {item?.createdOn && moment(item.createdOn).format(DATE_FORMAT)}
      </Table.Td>
    </Table.Tr>
  );
};

const TrainingTable = ({ courses }: { courses: ICourse[]; search: string }) => {
  const { t } = useTranslation();

  const Rows = () =>
    courses.map((item: any) => {
      return <TrainingRow item={item} key={item.id} />;
    });

  return (
    <>
      <Paper>
        <Table
          style={{ minWidth: 800 }}
          verticalSpacing="sm"
          striped
          highlightOnHover
          withTableBorder
          withColumnBorders
        >
          <Table.Thead>
            <Table.Tr>
              <Table.Th>{t('title')}</Table.Th>
              <Table.Th>{t('group')}</Table.Th>
              <Table.Th>{t('Language')}</Table.Th>
              <Table.Th>{t('level')}</Table.Th>
              <Table.Th>{t('created_date')}</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>{Rows()}</Table.Tbody>
        </Table>
      </Paper>
    </>
  );
};

export default TrainingTable;
