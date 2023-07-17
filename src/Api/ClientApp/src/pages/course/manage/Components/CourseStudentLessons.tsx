import { LessonType, ReadableEnum } from '@utils/enums';
import { IStudentInfoLesson } from '@utils/services/manageCourseService';
import { useState } from 'react';
import ManageCourseModal from './mangeCourseModal';
import StudentLessonDetails from './StudentDetails';
import LessonStatusColor from './StudentDetails/LessonStatusColor';
import { useTranslation } from 'react-i18next';
import { Text } from '@mantine/core';

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
  const { t } = useTranslation();

  return (
    <tr key={element.lessonId}>
      <td>
        <ManageCourseModal opened={opened} setOpened={setOpened} />
        <Text mah={'200px'}>{element.lessonName}</Text>
      </td>

      <td>
        <LessonStatusColor status={element} />
      </td>
      <td>
        {ReadableEnum[
          LessonType[element.lessonType] as keyof typeof ReadableEnum
        ] ?? t(LessonType[element.lessonType])}
      </td>
      <td>
        <StudentLessonDetails
          studentInfo={element}
          studentId={studentId}
          courseId={courseId}
        />
      </td>
    </tr>
  );
};

export default CourseStudentLessons;
