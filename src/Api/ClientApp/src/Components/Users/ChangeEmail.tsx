import { Alert, Button, Container } from '@mantine/core';
import { IconAlertCircle, IconTrophy } from '@tabler/icons-react';
import { useVerifyChangeEmail } from '@utils/services/authService';
import { useTranslation } from 'react-i18next';
import { Link, useSearchParams } from 'react-router-dom';

const ChangeEmail = () => {
  const [searchParams] = useSearchParams();
  const token = searchParams.get('token');
  const verifyChangeEmail = useVerifyChangeEmail(token as string);
  const { t } = useTranslation();

  return (
    <Container size="sm">
      <div style={{ marginTop: '150px' }}>
        {verifyChangeEmail.isSuccess && (
          <Alert icon={<IconTrophy size={20} />} title="Success" color="green">
            {t('email_change_success')}
          </Alert>
        )}

        {verifyChangeEmail.isError && (
          <Alert
            icon={<IconAlertCircle size={20} />}
            title="Error!"
            color="pink"
          >
            {t('something_terrible_email')}
          </Alert>
        )}
        <Button mt={20} variant={'outline'} component={Link} to="/">
          {t('go_back')}
        </Button>
      </div>
    </Container>
  );
};

export default ChangeEmail;
