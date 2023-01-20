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
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconEye, IconFileCheck } from "@tabler/icons";
import { CourseStatus, UserRole, CourseUserStatus } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import { ICourse, useCourseStatus } from "@utils/services/courseService";
import moment from "moment";
import { Link } from "react-router-dom";

const color = (status: CourseStatus) => {
  switch (status) {
    case CourseStatus.Draft:
      return "red";
    case CourseStatus.Published:
      return "green";
    case CourseStatus.Review:
      return "yellow";
  }
  //   ColorS: "green",
  //   Review: "yellow",
  //   Draft: "red",
};

const CourseRow = ({ course }: { course: ICourse }) => {
  const [confirmPublish, togglePublish] = useToggle();
  const [isRejected, toggleRejected] = useToggle();
  const courseStatus = useCourseStatus(course.id);
  const auth = useAuth();

  const canPreviewEdit =
    auth?.auth?.role === UserRole.Admin ||
    auth?.auth?.role === UserRole.SuperAdmin ||
    course.userStatus === CourseUserStatus.Author;

  const onPublish = async () => {
    try {
      await courseStatus.mutateAsync({
        id: course.id as string,
        status: CourseStatus.Published,
      });
      showNotification({
        message: "Training has been successfully published!",
      });

      course.status = CourseStatus.Published;
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
    togglePublish();
  };
  return (
    <tr>
      <td>
        <Modal
          opened={confirmPublish}
          onClose={togglePublish}
          title={
            isRejected
              ? `Leave a message for your rejection`
              : `Are you sure you want to publish "${course.name}"?`
          }
        >
          {!isRejected ? (
            <Group mt={10}>
              <Button onClick={onPublish}>Publish</Button>
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
            <Group>
              <Textarea w={"100%"} />
              <Button>Submit</Button>
              <Button variant="outline" onClick={() => toggleRejected()}>
                Cancel
              </Button>
            </Group>
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
              <ActionIcon onClick={() => togglePublish()} color={"green"}>
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
