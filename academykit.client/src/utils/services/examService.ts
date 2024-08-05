import { useMutation, useQuery } from '@tanstack/react-query';
import { CourseStatus, CourseUserStatus, QuestionType } from '@utils/enums';
import { IQuestion } from './questionService';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IUser } from './types';

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
  role: CourseUserStatus;
}

const getStartExam = (lessonId: string) =>
  httpClient.post<ILessonExamStart>(api.exam.startExam(lessonId), {});

export const useStartExam = (lessonId: string) =>
  useMutation({
    mutationKey: [api.exam.startExam(lessonId)],
    mutationFn: () => getStartExam(lessonId),
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
  useMutation({
    mutationKey: ['submitExam'],
    mutationFn: postExamSubmit,
  });

export interface QuestionSetSubmissionResult {
  attemptCount: 0;
  hasExceededAttempt: boolean;
  endDate: string;
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
  useQuery({
    queryKey: [api.exam.getStudentResults(lessonId, userId)],
    queryFn: () => getMyResult(lessonId, userId),
    select: (data) => data.data,
  });

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
  useQuery({
    queryKey: [api.exam.getOneExamResult(lessonId, questionSetSubmissionId)],
    queryFn: () => getOneExamResult(lessonId, questionSetSubmissionId),
    select: (data) => data.data,
  });

export interface IExamStatus {
  totalAttend: number;
  passStudents: number;
  failStudents: number;
  averageMarks: number;
}

export interface IExamSummary {
  lessonId: string;
  lessonName: string;
  lessonStatus: number;
  courseStatus: CourseStatus;
  examStatus: IExamStatus;
  mostWrongAnsQues: IQuestion[];
  weekStudents: IUser[];
  topStudents: IUser[];
  totalMarks: { marks: number }[];
}

const getExamSummary = (courseIdentity: string, lessonId: string) =>
  httpClient.get<IExamSummary>(
    api.course.examSummary(courseIdentity, lessonId)
  );

export const useGetExamSummary = (courseIdentity: string, lessonId: string) =>
  useQuery({
    queryKey: [api.course.examSummary(courseIdentity, lessonId)],
    queryFn: () => getExamSummary(courseIdentity, lessonId),
    select: (data) => data.data,
  });

export interface IExamSubmission {
  student: {
    id: string;
    fullName: string;
    imageUrl: string | null;
    email: string;
    mobileNumber: string | null;
    role: number;
    departmentId: string | null;
    departmentName: string | null;
  };
  totalMarks: number;
  submissionDate: Date;
}

const getExamSubmission = (courseIdentity: string, lessonId: string) =>
  httpClient.get<IExamSubmission[]>(
    api.course.examSubmission(courseIdentity, lessonId)
  );

export const useGetExamSubmission = (
  courseIdentity: string,
  lessonId: string
) =>
  useQuery({
    queryKey: [api.course.examSubmission(courseIdentity, lessonId)],
    queryFn: () => getExamSubmission(courseIdentity, lessonId),
    select: (data) => data.data,
  });
