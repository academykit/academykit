export enum UserRole {
  SuperAdmin = 1,
  Admin = 2,
  Trainer = 3,
  Trainee = 4,
}

export enum UserStatus {
  Active = 1,
  InActive = 2,
  Pending = 3,
}

export enum PoolRole {
  Creator = 1,
  Trainer = 2,
}

export enum CourseLanguage {
  English = 1,
  Nepali = 2,
}

export enum QuestionType {
  SingleChoice = 1,
  MultipleChoice = 2,
  Subjective = 3,
}

export enum FeedbackType {
  SingleChoice = 1,
  MultipleChoice = 2,
  Subjective = 3,
  Rating = 4,
}

export enum ReadableEnum {
  SingleChoice = "Single Choice",
  MultipleChoice = "Multiple Choice",
  LiveClass = "Live Class",
  RecordedVideo = "Recordings",
}

export enum LessonType {
  Video = 1,
  Document = 2,
  Exam = 3,
  Assignment = 4,
  LiveClass = 5,
  RecordedVideo = 6,
  Feedback = 7,
}

export enum CourseStatus {
  Draft = 1,
  Review = 2,
  Published = 3,
  Archived = 4,
  Rejected = 5,
  Completed = 6,
}

export enum CourseUserStatus {
  Author = 1,
  Enrolled = 2,
  NotEnrolled = 3,
  Teacher = 4,
}
export const CourseUserStatusValue = {
  [CourseUserStatus.Author]: "Author",
  [CourseUserStatus.Enrolled]: "Enrolled",
  [CourseUserStatus.NotEnrolled]: "Not Enrolled",
  [CourseUserStatus.Teacher]: "Trainer",
};

export enum EFileStorageType {
  server = 1,
  aws = 2,
}

export enum EFIleUploadType {
  image = 1,
  video = 2,
}

export enum LessonFileType {
  File = 1,
  Video = 2,
}

export enum LanguageString {
  ne = "ne-NP",
  en = "en-US",
  ja = "ja-JP",
}

export enum SeverityType {
  Info = 1,
  Error = 2,
  Warning = 3,
  Debug = 4,
}
