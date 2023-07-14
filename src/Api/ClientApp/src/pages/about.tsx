/* eslint-disable react/no-children-prop */
import ReactMarkdown from 'react-markdown';
import { Container } from '@mantine/core';
import { useTranslation } from 'react-i18next';

export const AboutPage = () => {
  const { t } = useTranslation();

  return (
    <Container>
      <ReactMarkdown children={t('about_app')} />
    </Container>
  );
};

export default AboutPage;
