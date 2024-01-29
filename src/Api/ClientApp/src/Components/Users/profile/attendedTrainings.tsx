import ProgressBar from '@components/Ui/ProgressBar';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import { Anchor, Box, Loader, Paper, Table, Title } from '@mantine/core';
import { DATE_FORMAT } from '@utils/constants';
import RoutePath from '@utils/routeConstants';
import { useMyCourse } from '@utils/services/courseService';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const AttendedTrainings = ({
  searchParams,
  pagination,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const { data, isLoading } = useMyCourse(id as string, searchParams);
  const { t } = useTranslation();

  return (
    <div>
      <Title mt={10} size={30} mb={10}>
        {t('attended_trainings')}{' '}
      </Title>
      <Paper>
        {data && data.totalCount > 0 && (
          <Table striped withTableBorder withColumnBorders highlightOnHover>
            <Table.Thead>
              <Table.Tr>
                <Table.Th>{t('title')}</Table.Th>
                <Table.Th>{t('enrolled_date')}</Table.Th>
                <Table.Th>{t('progress')}</Table.Th>
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {data.items.map((x) => (
                <Table.Tr key={x.id}>
                  <Table.Td>
                    <Anchor
                      component={Link}
                      to={RoutePath.courses.description(x.slug).route}
                    >
                      {x.name}
                    </Anchor>
                  </Table.Td>
                  <Table.Td>{moment(x.createdOn).format(DATE_FORMAT)}</Table.Td>

                  <Table.Td>
                    <ProgressBar total={100} positive={x.percentage} />
                  </Table.Td>
                </Table.Tr>
              ))}
              {isLoading && <Loader />}
            </Table.Tbody>
          </Table>
        )}
      </Paper>
      {data &&
        data.totalPage > 1 &&
        pagination(data.totalPage, data.items.length)}
      {data && data.totalCount === 0 && (
        <Box mt={5}>{t('no_trainings_found')}</Box>
      )}
    </div>
  );
};

export default withSearchPagination(AttendedTrainings);
