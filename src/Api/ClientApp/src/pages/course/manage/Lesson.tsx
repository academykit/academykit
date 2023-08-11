import ProgressBar from '@components/Ui/ProgressBar';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import {
  Table,
  ScrollArea,
  Badge,
  Paper,
  Center,
  Tooltip,
  Button,
  Anchor,
  Loader,
  Box,
  // Box,
} from '@mantine/core';
import { IconEye } from '@tabler/icons';
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
    <tr key={item?.id}>
      <td style={{ maxWidth: '200px' }}>
        <Anchor
          component={Link}
          to={`${RoutePath.classes}/${course_id}/${item.slug}`}
        >
          {item.name}
        </Anchor>
      </td>
      <td>{t(`${LessonType[item.lessonType]}`)}</td>
      <td>
        <ProgressBar
          total={item?.enrolledStudent}
          positive={item?.lessonWatched}
        />
      </td>
      <td>
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
      </td>
      <td>
        <Center>
          <Tooltip
            style={{
              maxWidth: '400px',
              textOverflow: 'ellipsis',
              overflow: 'hidden',
            }}
            label={`${t('view_details_for')} ${item.name} ${t('lesson')}`}
          >
            <Button component={Link} variant="subtle" to={`${item.slug}`}>
              <IconEye />
            </Button>
          </Tooltip>
        </Center>
      </td>
    </tr>
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
          sx={{ minWidth: 800 }}
          verticalSpacing="xs"
          styles={{}}
          striped
          highlightOnHover
          withBorder
          withColumnBorders
        >
          <thead>
            <tr>
              <th>{t('lesson_name')}</th>
              <th>{t('lesson_type')}</th>
              <th>
                <Center>{t('progress')}</Center>
              </th>
              <th>
                <Center>{t('is_mandatory')}</Center>
              </th>
              <th>
                <Center>{t('action')}</Center>
              </th>
            </tr>
          </thead>
          <tbody>
            {getLessonStatistics.data?.items?.map((item: ILessonStats) => (
              <Rows item={item} key={item.id} course_id={course_id} />
            ))}
          </tbody>
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
