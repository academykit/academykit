import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { FeedbackType } from "@utils/enums";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IUser } from "./types";

export interface IFeedbackOptions {
  id: string;
  feedbackId: string;
  feedbackName: string;
  option: string;
  order: number;
  user: IUser;
  isSelected: boolean;
}
export interface IFeedbackQuestions {
  id: string;
  lessonId: string;
  feedbackSubmissionId: string;
  lessonName: string;
  name: string;
  description: string;
  hint?: string;
  order: number;
  isActive: boolean;
  type: FeedbackType;
  user: IUser;
  rating: number;
  answer?: string;
  feedbackQuestionOptions?: IFeedbackOptions[];
  isTrainee: boolean;
}

export interface ICreateFeedback {
  lessonId: string;
  name: string;
  type: string;
  answers?: {
    option: string;
    isSelected: boolean;
  }[];
}

const addFeedbackQuestion = ({ data }: { data: ICreateFeedback }) => {
  return httpClient.post(api.feedback.add, {
    ...data,
    type: Number(data.type),
  });
};
export const useAddFeedbackQuestion = (lessonId: string, search: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: [api.feedback.list],
    mutationFn: addFeedbackQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.feedback.list(lessonId, search)],
      });
    },
  });
};

const getFeedbackQuestion = (lessonId: string, search: string) => {
  return httpClient.get<IFeedbackQuestions[]>(
    api.feedback.list(lessonId, search)
  );
};
export const useFeedbackQuestion = (lessonId: string, search: string) => {
  return useQuery({
    queryKey: [api.feedback.list(lessonId, search)],
    queryFn: () => getFeedbackQuestion(lessonId, search),
    select: (data) => data.data,
    enabled: lessonId ? true : false,
  });
};

const getUserFeedback = (lessonId: string, userId: string, search?: string) => {
  return httpClient.get<IFeedbackQuestions[]>(
    api.feedback.userFeedback(lessonId, userId, search)
  );
};
export const useGetUserFeedback = (
  lessonId: string,
  userId: string,
  search?: string
) => {
  return useQuery({
    queryKey: [api.feedback.userFeedback(lessonId, userId, search)],
    queryFn: () => getUserFeedback(lessonId, userId, search),
    select: (data) => data.data,
    enabled: !!lessonId,
  });
};

const deleteFeedbackQuestion = ({ feedbackId }: { feedbackId: string }) => {
  return httpClient.delete(api.feedback.listOne(feedbackId));
};

export const useDeleteFeedbackQuestion = (lessonId: string, search: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: [api.feedback.listOne(lessonId)],
    mutationFn: deleteFeedbackQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.feedback.list(lessonId, search)],
      });
    },
  });
};

const editFeedbackQuestion = ({
  data,
  feedbackId,
}: {
  data: ICreateFeedback;
  feedbackId: string;
}) => {
  return httpClient.put(api.feedback.listOne(feedbackId), {
    ...data,
    type: Number(data.type),
  });
};

export const useEditFeedbackQuestion = (lessonId: string, search: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: [api.feedback.listOne(lessonId)],
    mutationFn: editFeedbackQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.feedback.list(lessonId, search)],
      });
    },
  });
};

export interface IFeedbackSubmission {
  id: string;
  feedbackId: string;
  selectedOption: string[];
  answer: string;
  rating: number;
}

const postFeedbackSubmission = ({
  data,
  lessonId,
}: {
  data: IFeedbackSubmission[];
  lessonId: string;
}) => {
  return httpClient.post(api.feedback.submitFeedback(lessonId), data);
};
export const useFeedbackSubmission = ({ lessonId }: { lessonId: string }) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["submitFeeback"],
    mutationFn: postFeedbackSubmission,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.feedback.list(lessonId, "")],
      });
    },
  });
};

export const exportFeedback = (lessonId: string) =>
  httpClient.get(api.feedback.exportFeedback(lessonId));

export interface IFeedbackChart {
  id: string;
  lessonId: string;
  lessonName: string;
  feedbackId: string;
  feedbackName: string;
  order: number;
  isActive: boolean;
  type: number;
  singleChoiceCount: number;
  multipleChoiceCount: number;
  ratingCount: number;
  answerCount: number;
  user: {
    id: string;
    fullName: string;
    imageUrl: string;
    email: string;
    mobileNumber: string;
    role: number;
    departmentId: string;
    departmentName: string;
  };
  subjectiveAnswer: [
    {
      answer: string;
    },
  ];
  rating: {
    fiveRating: number;
    fourRating: number;
    threeRating: number;
    twoRating: number;
    oneRating: number;
  };
  feedbackQuestionOptions: [
    {
      id: string;
      feedbackId: string;
      feedbackName: string;
      option: string;
      isSelected: boolean;
      order: number;
      selectedCount: number;
    },
  ];
}

const getFeedbackGraph = (lessonId: string) => {
  return httpClient.get<IFeedbackChart[]>(api.feedback.graph(lessonId));
};

export const useGetFeedbackGraph = (lessonId: string) => {
  return useQuery({
    queryKey: [api.feedback.graph(lessonId)],
    queryFn: () => getFeedbackGraph(lessonId),
    select: (data) => data.data,
    retry: 2,
    enabled: false,
  });
};
