import NavOutlet from '@components/Layout/NavOutlet';
import { TOKEN_STORAGE } from '@utils/constants';
import { UserRole } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import { useTranslation } from 'react-i18next';

const AdminNav = () => {
  const { t } = useTranslation();
  const adminNavLink = [
    {
      label: t('profile'),
      to: RoutePath.settings.base,
      role: UserRole.Trainee,
    },
    {
      label: t('account'),
      to: RoutePath.settings.profile(),
      role: UserRole.Trainee,
    },
    {
      label: t('my_certificate'),
      to: RoutePath.settings.training(),
      role: UserRole.Trainee,
      // additional active path for certificates
      isActive: (pathName: string) =>
        pathName.includes(
          RoutePath.settings.training() ||
            RoutePath.settings.training() + '/external'
        ),
    },
    {
      label: t('admin_settings'),
      to: RoutePath.settings.base,
      separator: true,
      role: UserRole.Admin,
    },
    {
      label: t('general_settings'),
      to: RoutePath.settings.general(),
      role: UserRole.SuperAdmin,
    },
    {
      label: t('zoom_settings'),
      to: RoutePath.settings.zoom(),
      role: UserRole.SuperAdmin,
    },
    {
      label: t('zoom_license'),
      to: RoutePath.settings.zoomLicense(),
      role: UserRole.Admin,
    },
    {
      label: t('smtp'),
      to: RoutePath.settings.smtp(),
      role: UserRole.SuperAdmin,
    },
    {
      label: t('file_storage'),
      to: RoutePath.settings.fileStorage(),
      role: UserRole.SuperAdmin,
    },
    {
      label: t('levels'),
      to: RoutePath.level,
      role: UserRole.Admin,
    },

    {
      label: t('departments'),
      to: RoutePath.settings.department(),
      role: UserRole.Admin,
    },
    // {
    //   label: t('log'),
    //   to: RoutePath.settings.log(),
    //   role: UserRole.Admin,
    // },
    {
      label: t('reviews'),
      to: RoutePath.settings.base,
      separator: true,
      role: UserRole.Admin,
    },
    {
      label: t('trainings'),
      to: RoutePath.settings.courses(),
      role: UserRole.Admin,
    },
    {
      label: t('certificates'),
      to: RoutePath.settings.userCertificate(),
      role: UserRole.Admin,
    },
    // { label: "Payment System", to: "/paymentmethods" },
    {
      label: t('System Monitoring'),
      to: RoutePath.settings.hangfire(),
      separator: true,
      role: UserRole.SuperAdmin,
    },
    {
      label: t('hangfire'),
      to:
        RoutePath.settings.hangfire() +
        '?access_token=' +
        localStorage.getItem(TOKEN_STORAGE),
      role: UserRole.SuperAdmin,
      target: '_blank',
    },
  ];
  return <NavOutlet data={adminNavLink} />;
};

export default AdminNav;
