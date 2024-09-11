import Logo from "@components/Logo";
import UserProfileMenu from "@components/UserProfileMenu";
import useCustomLayout from "@context/LayoutProvider";
import useAuth from "@hooks/useAuth";
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
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { IconInfoSquare } from "@tabler/icons-react";
import { checkValidUrl } from "@utils/checkValidUrl";
import { UserRole } from "@utils/enums";
import { useGeneralSetting } from "@utils/services/adminService";
import { IUser } from "@utils/services/types";
import { setHeader } from "@utils/setHeader";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Link, Outlet } from "react-router-dom";
import { AppFooter } from "./AppFooter";
import { LeftMainLinks } from "./LeftMainLink";
import classes from "./styles/layout.module.css";

const Layout = ({ showNavBar = true }: { showNavBar?: boolean }) => {
  const generalSettings = useGeneralSetting();

  useEffect(() => {
    if (generalSettings.isSuccess) {
      setHeader({
        name: generalSettings.data.data.companyName,
        logoUrl: generalSettings.data.data.logoUrl,
      });
    }
  }, [generalSettings.isSuccess]);

  const auth = useAuth();

  const [opened, { toggle }] = useDisclosure(false);
  const footerMenu = {
    icon: <IconInfoSquare size={16} />,
    color: "blue",
    label: "help",
    href: "https://docs.academykit.co/",
    replace: true,
    role: UserRole.Trainee,
    target: "_blank",
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
            colorScheme === "dark"
              ? theme.colors.dark[8]
              : theme.colors.gray[0],
        },
      })}
      header={{ height: 60 }}
      navbar={{ width: 210, breakpoint: "sm", collapsed: { mobile: !opened } }}
      padding={"md"}
    >
      <AppShell.Header>
        {layout.meetPage ? (
          <></>
        ) : layout.examPage ? (
          <Group
            style={{
              justifyContent: "space-between",
              width: "100%",
              alignItems: "center",
              padding: "8px 16px 0 16px",
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
              <Link to="/" style={{ marginTop: "5px" }}>
                {checkValidUrl(generalSettings.data?.data?.logoUrl) ? (
                  <img
                    height={50}
                    src={generalSettings.data?.data?.logoUrl}
                    alt={generalSettings.data?.data?.companyName}
                  />
                ) : (
                  <Logo
                    url={generalSettings.data?.data?.logoUrl}
                    height={0}
                    width={0}
                  />
                )}
              </Link>
            </Group>
            {auth?.auth && (
              <UserProfileMenu
                user={
                  {
                    email: auth.auth.email,
                    fullName: auth.auth.firstName + " " + auth.auth.lastName,
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
                borderTop: "0.0125rem solid",
              }}
            />
          )}
        </AppShell.Section>
      </AppShell.Navbar>

      <AppShell.Main>
        <Outlet />
      </AppShell.Main>

      <AppFooter
        name={generalSettings.data?.data?.companyName ?? ""}
      ></AppFooter>
    </AppShell>
  );
};

export default Layout;
