import {
  AppShell,
  Button,
  Container,
  Group,
  useMantineColorScheme,
} from '@mantine/core';
import { useCompanySetting } from '@utils/services/adminService';
import { Link, Outlet } from 'react-router-dom';
import { AppFooter } from './AppFooter';

import UserProfileMenu from '@components/UserProfileMenu';
import useAuth from '@hooks/useAuth';
import { IUser } from '@utils/services/types';
import classes from './styles/layout.module.css';

const PrivacyLayout = () => {
  const companySettings = useCompanySetting();
  const auth = useAuth();
  const { colorScheme } = useMantineColorScheme();
  // const theme = useMantineTheme();

  return (
    <AppShell
      padding="md"
      styles={(theme) => ({
        main: {
          backgroundColor:
            colorScheme === 'dark'
              ? theme.colors.dark[8]
              : theme.colors.gray[0],
        },
      })}
    >
      <AppShell.Header>
        {
          <Container className={classes.inner} fluid>
            <Group justify={'space-between'} w={'100%'}>
              <Link to="/">
                <img
                  height={50}
                  src={companySettings.data?.data?.imageUrl}
                  alt=""
                />
              </Link>
              {auth?.loggedIn && auth?.auth ? (
                <UserProfileMenu
                  user={
                    {
                      email: auth.auth.email,
                      fullName: auth.auth.firstName + ' ' + auth.auth.lastName,
                      id: auth.auth.id,
                      role: auth.auth.role,
                      imageUrl: auth.auth.imageUrl,
                    } as IUser
                  }
                />
              ) : (
                <Button component={Link} to={'/login'}>
                  Login
                </Button>
              )}
            </Group>
          </Container>
        }
      </AppShell.Header>

      <AppShell.Main>
        <Outlet />
      </AppShell.Main>

      <AppFooter name={companySettings.data?.data?.name ?? ''}></AppFooter>
      {/* <AppShell.Footer>
      </AppShell.Footer> */}
    </AppShell>
  );
};

export default PrivacyLayout;
