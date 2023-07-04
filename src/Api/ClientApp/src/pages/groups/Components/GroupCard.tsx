import {
  Anchor,
  Box,
  Button,
  Card,
  createStyles,
  Group,
  Paper,
  Popover,
  Title,
  NavLink,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconChevronRight, IconDotsVertical } from "@tabler/icons";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import { IGroup, useDeleteGroup } from "@utils/services/groupService";
import { Link } from "react-router-dom";
import DeleteModal from "@components/Ui/DeleteModal";
import useAuth from "@hooks/useAuth";
import { UserRole } from "@utils/enums";
import { useTranslation } from "react-i18next";

const useStyle = createStyles((theme) => ({
  wrapper: {
    position: "relative",
    minWidth: "300px",
    [theme.fn.largerThan(450)]: {
      minWidth: "24rem",
    },
  },
}));
const GroupCard = ({ group, search }: { group: IGroup; search: string }) => {
  const { classes, theme } = useStyle();
  const [deleteModal, setDeleteModal] = useToggle();
  const deleteGroup = useDeleteGroup(group.id, search);
  const auth = useAuth();
  const { t } = useTranslation();

  const handleDelete = async () => {
    try {
      await deleteGroup.mutateAsync({ id: group.id });
      showNotification({
        title: t("successful"),
        message: t("group_deleted"),
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        color: "red",
        title: t("error"),
        message: err as string,
      });
    }
    setDeleteModal();
  };

  return (
    <div style={{ position: "relative" }}>
      <Link
        to={RoutePath.groups.details(group.slug).route}
        style={{
          textDecoration: "none",
          zIndex: 5,
          position: "absolute",
          height: "100%",
          width: "100%",
        }}
      ></Link>
      <Card
        className={classes.wrapper}
        key={group.id}
        withBorder
        radius={"md"}
        sx={{ maxWidth: 200 }}
      >
        <DeleteModal
          title={`${t("want_to_delete")} "${group.name}" ${t("group")}${t(
            "?"
          )}`}
          open={deleteModal}
          onClose={setDeleteModal}
          onConfirm={handleDelete}
        />
        <Group position="apart">
          <Title size={22} lineClamp={1} w={"80%"}>
            {group.name}
          </Title>
          {auth?.auth && auth.auth?.role < UserRole.Trainee && (
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
                  {/* <Group
                    p={0}
                    sx={{
                      flexDirection: "column",
                      alignItems: "start",
                    }}
                  > */}
                  <NavLink
                    variant="subtle"
                    label={t("edit")}
                    component={Link}
                    to={RoutePath.groups.details(group.slug).route}
                    rightSection={<IconChevronRight size={12} stroke={1.5} />}
                  ></NavLink>

                  <NavLink
                    onClick={() => setDeleteModal()}
                    variant="subtle"
                    label={t("delete")}
                    component={"button"}
                    rightSection={<IconChevronRight size={12} stroke={1.5} />}
                  ></NavLink>
                  {/* </Group> */}
                </Paper>
              </Popover.Dropdown>
            </Popover>
          )}
        </Group>
        <Box style={{ zIndex: "10", position: "relative" }}>
          <Anchor
            style={{ width: "100%", display: "inline-block" }}
            component={Link}
            to={RoutePath.groups.members(group.slug).route}
          >
            {group.memberCount} {t("members")}
          </Anchor>
        </Box>
        <Box style={{ zIndex: "10", position: "relative" }}>
          <Anchor
            style={{ width: "100%", display: "inline-block" }}
            component={Link}
            to={RoutePath.groups.courses(group.slug).route}
          >
            {group.courseCount} {t("trainings")}
          </Anchor>
        </Box>
        <Box style={{ zIndex: "10", position: "relative" }}>
          <Anchor
            style={{ width: "100%", display: "inline-block" }}
            component={Link}
            to={RoutePath.groups.attachments(group.slug).route}
          >
            {group.attachmentCount} {t("attachments")}
          </Anchor>
        </Box>
      </Card>
    </div>
  );
};

export default GroupCard;
