import DeleteModal from '@components/Ui/DeleteModal';
import UserShortProfile from '@components/UserShortProfile';
import { Button, Card, Group, Menu, Text, Title } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconDotsVertical, IconEdit, IconTrash } from '@tabler/icons';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import { IPool, useDeleteQuestionPool } from '@utils/services/poolService';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

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
    <div
      style={{
        position: 'relative',
      }}
    >
      <Link
        style={{ textDecoration: 'none' }}
        to={RoutePath.pool.questions(slug).route}
      >
        <div
          style={{
            position: 'absolute',
            zIndex: 10,
            width: '100%',
            height: '100%',
          }}
        ></div>
      </Link>
      <Card my={10} radius={'lg'}>
        <DeleteModal
          title={t(`pool_delete_confirmation`)}
          open={deleteModal}
          onClose={setDeleteModal}
          onConfirm={handleDelete}
        />

        <Group position="apart">
          <Title size={'lg'} lineClamp={1} w={'80%'}>
            {name}
          </Title>
          <Menu
            shadow="md"
            width={200}
            trigger="hover"
            withArrow
            position="right"
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
                to={RoutePath.pool.details(slug).route}
              >
                {t('edit')}
              </Menu.Item>
              <Menu.Divider />
              <Menu.Item
                color="red"
                icon={<IconTrash size={14} />}
                onClick={() => setDeleteModal()}
              >
                {t('delete')}
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        </Group>
        <Group py={5} position="apart">
          <div style={{ zIndex: 20 }}>
            <UserShortProfile size={'sm'} user={user} />
          </div>
          <Text color={'dimmed'} size={'sm'}>
            {t('total_question')} {questionCount}
          </Text>
        </Group>
      </Card>
    </div>
  );
};

export default PoolCard;
