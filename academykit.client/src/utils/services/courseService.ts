import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  CourseLanguage,
  CourseStatus,
  CourseUserStatus,
  LessonType,
} from "@utils/enums";
import { ResponseData } from "./authService";
import errorType from "./axiosError";
import { INotMember } from "./groupService";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import {
  ILessonAssignment,
  ILessonFeedback,
  ILessonLecture,
  ILessonMCQ,
  ILessonMeeting,
  IPaginated,
  IUser,
} from "./types";

interface ICourseTag {
  id: string;
  tagId: string;
  tagName: string;
}
export interface ICourse {
  createdOn: string;
  id: string;
  slug: string;
  name: string;
  thumbnailUrl: string;
  description: string;
  groupId: string;
  groupName: string;
  status: CourseStatus;
  language: CourseLanguage;
  duration: number;
  levelId: string;
  user: IUser;
  levelName: string;
  tags?: ICourseTag[];
  userStatus: CourseUserStatus;
}

const getCourse = async (search: string) =>
  await httpClient.get<IPaginated<ICourse>>(api.course.list + `?${search}`);

export const useCourse = (search: string) =>
  useQuery({
    queryKey: [api.course.list, search],
    queryFn: () => getCourse(search),
    select: (data) => data.data,
    enabled: !!search,
  });

export interface IMyCourse extends ICourse {
  percentage: number;
}

const getMyCourse = async (userId: string, search: string) =>
  await httpClient.get<IPaginated<IMyCourse>>(
    api.course.userList(userId) + `?${search}`
  );

export const useMyCourse = (userId: string, search: string) =>
  useQuery({
    queryKey: [api.course.userList(userId), search],
    queryFn: () => getMyCourse(userId, search),
    select: (data) => data.data,
  });

const getCourseTeacher = async (course_id: string, searchParams: string) =>
  await httpClient.get<IPaginated<ICreateCourseTeacher>>(
    api.courseTeacher.list + `?CourseIdentity=${course_id}&${searchParams}`
  );

export const useCourseTeacher = (course_id: string, searchParams: string) =>
  useQuery({
    queryKey: ["get_course_teachers" + api.courseTeacher.list + searchParams],
    queryFn: () => getCourseTeacher(course_id, searchParams),
    enabled: true,
    select: (data) => data.data,
  });

//start
export interface ICreateCourseTeacher {
  courseIdentity: string;
  email: string;
  id: string;
  user?: IUser;
  courseName: string;
  courseCreatedBy?: string;
}
const createTeacherCourse = async (data: {
  courseIdentity: string;
  email: string;
}) => {
  await httpClient.post<ICreateCourseTeacher>(api.courseTeacher.list, {
    email: data.email,
    courseIdentity: data.courseIdentity,
  });
};
export const useCreateTeacherCourse = (searchParams: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["post" + api.courseTeacher.list],
    mutationFn: createTeacherCourse,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [
          "get_course_teachers" + api.courseTeacher.list + searchParams,
        ],
      });
    },
  });
};
//end

const deleteCourseTeacher = async (id: string) => {
  return await httpClient.delete(api.courseTeacher.detail(id));
};
export const useDeleteCourseTeacher = (searchParams: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["delete" + api.courseTeacher.detail],
    mutationFn: deleteCourseTeacher,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [
          "get_course_teachers" + api.courseTeacher.list + searchParams,
        ],
      });
    },
  });
};

export interface IBaseTrainingEligibility {
  eligibility: number;
  eligibilityId: string;
}

interface ICreateCourse {
  name: string;
  thumbnailUrl: string;
  description: string;
  groupId: string;
  language?: number;
  duration?: number;
  levelId: string;
  tagIds: string[];
  startDate: string;
  endDate: string;
  isUnlimitedEndDate: boolean;
  trainingEligibilities: IBaseTrainingEligibility[];
}

const createCourse = async (course: ICreateCourse) =>
  await httpClient.post<ICourse>(api.course.list, course);

export const useCreateCourse = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.list],
    mutationFn: createCourse,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course],
      });
    },

    onError: (err) => {
      return errorType(err);
    },
  });
};

export interface IFullCourse extends ICourse {
  sections: ISection[];
  startDate: string;
  endDate: string;
  isUnlimitedEndDate: boolean;
  trainingEligibilities: IBaseTrainingEligibility[];
  isEligible: boolean;
}
const getCourseDescription = async (id: string) =>
  await httpClient.get<IFullCourse>(api.course.detail(id));

export const useCourseDescription = (id: string) =>
  useQuery({
    queryKey: [api.course.detail(id)],
    queryFn: () => getCourseDescription(id),
    select: (data) => data.data,
    retry: 2,
  });

