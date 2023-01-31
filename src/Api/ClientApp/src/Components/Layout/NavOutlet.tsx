import Breadcrumb from "@components/Ui/BreadCrumb";
import useAuth from "@hooks/useAuth";
import {
  AppShell,
  Navbar,
  Box,
  NavLink,
  Divider,
  Group,
  Burger,
  useMantineTheme,
  MediaQuery,
  Flex,
  Loader,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { UserRole } from "@utils/enums";
import { Suspense } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";

const NavOutlet = ({
  data,
  hideBreadCrumb,
}: {
  data: {
    label: string;
    to: string;
    separator?: boolean;
    role: UserRole;
  }[];
  hideBreadCrumb?: number;
}) => {
  const theme = useMantineTheme();
  const router = useLocation();
  const [opened, { toggle }] = useDisclosure(false);
  const auth = useAuth();
  return (
    <AppShell
      sx={{}}
      styles={(theme) => ({
        main: {
          padding: "0px",
          backgroundColor:
            theme.colorScheme === "dark"
              ? theme.colors.dark[8]
              : theme.colors.gray[0],
        },
        header: {
          height: "0",
          backgroundColor: "red",
        },
        inner: {
          height: "0px",
        },
      })}
      navbarOffsetBreakpoint="sm"
      asideOffsetBreakpoint="sm"
      navbar={
        <Navbar
          sx={{ position: "sticky", zIndex: 20, overflow: "hidden" }}
          p="xs"
          hiddenBreakpoint="sm"
          hidden={!opened}
          width={{ sm: 200 }}
        >
          <MediaQuery largerThan="sm" styles={{ display: "none" }}>
            <Flex direction={"row-reverse"}>
              <Burger
                sx={{}}
                aria-label="Toggle navbar"
                opened={opened}
                onClick={() => toggle()}
                size="sm"
                color={theme.colors.gray[6]}
                mr="xl"
              />
            </Flex>
          </MediaQuery>
          <Navbar.Section>
            <Box py="sm">
              {data.map((x) => {
                if (auth?.auth?.role) {
                  if (x.role >= auth?.auth?.role) {
                    return x.separator ? (
                      <div key={x.label}>
                        <Divider mt="sm" />
                        <span>{x.label}</span>
                        <Divider mb="sm" />
                      </div>
                    ) : (
                      <NavLink
                        key={x.label}
                        onClick={toggle}
                        component={Link}
                        to={x.to}
                        replace
                        active={router.pathname === x.to}
                        label={x.label}
                        sx={(theme) => ({
                          padding: theme.spacing.xs,
                          borderRadius: theme.radius.sm,
                        })}
                      ></NavLink>
                    );
                  }
                }
              })}
            </Box>
          </Navbar.Section>
        </Navbar>
      }
    >
      <Box mt={20} mx={20}>
        <MediaQuery
          smallerThan="sm"
          styles={{ display: opened ? "none" : "block" }}
        >
          <Box>
            <Group mx={5}>
              <MediaQuery largerThan="sm" styles={{ display: "none" }}>
                <Burger
                  aria-label="Toggle navbar"
                  opened={opened}
                  onClick={() => toggle()}
                  size="sm"
                  color={theme.colors.gray[6]}
                  mr="xl"
                />
              </MediaQuery>
              <Breadcrumb
                start={{ href: "/", title: "home" }}
                hide={hideBreadCrumb}
              />
            </Group>
            <Suspense fallback={<Loader />}>
              <Outlet />
            </Suspense>
          </Box>
        </MediaQuery>
      </Box>
    </AppShell>
  );
};

export default NavOutlet;
