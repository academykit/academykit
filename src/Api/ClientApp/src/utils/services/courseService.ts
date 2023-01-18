import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  CourseLanguage,
  CourseStatus,
  CourseUserStatus,
  LessonType,
} from "@utils/enums";
import axios from "axios";
import errorType from "./axiosError";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import {
  ILessonAssignment,
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
  useQuery([api.course.list, search], () => getCourse(search), {
    select: (data) => data.data,
  });

const getCourseTeacher = async (course_id: string) =>
  await httpClient.get<IPaginated<ICreateCourseTeacher>>(
    api.courseTeacher.list + `?CourseIdentity=${course_id}`
  );

export const useCourseTeacher = (course_id: string) =>
  useQuery(
    ["get" + api.courseTeacher.list],
    () => getCourseTeacher(course_id),
    {
      select: (data) => data.data,
    }
  );

//start
export interface ICreateCourseTeacher {
  courseIdentity: string;
  email: string;
  id: string;
  user?: IUser;
  courseName: string;
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
export const useCreateTeacherCourse = () => {
  const queryClient = useQueryClient();

  return useMutation(["post" + api.courseTeacher.list], createTeacherCourse, {
    onSuccess: () => {
      queryClient.invalidateQueries(["get" + api.courseTeacher.list]);
    },
  });
};
//end

const deleteCourseTeacher = async (id: string) => {
  return await httpClient.delete(api.courseTeacher.detail(id));
};
export const useDeleteCourseTeacher = () => {
  const queryClient = useQueryClient();
  return useMutation(
    ["delete" + api.courseTeacher.detail],
    deleteCourseTeacher,
    {
      onSuccess: () => {
        queryClient.invalidateQueries(["get" + api.courseTeacher.list]);
      },
    }
  );
};

interface ICreateCourse {
  name: string;
  thumbnailUrl: string;
  description: string;
  groupId: string;
  language?: number;
  duration?: number;
  levelId: string;
  tagIds: string[];
}

const createCourse = async (course: ICreateCourse) =>
  await httpClient.post<ICourse>(api.course.list, course);

export const useCreateCourse = () => {
  const queryClient = useQueryClient();
  return useMutation([api.course.list], createCourse, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course]);
    },
    onError: (err) => {
      return errorType(err);
    },
  });
};

export interface IFullCourse extends ICourse {
  sections: ISection[];
}
const getCourseDescription = async (id: string) =>
  await httpClient.get<IFullCourse>(api.course.detail(id));

export const useCourseDescription = (id: string) =>
  useQuery([api.course.detail(id)], () => getCourseDescription(id), {
    select: (data) => data.data,
    retry: 2,
  });


const lessonReorder = async ({id,data}: { id:string,data:{sectionIdentity:string,ids:string[]| undefined }}) =>
  await httpClient.put(api.course.reorder(id),data);


export const useLessonReorder = (id: string) => {
  const queryClient = useQueryClient()    
  return useMutation([], lessonReorder, {
    onSuccess: ()=>queryClient.invalidateQueries([api.course.detail(id)])
  })
}
  

const sectionReorder = async ({id,data}: { id:string,data:string[]}) =>
  await httpClient.put(api.course.reorderSection(id),data);


export const useSectionReorder = (id: string) => {
  const queryClient = useQueryClient()    
  return useMutation([], sectionReorder, {
    onSuccess: ()=>queryClient.invalidateQueries([api.course.detail(id)])
  })
  }


export const useUpdateCourse = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation(
    ["update" + api.course.list],
    (data: any) => {
      return httpClient.put(api.course.update(id), data);
    },
    {
      onSuccess: (data) => {
        queryClient.invalidateQueries([api.course.detail(id)]);
      },
    }
  );
};

export const useUpdateGeneralSetting = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation(
    ["update" + api.course.list],
    (data: any) => {
      return httpClient.put(api.course.update(id), data);
    },
    {
      onSuccess: (data) => {
        queryClient.invalidateQueries([api.adminUser.getGeneralSettings]);
      },
    }
  );
};

const deleteCourse = async (id: string) => {
  return await httpClient.delete(api.course.detail(id));
};
export const useDeleteCourse = (search: string) => {
  const queryClient = useQueryClient();
  return useMutation(["delete" + api.course.detail], deleteCourse, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.list, search]);
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
  description: string;
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
}

const createSection = async (data: { courseIdentity: string; name: string }) =>
  await httpClient.post<ISection>(api.section.section(data.courseIdentity), {
    name: data.name,
  });

export const useCreateSection = (slug: string) => {
  const queryClient = useQueryClient();

  return useMutation(["post" + api.section.common], createSection, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(slug)]);
    },
  });
};

