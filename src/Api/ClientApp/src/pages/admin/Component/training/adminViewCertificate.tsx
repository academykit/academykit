import useAuth from "@hooks/useAuth";
import { UserRole } from "@utils/enums";
import React from "react";
import { useNavigate } from "react-router";
import MyTrainingExternal from "./myTrainingExternal";

const AdminViewCertificate = () => {
  const auth = useAuth();
  const navigate = useNavigate();
  if (auth?.auth && auth?.auth?.role <= UserRole.Admin) {
    navigate("/404");
  }
  return (
    <div>
      <MyTrainingExternal isAdmin={true} />
    </div>
  );
};

export default AdminViewCertificate;
