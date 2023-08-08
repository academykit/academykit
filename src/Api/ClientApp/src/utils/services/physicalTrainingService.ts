/* eslint-disable @typescript-eslint/no-unused-vars */
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { ResponseData } from './authService';

const postAttendance = async ({ identity }: { identity: string }) =>
  await httpClient.post<ResponseData>(
    api.physicalTraining.attendance(identity),
    {}
  );

export const usePostAttendance = (slug: string) => {
  const queryClient = useQueryClient();

  return useMutation(['attend'], postAttendance, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(slug)]);
    },
  });
};

export interface IPhysicalReview {
  identity: string;
  isPassed: boolean;
  message: string;
  userId: string;
}

const reviewAttendance = async ({ data }: { data: IPhysicalReview }) => {
  await httpClient.post(api.physicalTraining.review, data);
};

export const useReviewAttendance = (courseId: string, lessonId: string) => {
  const queryClient = useQueryClient();

  return useMutation(['review'], reviewAttendance, {
    onSuccess: () => {
      queryClient.invalidateQueries([
        api.course.lessonStatDetails(courseId, lessonId, 'page=1&size=12'),
      ]);
    },
  });
};
