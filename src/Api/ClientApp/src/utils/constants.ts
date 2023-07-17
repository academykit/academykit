import { CourseStatus } from './enums';

export const COLOR_SCHEME_KEY = 'theme-color';
export const TOKEN_STORAGE = 'token';
export const REFRESH_TOKEN_STORAGE = 'refreshToken';
// export const PHONE_VALIDATION = /^(?:\+?977)?(?:\+?977-)?[98]\d{9}$/;
export const PHONE_VALIDATION = /^[0-9+]+$/;

export const DATE_FORMAT = 'MMM DD, YYYY';

export const color = (status: CourseStatus) => {
  switch (status) {
    case CourseStatus.Draft:
      return 'violet';
    case CourseStatus.Published:
      return 'green';
    case CourseStatus.Review:
      return 'yellow';
    case CourseStatus.Rejected:
      return 'red';
  }
};
