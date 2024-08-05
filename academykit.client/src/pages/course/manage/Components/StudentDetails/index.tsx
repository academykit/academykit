import {
  ActionIcon,
  Box,
  Button,
  Group,
  Loader,
  Modal,
  ScrollArea,
  Table,
  Textarea,
  Tooltip,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import UserResults from '@pages/course/exam/Components/UserResults';
import { IconCheck, IconEye } from '@tabler/icons-react';
import { DATE_FORMAT } from '@utils/constants';
import { LessonType } from '@utils/enums';
import formatDuration from '@utils/formatDuration';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import {
  IReportDetail,
  useGetMeetingReport,
} from '@utils/services/liveSessionService';
import { IStudentInfoLesson } from '@utils/services/manageCourseService';
import { useReviewAttendance } from '@utils/services/physicalTrainingService';
import { useWatchHistoryUser } from '@utils/services/watchHistory';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';
import { getType } from './LessonStatusColor';

const TableRow = ({ values }: { values: IReportDetail }) => {
  const { t } = useTranslation();

  return (
    <Table.Tr>
      <Table.Td>{moment(values.startDate).format(DATE_FORMAT)}</Table.Td>
      <Table.Td>{values.joinedTime}</Table.Td>
      <Table.Td>{values.leftTime}</Table.Td>
      <Table.Td>{formatDuration(values.duration ?? 0, true, t)}</Table.Td>
    </Table.Tr>
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
    attendanceReviewed,
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
  const form = useForm({
    initialValues: {
      message: '',
    },
    validate: {
      message: (value) =>
        value.length === 0 ? 'Rejection message is required!' : null,
    },
  });

  const [examResultModal, setExamResultModal] = useToggle();
  const { t } = useTranslation();

  const [liveClassReportModal, setLiveClassReportModal] = useToggle();
  const [confirmComplete, setConfirmComplete] = useToggle();
  const [isRejected, toggleRejected] = useToggle();
  const [confirmReview, toggleReview] = useToggle();

  const attendanceReview = useReviewAttendance(
    courseId,
    slug?.lessonId as string,
    studentId
  );

  const meetingReport = useGetMeetingReport(
    courseId,
    slug?.lessonId as string,
    studentId,
    liveClassReportModal
  );

  const onReview = async (message?: string) => {
    const data = {
      identity: lessonId,
      isPassed: message ? false : true,
      message: message ?? '',
      userId: studentId,
    };

    try {
      await attendanceReview.mutateAsync({ data });
      showNotification({
        message: message ? t('physical_deny') : t('physical_approve'),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
    toggleReview();
  };

  const getViewButton = () => {
    switch (type) {
      case LessonType.Exam:
        return (
          <Tooltip label={t('view_result')}>
            <ActionIcon
              c="green"
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
              c="green"
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
              c="green"
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
              c="green"
              variant="subtle"
              to={RoutePath.feedback.result(lessonId, studentId).route}
            >
              <IconEye />
            </ActionIcon>
          </Tooltip>
        );
      case LessonType.RecordedVideo:
        return (
          <Tooltip label={t('view_live_class_report')}>
            <ActionIcon
              c="green"
              variant="subtle"
              onClick={() => setLiveClassReportModal()}
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

  const toggleReviewed = () => {
    toggleReview();
    toggleRejected(false);
    form.reset();
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
          <UserResults
            isTrainee={false}
            studentId={studentId}
            lessonId={questionSetId}
          />
        )}
      </Modal>

      {/* attendance pop-up */}
      <Modal
        opened={confirmReview}
        onClose={toggleReviewed}
        title={
          isRejected
            ? t('leave_message_reject')
            : `${t('mark_as_attended')}${t('?')}`
        }
      >
        {!isRejected ? (
          <Group mt={10}>
            <Button
              onClick={() => onReview()}
              loading={attendanceReview.isLoading}
            >
              {t('approve')}
            </Button>
            <Button
              variant="outline"
              onClick={() => {
                toggleRejected();
              }}
            >
              {t('reject')}
            </Button>
          </Group>
        ) : (
          <form onSubmit={form.onSubmit((value) => onReview(value.message))}>
            <Group>
              <Textarea {...form.getInputProps('message')} w={'100%'} />
              <Button loading={attendanceReview.isLoading} type="submit">
                {t('submit')}
              </Button>
              <Button variant="outline" onClick={() => toggleRejected()}>
                {t('cancel')}
              </Button>
            </Group>
          </form>
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
                <Table
                  striped
                  withTableBorder
                  withColumnBorders
                  highlightOnHover
                >
                  <Table.Thead>
                    <Table.Tr>
                      <Table.Th>{t('start_date')}</Table.Th>
                      <Table.Th>{t('join_time')}</Table.Th>
                      <Table.Th>{t('left_time')}</Table.Th>
                      <Table.Th>{t('duration')}</Table.Th>
                    </Table.Tr>
                  </Table.Thead>

                  <Table.Tbody>
                    {meetingReport.data.map((x) => (
                      <TableRow values={x} key={x.joinedTime} />
                    ))}
                  </Table.Tbody>
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
        {/* button for other lesson types */}
        {(!isCompleted || !isPassed) && type !== LessonType.Physical && (
          <Tooltip label={`${t('mark_as')} ${getType(type).true}`}>
            <ActionIcon
              // open different pop up for physical lesson type
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

        {/* button for other physical type */}
        {!attendanceReviewed && type == LessonType.Physical && (
          <Tooltip label={`${t('mark_as')} ${getType(type).true}`}>
            <ActionIcon
              // open different pop up for physical lesson type
              onClick={() => toggleReview()}
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
