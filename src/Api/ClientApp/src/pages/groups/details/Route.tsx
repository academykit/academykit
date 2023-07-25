import { Route, Routes, useParams } from 'react-router-dom';
import GroupAttachment from '@pages/groups/details/attachment';
import TeamsNav from '@components/Layout/TeamsNav';
import lazyWithRetry from '@utils/lazyImportWithReload';
import { useGetGroupDetail } from '@utils/services/groupService';
import { useEffect } from 'react';
import useNav from '@hooks/useNav';
const GroupCourse = lazyWithRetry(() => import('@pages/groups/details/course'));
const GroupDetail = lazyWithRetry(() => import('@pages/groups/details'));
const GroupMember = lazyWithRetry(
  () => import('@pages/groups/details/members')
);

const TeamsRoute = () => {
  const { id } = useParams();
  const groupDetail = useGetGroupDetail(id as string);
  const { setBreadCrumb } = useNav();
  useEffect(() => {
    if (groupDetail.isSuccess) {
      setBreadCrumb &&
        setBreadCrumb([
          { title: 'Groups', href: '/groups' },
          {
            title: groupDetail.data.data.name,
            href: `/groups/${groupDetail.data.data.slug}`,
          },
        ]);
    }
  }, [groupDetail.isSuccess, groupDetail.isRefetching]);
  return (
    <Routes>
      <Route element={<TeamsNav />}>
        <Route path={'/'} element={<GroupDetail />} />
        <Route path={'/members'} element={<GroupMember />} />
        <Route path={'/courses'} element={<GroupCourse />} />
        <Route path={'/attachments'} element={<GroupAttachment />} />
      </Route>
    </Routes>
  );
};

export default TeamsRoute;
