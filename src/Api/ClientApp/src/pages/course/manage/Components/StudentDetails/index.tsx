import {
  ActionIcon,
  Button,
  Group,
  Loader,
  Modal,
  Tooltip,
  Box,
  Table,
  ScrollArea,
} from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import UserResults from '@pages/course/exam/Components/UserResults';
import { IconCheck, IconEye } from '@tabler/icons';
import { LessonType } from '@utils/enums';
import RoutePath from '@utils/routeConstants';
import { Link, useParams } from 'react-router-dom';
import { useWatchHistoryUser } from '@utils/services/watchHistory';
import { showNotification } from '@mantine/notifications';
import errorType from '@utils/services/axiosError';
import {
  IReportDetail,
  useGetMeetingReport,
} from '@utils/services/liveSessionService';
import moment from 'moment';
import formatDuration from '@utils/formatDuration';
import { getType } from './LessonStatusColor';
import { IStudentInfoLesson } from '@utils/services/manageCourseService';
import { useTranslation } from 'react-i18next';
import { DATE_FORMAT } from '@utils/constants';

const TableRow = ({ values }: { values: IReportDetail }) => {
  const { t } = useTranslation();

  return (
    <tr>
      <td>{moment(values.startDate).format(DATE_FORMAT)}</td>
      <td>{values.joinedTime}</td>
      <td>{values.leftTime}</td>
      <td>{formatDuration(values.duration ?? 0, true, t)}</td>
    </tr>
  );
};

const StudentLessonDetails = ({
  studentInfo: {
    lessonType: type,
    isCompleted,
    lessonId,
    lessonName,
    questionSetId,
    isPassed,
  },
  courseId,
  studentId,
}: {
  studentInfo: IStudentInfoLesson;
  courseId: string;
  studentId: string;
}) => {
  const slug = useParams();
  const watchHistory = useWatchHistoryUser(
    studentId,
    courseId,
    slug?.lessonId as string
  );
  const [examResultModal, setExamResultModal] = useToggle();
  const { t } = useTranslation();

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
          <Tooltip label={t('view_result')}>
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
          <Tooltip label={t('view_live_class_report')}>
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
          <Tooltip label={t('view_assignment_result')}>
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
          <Tooltip label={t('view_feedback')}>
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
        message: t('pass_student_success'),
      });
      setConfirmComplete();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  return (
    <>
      <Modal
        onClose={() => setExamResultModal()}
        trapFocus={true}
        opened={examResultModal}
        transitionProps={{ transition: 'slide-up' }}
        size={'100%'}
        styles={{
          inner: {
            paddingLeft: 0,
            paddingRight: 0,
            paddingBottom: 0,
            paddingTop: '100px',
            height: '100%',
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
        title={`${t('pass_student_confirmation')} "${lessonName}" ${t(
          'lesson?'
        )}`}
      >
        <Group>
          <Button onClick={onCompletedClick}>{t('confirm')}</Button>
          <Button onClick={() => setConfirmComplete()} variant="outline">
            {t('cancel')}
          </Button>
        </Group>
      </Modal>
      <Modal
        size={'xl'}
        scrollAreaComponent={ScrollArea.Autosize}
        opened={liveClassReportModal}
        onClose={() => setLiveClassReportModal()}
        title={t('meeting_report')}
        styles={{
          title: {
            fontWeight: 'bold',
            fontSize: '22px',
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
                      <th>{t('start_date')}</th>
                      <th>{t('join_time')}</th>
                      <th>{t('left_time')}</th>
                      <th>{t('duration')}</th>
                    </tr>
                  </thead>

                  <tbody>
                    {meetingReport.data.map((x) => (
                      <TableRow values={x} key={x.joinedTime} />
                    ))}
                  </tbody>
                </Table>
              )}
              {meetingReport.isError && <Box>{t('something_went_wrong')}</Box>}
            </>
          )}
        </>
      </Modal>
      <Group>
        {getViewButton()}
        {/* show button when student is failed or has not completed exam */}
        {(!isCompleted || !isPassed) && (
          <Tooltip label={`${t('mark_as')} ${getType(type).true}`}>
            <ActionIcon
              onClick={() => setConfirmComplete()}
              variant="subtle"
              color={'primary'}
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
