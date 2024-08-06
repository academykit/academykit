import { useQuery } from "@tanstack/react-query";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IPaginated, IUser } from "./types";

export interface DashboardStats {
  totalUsers: number;
  totalActiveUsers: number;
  totalGroups: number;
  totalTrainers: number;
  totalTrainings: number;
  totalActiveTrainings: number;
  totalCompletedTrainings: number;
  totalEnrolledCourses: number;
  totalInProgressCourses: number;
  totalCompletedCourses: number;
  totalLessons: number;
  totalEnrollments: number;
  totalTeachers: number;
  totalAssignments: number;
  totalLectures: number;
  totalExams: number;
  totalMeetings: number;
  totalDocuments: number;
}

export interface DashboardStatsData {
  key: string;
  label: string;
  icon: string;
  signLabel: string;
  pluLabel: string;
  color: string;
}

export interface DashboardCourses {
  id: string;
  slug: string;
  name: string;
  percentage: number;
  thumbnailUrl: string;
  user: IUser;
  students: IUser[];
}

export interface UpcomingEvents {
  lessonSlug: string;
  lessonType: number;
  lessonName: string;
  startDate: string;
  courseEnrollmentBool: boolean;
  courseSlug: string;
  isLive: true;
  courseName: string;
}

export const useDashboard = () => {
  return useQuery({
    queryKey: [api.course.dashboard],
    queryFn: () => httpClient.get<DashboardStats>(api.course.dashboard),
    select: (data) => data.data,
  });
};

export const useDashboardCourse = () => {
  return useQuery({
    queryKey: [api.course.dashboardCourse],

    queryFn: () =>
      httpClient.get<IPaginated<DashboardCourses>>(api.course.dashboardCourse),

    select: (data) => data.data,
  });
};

export const useUpcomingDashboardDetail = () => {
  return useQuery({
    queryKey: [api.course.dashboardUpcoming],
    queryFn: () =>
      httpClient.get<UpcomingEvents[]>(api.course.dashboardUpcoming),
    select: (data) => data.data,
  });
};
