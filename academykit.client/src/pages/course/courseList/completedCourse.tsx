import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
import { Box, Container, Flex, Loader } from '@mantine/core';
import { CourseStatus, UserRole } from '@utils/enums';
import { useCourse } from '@utils/services/courseService';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import CourseList from './component/List';

const CompletedCourseList = ({
  setInitialSearch,
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  useEffect(() => {
    setInitialSearch([
      {
        key: 'Status',
        value: CourseStatus.Completed.toString(),
      },
    ]);
  }, []);
  const auth = useAuth();
  const { data, isLoading } = useCourse(searchParams);
  const role = auth?.auth?.role ?? UserRole.Trainee;
  const { t } = useTranslation();
  const [showCompletedList, setShowCompletedList] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      setShowCompletedList(true);
    }, 1000); // 1 second

    return () => clearTimeout(timer);
  }, []);
  return (
    <Container fluid>
      <Container fluid>
        <Flex
          pb={20}
          style={{
            justifyContent: 'end',
            alignItems: 'center',
          }}
        >
          {searchComponent(t('search_trainings') as string)}
        </Flex>
      </Container>

      {data?.items &&
        (data.totalCount >= 1 ? (
          showCompletedList ? (
            <CourseList
              role={role}
              courses={data.items}
              search={searchParams}
            />
          ) : (
            <Loader />
          )
        ) : (
          <Box>{t('no_trainings_found')}</Box>
        ))}
      {isLoading && <Loader />}
      {showCompletedList &&
        data &&
        pagination(data.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(CompletedCourseList);
