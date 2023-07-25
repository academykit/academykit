import {
  AppShell,
  Container,
  Group,
  Header,
  Button,
  createStyles,
} from '@mantine/core';
import { Link, Outlet } from 'react-router-dom';
import { AppFooter } from './AppFooter';
import { useCompanySetting } from '@utils/services/adminService';

import useAuth from '@hooks/useAuth';
import UserProfileMenu from '@components/UserProfileMenu';
import { IUser } from '@utils/services/types';

const HEADER_HEIGHT = 60;
const useStyles = createStyles((theme) => ({
  header: {
    backgroundColor: theme.fn.variant({
      variant: 'filled',
      color: theme.primaryColor,
    }).background,
    borderBottom: 0,
  },

  inner: {
    height: HEADER_HEIGHT,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
  },

  burger: {
    [theme.fn.largerThan('sm')]: {
      display: 'none',
    },
  },

  links: {
    paddingTop: theme.spacing.lg,
    height: HEADER_HEIGHT,
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'space-between',

    [theme.fn.smallerThan('sm')]: {
      display: 'none',
    },
  },

  mainLinks: {
    marginRight: -theme.spacing.sm,
  },

  mainLink: {
    textTransform: 'uppercase',
    fontSize: 13,
    color: theme.white,
    padding: `7px ${theme.spacing.sm}px`,
    fontWeight: 700,
    borderBottom: '2px solid transparent',
    transition: 'border-color 100ms ease, opacity 100ms ease',
    opacity: 0.9,
    borderTopRightRadius: theme.radius.sm,
    borderTopLeftRadius: theme.radius.sm,

    '&:hover': {
      opacity: 1,
      textDecoration: 'none',
    },
  },

  secondaryLink: {
    color: theme.colors[theme.primaryColor][0],
    fontSize: theme.fontSizes.xs,
    textTransform: 'uppercase',
    transition: 'color 100ms ease',

    '&:hover': {
      color: theme.white,
      textDecoration: 'none',
    },
  },

  mainLinkActive: {
    color: theme.white,
    opacity: 1,
    borderBottomColor:
      theme.colorScheme === 'dark'
        ? theme.white
        : theme.colors[theme.primaryColor][5],
    backgroundColor: theme.fn.lighten(
      theme.fn.variant({ variant: 'filled', color: theme.primaryColor })
        .background!,
      0.1
    ),
  },
}));

const PrivacyLayout = () => {
  const companySettings = useCompanySetting();
  const auth = useAuth();

  const { classes } = useStyles();

  return (
    <AppShell
      padding="md"
      header={
        <Header height={60}>
          <Container className={classes.inner} fluid>
            <Group position={'apart'} w={'100%'}>
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
        </Header>
      }
      styles={(theme) => ({
        main: {
          backgroundColor:
            theme.colorScheme === 'dark'
              ? theme.colors.dark[8]
              : theme.colors.gray[0],
        },
      })}
      footer={
        <AppFooter name={companySettings.data?.data?.name ?? ''}></AppFooter>
      }
    >
      <Outlet />
    </AppShell>
  );
};

export default PrivacyLayout;
