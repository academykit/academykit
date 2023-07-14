import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
import { Box, Container, Flex, Loader } from '@mantine/core';
import { UserRole } from '@utils/enums';
import { useCourse } from '@utils/services/courseService';
import { CourseUserStatus } from '@utils/enums';

import CourseList from './component/List';
import { useTranslation } from 'react-i18next';

const CoursePage = ({
  filterComponent,
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { data, isSuccess, isLoading } = useCourse(searchParams);
  const auth = useAuth();
  const role = auth?.auth?.role ?? UserRole.Trainee;
  const { t } = useTranslation();

  const filterValue = [
    {
      value: CourseUserStatus.Author.toString(),
      label: t('author'),
    },
    {
      value: CourseUserStatus.Enrolled.toString(),
      label: t('enrolled'),
    },
    {
      value: CourseUserStatus.NotEnrolled.toString(),
      label: t('not_enrolled'),
    },
    {
      value: CourseUserStatus.Teacher.toString(),
      label: t('trainer'),
    },
  ];
  return (
    <Container fluid>
      <Container fluid>
        <Flex pb={20} justify={'end'} align={'center'}>
          {searchComponent(t('search_trainings') as string)}
          {filterComponent(
            filterValue,
            t('enrollment_status'),
            'Enrollmentstatus'
          )}
        </Flex>
      </Container>
      {data &&
        data?.items &&
        (data.totalCount >= 1 ? (
          <CourseList role={role} courses={data.items} search={searchParams} />
        ) : (
          <Box>{t('no_trainings_found')}</Box>
        ))}
      {isLoading && <Loader />}
      {data && pagination(data.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(CoursePage);
