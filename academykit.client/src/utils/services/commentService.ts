import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IPaginated, IUser } from './types';

export interface IComment {
  id: string;
  courseId: string;
  content: string;
  repliesCount: 0;
  user: IUser;
  createdOn: string;
}

const getComments = (courseId: string) => {
  return httpClient.get<IPaginated<IComment>>(api.comments.list(courseId));
};

export const useGetComments = (courseId: string) => {
  return useQuery({
    queryKey: [api.comments.list(courseId)],
    queryFn: () => getComments(courseId),
    select: (data) => data.data,
  });
};

const postComment = ({
  courseId,
  content,
}: {
  courseId: string;
  content: string;
}) => httpClient.post(api.comments.list(courseId), { content });

export const usePostComment = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.comments.list(courseId)],
    mutationFn: postComment,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.comments.list(courseId)],
      }),
  });
};

const deleteComment = ({
  courseId,
  commentId,
}: {
  courseId: string;
  commentId: string;
}) => httpClient.delete(api.comments.details(courseId, commentId));

export const useDeleteComment = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.comments.list(courseId)],
    mutationFn: deleteComment,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.comments.list(courseId)],
      }),
  });
};

const editComment = ({
  courseId,
  commentId,
  content,
}: {
  courseId: string;
  commentId: string;
  content: string;
}) => httpClient.put(api.comments.details(courseId, commentId), { content });

export const useEditComment = (courseId: string, commentId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.comments.details(courseId, commentId)],
    mutationFn: editComment,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.comments.list(courseId)],
      }),
  });
};

// replies

export interface ICommentReply {
  id: string;
  commentId: string;
  courseId: string;
  content: string;
  user: IUser;
  createdOn: string;
}

const getCommentReplies = (
  courseId: string,
  commentId: string,
  replyCount: number
) => {
  return httpClient.get<IPaginated<ICommentReply>>(
    api.comments.getRepliesList(courseId, commentId, replyCount)
  );
};

export const useGetCommentReplies = (
  courseId: string,
  commentId: string,
  replyCount: number
) => {
  return useQuery({
    queryKey: [api.comments.getRepliesList(courseId, commentId, replyCount)],
    queryFn: () => getCommentReplies(courseId, commentId, replyCount),
    select: (data) => data.data,
  });
};

const postCommentReply = ({
  courseId,
  commentId,
  content,
}: {
  courseId: string;
  content: string;
  commentId: string;
}) =>
  httpClient.post(api.comments.repliesList(courseId, commentId), {
    content,
  });

export const usePostCommentReply = (courseId: string, commentId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.comments.getRepliesList(courseId, commentId)],
    mutationFn: postCommentReply,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.comments.getRepliesList(courseId, commentId)],
      });
      queryClient.invalidateQueries({
        queryKey: [api.comments.list(courseId)],
      });
    },
  });
};

const deleteCommentReply = ({
  courseId,
  commentId,
  replyId,
}: {
  courseId: string;
  commentId: string;
  replyId: string;
}) =>
  httpClient.delete(api.comments.repliesDetails(courseId, commentId, replyId));

export const useDeleteCommentReply = (courseId: string, commentId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.comments.repliesDetails],
    mutationFn: deleteCommentReply,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.comments.getRepliesList(courseId, commentId)],
      });
      queryClient.invalidateQueries({
        queryKey: [api.comments.list(courseId)],
      });
    },
  });
};

const editCommentReply = ({
  courseId,
  commentId,
  replyId,
  content,
}: {
  courseId: string;
  commentId: string;
  replyId: string;
  content: string;
}) =>
  httpClient.put(api.comments.repliesDetails(courseId, commentId, replyId), {
    content,
  });

export const useEditCommentReply = (courseId: string, commentId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.comments.repliesDetails],
    mutationFn: editCommentReply,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.comments.getRepliesList(courseId, commentId)],
      });
    },
  });
};
