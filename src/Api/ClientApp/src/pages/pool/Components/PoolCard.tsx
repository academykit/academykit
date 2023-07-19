import DeleteModal from '@components/Ui/DeleteModal';
import UserShortProfile from '@components/UserShortProfile';
import {
  Button,
  Card,
  Group,
  Menu,
  Text,
  Title,
  createStyles,
  rem,
} from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconDotsVertical, IconEdit, IconTrash } from '@tabler/icons';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import { IPool, useDeleteQuestionPool } from '@utils/services/poolService';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

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
      theme.colorScheme === 'dark' ? theme.colors.dark[5] : theme.colors.gray[2]
    }`,
  },
}));

const PoolCard = ({
  pool: { id: poolId, name, slug, user, questionCount },
  search,
}: {
  pool: IPool;
  search: string;
}) => {
  const { classes } = useStyle();
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
        <Group mt={'sm'}>
          <UserShortProfile user={user} size={'sm'} />
        </Group>
        <Card.Section className={classes.footer}>
          <Group position="apart">
            <div>
              <Text size="xs" color="dimmed">
                {t('total_question')}
              </Text>
              <Text weight={500}>{questionCount}</Text>
            </div>
          </Group>
        </Card.Section>
      </Card>
    </div>
  );
};

export default PoolCard;
