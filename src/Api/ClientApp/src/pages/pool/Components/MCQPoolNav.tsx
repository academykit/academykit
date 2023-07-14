import { useParams } from 'react-router-dom';
import RoutePath from '@utils/routeConstants';
import NavOutlet from '@components/Layout/NavOutlet';
import { UserRole } from '@utils/enums';
import i18next from 'i18next';

function MCQPoolNav() {
  const params = useParams();

  const navLink = [
    {
      label: i18next.t('details'),
      to: RoutePath.pool.details(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: i18next.t('trainers'),
      to: RoutePath.pool.teachers(params.id).route,
      role: UserRole.Trainer,
    },
    {
      label: i18next.t('questions'),
      to: RoutePath.pool.questions(params.id).route,
      role: UserRole.Trainer,
    },
  ];

  return <NavOutlet data={navLink} hideBreadCrumb={4} />;
}

export default MCQPoolNav;
