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
  return useMutation([api.watchHistory.create], watchHistory, {
    onSuccess: () =>
      queryClient.invalidateQueries([
        api.lesson.courseLesson(courseIdentity, lessonIdentity),
      ]),
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
  return useMutation([api.watchHistory.updateUser(userId)], watchHistoryUser, {
    onSuccess: () => {
      queryClient.invalidateQueries([
        api.course.lessonStatDetails(courseId, lessonId, ''),
      ]);
    },
    onError: (err) => {
      return errorType(err);
    },
  });
};
