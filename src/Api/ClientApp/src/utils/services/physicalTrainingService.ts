import { useMutation, useQueryClient } from '@tanstack/react-query';
import { ResponseData } from './authService';
import { api } from './service-api';
import { httpClient } from './service-axios';

const postAttendance = async ({ identity }: { identity: string }) =>
  await httpClient.post<ResponseData>(
    api.physicalTraining.attendance(identity),
    {}
  );

export const usePostAttendance = (courseId: string, lessonId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ['attend'],
    mutationFn: postAttendance,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [
          api.lesson.courseLesson(courseId, lessonId),
        ]
      });
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(courseId)]
      });
    }
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

export const useReviewAttendance = (
  courseId: string,
  lessonId: string,
  userId: string
) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ['review'],
    mutationFn: reviewAttendance,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [
          api.course.lessonStatDetails(courseId, lessonId, 'page=1&size=12'),
        ]
      });
      queryClient.invalidateQueries({
        queryKey: [
          api.course.studentStatDetails(courseId, userId),
        ]
      });
    }
  });
};
