import TrainingTable from '@components/Course/TrainingTable';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
import {
  Box,
  Center,
  Container,
  Flex,
  Group,
  Loader,
  rem,
  SegmentedControl,
} from '@mantine/core';
import { IconList, IconTable } from '@tabler/icons-react';
import { CourseUserStatus, UserRole } from '@utils/enums';
import { useCourse } from '@utils/services/courseService';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import CourseList from './component/List';

const CoursePage = ({
  filterComponent,
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { data, isLoading } = useCourse(searchParams);
  const [selectedView, setSelectedView] = useState('list');
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

      <Group justify="flex-end" my={30}>
        <SegmentedControl
          value={selectedView}
          onChange={setSelectedView}
          data={[
            {
              value: 'list',
              label: (
                <Center style={{ gap: 10 }}>
                  <IconList style={{ width: rem(16), height: rem(16) }} />
                </Center>
              ),
            },
            {
              value: 'table',
              label: (
                <Center style={{ gap: 10 }}>
                  <IconTable style={{ width: rem(16), height: rem(16) }} />
                </Center>
              ),
            },
          ]}
        />
      </Group>

      {data &&
        data?.items &&
        (data.totalCount >= 1 ? (
          selectedView === 'table' ? (
            <TrainingTable courses={data?.items} search={searchParams} />
          ) : (
            <CourseList
              role={role}
              courses={data.items}
              search={searchParams}
            />
          )
        ) : (
          <Box>{t('no_trainings_found')}</Box>
        ))}
      {isLoading && <Flex justify={'center'}>{<Loader mx={'auto'} />}</Flex>}
      {selectedView === 'table' &&
        data &&
        pagination(data.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(CoursePage);
