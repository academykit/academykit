export const api = {
  auth: {
    login: "/api/Account/Login",
    logout: "/api/Account/Logout",
    refreshToken: "/api/Account/RefreshToken",
    changePassword: "/api/Account/ChangePassword",
    forgotPassword: "/api/Account/ForgotPassword",
    resetToken: "/api/Account/VerifyResetToken",
    resetPassword: "/api/Account/ResetPassword",
    getUser: (userId: string) => `/api/user/${userId}`,
    changeEmail: "/api/user/changeEmailRequest",
    resendEmailVerification: "/api/user/resendChangeEmailRequest",
    verifyChangeEmail: "/api/user/verifyChangeEmail",
    me: "/api/account",
    resendEmail: (id: string) => `/api/User/${id}/resendEmail`,
    microsoftSignIn: "/api/Account/signin-with-microsoft",
    googleSignIn: "/api/Account/signin-with-google",
  },
  adminUser: {
    users: (queryString: string) => `api/user?${queryString}`,
    addUsers: "api/user",
    editUsers: (userId: string) => `api/user/${userId}`,
    getCompanySettings: "api/admin/settings/company",
    getGeneralSettings: "api/admin/settings",
    getZoomSettings: "api/admin/settings/zoom",
    getZoomLicense: "api/zoomLicense",
    getActiveZoomLicense: (query: string) => `api/zoomLicense/active?${query}`,
    getSMTPSettings: "api/admin/settings/smtp",
    updateGeneralSettings: (id: string | undefined) =>
      `api/admin/settings/${id}`,
    createUpdateZoomSettings: () => "api/admin/settings/zoom",
    createUpdateSMTPSettings: () => "api/admin/settings/smtp",
    updateUserStatus: (userId: string, enabled: boolean) =>
      `/api/User/${userId}/status?enabled=${enabled}`,
    updateZoomLicenseStatus: (userId: string, enabled: boolean) =>
      `/api/ZoomLicense/${userId}/status?enabled=${enabled}`,
    deleteZoomLicense: (id: string) => `/api/ZoomLicense/${id}`,
    addZoomLicense: "/api/ZoomLicense",
    deleteLevelSetting: (id: string) => `api/level/${id}`,
    getDepartmentSettings: "api/department",
    postDepartmentSetting: () => "/api/department/",
    deleteDepartmentSetting: (id: string) => `api/department/${id}`,
    updateDepartmentStatus: (id: string, status: boolean) =>
      `/api/department/${id}/status?enabled=${status}`,

    getLevelSetting: "api/level",
    postLevelSetting: "api/level",
    updateLevelSetting: (id: string) => `api/level/${id}`,
    updateDepartmentSetting: (id: string) => `api/department/${id}`,
    getTrainer: (search: string, lessonType?: number, id?: string) =>
      `/api/user/trainer?${search}&LessonType=${lessonType}&Identity=${id}`,
    getLogs: (query: string) => `/api/ServerLog/logs?${query}`,
    getSingleLog: (id: string) => `/api/ServerLog/${id}`,
    getMailNotification: "/api/mailNotification",
    updateMailNotification: (id: string) => `/api/mailNotification/${id}`,
    testEmail: (id: string) => `/api/mailNotification/${id}/checkSendEmail`,
    initialSetup: "/api/initialSetup",
    allowedDomains: "/api/admin/settings/allowedDomains",
    defaultRole: "/api/admin/settings/defaultRole",
    signInOptions: "/api/admin/settings/signInOptions",
  },
  groups: {
    list: "/api/Group",
    details: (id: string) => `/api/group/${id}`,
    member: (id: string, query: string) => `/api/Group/${id}/members?${query}`,
    addMember: (id: string) => `/api/group/${id}/addMember`,
    removeMember: (id: string, memberId: string) =>
      `/api/group/${id}/removeMember/${memberId}`,
    course: (groupId: string) => `/api/group/${groupId}/courses`,
    attachment: "/api/group/files",
    addAttachment: "/api/group/file",
    removeAttachment: (identity: string, fileId: string) =>
      `/api/group/${identity}/files/${fileId}`,
    notMembers: (
      identity: string,
      query: string,
      departmentId: string | undefined
    ) => {
      const url = `/api/group/${identity}/notMembers?${query}`;
      if (departmentId) {
        return `${url}&departmentIdentity=${departmentId}`;
      }
      return url;
    },
  },

  course: {
    list: "/api/course",
    userList: (id: string) => `/api/course/user/${id}`,

    detail: (id: string) => `/api/course/${id}`,
    reorder: (id: string) => `/api/course/${id}/lesson/reorder`,
    reorderSection: (id: string) => `/api/course/${id}/section/reorder`,
    update: (id: string) => `/api/course/${id}`,
    enroll: (id: string) => `/api/Course/${id}/enroll`,
    status: "/api/Course/status",
    lessonStat: (id: string) => `/api/course/${id}/lessonStatistics`,
    lessonStatDetails: (id: string, lessonId: string, qs: string) =>
      `/api/course/${id}/lessonStatistics/${lessonId}?${qs}`,

    examSummary: (id: string, lessonId: string) =>
      `/api/course/${id}/lessonStatistics/${lessonId}/summary`,

    examSubmission: (id: string, lessonId: string) =>
      `/api/course/${id}/lessonStatistics/${lessonId}/submission`,

    assignmentSummary: (id: string, lessonId: string) =>
      `/api/course/${id}/lessonStatistics/${lessonId}/AssignmentSummary`,

    assignmentSubmission: (id: string, lessonId: string, qs: string) =>
      `/api/course/${id}/lessonStatistics/${lessonId}/AssignmentSubmission?${qs}`,

    studentStat: (id: string) => `/api/course/${id}/studentStatistics`,
    studentStatDetails: (id: string, userId: string) =>
      `/api/course/${id}/studentStatistics/${userId}`,
    certificate: (identity: string, search: string) =>
      `/api/course/${identity}/certificate?${search}`,
    postCertificate: (identity: string) =>
      `/api/course/${identity}/certificate/issue`,
    dashboard: "/api/dashboard",
    dashboardCourse: "/api/dashboard/course",
    dashboardUpcoming: "/api/dashboard/upcomingLesson",
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
    getShuffle: (trainingSlug: string, lessonSlug: string) =>
      `/api/course/${trainingSlug}/lesson/${lessonSlug}/getShuffle`,
    shuffle: (trainingSlug: string, lessonSlug: string) =>
      `/api/course/${trainingSlug}/lesson/${lessonSlug}/updateShuffle`,
  },
  section: {
    common: "/api/section",
    section: (id: string) => `/api/course/${id}/section`,
    updateSection: (id: string, sectionId: string) =>
      `/api/course/${id}/section/${sectionId}`,
  },
  lesson: {
    common: "/api/course/lesson",
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
      `/api/PhysicalLesson/Attendance?identity=${identity}`,
    review: "/api/PhysicalLesson/Review",
  },
  tags: {
    list: "/api/tag",
  },
  levels: {
    list: "/api/level",
  },
  pool: {
    list: "/api/questionPool",
    getOne: (poolId: string) => `/api/questionPool/${poolId}`,
  },
  courseTeacher: {
    list: "/api/courseTeacher",
    detail: (id: string) => `/api/courseTeacher/${id}`,
  },
  questions: {
    list: (poolId: string) => `/api/questionPool/${poolId}/question`,
    one: (poolId: string, questionId: string) =>
      `/api/questionPool/${poolId}/question/${questionId}`,
    delete: (poolId: string, questionId: string) =>
      `/api/questionPool/${poolId}/question/${questionId}`,
    put: (poolId: string, questionId: string) =>
      `/api/questionPool/${poolId}/question/${questionId}`,
  },
  questionSet: {
    common: "/api/questionSet",
    addQuestion: (identity: string) =>
      `/api/questionSet/${identity}/addQuestion`,
    getQuestion: (identity: string) => `/api/questionSet/${identity}/questions`,
  },
  poolTeacher: {
    list: "/api/questionPoolTeacher",
    get: (q_id: string) =>
      `/api/questionPoolTeacher?QuestionPoolIdentity=${q_id}`,
    status: (id: string) => `/api/questionPoolTeacher/${id}/status`,
    detail: (id: string) => `/api/questionPoolTeacher/${id}`,
  },
  assignment: {
    add: "/api/assignment",
    list: (lessonId: string, search?: string) =>
      search
        ? `/api/Assignment?${search}&LessonIdentity=${lessonId}`
        : `/api/Assignment?LessonIdentity=${lessonId}`,
    listOne: (id: string) => `/api/Assignment/${id}`,
    submitAssignment: (id: string) => `/api/assignment/${id}/submissions`,
    assignmentReview: (id: string, userId: string) =>
      `/api/assignment/${id}/user/${userId}`,
    addReview: (lessonId: string) => `/api/assignment/${lessonId}/review`,
    editReview: (lessonId: string, id: string) =>
      `/api/assignment/${lessonId}/review/${id}`,
  },
  feedback: {
    add: "/api/feedback",
    list: (lessonId: string, search?: string) =>
      search
        ? `/api/feedback?${search}&LessonIdentity=${lessonId}`
        : `/api/feedback?LessonIdentity=${lessonId}`,
    listOne: (id: string) => `/api/feedback/${id}`,
    submitFeedback: (id: string) => `/api/feedback/${id}/submissions`,
    userFeedback: (lessonId: string, userId: string, search?: string) =>
      search
        ? `/api/feedback?${search}&LessonIdentity=${lessonId}&UserId=${userId}`
        : `/api/feedback?LessonIdentity=${lessonId}&UserId=${userId}`,
    exportFeedback: (lessonId: string) => `/api/Feedback/${lessonId}/export`,
    graph: (id: string) => `/api/Feedback/${id}/chart`,
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
      `/api/course/${courseId}/lesson/${lessonId}/meetingReport/${userId}`,
  },
  watchHistory: {
    create: "/api/watchHistory",
    updateUser: (userId: string) => `/api/watchHistory/pass/${userId}`,
  },
  fileStorage: {
    getFileStorageSetting: "/api/media/setting",
    updateFileStorageSetting: "/api/media/setting",
    getFileStorageSettingValue: (type: string) =>
      `/api/media/settingValue?type=${type}`,
  },
  comments: {
    list: (courseId: string) => `/api/course/${courseId}/comments`,
    details: (courseId: string, commentId: string) =>
      `/api/course/${courseId}/comments/${commentId}`,
    getRepliesList: (
      courseId: string,
      commentId: string,
      replyCount?: number
    ) =>
      `/api/course/${courseId}/comments/${commentId}?page=1&size=${replyCount}`,
    repliesList: (courseId: string, commentId: string) =>
      `/api/course/${courseId}/comments/${commentId}/commentReply`,
    repliesDetails: (courseId: string, commentId: string, replyId: string) =>
      `/api/course/${courseId}/comments/${commentId}/commentReply/${replyId}`,
  },
  enrollment: {
    enrollTrainee: (courseIdentity: string) =>
      `/api/Enrollment/Enrollment?courseIdentity=${courseIdentity}`,
    trainee: (courseIdentity: string, query: string) =>
      `/api/Enrollment/User?CourseIdentity=${courseIdentity}&${query}`,
  },
  externalCertificate: {
    add: "/api/certificate/external",
    user: (id?: string) => `/api/certificate/external/${id}`,
    list: "/api/certificate/review",
    updateStatus: (id?: string) => `/api/certificate/${id}/verify`,
    update: (id?: string) => `/api/certificate/${id}/external`,
    internal: "/api/certificate/internal",
  },
  skill: {
    list: "/api/skills",
    update: (skillId: string) => `/api/skills/${skillId}`,
  },
  assessment: {
    list: "/api/assessment",
    getSingle: (id: string) => `/api/assessment/${id}`,
    update: (id: string) => `/api/assessment/${id}`,
    updateStatus: "/api/assessment/status",
    getResults: (id: string) => `/api/assessmentExam/${id}/getResults`,
    getStudentResult: (assessmentId: string, userId: string) =>
      `/api/assessmentExam/${assessmentId}/getStudentResults/${userId}`,
    getOneAssessmentResult: (
      assessmentId: string,
      assessmentSubmissionId: string
    ) =>
      `/api/assessmentExam/${assessmentId}/getResultDetail/${assessmentSubmissionId}/detail`,
  },
  assessmentQuestion: {
    list: "/api/assessmentQuestion",
    getSingle: (id: string) => `/api/assessmentQuestion/${id}`,
    update: (id: string) => `/api/assessmentQuestion/${id}`,
    getExam: (id: string) => `/api/assessmentQuestion/${id}/examQuestion`,
    submitExam: (assessmentId: string) =>
      `/api/assessmentExam/${assessmentId}/AnswerSubmission`,
  },
  ai: {
    trainingSuggest: "/api/AITrainingGenerator",
    aiMasterSetup: "/api/AIKey",
  },
  iframely: {
    oEmbed: (url: string) => `/api/iframely/oEmbed?url=${url}`,
  },
  update: {
    checkVersions: "/api/admin/settings/CheckUpdates",
  },
  license: {
    validate: (licenseKey: string) =>
      `/api/LemonSqueezy/validate?licenseKey=${licenseKey}`,
    activate: "/api/LemonSqueezy/activate",
    list: "/api/LemonSqueezy/license",
    checkout: "/api/LemonSqueezy/checkout",
  },
  apiKey: {
    list: "/api/ApiKey",
    add: "/api/ApiKey",
    delete: (id: string) => `/api/ApiKey/${id}`,
  },
};
