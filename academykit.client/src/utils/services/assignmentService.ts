import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { QuestionType } from "@utils/enums";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IPaginated, IUser } from "./types";

interface IAssignmentAttachment {
  assignmentId: string;
  assignmentName: string;
  fileUrl: string;
  order: number;
  name: string;
  mimeType: string;
  user: IUser;
}
export interface IAssignmentOptions {
  id: string;
  assignmentId: string;
  assignmentName: string;
  option: string;
  isCorrect?: true;
  order: number;
  user: IUser;
  isSelected: boolean;
}
export interface IAssignmentQuestion {
  id: string;
  lessonId: string;
  assignmentSubmissionId: string;
  lessonName: string;
  name: string;
  description: string;
  hints?: string;
  order: number;
  isActive: boolean;
  type: QuestionType;
  user: IUser;
  answer?: string;
  assignmentAttachments?: IAssignmentAttachment[];
  assignmentQuestionOptions?: IAssignmentOptions[];
  isTrainee: boolean;
}

const getAssignmentQuestion = (lessonId: string, search: string) => {
  return httpClient.get<IAssignmentQuestion[]>(
    api.assignment.list(lessonId, search)
  );
};

export const useAssignmentQuestion = (lessonId: string, search: string) => {
  return useQuery({
    queryKey: [api.assignment.list(lessonId, search)],
    queryFn: () => getAssignmentQuestion(lessonId, search),
    select: (data) => data.data,
    enabled: lessonId ? true : false,
  });
};

const getSingleAssignment = (assignmentId: string) => {
  return httpClient.get<IAssignmentQuestion>(
    api.assignment.listOne(assignmentId)
  );
};
export const useSingleAssignment = (assignmentId: string) => {
  return useQuery({
    queryKey: [api.assignment.listOne(assignmentId)],
    queryFn: () => getSingleAssignment(assignmentId),
    select: (data) => data.data,
    enabled: assignmentId ? true : false,
  });
};

export interface ICreateAssignment {
  lessonId: string;
  name: string;
  description: string;
  hints: string;
  type: string;
  fileUrls?: string[];
  answers?: {
    option: string;
    isCorrect: boolean;
    isSelected: boolean;
  }[];
}

const addAssignmentQuestion = ({ data }: { data: ICreateAssignment }) => {
  return httpClient.post(api.assignment.add, {
    ...data,
    type: Number(data.type),
  });
};

export const useAddAssignmentQuestion = (lessonId: string, search: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: [api.assignment.list],
    mutationFn: addAssignmentQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assignment.list(lessonId, search)],
      });
    },
  });
};

const editAssignmentQuestion = ({
  data,
  assignmentId,
}: {
  data: ICreateAssignment;
  assignmentId: string;
}) => {
  return httpClient.put(api.assignment.listOne(assignmentId), {
    ...data,
    type: Number(data.type),
  });
};

export const useEditAssignmentQuestion = (lessonId: string, search: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: [api.assignment.listOne(lessonId)],
    mutationFn: editAssignmentQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assignment.list(lessonId, search)],
      });
    },
  });
};

const deleteAssignmentQuestion = ({
  assignmentId,
}: {
  assignmentId: string;
}) => {
  return httpClient.delete(api.assignment.listOne(assignmentId));
};

export const useDeleteAssignmentQuestion = (
  lessonId: string,
  search: string
) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: [api.assignment.listOne(lessonId)],
    mutationFn: deleteAssignmentQuestion,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assignment.list(lessonId, search)],
      });
    },
  });
};

export interface IAssignmentSubmission {
  assignmentId?: string;
  selectedOption?: string[];
  answer?: string;
  attachmentUrls?: string[];
  id?: string;
}

const postAssigmentSubmit = ({
  data,
  lessonId,
}: {
  data: IAssignmentSubmission[];
  lessonId: string;
}) => {
  return httpClient.post(api.assignment.submitAssignment(lessonId), data);
};
export const useSubmitAssignment = ({ lessonId }: { lessonId: string }) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["submitAssignment"],
    mutationFn: postAssigmentSubmit,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.assignment.list(lessonId, "")],
      });
    },
  });
};

export interface IAssignmentReview {
  user: IUser;
  lessonId: string;
  lessonSlug: "string";
  assignmentReview: {
    id: string;
    lessonId: string;
    userId: string;
    mark: string;
    review: string;
    user: IUser;
    teacher: IUser;
    isPassed: boolean;
  };
  assignments: IAssignmentQuestion[];
}

const getAssignmentReview = (lessonId: string, userId: string) => {
  return httpClient.get<IAssignmentReview>(
    api.assignment.assignmentReview(lessonId, userId)
  );
};

export const useAssignmentReview = (lessonId: string, userId: string) => {
  return useQuery({
    queryKey: [api.assignment.assignmentReview(lessonId, userId)],
    queryFn: () => getAssignmentReview(lessonId, userId),
    select: (data) => data.data,
    enabled: lessonId ? true : false,
  });
};

export interface IAddAssignmentReview {
  userId: string;
  marks: number;
  review: string;
  isPassed: boolean;
}

const addReview = ({
  lessonId,
  data,
}: {
  lessonId: string;
  data: IAddAssignmentReview;
}) => {
  return httpClient.post(api.assignment.addReview(lessonId), data);
};

export const useAddReview = (lessonId: string, userId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.assignment.addReview(lessonId)],
    mutationFn: addReview,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.assignment.assignmentReview(lessonId, userId)],
      }),
  });
};

const editReview = ({
  lessonId,
  id,
  data,
}: {
  lessonId: string;
  id: string;
  data: IAddAssignmentReview;
}) => {
  return httpClient.put(api.assignment.editReview(lessonId, id), data);
};

export const useEditReview = (lessonId: string, userId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.assignment.addReview(lessonId)],
    mutationFn: editReview,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.assignment.assignmentReview(lessonId, userId)],
      }),
  });
};

export interface IAssignmentStatus {
  totalAttend: number;
  totalPass: number;
  totalFail: number;
  averageMarks: number;
}

export interface IAssignmentSummary {
  weekStudents: IUser[];
  topStudents: IUser[];
  assignmentStatus: IAssignmentStatus;
  mostWrongAnsQues: string[];
}

const getAssignmentSummary = (courseIdentity: string, lessonId: string) =>
  httpClient.get<IAssignmentSummary>(
    api.course.assignmentSummary(courseIdentity, lessonId)
  );

export const useGetAssignmentSummary = (
  courseIdentity: string,
  lessonId: string
) =>
  useQuery({
    queryKey: [api.course.assignmentSummary(courseIdentity, lessonId)],
    queryFn: () => getAssignmentSummary(courseIdentity, lessonId),
    select: (data) => data.data,
  });

export interface IAssignmentSubmission {
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

const getAssignmentSubmission = async (
  courseIdentity: string,
  lessonId: string,
  search: string
) =>
  await httpClient.get<IPaginated<IAssignmentSubmission>>(
    api.course.assignmentSubmission(courseIdentity, lessonId, search)
  );

export const useGetAssignmentSubmission = (
  courseIdentity: string,
  lessonId: string,
  search: string
) =>
  useQuery({
    queryKey: [
      api.course.assignmentSubmission(courseIdentity, lessonId, search),
    ],
    queryFn: () => getAssignmentSubmission(courseIdentity, lessonId, search),
    select: (data) => data.data,
  });
