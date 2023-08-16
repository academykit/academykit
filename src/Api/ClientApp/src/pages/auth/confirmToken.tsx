import {
  TextInput,
  PasswordInput,
  Paper,
  Image,
  Container,
  Button,
  Center,
  Anchor,
  Box,
  Group,
  Transition,
} from '@mantine/core';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { useForm } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { useEffect, useState } from 'react';
import useAuth from '../../hooks/useAuth';
import {
  useLogin,
  useResetPassword,
  useResetPasswordToken,
} from '@utils/services/authService';
import RoutePath from '@utils/routeConstants';
import { useToggle } from '@mantine/hooks';
import errorType from '@utils/services/axiosError';
import { useCompanySetting } from '@utils/services/adminService';
import { useTranslation } from 'react-i18next';

const ConfirmToken = () => {
  const navigate = useNavigate();
  const [passwordReset, setPasswordReset] = useState('');
  const { t } = useTranslation();

  const { mutateAsync } = useResetPasswordToken();
  const confirmReset = useResetPassword();

  const form = useForm({
    initialValues: {
      token: '',
    },
  });

  const passwordForm = useForm({
    initialValues: {
      confirmPassword: '',
      newPassword: '',
    },
  });

  const [toggle, setToggle] = useToggle();

  const login = useLogin();
  const auth = useAuth();
  const [params] = useSearchParams();

  const onFormSubmit = async (values: {
    token: string;
    email?: string | null;
  }) => {
    values = { ...values, email: params.get('email') };
    try {
      const response = await mutateAsync(values);
      setPasswordReset(response.data as string);

      setTimeout(() => setToggle(), 700);
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        title: t('error'),
        color: 'red',
      });
    }
  };

  const onPasswordFormSubmit = async (values: {
    newPassword: string;
    confirmPassword: string;
  }) => {
    try {
      await confirmReset.mutateAsync({
        ...values,
        passwordChangeToken: passwordReset,
      });
      showNotification({
        message: t('password_reset'),
      });
      navigate(RoutePath.login);
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        color: 'red',
      });
    }
    // setTimeout(() => , 700);
  };
  useEffect(() => {
    if (auth?.loggedIn) {
      navigate(RoutePath.login, { replace: true });
    }
  }, [auth?.auth]);

  const companySettings = useCompanySetting();

  const setHeader = () => {
    const info =
      localStorage.getItem('app-info') &&
      JSON.parse(localStorage.getItem('app-info') ?? '');
    if (info) {
      let link = document.querySelector("link[rel~='icon']") as HTMLLinkElement;
      document.title = info.name;
      if (!link) {
        link = document.createElement('link');
        link.rel = 'icon';
        document.getElementsByTagName('head')[0].appendChild(info.logo);
      }
      link.href = info.logo;
    }
  };

  useEffect(() => {
    setHeader();

    if (companySettings.isSuccess) {
      localStorage.setItem(
        'app-info',
        JSON.stringify({
          name: companySettings.data.data.name,
          logo: companySettings.data.data.imageUrl,
        })
      );
      setHeader();
    }
  }, [companySettings.isSuccess]);

  return (
    <Container size={470} my={40} sx={{ position: 'relative' }}>
      <Center m={'lg'}>
        <Link to={'/'}>
          <Image
            height={100}
            width={100}
            src={companySettings?.data?.data?.imageUrl}
          ></Image>
        </Link>
      </Center>

      <Box sx={{ position: 'absolute', width: '100%' }}>
        <Transition
          mounted={!toggle}
          transition="fade"
          duration={400}
          timingFunction="ease"
        >
          {(styles) => (
            <div style={styles}>
              <form onSubmit={form.onSubmit(onFormSubmit)}>
                <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                  <TextInput
                    {...form.getInputProps('token')}
                    label={t('token')}
                    placeholder={t('add_token_email') as string}
                    required
                  />

                  <Group position="right" mt={10}>
                    <Link to={RoutePath.login}>
                      <Anchor
                        component="button"
                        align="end"
                        type="button"
                        color="dimmed"
                        size="xs"
                      >
                        {t('want_login')}
                      </Anchor>
                    </Link>
                  </Group>
                  <Button
                    loading={login.isLoading}
                    fullWidth
                    mt="xl"
                    type="submit"
                  >
                    {t('proceed')}
                  </Button>
                </Paper>
              </form>
            </div>
          )}
        </Transition>
      </Box>

      <Box sx={{ position: 'absolute', width: '100%' }}>
        <Transition
          mounted={toggle}
          transition="pop"
          duration={400}
          timingFunction="ease"
        >
          {(styles) => (
            <div style={styles}>
              <form onSubmit={passwordForm.onSubmit(onPasswordFormSubmit)}>
                <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                  <PasswordInput
                    {...passwordForm.getInputProps('newPassword')}
                    label={t('password')}
                    placeholder={t('add_new_password') as string}
                    required
                  />

                  <PasswordInput
                    mt={20}
                    {...passwordForm.getInputProps('confirmPassword')}
                    label={t('confirm_password')}
                    placeholder={t('repeat_password') as string}
                    required
                  />

                  <Group position="right" mt={10}>
                    <Link to={RoutePath.login}>
                      <Anchor
                        component="button"
                        align="end"
                        type="button"
                        color="dimmed"
                        size="xs"
                      >
                        {t('want_login')}?
                      </Anchor>
                    </Link>
                  </Group>
                  <Button
                    loading={login.isLoading}
                    fullWidth
                    mt="xl"
                    type="submit"
                  >
                    {t('proceed')}
                  </Button>
                </Paper>
              </form>
            </div>
          )}
        </Transition>
      </Box>
    </Container>
  );
};
export default ConfirmToken;
