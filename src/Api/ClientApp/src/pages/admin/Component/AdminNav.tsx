import NavOutlet from "@components/Layout/NavOutlet";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";

const AdminNav = () => {
  const adminNavLink = [
    { label: "Profile", to: RoutePath.settings.base, role: UserRole.Trainee },
    {
      label: "Account",
      to: RoutePath.settings.profile(),
      role: UserRole.Trainee,
    },
    {
      label: "Your Trainings",
      to: RoutePath.settings.training(),
      role: UserRole.Trainee,
    },
    {
      label: "Admin Settings",
      to: RoutePath.settings.base,
      separator: true,
      role: UserRole.Admin,
    },
    {
      label: "General Settings",
      to: RoutePath.settings.general(),
      role: UserRole.SuperAdmin,
    },
    {
      label: "Zoom Settings",
      to: RoutePath.settings.zoom(),
      role: UserRole.SuperAdmin,
    },
    {
      label: "Zoom License",
      to: RoutePath.settings.zoomlicense(),
      role: UserRole.SuperAdmin,
    },
    {
      label: "SMTP",
      to: RoutePath.settings.smtp(),
      role: UserRole.Admin,
    },
    {
      label: "File Storage",
      to: RoutePath.settings.filestorage(),
      role: UserRole.SuperAdmin,
    },
    {
      label: "Levels",
      to: RoutePath.level,
      role: UserRole.Admin,
    },
    {
      label: "Departments",
      to: RoutePath.settings.department(),
      role: UserRole.Admin,
    },
    {
      label: "Reviews",
      to: RoutePath.settings.base,
      separator: true,
      role: UserRole.Admin,
    },
    {
      label: "Trainings",
      to: RoutePath.settings.courses(),
      role: UserRole.Admin,
    },
    // { label: "Payment System", to: "/paymentmethods" },
  ];
  return <NavOutlet data={adminNavLink} />;
};

export default AdminNav;
