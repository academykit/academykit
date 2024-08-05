import UserShortProfile from "@components/UserShortProfile";
import useAuth from "@hooks/useAuth";
import {
  ActionIcon,
  Badge,
  Button,
  Group,
  Modal,
  Table,
  Text,
  Textarea,
  Tooltip,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconEdit, IconEye, IconFileCheck } from "@tabler/icons-react";
import { color } from "@utils/constants";
import { CourseStatus, CourseUserStatus, UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import { ICourse, useCourseStatus } from "@utils/services/courseService";
import moment from "moment";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const CourseRow = ({ course, search }: { course: ICourse; search: string }) => {
  const [confirmPublish, togglePublish] = useToggle();
  const [isRejected, toggleRejected] = useToggle();
  const { t } = useTranslation();
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
    Number(auth?.auth?.role) === UserRole.Admin ||
    Number(auth?.auth?.role) === UserRole.SuperAdmin ||
    course.userStatus === CourseUserStatus.Author;

  const onPublish = async (message?: string) => {
    try {
      await courseStatus.mutateAsync({
        identity: course.id as string,
        status: message ? CourseStatus.Rejected : CourseStatus.Published,
        message: message ?? "",
      });
      showNotification({
        title: t("successful"),
        message: message
          ? t("training_rejected_success")
          : t("training_published_success"),
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
    <Table.Tr>
      <Table.Td>
        <Modal
          opened={confirmPublish}
          onClose={togglePublished}
          title={
            isRejected
              ? t("leave_message_reject")
              : `${t("publish_confirmation")} "${course.name}"${t("?")}`
          }
        >
          {!isRejected ? (
            <Group mt={10}>
              <Button
                onClick={() => onPublish()}
                loading={courseStatus.isLoading}
              >
                {t("publish")}
              </Button>
              <Button
                variant="outline"
                onClick={() => {
                  toggleRejected();
                }}
              >
                {t("reject")}
              </Button>
            </Group>
          ) : (
            <form onSubmit={form.onSubmit((value) => onPublish(value.message))}>
              <Group>
                <Textarea {...form.getInputProps("message")} w={"100%"} />
                <Button loading={courseStatus.isLoading} type="submit">
                  {t("submit")}
                </Button>
                <Button variant="outline" onClick={() => toggleRejected()}>
                  {t("cancel")}
                </Button>
              </Group>
            </form>
          )}
        </Modal>
        <Text maw={"300px"} style={{ wordBreak: "break-all" }}>
          {course.name}
        </Text>
      </Table.Td>
      <Table.Td>{moment(course.createdOn).format("DD/MM/YY")}</Table.Td>
      <Table.Td>
        <UserShortProfile size={"xs"} user={course.user} page="" />
      </Table.Td>

      <Table.Td>
        <Badge variant="light" color={color(course.status)}>
          {t(CourseStatus[course.status])}
        </Badge>
      </Table.Td>
      <Table.Td>
        <Group>
          {canPreviewEdit && (
            <Tooltip label={t("preview")}>
              <ActionIcon
                component={Link}
                to={RoutePath.courses.description(course.slug).route}
                variant="subtle"
                color={"gray"}
              >
                <IconEye />
              </ActionIcon>
            </Tooltip>
          )}
          {canPreviewEdit && (
            <Tooltip label={t("edit_course")}>
              <ActionIcon
                variant="subtle"
                color={"gray"}
                component={Link}
                to={RoutePath.manageCourse.edit(course.slug).route}
              >
                <IconEdit />
              </ActionIcon>
            </Tooltip>
          )}
          {course.status === CourseStatus.Review && (
            <Tooltip label={t("publish_course")}>
              <ActionIcon
                variant="subtle"
                onClick={togglePublished}
                color={"green"}
              >
                <IconFileCheck />
              </ActionIcon>
            </Tooltip>
          )}
        </Group>
      </Table.Td>
    </Table.Tr>
  );
};

export default CourseRow;
