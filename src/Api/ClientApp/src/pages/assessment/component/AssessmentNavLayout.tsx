import NavOutlet from '@components/Layout/NavOutlet';
import {
  IconChartInfographic,
  IconClipboard,
  IconListNumbers,
  IconTool,
} from '@tabler/icons';
import { UserRole } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';

const AssessmentNavLayout = () => {
  const params = useParams();
  const { t } = useTranslation();

  const navLink = [
    {
      label: t('settings'),
      to: RoutePath.manageAssessment.setting(params.id).route,
      role: UserRole.Trainer,
      icon: <IconTool size={14} />,
    },
    {
      label: t('details'),
      to: RoutePath.manageAssessment.edit(params.id).routes(),
      role: UserRole.Trainer,
      icon: <IconClipboard size={14} />,
    },
    {
      label: t('questions'),
      to: RoutePath.manageAssessment.question(params.id).route,
      role: UserRole.Trainer,
      icon: <IconListNumbers size={14} />,
    },
    {
      label: t('assessment_stats'),
      to: RoutePath.manageAssessment.assessmentStat(params.id).route,
      role: UserRole.Trainer,
      icon: <IconChartInfographic size={14} />,
    },
  ];

  return <NavOutlet data={navLink} hideBreadCrumb={2} />;
};

export default AssessmentNavLayout;
