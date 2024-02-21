import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useAuth from '@hooks/useAuth';
import { Flex, SimpleGrid } from '@mantine/core';
import { UserRole } from '@utils/enums';
import { useAssessments } from '@utils/services/assessmentService';
import { useTranslation } from 'react-i18next';
import AssessmentCard from './component/AssessmentCard';

const AssessmentList = ({
  searchComponent,
  searchParams,
  pagination,
  filterComponent,
}: IWithSearchPagination) => {
  const { t } = useTranslation();
  const assessmentData = useAssessments(searchParams);
  const auth = useAuth();

  return (
    <>
      <Flex pb={20} justify={'end'} align={'center'}>
        {searchComponent(t('search_assessments') as string)}
        {auth?.auth?.role !== UserRole.Trainee &&
          filterComponent(
            [
              { value: '1', label: t('Draft') },
              { value: '2', label: t('review') },
              { value: '3', label: t('Published') },
              { value: '5', label: t('rejected') },
            ],
            t('assessment_status'),
            'assessmentStatus'
          )}
      </Flex>

      <SimpleGrid cols={{ base: 1, sm: 2, lg: 3 }}>
        {assessmentData.data?.items.map((assessmentData) => (
          <AssessmentCard
            key={assessmentData.id}
            data={assessmentData}
            userRole={auth?.auth?.role as UserRole}
            currentUser={(auth?.auth?.id as string) ?? ''}
          />
        ))}
      </SimpleGrid>

      {assessmentData.data &&
        pagination(
          assessmentData.data?.totalPage,
          assessmentData.data?.items.length
        )}
    </>
  );
};

export default withSearchPagination(AssessmentList);
