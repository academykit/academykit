import { Button, Container, Group, Text, Title } from '@mantine/core';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useLocation } from 'react-router-dom';
import classes from './styles/serverError.module.css';

const ServerError = () => {
  const { t } = useTranslation();
  const [init, setInit] = useState(false);
  const location = useLocation();

  useEffect(() => {
    if (init) {
      window.location.reload();
    }
    setInit(true);
  }, [location.pathname]);

  return (
    <div className={classes.root}>
      <Container>
        <div className={classes.label}>{t('500')}</div>
        <Title className={classes.title}>{t('something_bad')}</Title>
        <Text size="lg" ta="center" className={classes.description}>
          {t('server_error')}
        </Text>
        <Group justify="center" style={{ flexDirection: 'column' }}>
          <Button
            variant="white"
            size="md"
            onClick={() => window.location.reload()}
          >
            {t('refresh_page')}
          </Button>
          <Button variant="white" size="md" component={Link} to={'/'}>
            {t('back_to_home')}
          </Button>
        </Group>
      </Container>
    </div>
  );
};

export default ServerError;
