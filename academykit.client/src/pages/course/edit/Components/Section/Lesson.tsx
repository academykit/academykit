import AddAssignment from "@components/Course/Lesson/AddAssignment";
import AddDocument from "@components/Course/Lesson/AddDocument";
import AddExam from "@components/Course/Lesson/AddExam";
import AddFeedback from "@components/Course/Lesson/AddFeedback";
import AddLecture from "@components/Course/Lesson/AddLecture";
import AddMeeting from "@components/Course/Lesson/AddMeeting";
import AddPhysical from "@components/Course/Lesson/AddPhysical";
import DeleteModal from "@components/Ui/DeleteModal";
import { Button, Grid, Group, Text } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconTrashX } from "@tabler/icons-react";
import { LessonType, ReadableEnum } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import { ILessons, useDeleteLesson } from "@utils/services/courseService";
import {
  ILessonAssignment,
  ILessonFeedback,
  ILessonFile,
  ILessonMCQ,
  ILessonMeeting,
  IPhysicalTraining,
} from "@utils/services/types";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import classes from "../styles/lesson.module.css";

const Lesson = ({
  lesson,
  sectionId,
}: {
  lesson: ILessons;
  sectionId: string;
}) => {
  const { id: slug } = useParams();
  const { t } = useTranslation();

  const [isEditing, setIsEditing] = useState<boolean>(false);
  const [value, toggle] = useToggle();
  const deleteLesson = useDeleteLesson(slug as string);
  const onDeleteLesson = async () => {
    try {
      await deleteLesson.mutateAsync({
        id: lesson.courseId,
        lessonId: lesson.id,
      });
      showNotification({
        message: t("delete_lesson_success"),
        title: t("success"),
      });
      toggle();
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        color: "red",
        title: t("error"),
      });
      toggle();
    }
  };
  return (
    <div>
      <DeleteModal
        title={t("sure_want_to_delete")}
        open={value}
        onClose={toggle}
        onConfirm={onDeleteLesson}
      />
      <div className={classes.item}>
        <Grid grow justify={"center"}>
          <Grid.Col span={4}>
            <div
              style={{ display: "flex", alignItems: "center", padding: "10px" }}
            >
              {lesson?.name}
            </div>
          </Grid.Col>
          <Grid.Col span={4}>
            <Group justify="center">
              <Text m={"auto"} style={{ padding: "10px" }}>
                {ReadableEnum[
                  t(`${LessonType[lesson.type]}`) as keyof typeof ReadableEnum
                ] ?? t(`${LessonType[lesson.type]}`)}
              </Text>
            </Group>
          </Grid.Col>
          <Grid.Col span={4}>
            <Group justify="flex-end">
              <IconTrashX
                size={18}
                style={{
                  color: "red",
                  marginLeft: "10px",
                  cursor: "pointer",
                }}
                onClick={() => {
                  toggle();
                }}
              />
              <Button
                variant="outline"
                onClick={() => setIsEditing(!isEditing)}
                style={{ marginRight: "10px", marginTop: "5px" }}
              >
                {isEditing ? t("cancel") : t("edit")}
              </Button>
            </Group>
          </Grid.Col>
        </Grid>
      </div>

      {isEditing &&
        LessonEditCase({
          item: lesson,
          lessonType: lesson?.type,
          setAddLessonClick: () => {},
          setAddState: () => {},
          sectionId: sectionId,
          setIsEditing,
        })}
    </div>
  );
};

const LessonEditCase = ({
  lessonType,
  item,
  setAddLessonClick,
  setAddState,
  sectionId,
  setIsEditing,
}: {
  lessonType: number;
  item: ILessons;
  setAddLessonClick: React.Dispatch<React.SetStateAction<boolean>>;
  setAddState: React.Dispatch<React.SetStateAction<string>>;
  sectionId: string;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  switch (lessonType) {
    case LessonType.Video:
      return (
        <AddLecture
          sectionId={sectionId}
          item={item}
          isEditing={true}
          setAddLessonClick={setAddLessonClick}
          setAddState={setAddState}
          setIsEditing={setIsEditing}
        />
      );
    case LessonType.Exam:
      return (
        <AddExam
          sectionId={sectionId}
          item={item as ILessonMCQ}
          setAddState={() => setAddState("")}
          isEditing={true}
          setIsEditing={setIsEditing}
        />
      );
    case LessonType.Assignment:
      return (
        <AddAssignment
          sectionId={sectionId}
          item={item as ILessonAssignment}
          setAddState={() => setAddState("")}
          isEditing={true}
          setIsEditing={setIsEditing}
        />
      );
    case LessonType.LiveClass:
      return (
        <AddMeeting
          sectionId={sectionId}
          item={item as ILessonMeeting}
          setAddState={setAddState}
          isEditing={true}
          setAddLessonClick={setAddLessonClick}
          setIsEditing={setIsEditing}
        />
      );
    case LessonType.Feedback:
      return (
        <AddFeedback
          sectionId={sectionId}
          item={item as ILessonFeedback}
          setAddState={setAddState}
          isEditing={true}
          setIsEditing={setIsEditing}
        />
      );
    case LessonType.Document:
      return (
        <AddDocument
          sectionId={sectionId}
          setAddLessonClick={setAddLessonClick}
          setAddState={setAddState}
          isEditing={true}
          item={item as ILessonFile}
          setIsEditing={setIsEditing}
        />
      );
    case LessonType.RecordedVideo:
      return (
        <AddLecture
          sectionId={sectionId}
          item={item}
          isEditing={true}
          setAddLessonClick={setAddLessonClick}
          setAddState={setAddState}
          setIsEditing={setIsEditing}
        />
      );
    case LessonType.Physical:
      return (
        <AddPhysical
          sectionId={sectionId}
          item={item as IPhysicalTraining}
          setAddState={setAddState}
          isEditing={true}
          setAddLessonClick={setAddLessonClick}
          setIsEditing={setIsEditing}
        />
      );
    default:
      break;
  }
};

export default Lesson;
