import useAuth from "@hooks/useAuth";
import { ActionIcon, Loader, Menu, ScrollArea, Tabs } from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import { IconDotsVertical, IconSettings, IconUser } from "@tabler/icons-react";
import { UserRole } from "@utils/enums";
import { t } from "i18next";
import { Suspense } from "react";
import { Link, Outlet, useLocation, useNavigate } from "react-router-dom";

const NavOutlet = ({
  data,
}: {
  data: {
    label: string;
    to: string;
    separator?: boolean;
    role: UserRole;
    isActive?: (pathName: string) => boolean;
    target?: string;
    icon?: JSX.Element;
  }[];
  hideBreadCrumb?: number;
}) => {
  const auth = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const exactLocation = location.pathname;
  const isMobileView = useMediaQuery("(max-width: 48em)");
  const isTabletView = useMediaQuery("(min-width: 48em) and (max-width: 64em)");

  const getExactLocation = (location: string) => {
    // when user is inspecting lesson details, highlight lesson-stat tab
    if (
      location.split("/")[5] !== undefined &&
      location.split("/")[3] !== "questions"
    ) {
      const loc = location.split("/lessons-stat")[0] + "/lessons-stat";
      return loc;
    } else if (
      // when user is inspecting create/edit/pull pools, highlight questions tab
      location.split("/questions")[1] !== "" &&
      location.split("/")[3] === "questions"
    ) {
      const loc = location.split("/questions")[0] + "/questions";
      return loc;
    }
    return location;
  };

  const routeData = data.filter((x) => {
    if (auth?.auth?.role) {
      if (x.role >= Number(auth?.auth?.role) && !x.separator) {
        return x;
      }
    }
  });

  return (
    <>
      <Tabs
        defaultChecked={true}
        defaultValue={location.pathname?.split("/").at(-1) ?? "settings"}
        value={getExactLocation(exactLocation)}
        onChange={(value) => {
          if (value == "#") {
            // route by menu items
            return;
          } else {
            navigate(`${value}`, { preventScrollReset: true });
          }
        }}
        styles={{
          // make tabs scrollable
          list: {
            flexWrap: "nowrap",
          },
        }}
        mb={15}
      >
        <ScrollArea scrollHideDelay={0}>
          <Tabs.List>
            {routeData
              .slice(0, isMobileView || isTabletView ? 2 : routeData.length)
              .map((x) => {
                return (
                  <Tabs.Tab
                    key={x.label}
                    value={x.to}
                    leftSection={x.icon ?? <IconUser size={14} />}
                  >
                    {x.label}
                  </Tabs.Tab>
                );
              })}

            {/* display only when mobile view and contains more than 2 elements */}
            {routeData.length > 2 && (isMobileView || isTabletView) && (
              <Tabs.Tab ml="auto" value="#">
                <Menu withArrow>
                  <Menu.Target>
                    <ActionIcon variant="light">
                      <IconDotsVertical size={18} />
                    </ActionIcon>
                  </Menu.Target>

                  <Menu.Dropdown>
                    {routeData.slice(2).map((x) => {
                      return (
                        <Menu.Item
                          key={x.label}
                          leftSection={x.icon ?? <IconSettings size={14} />}
                          component={Link}
                          to={x.to}
                        >
                          {t(`${x.label}`)}
                        </Menu.Item>
                      );
                    })}
                  </Menu.Dropdown>
                </Menu>
              </Tabs.Tab>
            )}
          </Tabs.List>
        </ScrollArea>
      </Tabs>

      <Suspense fallback={<Loader />}>
        <Outlet />
      </Suspense>
    </>
  );
};

export default NavOutlet;
