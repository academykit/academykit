import {
  PasswordInput,
  Paper,
  Title,
  Container,
  Button,
  Center,
  Anchor,
  Group,
  Image,
} from '@mantine/core';
import { Link } from 'react-router-dom';
import { useForm, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { useEffect } from 'react';
import useAuth from '../../hooks/useAuth';
import { useLogin } from '@utils/services/authService';
import RoutePath from '@utils/routeConstants';
import { IUserProfile } from '@utils/services/types';
import { useCompanySetting } from '@utils/services/adminService';
import { useTranslation } from 'react-i18next';
import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import * as Yup from 'yup';
import { AxiosError } from 'axios';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    email: Yup.string()
      .trim()
      .email(t('invalid_email') as string)
      .required(t('email_required') as string),
  });
};

const LoginPage = () => {
  const form = useForm({
    initialValues: {
      email: '',
      password: '',
    },
    validate: yupResolver(schema()),
  });

  const login = useLogin();
  const { t } = useTranslation();
  const auth = useAuth();
  const onFormSubmit = (values: { email: string; password: string }) => {
    login.mutate({ email: values.email, password: values.password });
  };

  useEffect(() => {
    if (login.isError) {
      showNotification({
        message:
          ((login.error as AxiosError).response?.data as any)?.message ??
          t('something_wrong'),
        title: t('error'),
        color: 'red',
      });
      login.reset();
    }
    if (login.isSuccess) {
      auth?.setToken(login.data?.data?.token);
      auth?.setRefreshToken(login?.data?.data?.refreshToken ?? null);
      auth?.setIsLoggedIn(true);
      auth?.setAuth({
        imageUrl: login.data.data.imageUrl,
        mobileNumber: '',
        firstName: login.data.data.firstName,
        lastName: login.data.data.firstName,
        id: login.data.data.userId,
        email: login.data.data.email,
      } as IUserProfile);
      showNotification({
        message: t('login_success'),
        title: t('successful'),
      });
    }
  }, [login.isError, login.isSuccess]);
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
    <Container size={420} my={40}>
      <Center m={'lg'}>
        <Link to={'/'}>
          <Image
            height={50}
            width={50}
            src={companySettings?.data?.data?.imageUrl}
          ></Image>
        </Link>
      </Center>
      <Title
        align="center"
        sx={(theme) => ({
          fontFamily: `Greycliff CF, ${theme.fontFamily}`,
          fontWeight: 900,
        })}
      >
        {t('welcome_back')}!
      </Title>
      <form onSubmit={form.onSubmit(onFormSubmit)}>
        <Paper withBorder shadow="md" p={30} mt={30} radius="md">
          <CustomTextFieldWithAutoFocus
            {...form.getInputProps('email')}
            autoComplete={'username'}
            label={t('email')}
            type={'email'}
            placeholder={t('your_email') as string}
            name="email"
          />
          <PasswordInput
            {...form.getInputProps('password')}
            label={t('password')}
            autoComplete={'password'}
            placeholder={t('your_password') as string}
            mt="md"
            name="password"
          />
          <Group position="right" mt={10}>
            <Link to={RoutePath.forgotPassword}>
              <Anchor
                component="button"
                align="end"
                type="button"
                color="dimmed"
                size="xs"
              >
                {t('forgot_password')}?
              </Anchor>
            </Link>
          </Group>
          <Button loading={login.isLoading} fullWidth mt="xl" type="submit">
            {t('sign_in')}
          </Button>
        </Paper>
      </form>
    </Container>
  );
};
export default LoginPage;
