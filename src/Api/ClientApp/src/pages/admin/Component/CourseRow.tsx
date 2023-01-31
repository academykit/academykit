import UserShortProfile from "@components/UserShortProfile";
import useAuth from "@hooks/useAuth";
import {
  ActionIcon,
  Badge,
  Button,
  Group,
  Modal,
  Textarea,
  Tooltip,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconEye, IconFileCheck } from "@tabler/icons";
import { color } from "@utils/constants";
import { CourseStatus, UserRole, CourseUserStatus } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import { ICourse, useCourseStatus } from "@utils/services/courseService";
import moment from "moment";
import { Link } from "react-router-dom";

const CourseRow = ({ course, search }: { course: ICourse; search: string }) => {
  const [confirmPublish, togglePublish] = useToggle();
  const [isRejected, toggleRejected] = useToggle();

  const courseStatus = useCourseStatus(course.id, search);
  const auth = useAuth();

  const form = useForm({
    initialValues: {
      message: "",
    },
    validate: {
      message: (value) =>
        value.length === 0 ? "Rejection message is required!" : null,
    },
  });

  const togglePublished = () => {
    togglePublish();
    toggleRejected(false);
    form.reset();
  };

  const canPreviewEdit =
    auth?.auth?.role === UserRole.Admin ||
    auth?.auth?.role === UserRole.SuperAdmin ||
    course.userStatus === CourseUserStatus.Author;

  const onPublish = async (message?: string) => {
    try {
      await courseStatus.mutateAsync({
        identity: course.id as string,
        status: message ? CourseStatus.Rejected : CourseStatus.Published,
        message: message ?? "",
      });
      showNotification({
        message: `Training has been ${
          message ? "rejected!" : "successfully published!"
        }`,
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
    togglePublished();
  };
  return (
    <tr>
      <td>
        <Modal
          opened={confirmPublish}
          onClose={togglePublished}
          title={
            isRejected
              ? `Leave a message for your rejection`
              : `Are you sure you want to publish "${course.name}"?`
          }
        >
          {!isRejected ? (
            <Group mt={10}>
              <Button
                onClick={() => onPublish()}
                loading={courseStatus.isLoading}
              >
                Publish
              </Button>
              <Button
                variant="outline"
                onClick={() => {
                  toggleRejected();
                }}
              >
                Reject
              </Button>
            </Group>
          ) : (
            <form onSubmit={form.onSubmit((value) => onPublish(value.message))}>
              <Group>
                <Textarea {...form.getInputProps("message")} w={"100%"} />
                <Button loading={courseStatus.isLoading} type="submit">
                  Submit
                </Button>
                <Button variant="outline" onClick={() => toggleRejected()}>
                  Cancel
                </Button>
              </Group>
            </form>
          )}
        </Modal>
        {course.name}
      </td>
      <td>{moment(course.createdOn).format("DD/MM/YY")}</td>
      <td>
        <UserShortProfile size={"xs"} user={course.user} />
      </td>

      <td>
        <Badge color={color(course.status)}>
          {CourseStatus[course.status]}
        </Badge>
      </td>
      <td>
        <Group>
          {canPreviewEdit && (
            <Tooltip label="Preview">
              <ActionIcon
                component={Link}
                to={RoutePath.courses.description(course.slug).route}
                color={"primary"}
              >
                <IconEye />
              </ActionIcon>
            </Tooltip>
          )}
          {canPreviewEdit && (
            <Tooltip label="Edit course">
              <ActionIcon
                component={Link}
                to={RoutePath.manageCourse.edit(course.slug).route}
              >
                <IconEdit />
              </ActionIcon>
            </Tooltip>
          )}
          {course.status === CourseStatus.Review && (
            <Tooltip label="Publish this course">
              <ActionIcon onClick={togglePublished} color={"green"}>
                <IconFileCheck />
              </ActionIcon>
            </Tooltip>
          )}
        </Group>
      </td>
    </tr>
  );
};

export default CourseRow;