const lessonReorder = async ({
  id,
  data,
}: {
  id: string;
  data: { sectionIdentity: string; ids: string[] | undefined };
}) => await httpClient.put(api.course.reorder(id), data);

export const useLessonReorder = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [],
    mutationFn: lessonReorder,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(id)],
      }),
  });
};

const sectionReorder = async ({ id, data }: { id: string; data: string[] }) =>
  await httpClient.put(api.course.reorderSection(id), data);

export const useSectionReorder = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [],
    mutationFn: sectionReorder,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(id)],
      }),
  });
};

const assignmentReorder = async ({
  id,
  data,
  lessonIdentity,
  lessonType,
}: {
  id: string;
  lessonIdentity: string;
  lessonType: number;
  data: string[];
}) =>
  await httpClient.post(
    api.course.reorder(id) +
    `?lessonIdentity=${lessonIdentity}&lessonType=${lessonType}`,
    data
  );

export const useQuestionReorder = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [],
    mutationFn: assignmentReorder,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(id)],
      });
      queryClient.invalidateQueries({
        queryKey: [api.questionSet.getQuestion(id)],
      });
    },
  });
};

export const useUpdateCourse = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.course.list],

    mutationFn: (data: any) => {
      return httpClient.put(api.course.update(id), data);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(id)],
      });
    },
  });
};

export const useUpdateGeneralSetting = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["update" + api.course.list],

    mutationFn: (data: any) => {
      return httpClient.put(api.course.update(id), data);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.adminUser.getGeneralSettings],
      });
    },
  });
};

const deleteCourse = async (id: string) => {
  return await httpClient.delete(api.course.detail(id));
};
export const useDeleteCourse = (search: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["delete" + api.course.detail],
    mutationFn: deleteCourse,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.list, search],
      });
    },
  });
};

export interface ISection {
  id: string;
  slug: string;
  description: string;
  order: number;
  courseId: string;
  duration: number;
  name: string;
  user: IUser;
  lessons?: ILessons[];
}

export interface ILessons {
  id: string;
  slug: string;
  name: string;
  description?: string;
  videoUrl: string;
  thumbnailUrl: string;
  documentUrl: string;
  order: number;
  duration: number;
  isMandatory: true;
  type: LessonType;
  isDeleted: true;
  status: 1;
  sectionIdentity: string;
  lessonIdentity: string;
  courseId: string;
  courseName: string;
  sectionId: string;
  sectionName: string;
  meetingId: string;
  meetingName: string;
  user: IUser;
  startDate: string;
  endDate: string;
  negativeMarking: number;
  questionMarking: number;
  passingWeightage: number;
  allowedRetake: number;
  isCompleted: boolean;
  isLoading: boolean;
}

const createSection = async (data: { courseIdentity: string; name: string }) =>
  await httpClient.post<ISection>(api.section.section(data.courseIdentity), {
    name: data.name,
  });

export const useCreateSection = (slug: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["post" + api.section.common],
    mutationFn: createSection,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(slug)],
      });
    },
  });
};

const getSection = async (courseIdentity: string) => {
  return await httpClient.get<IPaginated<ISection>>(
    api.section.section(courseIdentity)
  );
};
export const useGetSection = (courseIdentity: string) => {
  return useQuery({
    queryKey: ["get" + api.section.common],
    queryFn: () => getSection(courseIdentity),

    select: (data) => {
      return data.data;
    },
  });
};

const updateSectionName = async (data: {
  id: string;
  sectionId: string;
  sectionName: string;
}) => {
  return await httpClient.patch(
    api.section.updateSection(data.id, data.sectionId),
    { name: data.sectionName }
  );
};
export const useUpdateSectionName = (slug: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["patch" + api.section.common],
    mutationFn: updateSectionName,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(slug)],
      });
    },
  });
};

const deleteSection = async (data: { id: string; sectionId: string }) => {
  return await httpClient.delete(
    api.section.updateSection(data.id, data.sectionId)
  );
};
export const useDeleteSection = (slug: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["delete" + api.section.common],
    mutationFn: deleteSection,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(slug)],
      });
    },
  });
};

const createLesson = async (
  data:
    | ILessonLecture
    | ILessonMCQ
    | ILessonAssignment
    | ILessonMeeting
    | ILessonFeedback
) => await httpClient.post<ILessons>(api.lesson.addLesson(data.courseId), data);

export const useCreateLesson = (slug: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["post" + api.lesson.common],
    mutationFn: createLesson,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(slug)],
      });
    },
  });
};

