import DeleteModal from '@components/Ui/DeleteModal';
import UserShortProfile from '@components/UserShortProfile';
import { Anchor, Button, Card, Group, Menu, Text } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconDotsVertical, IconEdit, IconTrash } from '@tabler/icons-react';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import { IPool, useDeleteQuestionPool } from '@utils/services/poolService';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import classes from '../../styles/poolCard.module.css';

const PoolCard = ({
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
        title: t('successful'),
        message: t('question_pool_delete'),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        color: 'red',
        title: t('error'),
        message: error as string,
      });
    }
    setDeleteModal();
  };

  return (
    <div>
      <DeleteModal
        title={t(`pool_delete_confirmation`)}
        open={deleteModal}
        onClose={setDeleteModal}
        onConfirm={handleDelete}
      />
      <Card className={classes.card} p="md" withBorder radius={'md'}>
        <Group justify="space-between">
          <Anchor
            size={'lg'}
            lineClamp={1}
            component={Link}
            to={RoutePath.pool.questions(slug).route}
            maw={'80%'}
          >
            <Text truncate>{name}</Text>
          </Anchor>
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
                {t('edit')}
              </Menu.Item>
              <Menu.Divider />
              <Menu.Item
                c="red"
                leftSection={<IconTrash size={14} />}
                onClick={() => setDeleteModal()}
              >
                {t('delete')}
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        </Group>
        <Group mt={'sm'}>
          <UserShortProfile user={user} size={'sm'} />
        </Group>
        <Card.Section className={classes.footer}>
          <Group justify="space-between">
            <div>
              <Text size="xs" c="dimmed">
                {t('total_question')}
              </Text>
              <Text fw={500}>{questionCount}</Text>
            </div>
          </Group>
        </Card.Section>
      </Card>
    </div>
  );
};

export default PoolCard;
