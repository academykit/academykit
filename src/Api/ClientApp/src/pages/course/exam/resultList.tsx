import { useTranslation } from 'react-i18next';

const ResultList = () => {
  const { t } = useTranslation();
  return <div>{t('result_list')}</div>;
};

export default ResultList;
