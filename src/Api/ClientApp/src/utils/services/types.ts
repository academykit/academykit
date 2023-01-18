import { UserRole } from "@utils/enums";
import { ILessons } from "./courseService";
import { ICertificateList } from "./manageCourseService";

export interface IUser {
  id: string;
  fullName?: string;
  imageUrl: string | null;
  email: string;
  mobileNumber: string;
  role: UserRole;
}
export interface IUserProfile extends IUser {
  firstName: string;
  middleName: string;
  lastName: string;
  profession: string;
  address: string;
  bio: string;
  publicUrls: string;
  isActive: boolean;
  createdOn?: string;
  fullName: string;
  departmentId: string;
  role: UserRole;
  certificates: ICertificateList[];
}

export interface IAddUser {
  firstName?: string;
  middleName?: string;
  lastName?: string;
  email?: string;
  mobileNumber?: string;
  role?: UserRole;
  profession?: string;
  address?: string;
  bio?: string;
  publicUrls?: string;
  isActive?: true;
}

export interface IPaginated<T> {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPage: number;
  items: T[];
}

export interface IPasswordResetResponse {
  message: string;
  success: boolean;
}

export interface ILessonLecture extends ILessons {
  videoUrl: string;
  name: string;
  description: string;
}
export interface ILessonMCQ extends ILessons {
  questionSet: {
    name: string;
    thumbnailUrl: string;
    description: string;
    negativeMarking: 0;
    questionMarking: 0;
    passingWeightage: 0;
    allowedRetake: 0;
    duration: 0;
    startTime: string;
    endTime: string;
  };
}

export interface ILessonAssignment extends ILessons {
  name: string;
  description: string;
  startDate : string;
  endDate: string;
}

export interface ILessonFeedback extends ILessons {
  name: string;
  description: string;
}
export interface ILessonFile extends ILessons {
  name: string;
  description: string;
  documentUrl: string;
}

export interface ILessonMeeting extends ILessons {
  meeting: {
    meetingStartDate: string;
    meetingDuration: number;
    zoomLicenseId: string;
  };
}
