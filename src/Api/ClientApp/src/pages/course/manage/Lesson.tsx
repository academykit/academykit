import ProgressBar from '@components/Ui/ProgressBar';
import withSearchPagination, {
    IWithSearchPagination,
} from '@hoc/useSearchPagination';
import {
    Anchor,
    Badge,
    Box,
    Button,
    Center,
    Loader,
    Paper,
    ScrollArea,
    Table,
    Tooltip,
} from '@mantine/core';
import { IconEye } from '@tabler/icons-react';
import { LessonType } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import {
    ILessonStats,
    useGetLessonStatistics,
} from '@utils/services/manageCourseService';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const Rows = ({
  item,
  course_id,
}: {
  item: ILessonStats;
  course_id: string;
}) => {
  const { t } = useTranslation();
  return (
    <Table.Tr key={item?.id}>
      <Table.Td style={{ maxWidth: '200px' }}>
        <Anchor
          component={Link}
          to={`${RoutePath.classes}/${course_id}/${item.slug}`}
        >
          {item.name}
        </Anchor>
      </Table.Td>
      <Table.Td>{t(`${LessonType[item.lessonType]}`)}</Table.Td>
      <Table.Td>
        <ProgressBar
          total={item?.enrolledStudent}
          positive={item?.lessonWatched}
        />
      </Table.Td>
      <Table.Td>
        <Center>
          {item?.isMandatory ? (
            <Badge color="green" variant="outline">
              {t('yes')}
            </Badge>
          ) : (
            <Badge color="red" variant="outline">
              {t('no')}
            </Badge>
          )}
        </Center>
      </Table.Td>
      <Table.Td>
        <Center>
          <Tooltip
            style={{
              maxWidth: '400px',
              textOverflow: 'ellipsis',
              overflow: 'hidden',
            }}
            label={`${t('view_details_for')} ${item.name} ${t('lesson')}`}
          >
            <Button
              component={Link}
              variant="subtle"
              to={`${item.slug}`}
              state={{
                lessonType: item.lessonType,
                lessonName: item.name,
              }}
            >
              <IconEye />
            </Button>
          </Tooltip>
        </Center>
      </Table.Td>
    </Table.Tr>
  );
};

function TableReviews({ searchParams, pagination }: IWithSearchPagination) {
  const slug = useParams();
  const course_id = slug.id as string;

  const getLessonStatistics = useGetLessonStatistics(course_id, searchParams);

  const { t } = useTranslation();

  if (getLessonStatistics.isLoading) return <Loader />;

  if (
    getLessonStatistics.data &&
    getLessonStatistics.data.items &&
    getLessonStatistics.data?.items?.length === 0
  )
    return <Box>{t('no_lessons')}</Box>;

  return (
    <ScrollArea>
      <Paper>
        <Table
          cellSpacing={''}
          style={{ minWidth: 800 }}
          verticalSpacing="xs"
          styles={{}}
          striped
          highlightOnHover
          withTableBorder
          withColumnBorders
        >
          <Table.Thead>
            <Table.Tr>
              <Table.Th>{t('lesson_name')}</Table.Th>
              <Table.Th>{t('lesson_type')}</Table.Th>
              <Table.Th>
                <Center>{t('progress')}</Center>
              </Table.Th>
              <Table.Th>
                <Center>{t('is_mandatory')}</Center>
              </Table.Th>
              <Table.Th>
                <Center>{t('action')}</Center>
              </Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {getLessonStatistics.data?.items?.map((item: ILessonStats) => (
              <Rows item={item} key={item.id} course_id={course_id} />
            ))}
          </Table.Tbody>
        </Table>
      </Paper>

      {getLessonStatistics.data &&
        getLessonStatistics.data.items &&
        pagination(
          getLessonStatistics.data?.totalPage,
          getLessonStatistics.data.items.length
        )}
    </ScrollArea>
  );
}
export default withSearchPagination(TableReviews);
