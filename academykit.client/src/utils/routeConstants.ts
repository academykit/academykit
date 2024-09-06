import { isDevelopment } from "./env";

const RoutePath = {
  login: "/login",
  forgotPassword: "/forgot-password",
  confirmToken: "/confirm",
  initialSetup: "/initial/setup",
  signInRedirect: "/redirect/signIn",
  oAuthError: "/error",
  404: "/404",
  500: "/500",
  403: "/403",
  401: "/401",
  verify: "/verify",
  verifyChangeEmail: "/changeEmail",

  userDashboard: "/",
  courses: {
    courseList: "/trainings/list",
    base: "/trainings",
    enrolled: "/trainings/enrolled",
    created: "/trainings/created",
    create: "/trainings/create",
    description: function (id?: string) {
      return {
        signature: `${this.base}/:id`,
        route: `${this.base}/${id}`,
      };
    },
  },

  assessment: {
    assessmentList: "/assessment/list",
    base: "/assessment",
    create: "/assessment/create",
    description: function (id?: string) {
      return {
        signature: `${this.base}/:id`,
        route: `${this.base}/${id}`,
      };
    },
  },

  manageAssessment: {
    base: "/assessment/stat",
    description: function (id?: string) {
      return {
        signature: `${this.base}/:id`,
        route: `${this.base}/${id}`,
      };
    },
    manage: function (id?: string) {
      return {
        signature: `${this.base}/:id`,
        route: `${this.base}/${id}`,
      };
    },
    edit: function (id?: string) {
      return {
        signature: `${this.description("").signature}/edit`,
        route: `${this.description(id).route}/edit`,
        routes: () => `${this.description(id).route}/edit`,
      };
    },
    question: function (id?: string) {
      return {
        signature: `${this.base}/:id/questions`,
        route: `${this.base}/${id}/questions`,
      };
    },
    assessmentStat: function (id?: string) {
      return {
        signature: `${this.base}/:id/statistics`,
        route: `${this.base}/${id}/statistics`,
      };
    },
    setting: function (id?: string) {
      return {
        signature: `${this.base}/:id/setting`,
        route: `${this.base}/${id}/setting`,
      };
    },
  },

  assessmentExam: {
    base: "/assessment/exam",
    details: (id?: string) => ({
      signature: "/assessment/exam/:id",
      route: `/assessment/exam/${id}`,
    }),
    result: (id?: string) => ({
      signature: "/assessment/exam/:id/result",
      route: `/assessment/exam/${id}/result`,
    }),
    resultOne: (id?: string, submissionId?: string) => ({
      signature: "/assessment/exam/:id/result/:submissionId",
      route: `/assessment/exam/${id}/result/${submissionId}`,
    }),
  },

  manageCourse: {
    base: "/trainings/stat",
    description: function (id?: string) {
      return {
        signature: `${this.base}/:id`,
        route: `${this.base}/${id}`,
      };
    },
    manage: function (id?: string) {
      return {
        signature: `${this.base}/:id`,
        route: `${this.base}/${id}`,
      };
    },
    student: function (id?: string) {
      return {
        signature: `${this.base}/:id/students`,
        route: `${this.base}/${id}/students`,
      };
    },
    lessons: function (id?: string) {
      return {
        signature: `${this.base}/:id/lessons`,
        route: `${this.base}/${id}/lessons`,
      };
    },
    certificate: function (id?: string) {
      return {
        signature: `${this.base}/:id/certificate`,
        route: `${this.base}/${id}/certificate`,
      };
    },
    dashboard: function (id?: string) {
      return {
        signature: `${this.base}/:id/dashboard`,
        route: `${this.base}/${id}/dashboard`,
      };
    },
    edit: function (id?: string) {
      return {
        signature: `${this.description("").signature}/edit`,
        route: `${this.description(id).route}/edit`,
        routes: () => `${this.description(id).route}/edit`,
      };
    },
    teachers: function (id?: string) {
      return {
        signature: `${this.description("").signature}/teachers`,
        route: `${this.description(id).route}/teachers`,
      };
    },
    lessonsStat: function (id?: string) {
      return {
        signature: `${this.description("").signature}/lesson-stat`,
        route: `${this.description(id).route}/lessons-stat`,
      };
    },

    certificateStat: function (id?: string) {
      return {
        signature: `${this.description("").signature}/certificate-stat`,
        route: `${this.description(id).route}/certificate-stat`,
      };
    },
  },
  users: "/users",
  createSessions: "/sessions/create",
  userInfo: "/userInfo",
  userProfile: "/userProfile",

  classes: "/classes",
  settings: {
    base: "/settings",
    profile: function () {
      return `${this.base}/account`;
    },
    userCertificate: function () {
      return `${this.base}/certificate`;
    },

    training: function () {
      return `${this.base}/my-certificate`;
    },
    general: function () {
      return `${this.base}/general`;
    },
    zoom: function () {
      return `${this.base}/zoom`;
    },
    zoomLicense: function () {
      return `${this.base}/zoomLicense`;
    },
    smtp: function () {
      return `${this.base}/smtp`;
    },
    fileStorage: function () {
      return `${this.base}/fileStorage`;
    },
    ai: function () {
      return `${this.base}/ai-setup`;
    },
    department: function () {
      return `${this.base}/department`;
    },
    skill: function () {
      return `${this.base}/skill`;
    },
    sso: function () {
      return `${this.base}/sso`;
    },
    hangfire: () =>
      `${
        isDevelopment
          ? "https://localhost:7042"
          : `${location.protocol}//${window.location.host}`
      }/hangfire`,
    log: function () {
      return `${this.base}/log`;
    },
    courses: function () {
      return `${this.base}/courses`;
    },
    mail: function () {
      return `${this.base}/mail-notification`;
    },
  },
  profile: "/settings/account",
  general: "/settings/general",
  zoom: "/settings/zoomSettings",
  zoomLicense: "/settings/zoomLicense",
  smtp: "/settings/smtp",
  file: "/settings/fileStorage",
  payment: "/settings/paymentMethods",
  level: "/settings/level",
  department: "/settings/department",
  sso: {
    base: "/settings/sso",
  },
  pool: {
    base: "/pools",
    details: function (id?: string) {
      return { signature: `${this.base}/:id`, route: `${this.base}/${id}` };
    },
    teachers: function (id?: string) {
      return {
        signature: `${this.details("").signature}/authors`,
        route: `${this.details(id).route}/authors`,
      };
    },
    questions: function (id?: string) {
      return {
        signature: `${this.details("").signature}/questions`,
        route: `${this.details(id).route}/questions`,
      };
    },
  },
  groups: {
    base: "/groups",
    details: function (id?: string) {
      return { signature: `${this.base}/:id`, route: `${this.base}/${id}` };
    },
    members: function (id?: string) {
      return {
        signature: `${this.details("").signature}/members`,
        route: `${this.details(id).route}/members`,
      };
    },
    courses: function (id?: string) {
      return {
        signature: `${this.details().signature}/courses`,
        route: `${this.details(id).route}/courses`,
      };
    },
    attachments: function (id?: string) {
      return {
        signature: `${this.details().signature}/attachments`,
        route: `${this.details(id).route}/attachments`,
      };
    },
  },
  exam: {
    base: "/exam",
    details: (id?: string) => ({
      signature: "/exam/:id",
      route: `/exam/${id}`,
    }),
    result: (id?: string) => ({
      signature: "/exam/:id/result",
      route: `/exam/${id}/result`,
    }),
    resultOne: (id?: string, submissionId?: string) => ({
      signature: "/exam/:id/result/:submissionId",
      route: `/exam/${id}/result/${submissionId}`,
    }),
  },
  meeting: {
    base: "/meeting",
    details: (id?: string, courseSlug?: string) => ({
      signature: "/meeting/:courseSlug/:id",
      route: `/meeting/${courseSlug}/${id}`,
    }),
  },

  assignment: {
    base: "/assignment",
    details: (id?: string) => ({
      signature: "/assignment/:id",
      route: `/assignment/${id}`,
    }),
    result: (id?: string, studentId?: string) => ({
      signature: "/assignment/:id/result/:studentId",
      route: `/assignment/${id}/result/${studentId}`,
    }),
  },
  feedback: {
    base: "/feedback",
    details: (id?: string) => ({
      signature: "/feedback/:id",
      route: `/feedback/${id}`,
    }),
    myDetails: (id?: string) => ({
      signature: "/myfeedback/:id",
      route: `/myfeedback/${id}`,
    }),
    result: (id?: string, studentId?: string) => ({
      signature: "/feedback/:id/result/:studentId",
      route: `/feedback/${id}/result/${studentId}`,
    }),
  },
  knowledge: {
    base: "knowledge-base",
  },
};

export default RoutePath;
