export interface IRouteGroup {
  route: string;
  group: string;
}

export const routeGroupAdmin = [
  { route: '/settings', group: 'account' },
  { route: '/settings/account', group: 'account' },
  { route: '/settings/mycertificate', group: 'account' },
  { route: '/settings/mycertificate/external', group: 'account' },
  { route: '/settings/profile-view', group: 'account' },

  { route: '/settings/general', group: 'admin' },
  { route: '/settings/zoom', group: 'admin' },
  { route: '/settings/zoomLicense', group: 'admin' },
  { route: '/settings/smtp', group: 'admin' },
  { route: '/settings/fileStorage', group: 'admin' },
  { route: '/settings/level', group: 'admin' },
  { route: '/settings/department', group: 'admin' },
  { route: '/settings/skill', group: 'admin' },
  { route: '/settings/mail-notification', group: 'admin' },
  { route: '/settings/ai-setup', group: 'admin' },

  { route: '/settings/courses', group: 'reviews' },
  { route: '/settings/certificate', group: 'reviews' },
];

export const routeGroupOrg = [
  { route: '/organization', group: 'structure' },
  { route: '/organization/business-unit', group: 'structure' },
  { route: '/organization/sub-business-unit', group: 'structure' },
  { route: '/organization/department', group: 'structure' },
  { route: '/organization/sections', group: 'structure' },

  { route: '/organization/location', group: 'geo' },
  { route: '/organization/branch', group: 'geo' },

  { route: '/organization/position', group: 'position' },
  { route: '/organization/designation', group: 'position' },
  { route: '/organization/job', group: 'position' },
  { route: '/organization/employment-type', group: 'position' },

  { route: '/organization/skill', group: 'other' },
  { route: '/organization/level', group: 'other' },
  { route: '/organization/training', group: 'other' },
  { route: '/organization/list', group: 'other' },
  { route: '/organization/mail-notification', group: 'other' },
  { route: '/organization/certificate', group: 'other' },
  { route: '/organization/fiscal-year', group: 'other' },
];

export const adminAndOrgRouteGroups = [...routeGroupAdmin, ...routeGroupOrg];

export const getCurrentGroup = (
  currentLocation: string,
  group: IRouteGroup[]
) => {
  const match = group.find(
    (item) => currentLocation == item.route.split('/')[2]
  );
  return match ? match.group : '';
};

export const getCurrentNavGroup = (
  currentLocation: string,
  group: IRouteGroup[]
) => {
  const match = group.find((item) => currentLocation == item.route);
  return match ? match.group : '';
};
