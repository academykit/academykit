import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { QuestionType } from "@utils/enums";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IUser } from "./types";

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
}

const getAssignmentQuestion = (lessonId: string, search: string) => {
  return httpClient.get<IAssignmentQuestion[]>(
    api.assignment.list(lessonId, search)
  );
};

export const useAssignmentQuestion = (lessonId: string, search: string) => {
  return useQuery(
    [api.assignment.list(lessonId, search)],
    () => getAssignmentQuestion(lessonId, search),
    {
      select: (data) => data.data,
      enabled: lessonId ? true : false,
    }
  );
};

const getSingleAssignment = (assignmentId: string) => {
  return httpClient.get<IAssignmentQuestion>(
    api.assignment.listOne(assignmentId)
  );
};
export const useSingleAssignment = (assignmentId: string) => {
  return useQuery(
    [api.assignment.listOne(assignmentId)],
    () => getSingleAssignment(assignmentId),
    {
      select: (data) => data.data,
      enabled: assignmentId ? true : false,
    }
  );
};

export interface ICreateAssignment {
  lessonId: string;
  name: string;
  description: string;
  hints: string;
  type: string;
  fileUrls?: string[];
  answers?: [
    {
      option: string;
      isCorrect: boolean;
      isSelected: boolean;
    }
  ];
}

const addAssignmentQuestion = ({ data }: { data: ICreateAssignment }) => {
  // @ts-ignore
  data.type = Number(data.type);
  return httpClient.post(api.assignment.add, data);
};

export const useAddAssignmentQuestion = (lessonId: string, search: string) => {
  const queryClient = useQueryClient();

  return useMutation([api.assignment.list], addAssignmentQuestion, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.assignment.list(lessonId, search)]);
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
  // @ts-ignore
  data.type = Number(data.type);
  return httpClient.put(api.assignment.listOne(assignmentId), data);
};

export const useEditAssignmentQuestion = (lessonId: string, search: string) => {
  const queryClient = useQueryClient();

  return useMutation(
    [api.assignment.listOne(lessonId)],
    editAssignmentQuestion,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.assignment.list(lessonId, search)]);
      },
    }
  );
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

  return useMutation(
    [api.assignment.listOne(lessonId)],
    deleteAssignmentQuestion,
    {
      onSuccess: () => {
        queryClient.invalidateQueries([api.assignment.list(lessonId, search)]);
      },
    }
  );
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
  return useMutation(["submitAssignment"], postAssigmentSubmit, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.assignment.list(lessonId, "")]);
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
  return useQuery(
    [api.assignment.assignmentReview(lessonId, userId)],
    () => getAssignmentReview(lessonId, userId),
    {
      select: (data) => data.data,
      enabled: lessonId ? true : false,
    }
  );
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
  return useMutation([api.assignment.addReview(lessonId)], addReview, {
    onSuccess: () =>
      queryClient.invalidateQueries([
        api.assignment.assignmentReview(lessonId, userId),
      ]),
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
  return useMutation([api.assignment.addReview(lessonId)], editReview, {
    onSuccess: () =>
      queryClient.invalidateQueries([
        api.assignment.assignmentReview(lessonId, userId),
      ]),
  });
};
