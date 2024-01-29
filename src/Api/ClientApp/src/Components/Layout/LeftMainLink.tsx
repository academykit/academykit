import useAuth from '@hooks/useAuth';
import { NavLink, ThemeIcon } from '@mantine/core';
import {
  IconCertificate,
  IconDashboard,
  IconListDetails,
  IconSettings,
  IconUser,
  IconUsers,
} from '@tabler/icons';
import { TOKEN_STORAGE } from '@utils/constants';
import { UserRole } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import { adminAndOrgRouteGroups, getCurrentNavGroup } from '@utils/routeGroups';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useLocation } from 'react-router-dom';

type MainLinkProps = {
  icon: React.ReactNode;
  color: string;
  label: string;
  href: string;
  onClose(): void;
  replace: boolean;
  target?: string;
  childRoutes?: {
    label: string;
    href: string;
    target?: string;
    role: number;
  }[];
  authRole: string;
};

function MainLink({
  icon,
  color,
  label,
  href,
  onClose,
  replace,
  target = '_self',
  childRoutes,
  authRole,
}: MainLinkProps) {
  const router = useLocation();
  const { t } = useTranslation();

  return (
    <>
      {target === '_self' &&
        (childRoutes ? (
          <>
            <NavLink
              leftSection={<ThemeIcon color={color}>{icon}</ThemeIcon>}
              style={(theme) => ({
                padding: theme.spacing.xs,
                borderRadius: theme.radius.sm,
              })}
              childrenOffset={28}
              label={t(`${label}`)}
              mb={5}
              active={
                href ==
                '/' + decodeURI(router.pathname.split('/').at(1) as string) // decodeURI for multi-language support
              }
            >
              {childRoutes.map((route) => {
                if (route.role >= Number(authRole)) {
                  return (
                    <NavLink
                      onClick={onClose}
                      key={route.label}
                      component={Link}
                      to={route.href}
                      style={(theme) => ({
                        padding: theme.spacing.xs,
                        borderRadius: theme.radius.sm,
                      })}
                      label={t(`${route.label}`)}
                      active={
                        route.label ==
                        getCurrentNavGroup(
                          router.pathname as string,
                          adminAndOrgRouteGroups
                        )
                      }
                      target={route.target ?? '_self'}
                    />
                  );
                }
              })}
            </NavLink>
          </>
        ) : (
          <NavLink
            onClick={onClose}
            component={Link}
            to={href}
            target={target}
            replace={replace}
            active={router.pathname.split('/')[1] === href.split('/')[1]}
            label={t(`${label}`)}
            leftSection={<ThemeIcon color={color}>{icon}</ThemeIcon>}
            style={(theme) => ({
              padding: theme.spacing.xs,
              borderRadius: theme.radius.sm,
            })}
          />
        ))}

      {target !== '_self' && (
        <NavLink
          component="a"
          href={href}
          label={t(`${label}`)}
          target={target}
          leftSection={<ThemeIcon color={color}>{icon}</ThemeIcon>}
          style={(theme) => ({
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
      childRoutes: [
        { label: 'account', href: '/settings', role: UserRole.Trainee },
        {
          label: 'admin',
          href:
            Number(auth?.auth?.role) == UserRole.Admin // admin has no access to general settings
              ? '/settings/zoomLicense'
              : '/settings/general',
          role: UserRole.Admin,
        },
        { label: 'reviews', href: '/settings/courses', role: UserRole.Admin },
        {
          label: 'system',
          href:
            RoutePath.settings.hangfire() +
            '?access_token=' +
            localStorage.getItem(TOKEN_STORAGE),
          target: '_blank',
          role: UserRole.Admin,
        },
      ],
    },
  ];

  const links = data.map((link) => {
    if (auth?.auth?.role) {
      if (link.role >= Number(auth?.auth?.role)) {
        return (
          <MainLink
            {...link}
            key={link.label}
            onClose={onClose}
            target={link.label === 'help' ? '_blank' : '_self'}
            authRole={auth?.auth?.role.toString()}
          />
        );
      }
    }
  });
  return <div style={{ padding: '5px !important' }}>{links}</div>;
}
