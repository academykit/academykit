import DeleteModal from "@components/Ui/DeleteModal";
import UserShortProfile from "@components/UserShortProfile";
import {
  Button,
  Card,
  Group,
  NavLink,
  Paper,
  Popover,
  Text,
  Title,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconChevronRight, IconDotsVertical } from "@tabler/icons";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import { IPool, useDeleteQuestionPool } from "@utils/services/poolService";
import { Link } from "react-router-dom";

const PoolCard = ({
  pool: { id: poolId, name, slug, user, questionCount },
  search,
}: {
  pool: IPool;
  search: string;
}) => {
  const [deleteModal, setDeleteModal] = useToggle();
  const deletePool = useDeleteQuestionPool(poolId, search);
  const handleDelete = async () => {
    try {
      await deletePool.mutateAsync(poolId);
      showNotification({
        title: "Success",
        message: "Question Pool Deleted successfully.",
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        color: "red",
        title: "Error",
        message: error as string,
      });
    }
    setDeleteModal();
  };

  return (
    <div
      style={{
        position: "relative",
      }}
    >
      <Link
        style={{ textDecoration: "none" }}
        to={RoutePath.pool.questions(slug).route}
      >
        <div
          style={{
            position: "absolute",
            zIndex: 10,
            width: "100%",
            height: "100%",
          }}
        ></div>
      </Link>
      <Card my={10} radius={"lg"}>
        <DeleteModal
          title={`Do you want to delete this question pool?`}
          open={deleteModal}
          onClose={setDeleteModal}
          onConfirm={handleDelete}
        />

        <Group position="apart">
          <Title size={"lg"} lineClamp={1} w={"80%"}>
            {name}
          </Title>
          <Popover
            position={"left-start"}
            arrowSize={12}
            styles={{
              dropdown: { padding: 5 },
            }}
          >
            <Popover.Target>
              <Button sx={{ zIndex: 50 }} variant="subtle" px={4}>
                <IconDotsVertical />
              </Button>
            </Popover.Target>
            <Popover.Dropdown>
              <Paper>
                <Group
                  p={0}
                  sx={{
                    flexDirection: "column",
                    alignItems: "start",
                  }}
                >
                  <NavLink
                    variant="subtle"
                    label="Edit"
                    component={Link}
                    to={RoutePath.pool.details(slug).route}
                    rightSection={<IconChevronRight size={12} stroke={1.5} />}
                  ></NavLink>

                  <NavLink
                    onClick={() => setDeleteModal()}
                    variant="subtle"
                    label="Delete"
                    component={"button"}
                    rightSection={<IconChevronRight size={12} stroke={1.5} />}
                  ></NavLink>
                </Group>
              </Paper>
            </Popover.Dropdown>
          </Popover>
        </Group>
        <Group py={5} position="apart">
          <div style={{ zIndex: 20 }}>
            <UserShortProfile size={"sm"} user={user} />
          </div>
          <Text color={"dimmed"} size={"sm"}>
            Total Questions: {questionCount}
          </Text>
        </Group>
      </Card>
    </div>
  );
};

export default PoolCard;
