import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
import {
  Box,
  Button,
  Container,
  Drawer,
  Group,
  Loader,
  SimpleGrid,
  Title,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { UserRole } from '@utils/enums';
import { useGroups } from '@utils/services/groupService';

import AddGroups from './Components/AddGroups';
import GroupCard from './Components/GroupCard';
import { useTranslation } from 'react-i18next';

const GroupsPage = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const [opened, { open, close }] = useDisclosure(false);

  const { isLoading, data } = useGroups(searchParams);
  const auth = useAuth();
  const { t } = useTranslation();
  return (
    <Container fluid>
      <Box my={10}>
        <Group
          sx={{ justifyContent: 'space-between', alignItems: 'center' }}
          mb={15}
        >
          <Title>{t('groups')}</Title>

          {auth?.auth && auth.auth.role <= UserRole.Admin && (
            <Button onClick={open}>{t('add_group')}</Button>
          )}
        </Group>

        <Drawer
          opened={opened}
          onClose={close}
          title={t('groups')}
          overlayProps={{ opacity: 0.5, blur: 4 }}
        >
          <AddGroups onCancel={close} />
        </Drawer>

        <div>
          <Box>{searchComponent(t('search_groups') as string)}</Box>
        </div>
      </Box>
      <SimpleGrid
        cols={1}
        spacing={10}
        breakpoints={[
          { minWidth: 'sx', cols: 1 },
          { minWidth: 'sm', cols: 2 },
          { minWidth: 'md', cols: 3 },
          { minWidth: 1280, cols: 3 },
          { minWidth: 1780, cols: 4 },
        ]}
      >
        {isLoading && <Loader />}
        {data?.data &&
          (data.data.totalCount > 0 ? (
            data?.data.items.map((x) => (
              <GroupCard search={searchParams} group={x} key={x.id} />
            ))
          ) : (
            <Box>{t('no_groups')}</Box>
          ))}
      </SimpleGrid>
      {data && pagination(data.data.totalPage, data.data.items.length)}
    </Container>
  );
};

export default withSearchPagination(GroupsPage);
