import DeleteModal from "@components/Ui/DeleteModal";
import { Anchor, Button, Menu, Paper, Table, Text } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconDotsVertical, IconEdit, IconTrash } from "@tabler/icons-react";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import { IPool, useDeleteQuestionPool } from "@utils/services/poolService";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const PoolRow = ({
  pool: { id: poolId, name, slug, user, questionCount },
  search,
}: {
  pool: IPool;
  search: string;
}) => {
  const [deleteModal, setDeleteModal] = useToggle();
  const deletePool = useDeleteQuestionPool(poolId, search);
  const { t } = useTranslation();
  const handleDelete = async () => {
    try {
      await deletePool.mutateAsync(poolId);
      showNotification({
        title: t("successful"),
        message: t("question_pool_delete"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        color: "red",
        title: t("error"),
        message: error as string,
      });
    }
    setDeleteModal();
  };

  return (
    <>
      <Table.Tr key={poolId}>
        <Table.Td
          style={{
            minWidth: "122px",
            overflow: "hidden",
            textOverflow: "ellipsis",
          }}
        >
          <Anchor
            size={"lg"}
            lineClamp={1}
            component={Link}
            to={RoutePath.pool.questions(slug).route}
            maw={"80%"}
          >
            <Text truncate>{name}</Text>
          </Anchor>
        </Table.Td>

        <Table.Td>{user?.fullName}</Table.Td>
        <Table.Td>{questionCount}</Table.Td>
        <Table.Td style={{ display: "flex", justifyContent: "center" }}>
          <Menu
            shadow="md"
            width={200}
            trigger="hover"
            withArrow
            position="left"
          >
            <Menu.Target>
              <Button style={{ zIndex: 50 }} variant="subtle" px={4}>
                <IconDotsVertical />
              </Button>
            </Menu.Target>
            <Menu.Dropdown>
              <Menu.Item
                leftSection={<IconEdit size={14} />}
                component={Link}
                to={RoutePath.pool.details(slug).route}
              >
                {t("edit")}
              </Menu.Item>
              <Menu.Divider />
              <Menu.Item
                c="red"
                leftSection={<IconTrash size={14} />}
                onClick={() => setDeleteModal()}
              >
                {t("delete")}
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        </Table.Td>
      </Table.Tr>
      <DeleteModal
        title={t(`pool_delete_confirmation`)}
        open={deleteModal}
        onClose={setDeleteModal}
        onConfirm={handleDelete}
      />
    </>
  );
};

const PoolTable = ({ pool, search }: { pool: IPool[]; search: string }) => {
  const { t } = useTranslation();

  const Rows = () =>
    pool.map((data: IPool) => {
      return <PoolRow pool={data} search={search} key={data.id} />;
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
              <Table.Th>{t("pool_name")}</Table.Th>
              <Table.Th>{t("Creator")}</Table.Th>
              <Table.Th>{t("questions")}</Table.Th>
              <Table.Th></Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>{Rows()}</Table.Tbody>
        </Table>
      </Paper>
    </>
  );
};

export default PoolTable;
