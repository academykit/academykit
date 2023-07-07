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
} from "@mantine/core";
import { IServerLogs, useGetServerLogs } from "@utils/services/adminService";
import { useTranslation } from "react-i18next";
import { SeverityType } from "@utils/enums";

const Rows = ({ item }: { item: IServerLogs }) => {
  return (
    <>
      <tr>
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
            <Text size="sm" weight={500}>
              email
            </Text>
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
  endDateFilterComponent
}: IWithSearchPagination) => {
  const { t } = useTranslation();
  const getLogData = useGetServerLogs(searchParams);
  console.log(getLogData.data);
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
        {startDateFilterComponent("Pick Start Date", "StartDate")}
        {endDateFilterComponent("Pick End Date", "EndDate")}
        {searchComponent("Search logs")}
        <Flex>
          {filterComponent(
            [
              { value: "1", label: "Error" },
              { value: "2", label: "Warning" },
              { value: "3", label: "Debug" },
              { value: "4", label: "Info" },
            ],
            t("Severity"),
            "Severity"
          )}
        </Flex>
      </Flex>

      {/* table section */}
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
                <th>Severity</th>
                <th>Time Stamp</th>
                <th>Message</th>
                <th>Faced By</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {/* <Rows item={getLogData.data}/>
              <Rows /> */}
              {getLogData.data?.items.map((item: IServerLogs) => (
                <Rows item={item} key={item.id} />
              ))}
            </tbody>
          </Table>
        </Paper>
      </ScrollArea>

      {getLogData.data &&
        pagination(getLogData.data?.totalPage, getLogData.data?.items.length)}
    </>
  );
};

export default withSearchPagination(Log);
