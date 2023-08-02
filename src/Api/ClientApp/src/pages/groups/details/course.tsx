import CourseCard from '@components/Course/CourseCard';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
import {
  Box,
  Button,
  Container,
  Flex,
  Group,
  Loader,
  Title,
} from '@mantine/core';
import { UserRole } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import { useGroupCourse } from '@utils/services/groupService';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const GroupCourse = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const { data, isLoading, error } = useGroupCourse(id as string, searchParams);
  const auth = useAuth();
  const { t } = useTranslation();
  if (error) {
    throw error;
  }
  return (
    <Container fluid>
      <Flex
        my={10}
        wrap={'wrap'}
        sx={{ justifyContent: 'space-between', alignItems: 'center' }}
      >
        <Title sx={{ flexGrow: 2 }}>{t('trainings')}</Title>
        <Flex
          sx={{
            justifyContent: 'end',
            alignItems: 'center',
          }}
        >
          {auth?.auth && auth.auth?.role < UserRole.Trainee && (
            <Group position="right" mb={10}>
              <Link to={RoutePath.courses.create + `?group=${id}`}>
                <Button>{t('add_new_training')}</Button>
              </Link>
            </Group>
          )}
        </Flex>
      </Flex>

      {searchComponent(t('search_group_trainings') as string)}
      <Flex wrap="wrap" mt={15}>
        {data?.items &&
          (data.totalCount > 0 ? (
            data.items.map((x) => (
              <div key={x.id}>
                <CourseCard course={x} />
              </div>
            ))
          ) : (
            <Box>{t('no_trainings_found')}</Box>
          ))}
      </Flex>
      {isLoading && <Loader />}
      {data &&
        data.totalPage > 1 &&
        pagination(data?.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(GroupCourse);
