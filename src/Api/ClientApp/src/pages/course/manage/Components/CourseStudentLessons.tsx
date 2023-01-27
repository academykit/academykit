import { LessonType, ReadableEnum } from "@utils/enums";
import { IStudentInfoLesson } from "@utils/services/manageCourseService";
import { useState } from "react";
import ManageCourseModal from "./mangeCourseModal";
import StudentLessonDetails from "./StudentDetails";
import LessonStatusColor from "./StudentDetails/LessonStatusColor";

const CourseStudentLessons = ({
  element,
  studentId,
  courseId,
}: {
  element: IStudentInfoLesson;
  studentId: string;
  courseId: string;
}) => {
  const [opened, setOpened] = useState(false);

  return (
    <tr key={element.lessonId}>
      <td>
        <ManageCourseModal opened={opened} setOpened={setOpened} />
        {element.lessonName}
      </td>

      <td>
        <LessonStatusColor
          isCompleted={element.isCompleted}
          isPassed={element.isPassed}
          type={element.lessonType}
        />
      </td>
      <td>
        {ReadableEnum[
          LessonType[element.lessonType] as keyof typeof ReadableEnum
        ] ?? LessonType[element.lessonType]}
      </td>
      <td>
        <StudentLessonDetails
          questionSetId={element.questionSetId}
          type={element.lessonType}
          studentId={studentId}
          lessonId={element.lessonId}
          isCompleted={element.isPassed}
          courseId={courseId}
          lessonName={element.lessonName}
        />
      </td>
    </tr>
  );
};

export default CourseStudentLessons;
