import { useMutation, useQueryClient } from '@tanstack/react-query';
import errorType from './axiosError';
import { api } from './service-api';
import { httpClient } from './service-axios';

const watchHistory = ({
  courseId,
  lessonId,
}: {
  courseId: string;
  lessonId: string;
}) =>
  httpClient.post(api.watchHistory.create, {
    courseIdentity: courseId,
    lessonIdentity: lessonId,
    watchedPercentage: 100,
  });

export const useWatchHistory = (
  courseIdentity: string,
  lessonIdentity: string | undefined
) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.watchHistory.create],
    mutationFn: watchHistory,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [
          api.lesson.courseLesson(courseIdentity, lessonIdentity),
        ]
      });
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(courseIdentity)]
      });
    }
  });
};

const watchHistoryUser = ({
  courseId,
  lessonId,
  userId,
}: {
  courseId: string;
  lessonId: string;
  userId: string;
}) =>
  httpClient.patch(api.watchHistory.updateUser(userId), {
    courseIdentity: courseId,
    lessonIdentity: lessonId,
    watchedPercentage: 100,
  });

export const useWatchHistoryUser = (
  userId: string,
  courseId: string,
  lessonId: string
) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.watchHistory.updateUser(userId)],
    mutationFn: watchHistoryUser,

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
    },

    onError: (err) => {
      return errorType(err);
    }
  });
};
