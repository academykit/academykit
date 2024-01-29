import { Anchor, Button, Container, Group, Text, Title } from '@mantine/core';
import { useTranslation } from 'react-i18next';
import classes from './styles/error.module.css';

const UnAuthorize = () => {
  const { t } = useTranslation();
  return (
    <Container className={classes.root}>
      <div className={classes.label}>{t('401')}</div>
      <Title className={classes.title}>{t('not_authorized')}</Title>
      <Text c="dimmed" size="lg" ta="center" className={classes.description}>
        {/* Unfortunately, this is only a 404 page. You may have mistyped the
        address, or the page has been moved to another URL. */}
      </Text>
      <Group justify="center">
        <Anchor href={'/'}>
          <Button variant="subtle" size="md">
            {t('back_to_home')}
          </Button>
        </Anchor>
      </Group>
    </Container>
  );
};

export default UnAuthorize;
