namespace AcademyKit.Domain.Enums
{
    public enum UserActivityType
    {
        LogIn = 1,
        LogOut = 2,
        UserCreated = 3,
        UserUpdated = 4,
        UserMarkedInactive = 5,
        ResetPassword = 6,
        ChangePassword = 7,

        CreateCourse = 11,
        UpdateCourse = 12,
        DeleteCourse = 13,
        ArchiveCourse = 14,
        EnrolledCourse = 15,
        UnEnrolledCourse = 16,
        CompletedCourses = 17,
        AddedTeacher = 18,
        RemoveTeacher = 19,

        CreateLiveSession = 21,
        UpdatedLiveSession = 22,
        DeleteLiveSession = 23,
        ArchiveLiveSession = 24,
        EnrolledLiveSession = 25,
        UnEnrolledLiveSession = 26,
        JoinedLiveSession = 27,
        LeftLiveSession = 28,
        AddedModerator = 29,
        RemoveModerator = 30,

        CreateExam = 31,
        UpdateExam = 32,
        DeleteExam = 33,
        ArchiveExam = 34,
        MarkCompletedExam = 35,
        EnrolledExam = 36,
        UnEnrolledExam = 37,
        AttemptedExam = 38,

        SettingsUpdated = 41,
    }
}
