import { Badge, Button, Paper } from "@mantine/core";
import React, { useState } from "react";
import { useSection } from "@context/SectionProvider";
import AddLecture from "@components/Course/Lesson/AddLecture";
import AddExam from "@components/Course/Lesson/AddExam";
import AddAssignment from "@components/Course/Lesson/AddAssignment";
import AddMeeting from "@components/Course/Lesson/AddMeeting";
import AddFeedback from "@components/Course/Lesson/AddFeedback";
import AddDocument from "@components/Course/Lesson/AddDocument";

const LessonAddList = ({
  sectionId,
  setAddLessonClick,
}: {
  setAddLessonClick: React.Dispatch<React.SetStateAction<boolean>>;
  sectionId: string;
}) => {
  const [addState, setAddState] = React.useState<string>("");
  let returnDiv;
  switch (addState) {
    case "lecture":
      returnDiv = (
        <AddLecture
          sectionId={sectionId}
          setAddState={setAddState}
          isEditing={false}
          setAddLessonClick={setAddLessonClick}
        />
      );
      break;
    case "mcq":
      returnDiv = <AddExam setAddState={setAddState} sectionId={sectionId} />;
      break;
    case "assignment":
      returnDiv = (
        <AddAssignment setAddState={setAddState} sectionId={sectionId} />
      );
      break;
    case "meeting":
      returnDiv = (
        <AddMeeting
          setAddState={setAddState}
          sectionId={sectionId}
          setAddLessonClick={setAddLessonClick}
        />
      );
      break;
    case "feedback":
      returnDiv = (
        <AddFeedback setAddState={setAddState} sectionId={sectionId} />
      );
      break;
    case "document":
      returnDiv = (
        <AddDocument
          setAddState={setAddState}
          sectionId={sectionId}
          setAddLessonClick={setAddLessonClick}
        />
      );
    default:
      break;
  }
  return (
    <div>
      {!addState ? (
        <>
          <Badge
            style={{ cursor: "pointer" }}
            variant="outline"
            onClick={() => setAddState("lecture")}
          >
            + Lecture
          </Badge>{" "}
          <Badge
            style={{ cursor: "pointer" }}
            variant="outline"
            onClick={() => setAddState("mcq")}
          >
            + Exam
          </Badge>{" "}
          <Badge
            style={{ cursor: "pointer" }}
            variant="outline"
            onClick={() => setAddState("assignment")}
          >
            + Assignment
          </Badge>{" "}
          <Badge
            style={{ cursor: "pointer" }}
            variant="outline"
            onClick={() => setAddState("meeting")}
          >
            + Live Class
          </Badge>{" "}
          <Badge
            style={{ cursor: "pointer" }}
            variant="outline"
            onClick={() => setAddState("feedback")}
          >
            + Feedback
          </Badge>{" "}
          <Badge
            style={{ cursor: "pointer" }}
            variant="outline"
            onClick={() => setAddState("document")}
          >
            + Document
          </Badge>{" "}
          <Badge
            color="red"
            style={{ cursor: "pointer" }}
            variant="outline"
            onClick={() => setAddLessonClick(true)}
          >
            X Close
          </Badge>
        </>
      ) : (
        returnDiv
      )}
    </div>
  );
};

const AddLesson = ({ sectionId }: { sectionId: string }) => {
  const section = useSection();
  const [addLessonClick, setAddLessonClick] = useState<boolean>(true);

  return (
    <div style={{ marginTop: "10px", marginBottom: "10px" }}>
      {addLessonClick ? (
        <Button
          variant="outline"
          onClick={() => {
            // section?.setAddLessonClick(!section?.addLessonClick);

            setAddLessonClick(!addLessonClick);
            section?.setIsAddSection(false);
          }}
        >
          Add Lesson
        </Button>
      ) : (
        <LessonAddList
          sectionId={sectionId}
          setAddLessonClick={setAddLessonClick}
        />
      )}
    </div>
  );
};

export default AddLesson;