const getSection = async (courseIdentity: string) => {
  return await httpClient.get<IPaginated<ISection>>(
    api.section.section(courseIdentity)
  );
};
export const useGetSection = (courseIdentity: string) => {
  return useQuery(
    ["get" + api.section.common],
    () => getSection(courseIdentity),
    {
      select: (data) => {
        return data.data;
      },
    }
  );
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
  return useMutation(["patch" + api.section.common], updateSectionName, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(slug)]);
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
  return useMutation(["delete" + api.section.common], deleteSection, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(slug)]);
    },
  });
};

const createLesson = async (
  data: ILessonLecture | ILessonMCQ | ILessonAssignment | ILessonMeeting
) => {
  return await httpClient.post(api.lesson.addLesson(data.courseId), data);
};
export const useCreateLesson = (slug: string) => {
  const queryClient = useQueryClient();

  return useMutation(["post" + api.lesson.common], createLesson, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(slug)]);
    },
  });
};

const updateLesson = async (
  data: ILessonLecture | ILessonMCQ | ILessonAssignment | ILessonMeeting
) => {
  return await httpClient.put(
    api.lesson.updateLesson(data.courseId, data.lessonIdentity),
    data
  );
};

export const useUpdateLesson = (courseIdentity: string) => {
  const queryClient = useQueryClient();

  return useMutation(["update" + api.lesson.common], updateLesson, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.lesson.courseLesson(courseIdentity)]);
      queryClient.invalidateQueries([api.course.detail(courseIdentity)]);
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
  return useMutation(["delete" + api.lesson.common], deleteLesson, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(slug)]);
    },
  });
};

// courses status
const courseStatus = async (data: { id: string; status: CourseStatus }) => {
  return await httpClient.patch(api.course.status(data.id, data.status));
};
export const useCourseStatus = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation([api.course.enroll(id)], courseStatus, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(id)]);
    },
  });
};

//courses update to draft
const courseUpdateStatus = async (data: { id: string }) => {
  return await httpClient.patch(api.course.updateCourse(data.id));
};
export const useCourseUpdateStatus = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation([api.course.updateCourse(id)], courseUpdateStatus, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(id)]);
    },
  });
};
// courses student
const enrollCourse = async (data: { id: string }) => {
  return await httpClient.post(api.course.enroll(data.id), {});
};
export const useEnrollCourse = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation([api.course.enroll(id)], enrollCourse, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.course.detail(id)]);
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
  assignmentExpired: boolean
  startDate: string
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
  return useQuery(
    [api.lesson.courseLesson(courseIdentity, lessonIdentity)],
    () => getCourseLesson(courseIdentity, lessonIdentity),
    {
      select: (data) => {
        return data.data;
      },
      enabled,
      retry: 0,
      onError: (err) => {
        
      },
      refetchOnMount: false,
      refetchOnWindowFocus: false,
    }
  );
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
  return useMutation([api.course.createSignature], addSignature, {
    onSuccess: () =>
      queryClient.invalidateQueries([api.course.getSignature(courseId)]),
  });
};

const getSignatures = (identity: string) => {
  return httpClient.get<IGetSignature[]>(api.course.getSignature(identity));
};
export const useGetSignature = (identity: string) => {
  return useQuery(
    [api.course.getSignature(identity)],
    () => getSignatures(identity),
    {
      select: (data) => data.data,
    }
  );
};

const deleteSignature = ({ id, sigId }: { sigId: string; id: string }) => {
  return httpClient.delete(api.course.deleteSignature(id, sigId));
};
export const useDeleteSignature = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation([api.course.createSignature], deleteSignature, {
    onSuccess: () =>
      queryClient.invalidateQueries([api.course.getSignature(courseId)]),
  });
};

const editSignature = ({ data, id }: { data: IGetSignature; id: string }) => {
  return httpClient.put(api.course.editSignature(id, data.id), data);
};
export const useEditSignature = (courseId: string) => {
  const queryClient = useQueryClient();
  return useMutation([api.course.editSignature], editSignature, {
    onSuccess: () =>
      queryClient.invalidateQueries([api.course.getSignature(courseId)]),
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
  return useMutation(
    [api.course.addCertificateDetails(courseId)],
    addCertificate,
    {}
  );
};

const getSingleCertificate = (id: string) => {
  return httpClient.get<IGetCertificateDetails>(
    api.course.getCertificateDetails(id)
  );
};
export const useGetCertificateDetails = (id: string) => {
  return useQuery([api.course.getCertificateDetails(id)], () =>
    getSingleCertificate(id)
  );
};
