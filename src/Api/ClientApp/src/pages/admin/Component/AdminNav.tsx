import NavOutlet from "@components/Layout/NavOutlet";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useTranslation } from "react-i18next";

const AdminNav = () => {
  const { t } = useTranslation();
  const adminNavLink = [
    {
      label: t("profile"),
      to: RoutePath.settings.base,
      role: UserRole.Trainee,
    },
    {
      label: t("account"),
      to: RoutePath.settings.profile(),
      role: UserRole.Trainee,
    },
    {
      label: t("my_certificate"),
      to: RoutePath.settings.training(),
      role: UserRole.Trainee,
    },
    {
      label: t("admin_settings"),
      to: RoutePath.settings.base,
      separator: true,
      role: UserRole.Admin,
    },
    {
      label: t("general_settings"),
      to: RoutePath.settings.general(),
      role: UserRole.SuperAdmin,
    },
    {
      label: t("zoom_settings"),
      to: RoutePath.settings.zoom(),
      role: UserRole.SuperAdmin,
    },
    {
      label: t("zoom_license"),
      to: RoutePath.settings.zoomlicense(),
      role: UserRole.Admin,
    },
    {
      label: t("smtp"),
      to: RoutePath.settings.smtp(),
      role: UserRole.Admin,
    },
    {
      label: t("file_storage"),
      to: RoutePath.settings.filestorage(),
      role: UserRole.Admin,
    },
    {
      label: t("levels"),
      to: RoutePath.level,
      role: UserRole.Admin,
    },
    {
      label: t("departments"),
      to: RoutePath.settings.department(),
      role: UserRole.Admin,
    },
    {
      label: t("log"),
      to: RoutePath.settings.log(),
      role: UserRole.Admin,
    },
    {
      label: t("reviews"),
      to: RoutePath.settings.base,
      separator: true,
      role: UserRole.Admin,
    },
    {
      label: t("trainings"),
      to: RoutePath.settings.courses(),
      role: UserRole.Admin,
    },
    {
      label: t("certificates"),
      to: RoutePath.settings.userCertificate(),
      role: UserRole.Admin,
    },
    // { label: "Payment System", to: "/paymentmethods" },
  ];
  return <NavOutlet data={adminNavLink} />;
};

export default AdminNav;
