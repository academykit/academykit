import {
  Anchor,
  Badge,
  Button,
  Group,
  Menu,
  Paper,
  Table,
} from '@mantine/core';
import {
  IconChevronRight,
  IconDotsVertical,
  IconGraph,
  IconSettings,
  IconTrash,
  IconUsers,
} from '@tabler/icons-react';
import { DATE_FORMAT } from '@utils/constants';
import { CourseLanguage } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import { ICourse } from '@utils/services/courseService';
import { t } from 'i18next';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const TrainingRow = ({ item }: { item: ICourse }) => {
  return (
    <Table.Tr key={item?.id}>
      <Table.Td
        style={{
          minWidth: '122px',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
        }}
      >
        {
          <Anchor
            component={Link}
            to={RoutePath.courses.description(item?.slug).route}
            size="lg"
            fw="bold"
            maw={{ base: '100%', md: 400, lg: 600 }}
            style={{
              whiteSpace: 'nowrap',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
            }}
          >
            {item?.name ?? ''}
          </Anchor>
        }
      </Table.Td>
      <Table.Td>
        <Badge
          h={34}
          title={t('group')}
          component={Link}
          to={'/groups/' + item.groupId}
          style={{ maxWidth: '230px', cursor: 'pointer' }}
          leftSection={<IconUsers size={14} />}
          variant="light"
        >
          {item.groupName}
        </Badge>{' '}
      </Table.Td>
      <Table.Td>
        {item?.language && (
          <Badge color="pink" variant="light">
            {t(`${CourseLanguage[item.language]}`)}
          </Badge>
        )}
      </Table.Td>
      <Table.Td>
        <Badge color="blue" variant="light">
          {item?.levelName}
        </Badge>
      </Table.Td>
      <Table.Td>
        {item?.createdOn && moment(item.createdOn).format(DATE_FORMAT)}
      </Table.Td>
      <Table.Td style={{ display: 'flex', justifyContent: 'center' }}>
        <Menu
          shadow="md"
          width={200}
          trigger="hover"
          withArrow
          position="right"
        >
          <Menu.Target>
            <Group>
              <Button style={{ zIndex: 50 }} variant="subtle" px={4}>
                <IconDotsVertical />
              </Button>
            </Group>
          </Menu.Target>
          <Menu.Dropdown>
            <Menu.Label>{t('manage')}</Menu.Label>
            <Menu.Item
              leftSection={<IconSettings size={14} />}
              component={Link}
              to={RoutePath.manageCourse.manage(item.slug).route}
              rightSection={<IconChevronRight size={12} stroke={1.5} />}
            >
              {t('statistics')}
            </Menu.Item>
            <Menu.Item
              leftSection={<IconGraph size={14} />}
              component={Link}
              to={RoutePath.manageCourse.lessonsStat(item.slug).route}
              rightSection={<IconChevronRight size={12} stroke={1.5} />}
            >
              {t('lesson_stats')}
            </Menu.Item>
            <Menu.Item
              leftSection={<IconUsers size={14} />}
              component={Link}
              to={RoutePath.manageCourse.student(item.slug).route}
              rightSection={<IconChevronRight size={12} stroke={1.5} />}
            >
              {t('trainee')}
            </Menu.Item>
            <Menu.Divider />
            <Menu.Item c="red" leftSection={<IconTrash size={14} />}>
              {t('delete')}
            </Menu.Item>
          </Menu.Dropdown>
        </Menu>
      </Table.Td>
    </Table.Tr>
  );
};

const TrainingTable = ({ courses }: { courses: ICourse[]; search: string }) => {
  const { t } = useTranslation();

  const Rows = () =>
    courses.map((item: any) => {
      return <TrainingRow item={item} key={item.id} />;
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
              <Table.Th>{t('title')}</Table.Th>
              <Table.Th>{t('group')}</Table.Th>
              <Table.Th>{t('Language')}</Table.Th>
              <Table.Th>{t('level')}</Table.Th>
              <Table.Th>{t('created_date')}</Table.Th>
              <Table.Th></Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>{Rows()}</Table.Tbody>
        </Table>
      </Paper>
    </>
  );
};

export default TrainingTable;
