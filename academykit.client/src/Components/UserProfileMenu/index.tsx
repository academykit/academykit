import useAuth from "@hooks/useAuth";
import {
  Avatar,
  Box,
  Divider,
  Group,
  Menu,
  Text,
  useMantineColorScheme,
} from "@mantine/core";
import {
  IconLock,
  IconLogout,
  IconMoonStars,
  IconPencil,
  IconSun,
  IconUser,
} from "@tabler/icons-react";
import { UserRole } from "@utils/enums";
import { getInitials } from "@utils/getInitialName";
import { IUser } from "@utils/services/types";
import { FC } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

type Props = {
  user: IUser;
  size?: any;
  direction?: any;
};

const UserProfileMenu: FC<Props> = ({
  user: { fullName, id, imageUrl, role },
  size = "md",
}) => {
  const auth = useAuth();
  const { t } = useTranslation();
  const { colorScheme, toggleColorScheme } = useMantineColorScheme();

  return (
    <Group justify="center">
      <Menu
        withArrow
        styles={{
          item: {
            width: "130px",
          },
        }}
      >
        <Menu.Target>
          {auth?.auth && (
            <div>
              <Avatar
                my={3}
                src={imageUrl}
                c="cyan"
                radius={10000}
                size={size}
                style={{ cursor: "pointer" }}
              >
                {!imageUrl && getInitials(fullName ?? "")}
              </Avatar>
            </div>
          )}
        </Menu.Target>

        <Menu.Dropdown miw={"200px"}>
          <Box style={{ textAlign: "center" }}>
            <Text size={"xl"} fw="bolder">
              {fullName}
            </Text>
            <Text size={"sm"} c={"dimmed"}>
              {t(`${UserRole[Number(role)]}`)}
            </Text>
          </Box>
          <Divider />

          <Menu.Item
            component={Link}
            to={`/userProfile/${id}/certificate`}
            style={{ width: "100%" }}
            leftSection={<IconUser size={14} />}
          >
            <Text size={size}>{t("myProfile")}</Text>
          </Menu.Item>
          <Menu.Item
            component={Link}
            to={`/settings?edit=1`}
            style={{ width: "100%" }}
            leftSection={<IconPencil size={14} />}
          >
            <Text size={size}>{t("editProfile")}</Text>
          </Menu.Item>
          <Menu.Item
            component={Link}
            to={`/settings/account`}
            style={{ width: "100%" }}
            leftSection={<IconLock size={14} />}
          >
            <Text size={size}>{t("Account")}</Text>
          </Menu.Item>
          <Menu.Item
            onClick={() => toggleColorScheme()}
            style={{ width: "100%" }}
            leftSection={
              colorScheme === "dark" ? (
                <IconSun size={15} />
              ) : (
                <IconMoonStars size={15} />
              )
            }
          >
            <Text size={size}>{t("Theme")}</Text>
          </Menu.Item>
          <Menu.Item
            onClick={auth?.logout}
            style={{ width: "100%" }}
            leftSection={<IconLogout size={14} />}
          >
            <Text size={size}>{t("Logout")}</Text>
          </Menu.Item>
        </Menu.Dropdown>
      </Menu>
    </Group>
  );
};

export default UserProfileMenu;
