namespace Lingtren.Domain.Enums
{
    public enum LessonType
    {
        Video = 1,
        Document = 2,
        Exam = 3,
        Assignment = 4,
        LiveClass = 5,

        /// <summary>
        /// After completion of live class its recorded video is uploaded into lesson
        /// and status is changed to RecordedVideo from LiveClass
        /// </summary>
        RecordedVideo = 6
    }
}