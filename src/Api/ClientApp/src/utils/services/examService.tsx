import { useMutation, useQuery } from "@tanstack/react-query";
import { QuestionType } from "@utils/enums";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IUser } from "./types";

export interface ILessonStartQuestionOption {
  id: string;
  option: string;
  isCorrect: boolean;
  order: number;
}
export interface ILessonStartQuestion<T> {
  id: string;
  questionSetQuestionId: string;
  questionPoolQuestionId: string;
  name: string;
  type: QuestionType;
  description: string;
  hints: string;
  isCorrect?: boolean;
  user: IUser;
  questionOptions: T[];
}

export interface ILessonExamStart {
  id: string;
  startDateTime: string;
  duration: number;
  name: string;
  description: string;
  questions: ILessonStartQuestion<ILessonStartQuestionOption>[];
}

const getStartExam = (lessonId: string) =>
  httpClient.post<ILessonExamStart>(api.exam.startExam(lessonId), {});

export const useStartExam = (lessonId: string) =>
  useMutation([api.exam.startExam(lessonId)], () => getStartExam(lessonId), {
    retry: false,
  });

export interface ILessonExamSubmit {
  questionSetQuestionId: string;
  answers: string[];
}

const postExamSubmit = ({
  data,
  lessonId,
  questionSetSubmissionId,
}: {
  data: ILessonExamSubmit[];
  lessonId: string;
  questionSetSubmissionId: string;
}) =>
  httpClient.post(api.exam.submitExam(lessonId, questionSetSubmissionId), data);

export const useSubmitExam = () =>
  useMutation(["submitExam"], postExamSubmit, {});

export interface QuestionSetSubmissionResult {
  attemptCount: 0;
  user: IUser;
  questionSetSubmissions: {
    questionSetSubmissionId: string;
    submissionDate: string;
    totalMarks: string;
    negativeMarks: string;
    obtainedMarks: string;
    duration: string;
    completeDuration: string;
  }[];
}

const getMyResult = (lessonId: string, userId: string) =>
  httpClient.get<QuestionSetSubmissionResult>(
    api.exam.getStudentResults(lessonId, userId),
    {}
  );

export const useMyResult = (lessonId: string, userId: string) =>
  useQuery(
    [api.exam.getStudentResults(lessonId, userId)],
    () => getMyResult(lessonId, userId),
    {
      select: (data) => data.data,
    }
  );

export interface ILessonResultQuestionOption {
  id: string;
  value: string;
  isCorrect: boolean;
  isSelected: boolean;
}
export interface IOneExamResult {
  questionSetSubmissionId: string;
  name: string;
  description: string;
  thumbnailUrl: string;
  submissionDate: string;
  totalMarks: number;
  negativeMarks: string;
  obtainedMarks: number;
  teacher: IUser;
  user: IUser;
  duration: string;
  completeDuration: string;
  results: ILessonStartQuestion<ILessonResultQuestionOption>[];
}

const getOneExamResult = (lessonId: string, questionSetSubmissionId: string) =>
  httpClient.get<IOneExamResult>(
    api.exam.getOneExamResult(lessonId, questionSetSubmissionId)
  );

export const useGetOneExamResult = (
  lessonId: string,
  questionSetSubmissionId: string
) =>
  useQuery(
    [api.exam.getOneExamResult(lessonId, questionSetSubmissionId)],
    () => getOneExamResult(lessonId, questionSetSubmissionId),
    {
      select: (data) => data.data,
    }
  );
