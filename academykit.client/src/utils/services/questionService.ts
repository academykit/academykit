import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { QuestionType } from "@utils/enums";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { ITag } from "./tagService";
import { IPaginated, IUser } from "./types";

export interface IQuestion {
  id: string;
  name: string;
  type: QuestionType;
  description: string;
  hints: string;
  user: IUser;
  questionPoolQuestionId: string;
  tags: ITag[];
}

const getPoolQuestion = async (poolId: string, query: string) =>
  await httpClient.get<IPaginated<IQuestion>>(
    api.questions.list(poolId) + `?${query}`
  );

export const useQuestion = (poolId: string, query: string) =>
  useQuery({
    queryKey: [api.questions.list(poolId), query],
    queryFn: () => getPoolQuestion(poolId, query),
    select: (data) => data.data,
    enabled: !!poolId && !!query,
  });

export interface IAddQuestionType {
  name: string;
  hints: string;
  type: string;
  tags: string[];
  description: string;
  answers: {
    option: string;
    isCorrect: boolean;
  }[];
  questionPoolId: string;
  id?: string;
  questionPoolQuestionId?: string;
}
const addQuestion = ({
  poolId,
  data,
}: {
  poolId: string;
  data: IAddQuestionType;
}) => {
  return httpClient.post<IAddQuestionType>(api.questions.list(poolId), {
    ...data,
    type: Number(data.type),
  });
};

export const useAddQuestion = (poolId: string, search: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.questions.list(poolId) + `?${search}`],
    mutationFn: addQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.questions.list(poolId) + `?${search}`],
      });
    },
  });
};

const deleteQuestion = ({
  poolId,
  questionId,
}: {
  poolId: string;
  questionId: string;
}) => httpClient.delete(api.questions.delete(poolId, questionId));
export const useDeleteQuestion = (poolId: string, search: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.questions.list(poolId) + `?${search}`],
    mutationFn: deleteQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.questions.list(poolId), search],
      });
    },
  });
};

const addQueSet = (data: {
  identity: string;
  questionPoolQuestionIds: string[];
}) => {
  return httpClient.post(api.questionSet.addQuestion(data.identity), {
    questionPoolQuestionIds: data.questionPoolQuestionIds,
  });
};
export const useAddQuestionQuestionSet = (identity: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["post" + api.questionSet.common],
    mutationFn: addQueSet,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.questionSet.getQuestion(identity)],
      });
    },
  });
};

interface Question {
  id: string;
  questionSetQuestionId: string;
  questionPoolQuestionId: string;
  questionPoolId: string;
  name: string;
  type: number;
  description: string;
  hints: string;
  user: IUser;
  tags: {
    id: string;
    tagId: string;
    tagName: string;
  }[];
  questionOptions: {
    id: string;
    option: string;
    isCorrect: true;
    order: 0;
  }[];
}

const getQuestion = (poolId: string, questionId: string) => {
  return httpClient.get<Question>(api.questions.one(poolId, questionId));
};
export const useGetQuestion = (poolId: string, questionId: string) => {
  return useQuery({
    queryKey: [api.questions.one(poolId, questionId)],
    queryFn: () => getQuestion(poolId, questionId),
    select: (data) => data.data,
  });
};

const editQuestion = ({
  poolId,
  questionId,
  data,
}: {
  poolId: string;
  questionId: string;
  data: IAddQuestionType;
}) => {
  return httpClient.put(api.questions.put(poolId, questionId), {
    ...data,
    type: Number(data.type),
  });
};
export const useEditQuestion = (poolId: string, quesitonId: string) => {
  return useMutation({
    mutationKey: [api.questions.put(poolId, quesitonId)],
    mutationFn: editQuestion,
  });
};

export interface QuestionSetQuestions {
  id: string;
  questionSetQuestionId: string;
  questionPoolQuestionId: string;
  name: string;
  type: number;
  description: string;
  hints: string;
  user: IUser;
  tags: {
    id: string;
    tagId: string;
    tagName: string;
  }[];
  questionOptions: {
    id: string;
    option: string;
    isCorrect: boolean;
    order: number;
  }[];
}

const getQuestionSetQuestions = (identity: string) => {
  return httpClient.get<QuestionSetQuestions[]>(
    api.questionSet.getQuestion(identity)
  );
};
export const useQuestionSetQuestions = (identity: string) => {
  return useQuery({
    queryKey: [api.questionSet.getQuestion(identity)],
    queryFn: () => getQuestionSetQuestions(identity),
    select: (data) => data.data,
  });
};
