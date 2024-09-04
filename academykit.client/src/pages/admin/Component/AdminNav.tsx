import AdminNavOutlet from "@components/Layout/AdminNavOutlet";
import {
  IconBrandOpenai,
  IconCertificate,
  IconFileCertificate,
  IconLicense,
  IconMailCode,
  IconMailCog,
  IconServerCog,
  IconUser,
  IconUserCircle,
  IconVideo,
} from "@tabler/icons-react";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { useTranslation } from "react-i18next";

const AdminNav = () => {
  const { t } = useTranslation();

  const adminNavLinks = [
    {
      label: t("profile"),
      to: RoutePath.settings.base,
      role: UserRole.Trainee,
      group: "account",
      icon: <IconUserCircle size={14} />,
    },
    {
      label: t("account"),
      to: RoutePath.settings.profile(),
      role: UserRole.Trainee,
      group: "account",
      icon: <IconUser size={14} />,
    },
    {
      label: t("my_certificate"),
      to: RoutePath.settings.training(),
      role: UserRole.Trainee,
      // additional active path for certificates
      isActive: (pathName: string) =>
        pathName.includes(
          RoutePath.settings.training() ||
            RoutePath.settings.training() + "/external"
        ),
      group: "account",
      icon: <IconCertificate size={14} />,
    },
    {
      label: t("general_settings"),
      to: RoutePath.settings.general(),
      role: UserRole.SuperAdmin,
      group: "admin",
    },
    {
      label: t("zoom_settings"),
      to: RoutePath.settings.zoom(),
      role: UserRole.SuperAdmin,
      group: "admin",
      icon: <IconVideo size={14} />,
    },
    {
      label: t("zoom_license"),
      to: RoutePath.settings.zoomLicense(),
      role: UserRole.Admin,
      group: "admin",
      icon: <IconLicense size={14} />,
    },
    {
      label: t("smtp"),
      to: RoutePath.settings.smtp(),
      role: UserRole.SuperAdmin,
      group: "admin",
      icon: <IconMailCog size={14} />,
    },
    {
      label: t("file_storage"),
      to: RoutePath.settings.fileStorage(),
      role: UserRole.SuperAdmin,
      group: "admin",
      icon: <IconServerCog size={14} />,
    },
    {
      label: t("ai_setup"),
      to: RoutePath.settings.ai(),
      role: UserRole.SuperAdmin,
      group: "admin",
      icon: <IconBrandOpenai size={14} />,
    },
    {
      label: t("levels"),
      to: RoutePath.level,
      role: UserRole.Admin,
      group: "admin",
    },
    {
      label: t("departments"),
      to: RoutePath.settings.department(),
      role: UserRole.Admin,
      group: "admin",
    },
    {
      label: t("skills"),
      to: RoutePath.settings.skill(),
      role: UserRole.Admin,
      group: "admin",
    },
    {
      label: t("mail-notification"),
      to: RoutePath.settings.mail(),
      role: UserRole.Admin,
      group: "admin",
      icon: <IconMailCode size={14} />,
    },
    // {
    //   label: t('log'),
    //   to: RoutePath.settings.log(),
    //   role: UserRole.Admin,
    // },
    // {
    //   label: t('trainings'),
    //   to: RoutePath.settings.courses(),
    //   role: UserRole.Admin,
    //   group: 'reviews',
    // },
    {
      label: t("certificates"),
      to: RoutePath.settings.userCertificate(),
      role: UserRole.Admin,
      group: "reviews",
      icon: <IconFileCertificate size={14} />,
    },
    // { label: "Payment System", to: "/paymentMethods" },
    {
      label: t("hangfire"),
      to: "hangfire",
      role: UserRole.SuperAdmin,
      target: "_blank",
      group: "hangfire",
    },
    {
      label: t("license_management"),
      to: RoutePath.settings.license(),
      role: UserRole.SuperAdmin,
      group: "admin",
      icon: <IconLicense size={14} />,
    },
  ];
  return <AdminNavOutlet data={adminNavLinks} />;
};

export default AdminNav;
