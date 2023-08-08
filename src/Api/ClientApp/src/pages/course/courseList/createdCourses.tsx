import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
import { Box, Container, Flex, Loader } from '@mantine/core';
import { UserRole, CourseStatus } from '@utils/enums';
import { useCourse } from '@utils/services/courseService';
import { useEffect, useState } from 'react'; // Import useState
import CourseList from './component/List';
import { useTranslation } from 'react-i18next';

const ReviewedCourse = ({
  setInitialSearch,
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  useEffect(() => {
    setInitialSearch([
      {
        key: 'Status',
        value: CourseStatus.Review.toString(),
      },
    ]);
  }, []);
  const auth = useAuth();
  const { data, isLoading } = useCourse(searchParams);
  const role = auth?.auth?.role ?? UserRole.Trainee;
  const { t } = useTranslation();

  const [showCourseList, setShowCourseList] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      setShowCourseList(true);
    }, 1000); // 1 second

    return () => clearTimeout(timer);
  }, []);

  return (
    <Container fluid>
      <Container fluid>
        <Flex
          pb={20}
          sx={{
            justifyContent: 'end',
            alignItems: 'center',
          }}
        >
          {searchComponent(t('search_trainings') as string)}
        </Flex>
      </Container>

      {data &&
        data?.items &&
        (data.totalCount >= 1 ? (
          // Show CourseList based on the state value
          showCourseList ? (
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
      {showCourseList && data && pagination(data.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(ReviewedCourse);
