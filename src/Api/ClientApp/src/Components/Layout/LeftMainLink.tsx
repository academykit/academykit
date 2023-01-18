import React from "react";
import {
  IconCertificate,
  IconListDetails,
  IconUser,
  IconSettings,
  IconDashboard,
  IconUsers,
} from "@tabler/icons";
import { ThemeIcon, NavLink } from "@mantine/core";
import { Link, useLocation } from "react-router-dom";
import useAuth from "@hooks/useAuth";
import { UserRole } from "@utils/enums";

type MainLinkProps = {
  icon: React.ReactNode;
  color: string;
  label: string;
  href: string;
  onClose(): void;
  replace: boolean;
};

function MainLink({
  icon,
  color,
  label,
  href,
  onClose,
  replace,
}: MainLinkProps) {
  const router = useLocation();

  return (
    <NavLink
      onClick={onClose}
      component={Link}
      to={href}
      replace={replace}
      active={router.pathname.split("/")[1] === href.split("/")[1]}
      label={label}
      icon={<ThemeIcon color={color}>{icon}</ThemeIcon>}
      sx={(theme) => ({
        padding: theme.spacing.xs,
        borderRadius: theme.radius.sm,
      })}
    ></NavLink>
  );
}

type LeftMainLinksProps = {
  onClose(): void;
};

export function LeftMainLinks({ onClose }: LeftMainLinksProps) {
  const auth = useAuth();

  const data = [
    {
      icon: <IconDashboard size={16} />,
      color: "purple",
      label: "Dashboard",
      href: "/",
      replace: true,
      role: UserRole.Trainee,
    },
    {
      icon: <IconUser size={16} />,
      color: "blue",
      label: "Users",
      href: "/users",
      replace: true,
      role: UserRole.Admin,
    },
    {
      icon: <IconUsers size={16} />,
      color: "yellow",
      label: "Groups",
      href: "/groups",
      replace: true,
      role: UserRole.Trainee,
    },
    {
      icon: <IconCertificate size={16} />,
      color: "red",
      label: "Trainings",
      href: "/trainings/list",
      replace: true,
      role: UserRole.Trainee,
    },
    {
      icon: <IconListDetails size={16} />,
      color: "violet",
      label: "MCQ Pools",
      href: "/pools",
      replace: true,
      role: UserRole.Trainer,
    },


    {
      icon: <IconSettings size={16} />,
      color: "teal",
      label: "Settings",
      href: "/settings",
      replace: false,
      role: UserRole.Trainee,
    },
  ];

  const links = data.map((link) => {
    if (auth?.auth?.role) {
      if (link.role >= auth?.auth?.role) {
        return <MainLink {...link} key={link.label} onClose={onClose} />;
      }
    }
  });
  return <div style={{ padding: "5px !important" }}>{links}</div>;
}
