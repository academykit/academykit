import { useTranslation } from 'react-i18next';

const DisplaySignature = () => {
  const { t } = useTranslation();
  return <div>{t('display_signature')}</div>;
};

export default DisplaySignature;
