import {
  ActionIcon,
  Button,
  Group,
  Loader,
  Modal,
  Text,
  Tooltip,
  SimpleGrid,
  Box,
  Table,
  useMantineTheme,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import UserResults from "@pages/course/exam/Components/UserResults";
import { IconCheck, IconEye } from "@tabler/icons";
import { LessonType } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { Link, useParams } from "react-router-dom";
import { useWatchHistoryUser } from "@utils/services/watchHistory";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  IReportDetail,
  useGetMeetingReport,
} from "@utils/services/liveSessionService";
import moment from "moment";
import formatDuration from "@utils/formatDuration";
import { getType } from "./LessonStatusColor";

const TableRow = ({ values }: { values: IReportDetail }) => {
  const theme = useMantineTheme();
  return (
    <tr>
      <td>{moment(values.startDate).format(theme.dateFormat)}</td>
      <td>{values.joinedTime}</td>
      <td>{values.leftTime}</td>
      <td>{formatDuration(values.duration ?? 0, true)}</td>
    </tr>
  );
};

const StudentLessonDetails = ({
  type,
  questionSetId,
  studentId,
  lessonId,
  isCompleted,
  courseId,
  lessonName,
}: {
  type: LessonType;
  questionSetId?: string;
  studentId: string;
  lessonId: string;
  isCompleted: boolean;
  courseId: string;
  lessonName: string;
}) => {
  const slug = useParams();
  const watchHistory = useWatchHistoryUser(
    studentId,
    courseId,
    slug?.lessonId as string
  );
  const [examResultModal, setExamResultModal] = useToggle();

  const [liveClassReportModal, setLiveClassReportModal] = useToggle();
  const [confirmComplete, setConfirmComplete] = useToggle();

  const meetingReport = useGetMeetingReport(
    courseId,
    slug?.lessonId as string,
    studentId,
    liveClassReportModal
  );

  const getViewButton = () => {
    switch (type) {
      case LessonType.Exam:
        return (
          <Tooltip label="View Result">
            <ActionIcon
              color="green"
              variant="subtle"
              onClick={() => setExamResultModal()}
            >
              <IconEye />
            </ActionIcon>
          </Tooltip>
        );
      case LessonType.LiveClass:
        return (
          <Tooltip label="View Live Class Report">
            <ActionIcon
              color="green"
              variant="subtle"
              onClick={() => setLiveClassReportModal()}
            >
              <IconEye />
            </ActionIcon>
          </Tooltip>
        );
      case LessonType.Assignment:
        return (
          <Tooltip label="View Assignment Result">
            <ActionIcon
              component={Link}
              color="green"
              variant="subtle"
              to={RoutePath.assignment.result(lessonId, studentId).route}
            >
              <IconEye />
            </ActionIcon>
          </Tooltip>
        );

      case LessonType.Feedback:
        return (
          <Tooltip label="View Feedback">
            <ActionIcon
              component={Link}
              color="green"
              variant="subtle"
              to={RoutePath.feedback.result(lessonId, studentId).route}
            >
              <IconEye />
            </ActionIcon>
          </Tooltip>
        );
      default:
        return <div></div>;
    }
  };

  const onCompletedClick = async () => {
    try {
      await watchHistory.mutateAsync({ courseId, lessonId, userId: studentId });
      showNotification({
        message: "Successfully passed student.",
      });
      setConfirmComplete();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <>
      <Modal
        onClose={() => setExamResultModal()}
        trapFocus={true}
        opened={examResultModal}
        transition="slide-up"
        size={"100%"}
        styles={{
          modal: {
            height: "100%",
          },
          inner: {
            paddingLeft: 0,
            paddingRight: 0,
            paddingBottom: 0,
            paddingTop: "100px",
            height: "100%",
          },
        }}
      >
        {examResultModal && questionSetId && (
          <UserResults studentId={studentId} lessonId={questionSetId} />
        )}
      </Modal>
      <Modal
        opened={confirmComplete}
        onClose={() => setConfirmComplete()}
        title={`Are you sure you want to pass this student for "${lessonName}" lesson?`}
      >
        <Group>
          <Button onClick={onCompletedClick}>Confirm</Button>
          <Button onClick={() => setConfirmComplete()} variant="outline">
            Cancel
          </Button>
        </Group>
      </Modal>
      <Modal
        size={"xl"}
        overflow="inside"
        opened={liveClassReportModal}
        onClose={() => setLiveClassReportModal()}
        title={`Meeting Report`}
        styles={{
          title: {
            fontWeight: "bold",
            fontSize: "22px",
          },
        }}
      >
        <>
          {meetingReport.isLoading ? (
            <Loader />
          ) : (
            <>
              {!meetingReport.isError && (
                <Table>
                  <thead>
                    <tr>
                      <th>Start Date</th>
                      <th>Joined Time</th>
                      <th>Left Time</th>
                      <th>Duration</th>
                    </tr>
                  </thead>

                  <tbody>
                    {meetingReport.data.map((x) => (
                      <TableRow values={x} key={x.joinedTime} />
                    ))}
                  </tbody>
                </Table>
              )}
              {meetingReport.isError && <Box>Something went wrong.</Box>}
            </>
          )}
        </>
      </Modal>
      <Group>
        {getViewButton()}
        {!isCompleted && (
          <Tooltip label={`Mark as ${getType(type).true}`}>
            <ActionIcon
              onClick={() => setConfirmComplete()}
              variant="subtle"
              color={"primary"}
            >
              {watchHistory.isLoading ? (
                <Loader variant="oval" />
              ) : (
                <IconCheck />
              )}
            </ActionIcon>
          </Tooltip>
        )}
      </Group>
    </>
  );
};

export default StudentLessonDetails;
