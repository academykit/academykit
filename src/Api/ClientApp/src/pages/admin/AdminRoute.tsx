import { Container, Loader, Tabs } from '@mantine/core';
import AdminAuthRoute, { SuperAdminRoute } from '@routes/AdminRoute';
import lazyWithRetry from '@utils/lazyImportWithReload';
import RoutePath from '@utils/routeConstants';
import { Suspense } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Navigate,
  Outlet,
  Route,
  Routes,
  useLocation,
  useNavigate,
} from 'react-router-dom';
import AdminNav from './Component/AdminNav';
import AdminViewCertificate from './Component/training/adminViewCertificate';
import CertificateList from './Component/training/certificateList';
import MyTrainingInternal from './Component/training/myTrainingInternal';
import Account from './account';

const AdminCourseList = lazyWithRetry(() => import('./course'));
const Department = lazyWithRetry(() => import('./department'));
const FileStorage = lazyWithRetry(() => import('./fileStorage'));
const GeneralSettings = lazyWithRetry(() => import('./generalSettings'));
const Level = lazyWithRetry(() => import('./level'));
const Settings = lazyWithRetry(() => import('./settings'));
const SMTP = lazyWithRetry(() => import('./smtp'));
const ZoomLicense = lazyWithRetry(() => import('./zoomLicense'));
const ZoomSettings = lazyWithRetry(() => import('./zoomSettings'));
const Log = lazyWithRetry(() => import('./log'));

const AdminRoute = () => {
  return (
    <Routes>
      <Route element={<AdminNav />}>
        <Route path="*" element={<AdminRoutesChild />} />
      </Route>
    </Routes>
  );
};

const AdminRoutesChild = () => {
  return (
    <Suspense fallback={<Loader />}>
      <Routes>
        <Route path={'/'} element={<Settings />} />
        <Route path={'/account'} element={<Account />} />
        <Route element={<MyTrainings />}>
          <Route path={'/mycertificate'} element={<MyTrainingInternal />} />
          <Route
            path={'/mycertificate/external'}
            element={<AdminViewCertificate />}
          />
        </Route>
        <Route element={<AdminAuthRoute />}>
          <Route path={'/smtp'} element={<SMTP />} />
          <Route path={'/level'} element={<Level />} />
          <Route path={'/department'} element={<Department />} />
          <Route path={'/log'} element={<Log />} />
          <Route path={'/courses'} element={<AdminCourseList />} />
          <Route path={'/certificate'} element={<CertificateList />} />
          <Route path={'/zoomLicense'} element={<ZoomLicense />} />
          <Route path={'/fileStorage'} element={<FileStorage />} />
          <Route path="*" element={<Navigate to={RoutePath[404]} replace />} />
        </Route>
        <Route element={<SuperAdminRoute />}>
          <Route path={'/zoom'} element={<ZoomSettings />} />
          <Route path={'/general'} element={<GeneralSettings />} />
        </Route>
      </Routes>
    </Suspense>
  );
};

const MyTrainings = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { t } = useTranslation();

  return (
    <Container fluid>
      <Tabs
        my={20}
        value={location.pathname}
        onTabChange={(value) => navigate(`${value}`)}
      >
        <Tabs.List>
          <Tabs.Tab value="/settings/mycertificate">{t('internal')}</Tabs.Tab>
          <Tabs.Tab value="/settings/mycertificate/external">
            {t('external')}
          </Tabs.Tab>
        </Tabs.List>
      </Tabs>
      <Suspense fallback={<Loader />}>
        <Outlet />
      </Suspense>
    </Container>
  );
};

export default AdminRoute;
