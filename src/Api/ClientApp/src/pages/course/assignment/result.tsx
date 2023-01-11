import UserShortProfile from "@components/UserShortProfile";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Card,
  Container,
  createStyles,
  Group,
  Loader,
  Modal,
  Paper,
  Text,
  Title,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import RichTextEditor from "@mantine/rte";
import { QuestionType, UserRole } from "@utils/enums";
import {
  IAssignmentReview,
  useAssignmentReview,
} from "@utils/services/assignmentService";
import { useNavigate, useParams } from "react-router-dom";
import AssignmentReviewForm from "./Component/AssignmentReviewForm";

const useStyle = createStyles((theme) => ({
  option: {
    ">label": {
      cursor: "pointer",
    },
  },
  wrong: {
    border: `2px solid ${theme.colors.red[6]}`,
  },
  correct: {
    border: `2px solid ${theme.colors.green[6]}`,
  },
  active: {
    outline: `2px solid ${theme.colors[theme.primaryColor][1]}`,
  },
}));

const AssignmentResult = () => {
  const { classes, cx } = useStyle();
  const [showReviewBox, setShowReviewBox] = useToggle();
  const { id, studentId } = useParams();
  const navigate = useNavigate();
  const auth = useAuth();

  const getAssignment = useAssignmentReview(id as string, studentId as string);

  if (getAssignment.isError) {
    throw getAssignment.error;
  }
  if (getAssignment.isLoading) {
    return <Loader />;
  }
  return (
    <Container>
      <Modal
        opened={showReviewBox}
        onClose={() => setShowReviewBox()}
        title={
          !!getAssignment.data?.assignmentReview?.review
            ? "Edit Assignment Review"
            : "Review Assignment"
        }
      >
        <AssignmentReviewForm
          reviewId={getAssignment.data?.assignmentReview?.id}
          closeModal={() => setShowReviewBox()}
          edit={!!getAssignment.data.assignmentReview}
          editData={getAssignment.data}
        />
      </Modal>
      {getAssignment.data.assignments.map((x) => (
        <Card key={x.id} shadow="sm" my={10} withBorder>
          <Title>{x.name}</Title>
          {x.description && (
            <RichTextEditor
              styles={{
                root: {
                  border: "none",
                },
              }}
              my={10}
              value={x.description}
              readOnly
            ></RichTextEditor>
          )}
          {x.type === QuestionType.Subjective ? (
            <RichTextEditor readOnly={true} value={x.answer} />
          ) : (
            x.assignmentQuestionOptions &&
            x.assignmentQuestionOptions.map((option) => (
              <Card
                shadow={"md"}
                my={10}
                p={10}
                className={cx({
                  [classes.active]: option.isSelected,
                  [classes.wrong]: !option.isCorrect && option.isSelected,
                  [classes.correct]: option.isCorrect,
                })}
              >
                <input type={"checkbox"} style={{ display: "none" }} />
                <RichTextEditor
                  styles={{
                    root: {
                      border: "none",
                    },
                  }}
                  readOnly
                  value={option.option}
                ></RichTextEditor>
              </Card>
            ))
          )}
        </Card>
      ))}
      {getAssignment.data.assignmentReview && (
        <Paper p={20}>
          <Title>Review</Title>
          <Group>
            <UserShortProfile
              size={"md"}
              user={getAssignment.data?.assignmentReview?.teacher}
            />
            <Paper withBorder shadow={"xl"} px={40} py={20} mx={20}>
              <Group>
                <Text> Mark</Text>
                <Text color={"dimmed"}>
                  {getAssignment.data?.assignmentReview?.mark}/100
                </Text>
              </Group>
              <Group>
                <Text>Review</Text>
                <Text color={"dimmed"}>
                  {getAssignment.data?.assignmentReview?.review}
                </Text>
              </Group>
            </Paper>
          </Group>
        </Paper>
      )}

      <Group my={20}>
        {auth?.auth && auth?.auth?.role <= UserRole.Trainer && (
          <Button onClick={() => setShowReviewBox()}>
            {getAssignment.data?.assignmentReview?.review
              ? "Edit Review"
              : "Add Review"}
          </Button>
        )}
        <Button
          variant="outline"
          onClick={() => {
            navigate(-1);
          }}
        >
          Go Back
        </Button>
      </Group>
    </Container>
  );
};

export default AssignmentResult;
