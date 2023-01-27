import { httpClient } from "./service-axios";
import { api } from "./service-api";
import { useQuery } from "@tanstack/react-query";

export interface IStartExam {
  slug: string;
  roomName: string;
  jwtToken: string;
  zakToken: string;
  sdkKey: string;
  meetingId: string;
  passcode: string;
}

const joinMeeting = (courseId: string, lessonId: string) =>
  httpClient.get<IStartExam>(api.meeting.joinMeeting(courseId, lessonId));

export const useJoinMeeting = (courseId: string, lessonId: string) =>
  useQuery(
    [api.meeting.joinMeeting(courseId, lessonId)],
    () => joinMeeting(courseId, lessonId),
    {
      select: (data) => data.data,
      retry: false,
      retryOnMount: false,
      refetchOnMount: false,
    }
  );

export interface IReportDetail {
  userId: string;
  name: string;
  email: string;
  mobileNumber: string;
  startDate: string;
  joinedTime: string;
  leftTime: string;
  duration: string;
  lessonId: string;
}

const getMeetingReport = (courseId: string, lessonId: string, userId: string) =>
  httpClient.get<IReportDetail>(
    api.meeting.meetingReport(courseId, lessonId, userId)
  );

export const useGetMeetingReport = (
  courseId: string,
  lessonId: string,
  userId: string,
  enabled: boolean
) =>
  useQuery(
    [api.meeting.meetingReport(courseId, lessonId, userId)],
    () => getMeetingReport(courseId, lessonId, userId),
    {
      select: (data) => data.data,
      retry: false,
      enabled,
    }
  );
