import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  Group,
  Button,
  Title,
  Paper,
  Table,
  Text,
  Flex,
  ScrollArea,
  Modal,
  Grid,
  Box,
  Loader,
} from "@mantine/core";
import {
  IServerLogs,
  useGetServerLogs,
  useGetSingleLog,
} from "@utils/services/adminService";
import { useTranslation } from "react-i18next";
import { SeverityType } from "@utils/enums";
import { IPaginated } from "@utils/services/types";
import { useState } from "react";
import { IconEye } from "@tabler/icons";

const serverLogs: IServerLogs[] = [
  {
    id: "1",
    type: 1,
    message: "Error occurred",
    trackBy: "ABC123",
    timeStamp: new Date("2023-07-07T10:30:00Z"),
  },
  {
    id: "2",
    type: 2,
    message: "Warning: Disk space low",
    trackBy: "DEF456",
    timeStamp: new Date("2023-07-07T11:45:00Z"),
  },
  {
    id: "3",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "4",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "5",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "6",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "7",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "8",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "9",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "10",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "11",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "12",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "13",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
  },
  {
    id: "14",
    type: 1,
    message: "Critical issue detected",
    trackBy: "GHI789",
    timeStamp: new Date("2023-07-07T13:15:00Z"),
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
        <Text weight={"bold"}>{title}</Text>
        <Text>{content}</Text>
      </div>
    </Grid.Col>
  );
};

const LogDetails = ({ logId }: { logId: string }) => {
  const { data } = useGetSingleLog(logId);
  return (
    <>
      <Grid>
        <DetailFields
          title="Severity"
          content={(data && SeverityType[data?.type]) ?? "-"}
        />
        <DetailFields
          title="Time Stamp"
          content={data?.timeStamp.toISOString() ?? "-"}
        />
        <DetailFields title="Message" content={data?.message ?? "-"} />
        <DetailFields title="Faced By" content={data?.trackBy ?? "-"} />
      </Grid>
    </>
  );
};

const Rows = ({ item }: { item: IServerLogs }) => {
  const [viewLog, setViewLog] = useState(false);

  return (
    <>
      <tr>
        <Modal
          title={`Log Details`}
          opened={viewLog}
          onClose={() => {
            setViewLog(false);
          }}
        >
          <LogDetails logId={item.id} />
        </Modal>
        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              {SeverityType[item.type]}
            </Text>
          </Group>
        </td>

        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              {`${item.timeStamp}`}
            </Text>
          </Group>
        </td>
        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              {item.message}
            </Text>
          </Group>
        </td>
        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              {item.trackBy}
            </Text>
          </Group>
        </td>
        <td>
          <Group spacing="sm">
            <IconEye
              onClick={() => setViewLog(true)}
              style={{ cursor: "pointer" }}
            />
          </Group>
        </td>
      </tr>
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
        sx={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>{t("logs")}</Title>
      </Group>

      {/* Search and Filter section */}
      <Flex mb={10}>
        {startDateFilterComponent(t('start_date'), "StartDate")}
        {endDateFilterComponent(t('end_date'), "EndDate")}
        <div style={{ marginRight: "8px" }}>
          {filterComponent(
            [
              { value: "1", label: t('error') },
              { value: "2", label: t('warning') },
              { value: "3", label: t('debug') },
              { value: "4", label: t('info') },
            ],
            t("severity"),
            "Severity"
          )}
        </div>
        {searchComponent(t("search_logs") as string)}
      </Flex>

      {/* table section */}
      {getLogData.data && getLogData.data.totalCount > 0 ? (
        <ScrollArea>
          <Paper>
            <Table
              sx={{ minWidth: 800 }}
              verticalSpacing="sm"
              striped
              highlightOnHover
            >
              <thead>
                <tr>
                  <th>{t('severity')}</th>
                  <th>{t('time_stamp')}</th>
                  <th>{t('message')}</th>
                  <th>{t('faced_by')}</th>
                  <th>{t('actions')}</th>
                </tr>
              </thead>
              <tbody>
                {/* <Rows item={getLogData.data}/>
              <Rows /> */}
                {serverLogs.map((item: IServerLogs) => (
                  <Rows item={item} key={item.id} />
                ))}
              </tbody>
            </Table>
          </Paper>
        </ScrollArea>
      ) : (
        getLogData.isLoading ? <Loader /> : <Box mt={10}>{t('no_logs')}</Box>
      )}

      {getLogData.data &&
        pagination(getLogData.data?.totalPage, getLogData.data?.items.length)}
    </>
  );
};

export default withSearchPagination(Log);
