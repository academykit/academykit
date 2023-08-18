import {
  Anchor,
  Button,
  Card,
  createStyles,
  Group,
  Menu,
  rem,
  Text,
} from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import {
  IconChevronRight,
  IconDotsVertical,
  IconEdit,
  IconTrash,
} from '@tabler/icons';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import { IGroup, useDeleteGroup } from '@utils/services/groupService';
import { Link } from 'react-router-dom';
import DeleteModal from '@components/Ui/DeleteModal';
import useAuth from '@hooks/useAuth';
import { UserRole } from '@utils/enums';
import { useTranslation } from 'react-i18next';
import UserShortProfile from '@components/UserShortProfile';

const useStyle = createStyles((theme) => ({
  card: {
    transition: 'transform 150ms ease, box-shadow 150ms ease',
    '&:hover': {
      transform: 'scale(1.01)',
      boxShadow: theme.shadows.md,
    },
  },
  footer: {
    padding: `${theme.spacing.xs} ${theme.spacing.lg}`,
    marginTop: theme.spacing.sm,
    borderTop: `${rem(1)} solid ${
      theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[2]
    }`,
  },
  anchor: {
    color: theme.colors[theme.primaryColor][7],
  },
}));
const GroupCard = ({ group, search }: { group: IGroup; search: string }) => {
  const { classes } = useStyle();
  const [deleteModal, setDeleteModal] = useToggle();
  const deleteGroup = useDeleteGroup(group.id, search);
  const auth = useAuth();
  const { t } = useTranslation();

  const handleDelete = async () => {
    try {
      await deleteGroup.mutateAsync({ id: group.id });
      showNotification({
        title: t('successful'),
        message: t('group_deleted'),
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        color: 'red',
        title: t('error'),
        message: err as string,
      });
    }
    setDeleteModal();
  };

  return (
    <div>
      <DeleteModal
        title={`${
          group.memberCount > 0
            ? t('Delete_group_withMember')
            : t('want_to_delete') + ' ' + group.name + ' ' + t('group') + t('?')
        }`}
        open={deleteModal}
        onClose={setDeleteModal}
        onConfirm={handleDelete}
      />
      <Card
        className={classes.card}
        p="md"
        key={group.id}
        withBorder
        radius={'md'}
      >
        <Group position="apart">
          <Anchor
            component={Link}
            to={RoutePath.groups.details(group.slug).route}
            size={22}
            lineClamp={1}
            className={classes.anchor}
          >
            {group.name}
          </Anchor>
          {auth?.auth && auth.auth?.role < UserRole.Trainer && (
            <Menu
              shadow="md"
              width={150}
              trigger="hover"
              withArrow
              position="left"
            >
              <Menu.Target>
                <Button sx={{ zIndex: 50 }} variant="subtle" px={4}>
                  <IconDotsVertical />
                </Button>
              </Menu.Target>
              <Menu.Dropdown>
                <Menu.Item
                  icon={<IconEdit size={14} />}
                  component={Link}
                  to={RoutePath.groups.details(group.slug).route}
                  rightSection={<IconChevronRight size={12} stroke={1.5} />}
                >
                  {t('edit')}
                </Menu.Item>
                <Menu.Divider />
                <Menu.Item
                  color="red"
                  icon={<IconTrash size={14} />}
                  onClick={(e) => {
                    e.preventDefault();
                    setDeleteModal();
                  }}
                >
                  {t('delete')}
                </Menu.Item>
              </Menu.Dropdown>
            </Menu>
          )}
        </Group>
        <Group mt={'sm'}>
          <UserShortProfile user={group.user} size={'sm'} />
        </Group>
        <Card.Section className={classes.footer}>
          <Group position="apart">
            <div>
              <Text size="xs" color="dimmed">
                {t('members')}
              </Text>
              <Anchor
                weight={500}
                component={Link}
                to={RoutePath.groups.members(group.slug).route}
                className={classes.anchor}
              >
                {group.memberCount}
              </Anchor>
            </div>
            <div>
              <Text size="xs" color="dimmed">
                {t('trainings')}
              </Text>
              <Anchor
                weight={500}
                component={Link}
                to={RoutePath.groups.courses(group.slug).route}
                className={classes.anchor}
              >
                {group.courseCount}
              </Anchor>
            </div>
            <div>
              <Text size="xs" color="dimmed">
                {t('attachments')}
              </Text>
              <Anchor
                weight={500}
                component={Link}
                to={RoutePath.groups.attachments(group.slug).route}
                className={classes.anchor}
              >
                {group.attachmentCount}
              </Anchor>
            </div>
          </Group>
        </Card.Section>
      </Card>
    </div>
  );
};

export default GroupCard;