const updateLesson = async (
  data:
    | ILessonLecture
    | ILessonMCQ
    | ILessonAssignment
    | ILessonMeeting
    | ILessonFeedback
) => {
  return await httpClient.put(
    api.lesson.updateLesson(data.courseId, data.lessonIdentity),
    data
  );
};

export const useUpdateLesson = (
  courseIdentity: string,
  courseId?: string,
  lessonId?: string
) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["update" + api.lesson.common],
    mutationFn: updateLesson,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.lesson.courseLesson(courseIdentity)],
      });
      queryClient.invalidateQueries({
        queryKey: [api.lesson.courseLesson(courseId ?? "", lessonId)],
      });
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(courseIdentity)],
      });
    },
  });
};

const deleteLesson = async (data: { id: string; lessonId: string }) => {
  return await httpClient.delete(
    api.lesson.deleteLesson(data.id, data.lessonId)
  );
};
export const useDeleteLesson = (slug: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["delete" + api.lesson.common],
    mutationFn: deleteLesson,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(slug)],
      });
    },
  });
};

// courses status
const courseStatus = async (data: {
  identity: string;
  status: CourseStatus;
  message?: string;
}) => {
  return await httpClient.patch<ResponseData>(api.course.status, data);
};
export const useCourseStatus = (id: string, search: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.enroll(id)],
    mutationFn: courseStatus,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(id)],
      });
      queryClient.invalidateQueries({
        queryKey: [api.course.list, search],
      });
    },
  });
};

//courses update to draft
const courseUpdateStatus = async (data: { id: string }) => {
  return await httpClient.patch(api.course.updateCourse(data.id));
};
export const useCourseUpdateStatus = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.updateCourse(id)],
    mutationFn: courseUpdateStatus,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(id)],
      });
    },
  });
};
// courses student
const enrollCourse = async (data: { id: string }) => {
  return await httpClient.post(api.course.enroll(data.id), {});
};
export const useEnrollCourse = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.enroll(id)],
    mutationFn: enrollCourse,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.detail(id)],
      });
    },
  });
};

// course Lesson
export interface ICourseMcq {
  allowedRetake: number;
  description: string;
  duration: number;
  endTime: string;
  id: string;
  name: string;
  negativeMarking: number;
  passingWeightage: number;
  questionMarking: number;
  slug: string;
  startTime?: string;
  thumbnailUrl: string;
  updatedOn?: string;
  totalMarks: number;
  totalQuestions: number;
}

export interface ICourseMeeting {
  id: string;
  meetingNumber: 0;
  passCode: string;
  zoomLicenseId: string;
  duration: string;
  startDate: string;
  user: IUser;
}

export interface ICourseLessonAssignmentReview {
  id: string;
  lessonId: string;
  userId: string;
  mark: number;
  review: string;
  user: IUser;
  teacher: IUser;
}
export interface ICourseLesson {
  id: string;
  slug: string;
  name: string;
  description: string;
  videoUrl: string;
  thumbnailUrl: string;
  documentUrl: string;
  order: number;
  duration: number;
  isMandatory: boolean;
  type: LessonType;
  isDeleted: boolean;
  status: CourseStatus;
  courseId: string;
  courseName: string;
  sectionId: string;
  sectionName: string;
  meetingId: string;
  questionSetId: string;
  user: IUser;
  isCompleted: boolean;
  questionSet: ICourseMcq;
  meeting: ICourseMeeting;
  lessonWatched: number;
  enrolledStudent: number;
  nextLessonSlug: string;
  hasResult: boolean;
  hasFeedbackSubmitted: boolean;
  remainingAttempt: number;
  hasReviewedAssignment: boolean;
  assignmentReview?: ICourseLessonAssignmentReview;
  assignmentExpired: boolean;
  startDate: string;
  zoomId: string;
  password: string;
  hasAttended: boolean;
  isTrainee: boolean;
  externalUrl: string;
  content: string;
}

const getCourseLesson = async (
  courseIdentity: string,
  lessonIdentity?: string
) => {
  return await httpClient.get<ICourseLesson>(
    api.lesson.courseLesson(courseIdentity, lessonIdentity)
  );
};
export const useGetCourseLesson = (
  courseIdentity: string,
  lessonIdentity?: string,
  enabled?: boolean
) => {
  return useQuery({
    queryKey: [api.lesson.courseLesson(courseIdentity, lessonIdentity)],
    queryFn: () => getCourseLesson(courseIdentity, lessonIdentity),

    select: (data) => {
      return data.data;
    },

    enabled,
    retry: 0,

    // to reflect the changes made after submission of various assignments
    refetchOnMount: true,

    refetchOnWindowFocus: false,
  });
};

