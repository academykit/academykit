/* eslint-disable react/no-children-prop */
import ReactMarkdown from 'react-markdown';
import { Container } from '@mantine/core';
import { useTranslation } from 'react-i18next';

export const PrivacyPage = () => {
  const { t } = useTranslation();

  return (
    <Container>
      <ReactMarkdown children={t('privacy_and_policy')} />
    </Container>
  );
};

export default PrivacyPage;
