import DeleteModal from '@components/Ui/DeleteModal';
import UserShortProfile from '@components/UserShortProfile';
import useAuth from '@hooks/useAuth';
import { Anchor, Button, Card, Group, Menu, Text } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import {
    IconChevronRight,
    IconDotsVertical,
    IconEdit,
    IconTrash,
} from '@tabler/icons-react';
import { UserRole } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import { IGroup, useDeleteGroup } from '@utils/services/groupService';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import classes from '../styles/groupCard.module.css';

const GroupCard = ({ group, search }: { group: IGroup; search: string }) => {
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
        <Group justify="space-between">
          <Anchor
            component={Link}
            to={RoutePath.groups.details(group.slug).route}
            size={'md'}
            lineClamp={1}
            className={classes.anchor}
            maw={'80%'}
          >
            <Text truncate>{group.name}</Text>
          </Anchor>
          {auth?.auth && Number(auth.auth?.role) < UserRole.Trainer && (
            <Menu
              shadow="md"
              width={150}
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
                  to={RoutePath.groups.details(group.slug).route}
                  rightSection={<IconChevronRight size={12} stroke={1.5} />}
                >
                  {t('manage')}
                </Menu.Item>
                <Menu.Divider />
                <Menu.Item
                  c="red"
                  leftSection={<IconTrash size={14} />}
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
          <Group justify="space-between">
            <div>
              <Text size="xs" c="dimmed">
                {t('members')}
              </Text>
              <Anchor
                fw={500}
                component={Link}
                to={RoutePath.groups.members(group.slug).route}
                className={classes.anchor}
              >
                {group.memberCount}
              </Anchor>
            </div>
            <div>
              <Text size="xs" c="dimmed">
                {t('trainings')}
              </Text>
              <Anchor
                fw={500}
                component={Link}
                to={RoutePath.groups.courses(group.slug).route}
                className={classes.anchor}
              >
                {group.courseCount}
              </Anchor>
            </div>
            <div>
              <Text size="xs" c="dimmed">
                {t('attachments')}
              </Text>
              <Anchor
                fw={500}
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
