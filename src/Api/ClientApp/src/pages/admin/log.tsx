import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import {
  Box,
  Flex,
  Grid,
  Group,
  Loader,
  Modal,
  Paper,
  ScrollArea,
  Table,
  Text,
  Title,
} from '@mantine/core';
import { IconEye } from '@tabler/icons-react';
import { SeverityType } from '@utils/enums';
import {
  IServerLogs,
  useGetServerLogs,
  useGetSingleLog,
} from '@utils/services/adminService';
import { IPaginated } from '@utils/services/types';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';

const serverLogs: IServerLogs[] = [
  {
    id: '1',
    type: 1,
    message: 'Error occurred',
    trackBy: 'ABC123',
    timeStamp: new Date('2023-07-07T10:30:00Z'),
  },
  {
    id: '2',
    type: 2,
    message: 'Warning: Disk space low',
    trackBy: 'DEF456',
    timeStamp: new Date('2023-07-07T11:45:00Z'),
  },
  {
    id: '3',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '4',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '5',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '6',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '7',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '8',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '9',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '10',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '11',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '12',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '13',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  {
    id: '14',
    type: 1,
    message: 'Critical issue detected',
    trackBy: 'GHI789',
    timeStamp: new Date('2023-07-07T13:15:00Z'),
  },
  // Add more dummy data as needed
];

const paginatedData: IPaginated<IServerLogs> = {
  currentPage: 1,
  pageSize: 10,
  totalCount: serverLogs.length,
  totalPage: Math.ceil(serverLogs.length / 10),
  items: serverLogs,
};
console.log(paginatedData);
const DetailFields = ({
  title,
  content,
}: {
  title: string;
  content: string;
}) => {
  return (
    <Grid.Col span={6}>
      <div>
        <Text fw={'bold'}>{title}</Text>
        <Text>{content}</Text>
      </div>
    </Grid.Col>
  );
};

const LogDetails = ({ logId }: { logId: string }) => {
  const { data } = useGetSingleLog(logId);
  const { t } = useTranslation();

  return (
    <>
      <Grid>
        <DetailFields
          title={t('severity')}
          content={(data && SeverityType[data?.type]) ?? '-'}
        />
        <DetailFields
          title={t('time_stamp')}
          content={data?.timeStamp.toISOString() ?? '-'}
        />
        <DetailFields title={t('message')} content={data?.message ?? '-'} />
        <DetailFields title={t('faced_by')} content={data?.trackBy ?? '-'} />
      </Grid>
    </>
  );
};

const Rows = ({ item }: { item: IServerLogs }) => {
  const [viewLog, setViewLog] = useState(false);
  const { t } = useTranslation();

  return (
    <>
      <Table.Tr>
        <Modal
          title={t('log_details')}
          opened={viewLog}
          onClose={() => {
            setViewLog(false);
          }}
        >
          <LogDetails logId={item.id} />
        </Modal>
        <Table.Td>
          <Group gap="sm">
            <Text size="sm" fw={500}>
              {SeverityType[item.type]}
            </Text>
          </Group>
        </Table.Td>

        <Table.Td>
          <Group gap="sm">
            <Text size="sm" fw={500}>
              {`${item.timeStamp}`}
            </Text>
          </Group>
        </Table.Td>
        <Table.Td>
          <Group gap="sm">
            <Text size="sm" fw={500}>
              {item.message}
            </Text>
          </Group>
        </Table.Td>
        <Table.Td>
          <Group gap="sm">
            <Text size="sm" fw={500}>
              {item.trackBy}
            </Text>
          </Group>
        </Table.Td>
        <Table.Td>
          <Group gap="sm">
            <IconEye
              onClick={() => setViewLog(true)}
              style={{ cursor: 'pointer' }}
            />
          </Group>
        </Table.Td>
      </Table.Tr>
    </>
  );
};

const Log = ({
  searchParams,
  pagination,
  searchComponent,
  filterComponent,
  startDateFilterComponent,
  endDateFilterComponent,
}: IWithSearchPagination) => {
  const { t } = useTranslation();
  const getLogData = useGetServerLogs(searchParams);

  return (
    <>
      <Group
        style={{ justifyContent: 'space-between', alignItems: 'center' }}
        mb={15}
      >
        <Title>{t('log')}</Title>
      </Group>

      {/* Search and Filter section */}
      <Flex mb={10}>
        {startDateFilterComponent(t('start_date'), 'StartDate')}
        {endDateFilterComponent(t('end_date'), 'EndDate')}
        <div style={{ marginRight: '8px' }}>
          {filterComponent(
            [
              { value: '1', label: t('info') },
              { value: '2', label: t('error') },
              { value: '3', label: t('warning') },
              { value: '4', label: t('debug') },
            ],
            t('severity'),
            'Severity'
          )}
        </div>
        {searchComponent(t('search_logs') as string)}
      </Flex>

      {/* table section */}
      {getLogData.data && getLogData.data.totalCount > 1 ? (
        <ScrollArea>
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
                  <Table.Th>{t('severity')}</Table.Th>
                  <Table.Th>{t('time_stamp')}</Table.Th>
                  <Table.Th>{t('message')}</Table.Th>
                  <Table.Th>{t('faced_by')}</Table.Th>
                  <Table.Th>{t('actions')}</Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
                {getLogData.data.items.map((item) => (
                  <Rows key={item.id} item={item} />
                ))}
              </Table.Tbody>
            </Table>
          </Paper>
        </ScrollArea>
      ) : getLogData.isLoading ? (
        <Loader />
      ) : (
        <Box mt={10}>{t('no_logs')}</Box>
      )}

      {getLogData.data &&
        pagination(getLogData.data?.totalPage, getLogData.data?.items.length)}
    </>
  );
};

export default withSearchPagination(Log);
