import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import {
  Box,
  Container,
  Group,
  Loader,
  Paper,
  ScrollArea,
  Table,
} from '@mantine/core';
import { useCourse } from '@utils/services/courseService';
import CourseRow from './Component/CourseRow';
import { useTranslation } from 'react-i18next';

const AdminCourseList = ({
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  const { data, isLoading, isError } = useCourse(searchParams);
  const { t } = useTranslation();
  return (
    <Container fluid>
      <Group my={10}>{t('all_trainings_list')}</Group>
      {searchComponent(t('search_courses') as string)}

      <ScrollArea>
        {data &&
          (data.totalCount > 0 ? (
            <Paper mt={10}>
              <Table striped highlightOnHover withBorder>
                <thead>
                  <tr>
                    <th>{t('training_name')}</th>
                    <th>{t('created_date')}</th>
                    <th>{t('author')}</th>

                    <th>{t('status')}</th>
                    <th>{t('actions')}</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map((x) => (
                    <CourseRow course={x} key={x.id} search={searchParams} />
                  ))}
                </tbody>
              </Table>
            </Paper>
          ) : (
            <Box>{t('no_trainings_found')}</Box>
          ))}
      </ScrollArea>
      {isLoading && <Loader />}
      {isError && <Box>{t('something_went_wrong')}</Box>}
      {data && pagination(data.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(AdminCourseList);
