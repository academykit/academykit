import { Loader } from "@mantine/core";
import { Suspense } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import UserCertificates from "../profile/certificate";
import UserCourse from "../profile/course";

const UserProfileRoute = () => {
  return (
    <Suspense fallback={<Loader />}>
      <Routes>
        <Route index element={<UserCertificates />} />
        <Route path="certificate" element={<UserCertificates />} />
        <Route path="training" element={<UserCourse />} />
        <Route path="*" element={<Navigate replace to={"/404"} />} />
      </Routes>
    </Suspense>
  );
};

export default UserProfileRoute;
