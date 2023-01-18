import AddAssignment from "@components/Course/Lesson/AddAssignment";
import AddDocument from "@components/Course/Lesson/AddDocument";
import AddExam from "@components/Course/Lesson/AddExam";
import AddFeedback from "@components/Course/Lesson/AddFeedback";
import AddLecture from "@components/Course/Lesson/AddLecture";
import AddMeeting from "@components/Course/Lesson/AddMeeting";
import DeleteModal from "@components/Ui/DeleteModal";
import { Button, createStyles, Grid, Group, Modal, Text } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconTrashX } from "@tabler/icons";
import { LessonType } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import { ILessons, useDeleteLesson } from "@utils/services/courseService";
import {
  ILessonAssignment,
  ILessonFeedback,
  ILessonFile,
  ILessonMCQ,
  ILessonMeeting,
} from "@utils/services/types";
import { useState } from "react";
import { useParams } from "react-router-dom";

const useStyles = createStyles((theme) => ({
  item: {
    alignItems: "center",
    marginTop: "10px",
    borderRadius: theme.radius.md,
    border: `1px solid ${theme.colorScheme === "dark" ? theme.colors.dark[5] : theme.colors.gray[7]
      }`,
    padding: `${theme.spacing.sm}px ${theme.spacing.xl}px`,
    paddingLeft: theme.spacing.xl - theme.spacing.md, // to offset drag handle
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[5] : theme.white,
    marginBottom: theme.spacing.sm,
  },

  itemDragging: {
    boxShadow: theme.shadows.sm,
  },

  dragHandle: {
    ...theme.fn.focusStyles(),
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    height: "100%",
    color:
      theme.colorScheme === "dark"
        ? theme.colors.dark[1]
        : theme.colors.gray[6],
    paddingLeft: theme.spacing.md,
    paddingRight: theme.spacing.md,
  },
}));

const Lesson = ({
  lesson,
  sectionId,
}: {
  lesson: ILessons;
  sectionId: string;
}) => {
  const { classes, cx } = useStyles();
  const { id: slug } = useParams();

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
        message: "Lesson Deleted successfully!",
        title: "Success",
      });
      toggle();
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        color: "red",
        title: "Error",
      });
    }
  };
  return (
    <div>
      <DeleteModal
        title={`Are you sure you want to delete?`}
        open={value}
        onClose={toggle}
        onConfirm={onDeleteLesson}
      />
      <div className={classes.item}>
        <Grid grow justify={"center"}>
          <Grid.Col span={4}>
            <div style={{ display: "flex", alignItems: "center" }}>
              {lesson?.name}
            </div>
          </Grid.Col>
          <Grid.Col span={4}>
            <Group position="center">
              <Text m={"auto"}>{LessonType[lesson.type]}</Text>
            </Group>
          </Grid.Col>
          <Grid.Col span={4}>
            <Group position="right">
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
              >
                {isEditing ? "Cancel" : "Edit"}
              </Button>
            </Group>
          </Grid.Col>
        </Grid>
      </div>

      {isEditing &&
        LessonEditCase({
          item: lesson,
          lessonType: lesson?.type,
          setAddLessonClick: () => { },
          setAddState: () => { },
          sectionId: sectionId,
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
}: {
  lessonType: number;
  item: ILessons;
  setAddLessonClick: React.Dispatch<React.SetStateAction<boolean>>;
  setAddState: React.Dispatch<React.SetStateAction<string>>;
  sectionId: string;
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
        />
      );
    case LessonType.Exam:
      return (
        <AddExam
          sectionId={sectionId}
          item={item as ILessonMCQ}
          setAddState={setAddState}
          isEditing={true}
        />
      );
    case LessonType.Assignment:
      return (
        <AddAssignment
          sectionId={sectionId}
          item={item as ILessonAssignment}
          setAddState={setAddState}
          isEditing={true}
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
        />
      );
    case LessonType.Feedback:
      return (
        <AddFeedback
          sectionId={sectionId}
          item={item as ILessonFeedback}
          setAddState={setAddState}
          isEditing={true}
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
        />
      );
    default:
      break;
  }
};

export default Lesson;
