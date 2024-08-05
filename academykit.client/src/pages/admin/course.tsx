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
import { useTranslation } from 'react-i18next';
import CourseRow from './Component/CourseRow';

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
              <Table striped highlightOnHover withTableBorder withColumnBorders>
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>{t('training_name')}</Table.Th>
                    <Table.Th>{t('created_date')}</Table.Th>
                    <Table.Th>{t('author')}</Table.Th>

                    <Table.Th>{t('status')}</Table.Th>
                    <Table.Th>{t('actions')}</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {data.items.map((x) => (
                    <CourseRow course={x} key={x.id} search={searchParams} />
                  ))}
                </Table.Tbody>
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
