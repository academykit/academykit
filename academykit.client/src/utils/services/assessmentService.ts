import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  AssessmentStatus,
  AssessmentType,
  SkillAssessmentRule,
  UserRole,
} from "@utils/enums";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IPaginated, IUser } from "./types";

interface IAssessmentOption {
  option: string;
  isCorrect: boolean;
}

export interface IAddAssessment {
  questionName: string;
  description: string;
  hints: string;
  type: string;
  assessmentQuestionOptions: IAssessmentOption[];
}

export interface IAssessmentQuestion {
  id: string;
  assessmentId: string;
  assessmentName: string;
  questionName: string;
  order: number;
  isActive: boolean;
  description: string;
  hints: string;
  type: number;
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
  assessmentQuestionOptions: [
    {
      id: string;
      option: string;
      order: number;
      isCorrect: boolean;
    },
  ];
}

interface IEligibilityCreation {
  role: UserRole | 0;
  skillId: string;
  departmentId: string;
  groupId: string;
  assessmentId: string;
  trainingId: string;
}
export interface IResponseEligibilityCreation extends IEligibilityCreation {
  id: string;
  skillName: string | null;
  assessmentName: string | null;
  departmentName: string | null;
  groupName: string | null;
  trainingName: string | null;
  isEligible: boolean;
}
interface ISkillCriteria {
  skillAssessmentRule: SkillAssessmentRule;
  percentage: number;
  skillId: string;
}

interface IResponseSkillCriteria extends ISkillCriteria {
  id: string;
  skillTypeName: string;
}

interface IBaseAssessment {
  id: string;
  slug: string;
  title: string;
  description: string;
  retakes: number;
  startDate: Date | null;
  endDate: Date | null;
  duration: number;
  weightage: number;
  user: IUser;
  assessmentStatus: AssessmentStatus;
}

export interface IAssessment extends IBaseAssessment {
  skillsCriteriaRequestModels: ISkillCriteria[];
  eligibilityCreationRequestModels: IEligibilityCreation[];
}

export interface IAssessmentResponse extends IBaseAssessment {
  skillsCriteriaRequestModels: IResponseSkillCriteria[];
  eligibilityCreationRequestModels: IResponseEligibilityCreation[];
  isEligible: boolean;
}

type IPostAssessment = Omit<
  IAssessment,
  "id" | "user" | "slug" | "assessmentStatus"
>;

interface ISingleAssessmentDescription extends IAssessmentResponse {
  noOfQuestion: number;
  hasCompleted: boolean;
  remainingAttempt: number;
  isEligible: boolean;
}

export interface IAssessmentResult {
  id: string;
  assessmentId: string;
  obtainedMarks: number;
  user: IUser;
}

interface IStudentAssessmentResult {
  attemptCount: number;
  user: IUser;
  assessmentSetResultDetails: [
    {
      questionSetSubmissionId: string;
      submissionDate: Date;
      totalMarks: string;
      negativeMarks: string;
      obtainedMarks: string;
      duration: string;
      completeDuration: string;
    },
  ];
  hasExceededAttempt: true;
  endDate: Date;
}

export interface IAssessmentExamSubmit {
  assessmentQuestionId: string;
  answers: string[];
}

export interface IResults {
  id: string;
  name: string;
  description: string;
  hints: string;
  attachments: string;
  type: 1;
  questionOptions: [
    {
      id: string;
      value: string;
      isCorrect: true;
      isSelected: true;
    },
  ];
  isCorrect: true;
  orderNumber: 0;
}

export interface IAssessmentOneResult {
  questionSetSubmissionId: string;
  name: string;
  description: string;
  submissionDate: string;
  totalMarks: number;
  negativeMarks: number;
  obtainedMarks: number;
  teacher: IUser;
  user: IUser;
  duration: string;
  completeDuration: string;
  results: IResults[];
}

const getAssessments = async (search: string) =>
  await httpClient.get<IPaginated<IAssessmentResponse>>(
    api.assessment.list + `?${search}`
  );

export const useAssessments = (search: string) => {
  return useQuery({
    queryKey: [api.assessment.list, search],
    queryFn: () => getAssessments(search),
    select: (data) => data.data,
  });
};

const getSingleAssessment = async (slug: string) =>
  await httpClient.get<ISingleAssessmentDescription>(
    api.assessment.getSingle(slug)
  );

export const useGetSingleAssessment = (slug: string) => {
  return useQuery({
    queryKey: [api.assessment.getSingle(slug), slug],
    queryFn: () => getSingleAssessment(slug),
    select: (data) => data.data,
  });
};

export const usePostAssessment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["post" + api.assessment.list],

    mutationFn: (data: IPostAssessment) => {
      return httpClient.post<IAssessment>(api.assessment.list, data);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assessment.list],
      });
    },
  });
};

const updateAssessmentDetails = async ({
  id,
  data,
}: {
  id: string;
  data: IPostAssessment;
}) => await httpClient.put<IAssessment>(api.assessment.update(id), data);

export const useUpdateAssessment = (slug: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.assessment.getSingle(slug)],
    mutationFn: updateAssessmentDetails,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assessment.getSingle(slug)],
      });
    },
  });
};

const deleteAssessment = async (id: string) =>
  httpClient.delete(api.assessment.update(id));

