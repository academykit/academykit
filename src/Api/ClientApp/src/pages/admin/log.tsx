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
              version
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
        {searchComponent("Search logs")}
        <Flex>
          {filterComponent(
            [
              { value: "true", label: t("active") },
              { value: "false", label: t("inactive") },
            ],
            t("Severity"),
            "IsActive"
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
                <th>App Version</th>
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
