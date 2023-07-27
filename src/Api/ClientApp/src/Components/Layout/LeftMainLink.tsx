import React from 'react';
import {
  IconCertificate,
  IconListDetails,
  IconUser,
  IconSettings,
  IconDashboard,
  IconUsers,
} from '@tabler/icons';
import { ThemeIcon, NavLink } from '@mantine/core';
import { Link, useLocation } from 'react-router-dom';
import useAuth from '@hooks/useAuth';
import { UserRole } from '@utils/enums';
import { useTranslation } from 'react-i18next';

type MainLinkProps = {
  icon: React.ReactNode;
  color: string;
  label: string;
  href: string;
  onClose(): void;
  replace: boolean;
  target?: string;
};

function MainLink({
  icon,
  color,
  label,
  href,
  onClose,
  replace,
  target = '_self',
}: MainLinkProps) {
  const router = useLocation();
  const { t } = useTranslation();

  return (
    <>
      {target === '_self' && (
        <NavLink
          onClick={onClose}
          component={Link}
          to={href}
          target={target}
          replace={replace}
          active={router.pathname.split('/')[1] === href.split('/')[1]}
          label={t(`${label}`)}
          icon={<ThemeIcon color={color}>{icon}</ThemeIcon>}
          sx={(theme) => ({
            padding: theme.spacing.xs,
            borderRadius: theme.radius.sm,
          })}
        />
      )}
      {target !== '_self' && (
        <NavLink
          component="a"
          href={href}
          label={t(`${label}`)}
          target={target}
          icon={<ThemeIcon color={color}>{icon}</ThemeIcon>}
          sx={(theme) => ({
            padding: theme.spacing.xs,
            borderRadius: theme.radius.sm,
          })}
        />
      )}
    </>
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
      color: 'purple',
      label: 'dashboard',
      href: '/',
      replace: true,
      role: UserRole.Trainee,
    },
    {
      icon: <IconUser size={16} />,
      color: 'blue',
      label: 'users',
      href: '/users',
      replace: true,
      role: UserRole.Admin,
    },
    {
      icon: <IconUsers size={16} />,
      color: 'yellow',
      label: 'groups',
      href: '/groups',
      replace: true,
      role: UserRole.Trainee,
    },
    {
      icon: <IconCertificate size={16} />,
      color: 'red',
      label: 'trainings',
      href: '/trainings/list',
      replace: true,
      role: UserRole.Trainee,
    },

    {
      icon: <IconListDetails size={16} />,
      color: 'violet',
      label: 'mcq_pools',
      href: '/pools',
      replace: true,
      role: UserRole.Trainer,
    },

    {
      icon: <IconSettings size={16} />,
      color: 'teal',
      label: 'settings',
      href: '/settings',
      replace: false,
      role: UserRole.Trainee,
    },
  ];

  const links = data.map((link) => {
    if (auth?.auth?.role) {
      if (link.role >= auth?.auth?.role) {
        return (
          <MainLink
            {...link}
            key={link.label}
            onClose={onClose}
            target={link.label === 'help' ? '_blank' : '_self'}
          />
        );
      }
    }
  });
  return <div style={{ padding: '5px !important' }}>{links}</div>;
}
