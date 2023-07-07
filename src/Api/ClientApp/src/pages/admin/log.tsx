import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import { Group, Button, Title, Paper, Table, Text, Flex, ScrollArea } from "@mantine/core";
import { useTranslation } from "react-i18next";

const Rows = () => {
  return (
    <>
      <tr>
        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              Severity
            </Text>
          </Group>
        </td>

        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              TimeStamp
            </Text>
          </Group>
        </td>
        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              App Version
            </Text>
          </Group>
        </td>
        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              Message
            </Text>
          </Group>
        </td>
        <td>
          <Group spacing="sm">
            <Text size="sm" weight={500}>
              Faced By
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
              <Rows />
              <Rows />
              {/* {getDepartment.data?.items.map((item: any) => (
                <Rows item={item} key={item.id} />
              ))} */}
            </tbody>
          </Table>
        </Paper>
      </ScrollArea>
    </>
  );
};

export default withSearchPagination(Log);
