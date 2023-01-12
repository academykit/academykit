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

export interface DashboardCourses {
  id: string;
  slug: string;
  name: string;
  percentage: number;
  thumbnailUrl: string;
  user: IUser;
  students: IUser[];
}

export const useDashboard = () => {
  return useQuery(
    [api.course.dashboard],
    () => httpClient.get<DashboardStats>(api.course.dashboard),
    {
      select: (data) => data.data,
    }
  );
};

export const useDashboardCourse = () => {
  return useQuery(
    [api.course.dashboardCourse],
    () =>
      httpClient.get<IPaginated<DashboardCourses>>(api.course.dashboardCourse),
    {
      select: (data) => data.data,
    }
  );
};