export const useDeleteAssessment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["delete" + api.assessment.list],
    mutationFn: deleteAssessment,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assessment.list],
      });
    },
  });
};

// ---------------------------------- Assessment Questions ----------------------------------------------

const getAssessmentQuestion = async (search: string, id: string) =>
  httpClient.get<IPaginated<IAssessmentQuestion>>(
    api.assessmentQuestion.list + `?${search}&identity=${id}`
  );

export const useAssessmentQuestion = (search: string, id: string) => {
  return useQuery({
    queryKey: [api.assessmentQuestion.list, search, id],
    queryFn: () => getAssessmentQuestion(search, id),
    select: (data) => data.data,
  });
};

interface IPostAssessmentQuestion {
  questionName: string;
  description: string;
  hints: string;
  type: AssessmentType;
  assessmentQuestionOptions: IAssessmentOption[];
}

export const usePostAssessmentQuestion = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["post" + api.assessmentQuestion.list],

    mutationFn: ({
      id,
      data,
    }: {
      id: string;
      data: IPostAssessmentQuestion;
    }) => {
      return httpClient.post(api.assessmentQuestion.update(id), data);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assessmentQuestion.list],
      });
    },
  });
};

const deleteAssessmentQuestion = async (id: string) =>
  httpClient.delete(api.assessmentQuestion.update(id));

export const useDeleteAssessmentQuestion = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["delete" + api.assessmentQuestion.list],
    mutationFn: deleteAssessmentQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assessmentQuestion.list],
      });
    },
  });
};

const updateAssessmentQuestion = async ({
  id,
  data,
}: {
  id: string;
  data: IPostAssessmentQuestion;
}) => httpClient.put(api.assessmentQuestion.update(id), data);

export const useUpdateAssessmentQuestion = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.assessmentQuestion.list],
    mutationFn: updateAssessmentQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assessmentQuestion.list],
      });
    },
  });
};

// ---------------------------------- Assessment Exam ----------------------------------------------

export interface IAssessmentExam {
  questionId: string;
  questionName: string;
  order: number;
  description: string;
  hints: string;
  type: number;
  assessmentQuestionOptions: [
    {
      optionId: string;
      option: string;
      order: number;
      isCorrect: boolean | null;
    },
  ];
}

export interface IAssessmentExamDetail {
  assessmentId: string;
  startDateTime: Date;
  endDateTime: Date;
  duration: number;
  assessmentName: string;
  description: string;
  questions: IAssessmentExam[];
}

const getAssessmentExamQuestions = async (id: string) =>
  await httpClient.get<IAssessmentExamDetail>(
    api.assessmentQuestion.getExam(id)
  );

export const useAssessmentExamQuestions = (id: string) => {
  return useQuery({
    queryKey: [api.assessmentQuestion.getExam(id)],
    queryFn: () => getAssessmentExamQuestions(id),
    select: (data) => data.data,
    retry: false,
    enabled: false,
  });
};

interface IAssessmentStatusUpdate {
  identity: string;
  status: AssessmentStatus;
  message?: string;
}

interface IStatusUpdateResponse {
  message: string;
}

const updateAssessmentStatus = async (data: IAssessmentStatusUpdate) =>
  httpClient.patch<IStatusUpdateResponse>(api.assessment.updateStatus, data);

export const useUpdateAssessmentStatus = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.assessment.updateStatus],
    mutationFn: updateAssessmentStatus,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assessment.getSingle(id)],
      });
    },
  });
};

const postAssessmentExam = async ({
  assessmentId,
  data,
}: {
  assessmentId: string;
  data: IAssessmentExamSubmit[];
}) =>
  await httpClient.post(api.assessmentQuestion.submitExam(assessmentId), data);

export const useSubmitAssessmentExam = () =>
  useMutation({
    mutationKey: ["submitAssessmentExam"],
    mutationFn: postAssessmentExam,
  });

// ---------------------------------- Assessment Stats ----------------------------------------------

const getAllResults = async (id: string) =>
  await httpClient.get<IPaginated<IAssessmentResult>>(
    api.assessment.getResults(id)
  );

export const useGetAllResults = (id: string) => {
  return useQuery({
    queryKey: [api.assessment.getResults(id)],
    queryFn: () => getAllResults(id),
    select: (data) => data.data,
  });
};

const getStudentResult = async (assessmentId: string, studentId: string) =>
  await httpClient.get<IStudentAssessmentResult>(
    api.assessment.getStudentResult(assessmentId, studentId)
  );

export const useGetStudentResult = (
  assessmentId: string,
  studentId: string
) => {
  return useQuery({
    queryKey: [api.assessment.getStudentResult(assessmentId, studentId)],
    queryFn: () => getStudentResult(assessmentId, studentId),
    select: (data) => data.data,
  });
};

const getOneExamResult = (
  assessmentId: string,
  assessmentSubmissionId: string
) =>
  httpClient.get<IAssessmentOneResult>(
    api.assessment.getOneAssessmentResult(assessmentId, assessmentSubmissionId)
  );

export const useGetOneExamResult = (
  assessmentId: string,
  assessmentSubmissionId: string
) =>
  useQuery({
    queryKey: [
      api.assessment.getOneAssessmentResult(
        assessmentId,
        assessmentSubmissionId
      ),
    ],

    queryFn: () => getOneExamResult(assessmentId, assessmentSubmissionId),
    select: (data) => data.data,
  });
