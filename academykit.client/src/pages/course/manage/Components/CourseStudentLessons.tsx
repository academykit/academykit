import { Table, Text } from "@mantine/core";
import { LessonType, ReadableEnum } from "@utils/enums";
import { IStudentInfoLesson } from "@utils/services/manageCourseService";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import StudentLessonDetails from "./StudentDetails";
import LessonStatusColor from "./StudentDetails/LessonStatusColor";
import ManageCourseModal from "./mangeCourseModal";

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
    <Table.Tr key={element.lessonId}>
      <Table.Td>
        <ManageCourseModal opened={opened} setOpened={setOpened} />
        <Text mah={"200px"}>{element.lessonName}</Text>
      </Table.Td>

      <Table.Td>
        <LessonStatusColor status={element} />
      </Table.Td>
      <Table.Td>
        {ReadableEnum[
          LessonType[element.lessonType] as keyof typeof ReadableEnum
        ] ?? t(LessonType[element.lessonType])}
      </Table.Td>
      <Table.Td>
        <StudentLessonDetails
          studentInfo={element}
          studentId={studentId}
          courseId={courseId}
        />
      </Table.Td>
    </Table.Tr>
  );
};

export default CourseStudentLessons;
