/* eslint-disable */
import { useReAuth } from '@utils/services/authService';
import { useCourse } from '@utils/services/courseService';
import {
  Title,
  Anchor,
  Badge,
  Tooltip,
  ActionIcon,
  Paper,
  Table,
  Box,
  Loader,
  Pagination,
  ScrollArea,
} from '@mantine/core';
import { IconEdit } from '@tabler/icons';
import RoutePath from '@utils/routeConstants';
import { CourseLanguage, UserRole } from '@utils/enums';
import moment from 'moment';
import { Link, useParams } from 'react-router-dom';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { DATE_FORMAT } from '@utils/constants';

const MyTrainings = () => {
  const auth = useReAuth();
  const [page, setPage] = useState(1);
  const { id } = useParams();
  const { t } = useTranslation();
  const authorCourse = useCourse(
    `UserId=${id}&Enrollmentstatus=1&size=12&page=${page}`
  );

  return (
    <>
      {auth.data && auth.data?.role !== UserRole.Trainee && (
        <div>
          <Title mt={20} size={30} mb={10}>
            {t('my_trainings')}
          </Title>

          <ScrollArea>
            <Paper>
              <Table striped>
                <thead>
                  <tr>
                    <th>{t('title')}</th>
                    <th>{t('created_date')}</th>
                    <th>
                      {t('Language')} / {t('level')}
                    </th>
                    <th>{t('action')}</th>
                  </tr>
                </thead>
                <tbody>
                  {authorCourse.data &&
                    authorCourse.data.totalCount > 0 &&
                    authorCourse.data.items.map((x) => (
                      <tr key={x.id}>
                        <td>
                          <Anchor
                            component={Link}
                            to={RoutePath.courses.description(x.slug).route}
                          >
                            {x.name}
                          </Anchor>
                        </td>
                        <td>{moment(x.createdOn).format(DATE_FORMAT)}</td>
                        <td>
                          <Badge color="pink" variant="light">
                            {CourseLanguage[x.language]}
                          </Badge>{' '}
                          /
                          <Badge color="blue" variant="light">
                            {x?.levelName}
                          </Badge>
                        </td>
                        <td>
                          <Tooltip label={t('edit_this_course')}>
                            <ActionIcon
                              component={Link}
                              to={RoutePath.manageCourse.edit(x.slug).route}
                            >
                              <IconEdit />
                            </ActionIcon>
                          </Tooltip>
                        </td>
                      </tr>
                    ))}
                  {authorCourse.isLoading && <Loader />}
                </tbody>
              </Table>
            </Paper>
          </ScrollArea>

          {
            authorCourse.data && authorCourse.data.totalPage > 1 && (
              <Pagination
                my={20}
                total={authorCourse.data.totalPage}
                value={page}
                onChange={setPage}
              />
            )
            // pagination(authorCourse.data.totalPage)
          }

          {authorCourse.data && authorCourse.data.totalCount === 0 && (
            <Box mt={5}>{t('no_trainings_found')}</Box>
          )}
        </div>
      )}
    </>
  );
};

export default MyTrainings;