export interface ISignature {
  courseIdentity: string;
  signatures: {
    fileURL: string;
    designation: string;
    fullName: string;
  }[];
}

export interface IGetSignature {
  id: string;
  courseId: string;
  designation: string;
  fullName: string;
  fileUrl: string;
  createdOn: string;
  updatedOn: string;
}

const addSignature = ({ data, id }: { data: IGetSignature; id: string }) => {
  return httpClient.post(api.course.createSignature(id), data);
};
export const useAddSignature = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.createSignature],
    mutationFn: addSignature,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.course.getSignature(courseId)],
      }),
  });
};

const getSignatures = (identity: string) => {
  return httpClient.get<IGetSignature[]>(api.course.getSignature(identity));
};
export const useGetSignature = (identity: string) => {
  return useQuery({
    queryKey: [api.course.getSignature(identity)],
    queryFn: () => getSignatures(identity),
    select: (data) => data.data,
  });
};

const deleteSignature = ({ id, sigId }: { sigId: string; id: string }) => {
  return httpClient.delete(api.course.deleteSignature(id, sigId));
};
export const useDeleteSignature = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.createSignature],
    mutationFn: deleteSignature,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.course.getSignature(courseId)],
      }),
  });
};

const editSignature = ({ data, id }: { data: IGetSignature; id: string }) => {
  return httpClient.put(api.course.editSignature(id, data.id), data);
};
export const useEditSignature = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.editSignature],
    mutationFn: editSignature,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.course.getSignature(courseId)],
      }),
  });
};

export interface IAddCertificate {
  title: string;
  eventStartDate: string;
  eventEndDate: string;
}

export interface IGetCertificateDetails {
  id: string;
  courseId: string;
  title: string;
  eventStartDate: string;
  eventEndDate: string;
  sampleUrl: string;
}

const addCertificate = ({
  data,
  id,
}: {
  data: IAddCertificate;
  id: string;
}) => {
  return httpClient.post(api.course.addCertificateDetails(id), data);
};
export const useAddCertificate = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.addCertificateDetails(courseId)],
    mutationFn: addCertificate,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.course.getCertificateDetails(courseId)],
      });
    },
  });
};

const getSingleCertificate = (id: string) => {
  return httpClient.get<IGetCertificateDetails>(
    api.course.getCertificateDetails(id)
  );
};
export const useGetCertificateDetails = (id: string) => {
  return useQuery({
    queryKey: [api.course.getCertificateDetails(id)],

    queryFn: () => getSingleCertificate(id),
  });
};

const addTrainee = ({ courseId, data }: { data: any; courseId: string }) => {
  return httpClient.post(api.enrollment.enrollTrainee(courseId), data);
};

export const useAddTrainee = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.enrollment.enrollTrainee(courseId)],
    mutationFn: addTrainee,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [],
      });
    },
  });
};

const getTrainee = (courseId: string, query: string) => {
  return httpClient.get<IPaginated<INotMember>>(
    api.enrollment.trainee(courseId, query)
  );
};

export const useGetTrainee = (courseId: string, query: string) => {
  return useQuery({
    queryKey: [api.enrollment.trainee(courseId, query)],
    queryFn: () => getTrainee(courseId, query),
    select: (data) => data.data,
  });
};

export interface IUpdateShuffle {
  noOfQuestion: number;
  isShuffle: boolean;
  showAll: boolean;
}

const getShuffleDetails = (trainingSlug: string, lessonSlug: string) => {
  return httpClient.get<IUpdateShuffle>(
    api.course.getShuffle(trainingSlug, lessonSlug)
  );
};

export const useGetShuffleDetails = (
  trainingSlug: string,
  lessonSlug: string
) => {
  return useQuery({
    queryKey: [api.course.getShuffle(trainingSlug, lessonSlug)],
    queryFn: () => getShuffleDetails(trainingSlug, lessonSlug),
    select: (data) => data.data,
  });
};

const updateShuffleDetails = ({
  trainingSlug,
  lessonSlug,
  data,
}: {
  trainingSlug: string;
  lessonSlug: string;
  data: IUpdateShuffle;
}) => {
  return httpClient.put(api.course.shuffle(trainingSlug, lessonSlug), data);
};

export const useUpdateShuffle = (trainingSlug: string, lessonSlug: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: [api.course.getShuffle(trainingSlug, lessonSlug)],
    mutationFn: updateShuffleDetails,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: [api.course.getShuffle(trainingSlug, lessonSlug)],
      }),
  });
};
