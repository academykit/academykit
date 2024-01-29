//manage lessons stat

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { LessonType } from '@utils/enums';
import errorType from './axiosError';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IPaginated, IUser } from './types';

export interface ILessonStats {
  id: string;
  slug: string;
  name: string;
  lessonType: number;
  courseId: string;
  courseSlug: string;
  courseName: string;
  sectionId: string;
  sectionSlug: string;
  sectionName: string;
  enrolledStudent: number;
  lessonWatched: number;
  isMandatory: boolean;
}

const getLessonStatistics = async (courseIdentity: string, query: string) => {
  return await httpClient.get<IPaginated<ILessonStats>>(
    api.course.lessonStat(courseIdentity) + `?${query}`
  );
};
export const useGetLessonStatistics = (
  courseIdentity: string,
  query: string
) => {
  return useQuery(
    [api.course.lessonStat(courseIdentity), query],
    () => getLessonStatistics(courseIdentity, query),
    {
      select: (data) => {
        return data.data;
      },
    }
  );
};

// manage one lesson stat
export interface LessonStatDetails {
  lessonId: string;
  lessonSlug: string;
  lessonName: string;
  lessonType: number;
  isCompleted: boolean;
  isPassed: boolean;
  user: IUser;
  questionSetId: string;
  isAssignmentReviewed: boolean | null;
  attendanceReviewed: boolean;
}

const getLessonStatisticsDetails = async (
  courseIdentity: string,
  lessonId: string,
  qs: string
) => {
  return await httpClient.get<IPaginated<LessonStatDetails>>(
    api.course.lessonStatDetails(courseIdentity, lessonId, qs)
  );
};
export const useGetLessonStatisticsDetails = (
  courseIdentity: string,
  lessonId: string,
  qs: string
) => {
  return useQuery(
    [api.course.lessonStatDetails(courseIdentity, lessonId, qs)],
    () => getLessonStatisticsDetails(courseIdentity, lessonId, qs),
    {
      select: (data) => {
        return data.data;
      },
    }
  );
};

//manage student stat
export interface IStudentStat {
  userId: string;
  fullName: string;
  percentage: number;
  lessonId: string;
  lessonSlug: string;
  lessonName: string;
  certificateUrl: string;
  certificateIssuedDate: string;
  hasCertificateIssued: boolean;
  imageUrl: string;
}

const getStudentStatistics = async (courseIdentity: string, query: string) => {
  return await httpClient.get<IPaginated<IStudentStat>>(
    api.course.studentStat(courseIdentity) + `?${query}`
  );
};
export const useGetStudentStatistics = (
  courseIdentity: string,
  query: string
) => {
  return useQuery(
    [api.course.studentStat(courseIdentity), query],
    () => getStudentStatistics(courseIdentity, query),
    {
      select: (data) => {
        return data.data;
      },
    }
  );
};

// student Statics for one user

export interface IStudentInfoLesson {
  lessonId: string;
  lessonSlug: string;
  lessonName: string;
  lessonType: LessonType;
  isCompleted: boolean;
  isPassed: boolean;
  user: IUser;
  questionSetId: string;
  isAssignmentReviewed: boolean | null;
  attendanceReviewed: boolean;
}

const getStudentStatisticsDetails = async (
  courseIdentity: string,
  userId: string
) => {
  return await httpClient.get<IStudentInfoLesson[]>(
    api.course.studentStatDetails(courseIdentity, userId)
  );
};

export const useGetStudentStatisticsDetails = (
  courseIdentity: string,
  userId: string
) => {
  return useQuery(
    [api.course.studentStatDetails(courseIdentity, userId)],
    () => getStudentStatisticsDetails(courseIdentity, userId),
    {
      select: (data) => {
        return data.data;
      },
    }
  );
};

//course certificate
export interface ICertificateList {
  courseId: string;
  courseSlug: string;
  courseName: string;
  user: IUser;
  percentage: number;
  hasCertificateIssued: boolean;
  certificateUrl: string;
  certificateIssuedDate: string;
}
export interface ICertificate {
  items: ICertificateList[];
}
const getStudentStatisticsCertificate = async (
  courseIdentity: string,
  search: string
) => {
  return await httpClient.get<IPaginated<ICertificate>>(
    api.course.certificate(courseIdentity, search)
  );
};

export const useGetStudentStatisticsCertificate = (
  courseIdentity: string,
  search: string
) => {
  return useQuery(
    [api.course.certificate(courseIdentity, search)],
    () => getStudentStatisticsCertificate(courseIdentity, search),
    {
      select: (data) => {
        return data.data;
      },
    }
  );
};

//stat course certificate post

const postStudentStatisticsCertificate = async ({
  identity,
  data,
  issueAll,
}: {
  identity: string;
  data: any;
  issueAll: boolean;
}) =>
  await httpClient.post(api.course.postCertificate(identity), {
    userIds: data,
    issueAll,
  });

export const usePostStatisticsCertificate = (
  identity: string,
  search: string
) => {
  const queryClient = useQueryClient();
  return useMutation(
    [api.course.postCertificate(identity)],
    postStudentStatisticsCertificate,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([
          api.course.certificate(identity, search),
        ]);
        queryClient.invalidateQueries([
          api.course.studentStat(identity),
          search,
        ]);
      },
      onError: (err) => {
        return errorType(err);
      },
    }
  );
};

//manage stat
interface ICourseStat {
  totalLessons: number;
  totalEnrollments: number;
  totalTeachers: number;
  totalAssignments: number;
  totalLectures: number;
  totalExams: number;
  totalMeetings: number;
  totalDocuments: number;
}

const getCourseManageStatistics = async (courseIdentity: string) => {
  return await httpClient.get<ICourseStat>(
    api.course.getManageStat(courseIdentity)
  );
};
export const useGetCourseManageStatistics = (courseIdentity: string) => {
  return useQuery(
    [api.course.getManageStat(courseIdentity)],
    () => getCourseManageStatistics(courseIdentity),
    {
      select: (data) => {
        return data.data;
      },
      retry: false,
    }
  );
};
