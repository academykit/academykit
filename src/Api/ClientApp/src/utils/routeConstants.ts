import { isDevelopment } from './env';

const RoutePath = {
  login: '/login',
  forgotPassword: '/forgot-password',
  confirmToken: '/confirm',
  404: '/404',
  500: '/500',
  403: '/403',
  401: '/401',
  verify: '/verify',
  verifyChangeEmail: '/changeEmail',

  userDashboard: '/',
  courses: {
    courseList: '/trainings/list',
    base: '/trainings',
    enrolled: '/trainings/enrolled',
    created: '/trainings/created',
    create: '/trainings/create',
    description: function (id?: string) {
      return {
        signature: this.base + '/:id',
        route: `${this.base}/${id}`,
      };
    },
  },

  manageCourse: {
    base: '/trainings/stat',
    description: function (id?: string) {
      return {
        signature: this.base + '/:id',
        route: `${this.base}/${id}`,
      };
    },
    manage: function (id?: string) {
      return {
        signature: this.base + '/:id',
        route: `${this.base}/${id}`,
      };
    },
    student: function (id?: string) {
      return {
        signature: this.base + '/:id/students',
        route: `${this.base}/${id}/students`,
      };
    },
    lessons: function (id?: string) {
      return {
        signature: this.base + '/:id/lessons',
        route: `${this.base}/${id}/lessons`,
      };
    },
    certificate: function (id?: string) {
      return {
        signature: this.base + '/:id/certificate',
        route: `${this.base}/${id}/certificate`,
      };
    },
    dashboard: function (id?: string) {
      return {
        signature: this.base + '/:id/dashboard',
        route: `${this.base}/${id}/dashboard`,
      };
    },
    edit: function (id?: string) {
      return {
        signature: this.description('').signature + '/edit',
        route: `${this.description(id).route}/edit`,
        routes: () => `${this.description(id).route}/edit`,
      };
    },
    teachers: function (id?: string) {
      return {
        signature: this.description('').signature + '/teachers',
        route: `${this.description(id).route}/teachers`,
      };
    },
    lessonsStat: function (id?: string) {
      return {
        signature: this.description('').signature + '/lesson-stat',
        route: `${this.description(id).route}/lessons-stat`,
      };
    },

    certificateStat: function (id?: string) {
      return {
        signature: this.description('').signature + '/certificate-stat',
        route: `${this.description(id).route}/certificate-stat`,
      };
    },
  },
  users: '/users',
  createSessions: '/sessions/create',
  userInfo: '/userInfo',
  userProfile: '/userProfile',

  classes: '/classes',
  settings: {
    base: '/settings',
    profile: function () {
      return this.base + '/account';
    },
    userCertificate: function () {
      return this.base + '/certificate';
    },

    training: function () {
      return this.base + '/myTraining';
    },
    general: function () {
      return this.base + '/general';
    },
    zoom: function () {
      return this.base + '/zoom';
    },
    zoomLicense: function () {
      return this.base + '/zoomLicense';
    },
    smtp: function () {
      return this.base + '/smtp';
    },
    fileStorage: function () {
      return this.base + '/fileStorage';
    },
    department: function () {
      return this.base + '/department';
    },
    hangfire: function () {
      return (
        (isDevelopment
          ? 'https://localhost:7042'
          : location.protocol + '//' + window.location.host) + '/hangfire'
      );
    },
    log: function () {
      return this.base + '/log';
    },
    courses: function () {
      return this.base + '/courses';
    },
  },
  profile: '/settings/account',
  general: '/settings/general',
  zoom: '/settings/zoomSettings',
  zoomLicense: '/settings/zoomLicense',
  smtp: '/settings/smtp',
  file: '/settings/fileStorage',
  payment: '/settings/paymentMethods',
  level: '/settings/level',
  department: '/settings/department',
  pool: {
    base: '/pools',
    details: function (id?: string) {
      return { signature: this.base + '/:id', route: `${this.base}/${id}` };
    },
    teachers: function (id?: string) {
      return {
        signature: this.details('').signature + '/authors',
        route: `${this.details(id).route}/authors`,
      };
    },
    questions: function (id?: string) {
      return {
        signature: this.details('').signature + '/questions',
        route: `${this.details(id).route}/questions`,
      };
    },
  },
  groups: {
    base: '/groups',
    details: function (id?: string) {
      return { signature: this.base + '/:id', route: `${this.base}/${id}` };
    },
    members: function (id?: string) {
      return {
        signature: this.details('').signature + '/members',
        route: `${this.details(id).route}/members`,
      };
    },
    courses: function (id?: string) {
      return {
        signature: this.details().signature + '/courses',
        route: `${this.details(id).route}/courses`,
      };
    },
    attachments: function (id?: string) {
      return {
        signature: this.details().signature + '/attachments',
        route: `${this.details(id).route}/attachments`,
      };
    },
  },
  exam: {
    base: '/exam',
    details: function (id?: string) {
      return {
        signature: '/exam/:id',
        route: `/exam/${id}`,
      };
    },
    result: function (id?: string) {
      return {
        signature: '/exam/:id/result',
        route: `/exam/${id}/result`,
      };
    },
    resultOne: function (id?: string, submissionId?: string) {
      return {
        signature: '/exam/:id/result/:submissionId',
        route: `/exam/${id}/result/${submissionId}`,
      };
    },
  },
  meeting: {
    base: '/meeting',
    details: function (id?: string, courseSlug?: string) {
      return {
        signature: '/meeting/:courseSlug/:id',
        route: `/meeting/${courseSlug}/${id}`,
      };
    },
  },

  assignment: {
    base: '/assignment',
    details: function (id?: string) {
      return {
        signature: '/assignment/:id',
        route: `/assignment/${id}`,
      };
    },
    result: function (id?: string, studentId?: string) {
      return {
        signature: '/assignment/:id/result/:studentId',
        route: `/assignment/${id}/result/${studentId}`,
      };
    },
  },
  feedback: {
    base: '/feedback',
    details: function (id?: string) {
      return {
        signature: '/feedback/:id',
        route: `/feedback/${id}`,
      };
    },
    myDetails: function (id?: string) {
      return {
        signature: '/myfeedback/:id',
        route: `/myfeedback/${id}`,
      };
    },
    result: function (id?: string, studentId?: string) {
      return {
        signature: '/feedback/:id/result/:studentId',
        route: `/feedback/${id}/result/${studentId}`,
      };
    },
  },
};

export default RoutePath;
