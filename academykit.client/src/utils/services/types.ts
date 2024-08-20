import { UserRole, UserStatus } from "@utils/enums";
import { ILessons } from "./courseService";
import { ICertificateList } from "./manageCourseService";

export interface ISkill {
  id: string;
  skillName: string;
}

export interface IUser {
  id: string;
  fullName?: string;
  imageUrl: string | null;
  email: string;
  mobileNumber: string;
  role: UserRole;
}
export interface IUserProfile extends IUser {
  memberId?: string;
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
  status: UserStatus;
  certificates: ICertificateList[];
  skills: ISkill[];
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
export interface ILessonRecording extends ILessons {
  videoUrl: string;
  name: string;
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
}

export interface ILessonFeedback extends ILessons {
  name: string;
  description?: string;
}
export interface ILessonExternalUrl extends ILessons {
  name: string;
  description?: string;
  externalUrl: string;
}
export interface ILessonContent extends ILessons {
  name: string;
  description?: string;
  content: string;
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
  description?: string;
}

export interface IPhysicalTraining extends ILessons {
  name: string;
  description: string;
}

export interface IApiKey {
  id: string;
  name: string;
  key: string;
  userId: string;
  createdOn: string;
  createdBy: string;
}
