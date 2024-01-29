import UserProfileMenu from '@components/UserProfileMenu';
import useCustomLayout from '@context/LayoutProvider';
import useAuth from '@hooks/useAuth';
import {
  AppShell,
  Box,
  Burger,
  Container,
  Group,
  NavLink,
  ScrollArea,
  ThemeIcon,
  useMantineColorScheme,
  useMantineTheme,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconInfoSquare } from '@tabler/icons';
import { UserRole } from '@utils/enums';
import { useGeneralSetting } from '@utils/services/adminService';
import { IUser } from '@utils/services/types';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Outlet } from 'react-router-dom';
import { AppFooter } from './AppFooter';
import { LeftMainLinks } from './LeftMainLink';
import classes from './styles/layout.module.css';

const Layout = ({ showNavBar = true }: { showNavBar?: boolean }) => {
  const settings = useGeneralSetting();

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

    if (settings.isSuccess) {
      localStorage.setItem(
        'app-info',
        JSON.stringify({
          name: settings.data.data.companyName,
          logo: settings.data.data.logoUrl,
        })
      );
      setHeader();
    }
  }, [settings.isSuccess]);

  const auth = useAuth();

  const [opened, { toggle }] = useDisclosure(false);
  const footerMenu = {
    icon: <IconInfoSquare size={16} />,
    color: 'blue',
    label: 'help',
    href: 'https://docs.google.com/document/d/1S_wlCY7XH2oELa8ZvNPSGVik2117HuclOGC04iHpP-w/edit#heading=h.yf9se9jmezfr',
    replace: true,
    role: UserRole.Trainee,
    target: '_blank',
  };
  const layout = useCustomLayout();
  const { t } = useTranslation();
  const { colorScheme } = useMantineColorScheme();
  const theme = useMantineTheme();

  return (
    <AppShell
      styles={(theme) => ({
        main: {
          backgroundColor:
            colorScheme === 'dark'
              ? theme.colors.dark[8]
              : theme.colors.gray[0],
        },
      })}
      header={{ height: 60 }}
      navbar={{ width: 210, breakpoint: 'sm', collapsed: { mobile: !opened } }}
      padding={'md'}
    >
      <AppShell.Header>
        {layout.meetPage ? (
          <></>
        ) : layout.examPage ? (
          <Group
            style={{
              justifyContent: 'space-between',
              width: '100%',
              alignItems: 'center',
            }}
          >
            <Box>{layout.examPageTitle}</Box>
            <Box>{layout.examPageAction}</Box>
          </Group>
        ) : (
          <Container className={classes.inner} fluid>
            <Group>
              <Burger
                opened={opened}
                onClick={toggle}
                hiddenFrom="sm"
                size="sm"
              />
              <Link to="/" style={{ marginTop: '5px' }}>
                <img height={50} src={settings.data?.data?.logoUrl} alt="" />
              </Link>
            </Group>
            {auth?.auth && (
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
            )}
          </Container>
        )}
      </AppShell.Header>

      <AppShell.Navbar p="sm">
        <AppShell.Section grow component={ScrollArea}>
          {layout.meetPage ? (
            <></>
          ) : (!layout.examPage || !layout.meetPage) && showNavBar ? (
            <Box py="sm">
              {!layout.meetPage && <LeftMainLinks onClose={() => toggle()} />}
            </Box>
          ) : null}
        </AppShell.Section>

        <AppShell.Section>
          {!layout.meetPage && (
            <NavLink
              href={footerMenu.href}
              label={t(`${footerMenu.label}`)}
              target={footerMenu.target}
              leftSection={
                <ThemeIcon color={footerMenu.color}>
                  {footerMenu.icon}
                </ThemeIcon>
              }
              style={{
                padding: theme.spacing.xs,
                borderTop: '0.0125rem solid',
              }}
            />
          )}
        </AppShell.Section>
      </AppShell.Navbar>

      <AppShell.Main>
        <Outlet />
      </AppShell.Main>

      <AppFooter name={settings.data?.data?.companyName ?? ''}></AppFooter>
    </AppShell>
  );
};

export default Layout;
