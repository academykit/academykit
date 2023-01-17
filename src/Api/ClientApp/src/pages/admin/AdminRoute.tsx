import { Container, Loader, Tabs } from "@mantine/core";
import NotFound from "@pages/404";
import AdminAuthRoute, { SuperAdminRoute } from "@routes/AdminRoute";
import lazyWithRetry from "@utils/lazyImportWithReload";
import RoutePath from "@utils/routeConstants";
import React, { Suspense } from "react";
import {
  Navigate,
  Outlet,
  Route,
  Routes,
  useLocation,
  useNavigate,
} from "react-router-dom";
import Account from "./account";
import AdminNav from "./Component/AdminNav";
import AdminViewCertificate from "./Component/training/adminViewCertificate";
import MyTrainingInternal from "./Component/training/myTrainingInternal";

const AdminCourseList = lazyWithRetry(() => import("./course"));
const Department = lazyWithRetry(() => import("./department"));
const FileStorage = lazyWithRetry(() => import("./filestorage"));
const GeneralSettings = lazyWithRetry(() => import("./generalSettings"));
const Level = lazyWithRetry(() => import("./level"));
const Settings = lazyWithRetry(() => import("./settings"));
const SMTP = lazyWithRetry(() => import("./smtp"));
const ZoomLicense = lazyWithRetry(() => import("./zoomLicense"));
const ZoomSettings = lazyWithRetry(() => import("./zoomSettings"));

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
        <Route path={"/"} element={<Settings />} />
        <Route path={"/account"} element={<Account />} />
        <Route element={<MyTrainings />}>
          <Route path={"/mytraining"} element={<MyTrainingInternal />} />
          <Route
            path={"/mytraining/external"}
            element={<AdminViewCertificate />}
          />
          <Route path={"/*"} element={<NotFound />} />
        </Route>
        <Route element={<AdminAuthRoute />}>
          <Route path={"/smtp"} element={<SMTP />} />
          <Route path={"/level"} element={<Level />} />
          <Route path={"/department"} element={<Department />} />
          <Route path={"/courses"} element={<AdminCourseList />} />
          <Route path="*" element={<Navigate to={RoutePath[404]} replace />} />
        </Route>
        <Route element={<SuperAdminRoute />}>
          <Route path={"/zoomlicense"} element={<ZoomLicense />} />
          <Route path={"/filestorage"} element={<FileStorage />} />
          <Route path={"/zoom"} element={<ZoomSettings />} />
          <Route path={"/general"} element={<GeneralSettings />} />
        </Route>
      </Routes>
    </Suspense>
  );
};

const MyTrainings = () => {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <Container fluid>
      <Tabs
        my={20}
        value={location.pathname}
        onTabChange={(value) => navigate(`${value}`)}
      >
        <Tabs.List>
          <Tabs.Tab value="/settings/mytraining">Internal</Tabs.Tab>
          <Tabs.Tab value="/settings/mytraining/external">External</Tabs.Tab>
        </Tabs.List>
      </Tabs>
      <Suspense fallback={<Loader />}>
        <Outlet />
      </Suspense>
    </Container>
  );
};

export default AdminRoute;
