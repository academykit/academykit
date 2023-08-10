export const api = {
  auth: {
    login: '/api/Account/Login',
    logout: '/api/Account/Logout',
    refreshToken: '/api/Account/RefreshToken',
    changePassword: '/api/Account/ChangePassword',
    forgotPassword: '/api/Account/ForgotPassword',
    resetToken: '/api/Account/VerifyResetToken',
    resetPassword: '/api/Account/ResetPassword',
    getUser: (userId: string) => `/api/user/${userId}`,
    changeEmail: '/api/user/changeEmailRequest',
    resendEmailVerification: '/api/user/resendChangeEmailRequest',
    verifyChangeEmail: '/api/user/verifychangeemail',
    me: '/api/account',
    resendEmail: (id: string) => `/api/User/${id}/resendemail`,
  },
  adminUser: {
    users: (queryString: any) => `api/user?${queryString}`,
    addUsers: `api/user`,
    editUsers: (userId: string) => `api/user/${userId}`,
    getCompanySettings: `api/admin/settings/company`,
    getGeneralSettings: `api/admin/settings`,
    getZoomSettings: `api/admin/settings/zoom`,
    getZoomLicense: `api/zoomlicense`,
    getActiveZoomLicense: (query: string) => `api/zoomlicense/active?${query}`,
    getSMTPSettings: `api/admin/settings/smtp`,
    updateGeneralSettings: (id: string | undefined) =>
      `api/admin/settings/${id}`,
    updateZoomSettings: (id: string | undefined) =>
      `api/admin/settings/zoom/${id}`,
    updateSMTPSettings: (id: string | undefined) =>
      `api/admin/settings/smtp/${id}`,
    updateUserStatus: (userId: string, enabled: boolean) =>
      `/api/User/${userId}/status?enabled=${enabled}`,
    updateZoomLicenseStatus: (userId: string, enabled: boolean) =>
      `/api/ZoomLicense/${userId}/status?enabled=${enabled}`,
    deleteZoomLicense: (id: string) => `/api/ZoomLicense/${id}`,
    addZoomLicense: `/api/ZoomLicense`,
    deleteLevelSetting: (id: string) => `api/level/${id}`,
    getDepartmentSettings: `api/department`,
    postDepartmentSetting: () => `/api/department/`,
    deleteDepartmentSetting: (id: string) => `api/department/${id}`,
    updateDepartmentStatus: (id: string, status: boolean) =>
      `/api/department/${id}/status?enabled=${status}`,

    getLevelSetting: `api/level`,
    postLevelSetting: `api/level`,
    updateLevelSetting: (id: string) => `api/level/${id}`,
    updateDepartmentSetting: (id: string) => `api/department/${id}`,
    getTrainer: (search: string, lessonType?: number, id?: string) =>
      `/api/user/trainer?${search}&LessonType=${lessonType}&Identity=${id}`,
    getLogs: (query: string) => `/api/ServerLog/logs?${query}`,
    getSingleLog: (id: string) => `/api/ServerLog/${id}`,
  },
  groups: {
    list: '/api/Group',
    details: (id: string) => `/api/group/${id}`,
    member: (id: string, query: string) => `/api/Group/${id}/members?${query}`,
    addMember: (id: string) => `/api/group/${id}/addMember`,
    removeMember: (id: string, memberId: string) =>
      `/api/group/${id}/removeMember/${memberId}`,
    course: (groupId: string) => `/api/group/${groupId}/courses`,
    attachment: `/api/group/files`,
    addAttachment: `/api/group/file`,
    removeAttachment: (identity: string, fileId: string) =>
      `/api/group/${identity}/files/${fileId}`,
    notMembers: (
      identity: string,
      query: string,
      departmentId: string | undefined
    ) => {
      const url = `/api/group/${identity}/notMembers?${query}`;
      if (departmentId) {
        return url + `&departmentIdentity=${departmentId}`;
      }
      return url;
    },
  },

  course: {
    list: '/api/course',
    userList: (id: string) => `/api/course/user/${id}`,

    detail: (id: string) => `/api/course/${id}`,
    reorder: (id: string) => `/api/course/${id}/lesson/reorder`,
    reorderSection: (id: string) => `/api/course/${id}/section/reorder`,
    update: (id: string) => `/api/course/${id}`,
    enroll: (id: string) => `/api/Course/${id}/enroll`,
    status: `/api/Course/status`,
    lessonStat: (id: string) => `/api/course/${id}/lessonStatistics`,
    lessonStatDetails: (id: string, lessonId: string, qs: string) =>
      `/api/course/${id}/lessonStatistics/${lessonId}?${qs}`,

    studentStat: (id: string) => `/api/course/${id}/studentStatistics`,
    studentStatDetails: (id: string, userId: string) =>
      `/api/course/${id}/studentStatistics/${userId}`,
    certificate: (identity: string, search: string) =>
      `/api/course/${identity}/certificate?${search}`,
    postCertificate: (identity: string) =>
      `/api/course/${identity}/certificate/issue`,
    dashboard: `/api/dashboard`,
    dashboardCourse: `/api/dashboard/course`,
    updateCourse: (id: string) => `/api/course/${id}/updateCourse`,

    createSignature: (courseId: string) => `/api/course/${courseId}/signature`,
    getSignature: (id: string) => `/api/course/${id}/signature`,
    deleteSignature: (id: string, sigId: string) =>
      `/api/course/${id}/signature/${sigId}`,
    editSignature: (id: string, sigId: string) =>
      `/api/course/${id}/signature/${sigId}`,

    addCertificateDetails: (courseId: string) =>
      `/api/course/${courseId}/certificate`,
    getCertificateDetails: (courseId: string) =>
      `/api/course/${courseId}/certificate/detail`,
    getManageStat: (id: string) => `/api/course/${id}/statistics`,
  },
  section: {
    common: '/api/section',
    section: (id: string) => `/api/course/${id}/section`,
    updateSection: (id: string, sectionId: string) =>
      `/api/course/${id}/section/${sectionId}`,
  },
  lesson: {
    common: '/api/course/lesson',
    addLesson: (courseIdentity: string) =>
      `/api/course/${courseIdentity}/lesson`,
    updateLesson: (courseId: string, lessonId: string) =>
      `/api/course/${courseId}/lesson/${lessonId}`,
    deleteLesson: (id: string, lessonId: string) =>
      `/api/course/${id}/lesson/${lessonId}`,
    courseLesson: (courseId: string, lessonId?: string) =>
      lessonId
        ? `/api/course/${courseId}/lesson/detail?lessonIdentity=${lessonId}`
        : `/api/course/${courseId}/lesson/detail`,
  },
  physicalTraining: {
    attendance: (identity: string) =>
      `/api/PhysicalLesson/Attendance?Idenity=${identity}`,
    review: '/api/PhysicalLesson/Review',
  },
  tags: {
    list: '/api/tag',
  },
  levels: {
    list: '/api/level',
  },
  pool: {
    list: '/api/questionPool',
    getOne: (poolId: string) => `/api/questionPool/${poolId}`,
  },
  courseTeacher: {
    list: '/api/courseTeacher',
    detail: (id: string) => `/api/courseTeacher/${id}`,
  },
  questions: {
    list: (poolId: string) => `/api/questionpool/${poolId}/question`,
    one: (poolId: string, questionId: string) =>
      `/api/questionpool/${poolId}/question/${questionId}`,
    delete: (poolId: string, questionId: string) =>
      `/api/questionpool/${poolId}/question/${questionId}`,
    put: (poolId: string, quesitonId: string) =>
      `/api/questionpool/${poolId}/question/${quesitonId}`,
  },
  questionSet: {
    common: '/api/questionset',
    addQuestion: (identity: string) =>
      `/api/questionSet/${identity}/addquestion`,
    getQuestion: (identity: string) => `/api/questionset/${identity}/questions`,
  },
  poolTeacher: {
    list: `/api/questionPoolTeacher`,
    get: (q_id: string) =>
      `/api/questionPoolTeacher?QuestionPoolIdentity=${q_id}`,
    status: (id: string) => `/api/questionPoolTeacher/${id}/status`,
    detail: (id: string) => `/api/questionPoolTeacher/${id}`,
  },
  assignment: {
    add: '/api/assignment',
    list: (lessonId: string, search?: string) =>
      search
        ? '/api/Assignment' + `?${search}&LessonIdentity=${lessonId}`
        : '/api/Assignment' + `?LessonIdentity=${lessonId}`,
    listOne: (id: string) => `/api/Assignment/${id}`,
    submitAssignment: (id: string) => `/api/assignment/${id}/submissions`,
    assignmentReview: (id: string, userId: string) =>
      `/api/assignment/${id}/user/${userId}`,
    addReview: (lessonId: string) => `/api/assignment/${lessonId}/review`,
    editReview: (lessonId: string, id: string) =>
      `/api/assignment/${lessonId}/review/${id}`,
  },
  feedback: {
    add: '/api/feedback',
    list: (lessonId: string, search?: string) =>
      search
        ? '/api/feedback' + `?${search}&LessonIdentity=${lessonId}`
        : '/api/feedback' + `?LessonIdentity=${lessonId}`,
    listOne: (id: string) => `/api/feedback/${id}`,
    submitFeedback: (id: string) => `/api/feedback/${id}/submissions`,
    userFeedback: (lessonId: string, userId: string, search?: string) =>
      search
        ? '/api/feedback' +
          `?${search}&LessonIdentity=${lessonId}&UserId=${userId}`
        : '/api/feedback' + `?LessonIdentity=${lessonId}&UserId=${userId}`,
    exportFeedback: (lessonId: string) => `/api/Feedback/${lessonId}/export`,
  },
  exam: {
    startExam: (id: string) => `/api/QuestionSet/${id}/startExam`,
    submitExam: (id: string, questionSetSubmissionId: string) =>
      `/api/QuestionSet/${id}/submission/${questionSetSubmissionId}`,
    getStudent: (id: string) => `/api/QuestionSet/${id}/results`,
    getStudentResults: (id: string, userId: string) =>
      `/api/QuestionSet/${id}/results/${userId}`,
    getOneResult: (id: string, userId: string) =>
      `/api/QuestionSet/${id}/results/${userId}`,
    getOneExamResult: (id: string, questionSetSubmissionId: string) =>
      `/api/QuestionSet/${id}/results/${questionSetSubmissionId}/detail`,
  },
  meeting: {
    joinMeeting: (courseId?: string, lessonId?: string) =>
      `/api/course/${courseId}/lesson/${lessonId}/join`,
    meetingReport: (courseId: string, lessonId: string, userId: string) =>
      `/api/course/${courseId}/lesson/${lessonId}/meetingreport/${userId}`,
  },
  watchHistory: {
    create: '/api/watchHistory',
    updateUser: (userId: string) => `/api/watchHistory/pass/${userId}`,
  },
  fileStorage: {
    getFileStorageSetting: '/api/media/setting',
    updateFileStorageSetting: '/api/media/setting',
    getFileStorageSettingValue: (type: string) =>
      `/api/media/settingvalue?type=${type}`,
  },
  comments: {
    list: (courseId: string) => `/api/course/${courseId}/comments`,
    details: (courseId: string, commentId: string) =>
      `/api/course/${courseId}/comments/${commentId}`,
    getRepliesList: (courseId: string, commentId: string) =>
      `/api/course/${courseId}/comments/${commentId}`,
    repliesList: (courseId: string, commentId: string) =>
      `/api/course/${courseId}/comments/${commentId}/commentReply`,
    repliesDetails: (courseId: string, commentId: string, replyId: string) =>
      `/api/course/${courseId}/comments/${commentId}/commentReply/${replyId}`,
  },
  externalCertificate: {
    add: '/api/certificate/external',
    user: (id?: string) => `/api/certificate/external/${id}`,
    list: `/api/certificate/review`,
    updateStatus: (id?: string) => `/api/certificate/${id}/verify`,
    update: (id?: string) => `/api/certificate/${id}/external`,
    internal: `/api/certificate/internal`,
  },
};
