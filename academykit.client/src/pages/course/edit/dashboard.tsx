import {
  Box,
  Button,
  Card,
  Container,
  Divider,
  Group,
  Loader,
  Modal,
  Text,
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { CourseStatus } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import {
  useCourseDescription,
  useCourseStatus,
  useCourseUpdateStatus,
} from "@utils/services/courseService";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useParams } from "react-router-dom";
import classes from "../styles/dashboard.module.css";

const Dashboard = () => {
  const { id } = useParams();
  const course = useCourseDescription(id as string);
  const courseStatus = useCourseStatus(id as string, "");
  const courseUpdateStatus = useCourseUpdateStatus(id as string);
  const [courseButton, setCourseButton] = useState(
    course.data?.status === CourseStatus.Published
  );
  const { t } = useTranslation();
  const [opened, setOpened] = useState(false);

  const onPublish = async () => {
    try {
      const res = await courseStatus.mutateAsync({
        identity: id as string,
        status: CourseStatus.Review,
      });

      showNotification({
        message: res.data.message,
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  // disable after publishing
  useEffect(() => {
    setCourseButton(true);
  }, [courseStatus.isSuccess]);

  const onUpdatePublish = async () => {
    try {
      await courseUpdateStatus.mutateAsync({
        id: id as string,
      });
      showNotification({
        message: t("training_sent_to_draft"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  if (course.isLoading) {
    return <Loader />;
  }
  const handleCompleted = async () => {
    try {
      await courseStatus.mutateAsync({
        identity: id as string,
        status: CourseStatus.Completed,
      });
      showNotification({
        message: t("training_sent_to_complete"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
    setOpened(false);
  };
  if (course.data?.status === CourseStatus.Published) {
    return (
      <Container fluid>
        <Modal
          opened={opened}
          onClose={() => setOpened(false)}
          title={t("status_change_completed") as string}
        >
          <Button
            loading={courseStatus.isPending}
            mr={5}
            onClick={handleCompleted}
          >
            {t("yes")}
          </Button>
          <Button
            loading={courseStatus.isPending}
            variant="outline"
            onClick={() => setOpened(false)}
          >
            {t("no")}
          </Button>
        </Modal>
        <Group className={classes.group}>
          <Card shadow={"sm"} className={classes.right}>
            <Box>{t("training_in_published_mode")}</Box>
            <Button
              mt={20}
              component={Link}
              onClick={onUpdatePublish}
              to={RoutePath.manageCourse.edit(id).route}
            >
              {t("update_training")}
            </Button>
          </Card>
          <Text>{t("or")}</Text>
          <Card shadow={"sm"} className={classes.left}>
            <Text>{t("done_editing_training")}</Text>
            <ol>
              <li>{t("correct_training_detail")}</li>
              <li>{t("lessons_sections_properly_added")}</li>

              <li>{t("lesson_description_added")}</li>
            </ol>
            <Button
              loading={courseStatus.isPending}
              onClick={onPublish}
              disabled={courseButton}
            >
              {t("update_publish")}
            </Button>
          </Card>
        </Group>
        <Divider my={20} />
        <Card
          shadow={"sm"}
          className={classes.center}
          style={{
            display: "flex",
            alignItems: "center",
            flexDirection: "column",
          }}
        >
          <Text mb={10}>{t("complete_your_training")}</Text>
          <Button onClick={() => setOpened(true)}>
            {t("complete_training")}
          </Button>
        </Card>
      </Container>
    );
  }

  if (course.data?.status === CourseStatus.Review) {
    return <>{t("training_under_review")}</>;
  }

  if (course.data?.status === CourseStatus.Rejected) {
    return (
      <>
        <Card
          shadow={"sm"}
          className={classes.center}
          style={{
            display: "flex",
            alignItems: "center",
            flexDirection: "column",
          }}
        >
          <Text mb={10}>{t("training_rejected")}</Text>
          <Button onClick={() => onUpdatePublish()}>
            {t("update_to_draft")}
          </Button>
        </Card>
      </>
    );
  }

  if (course.data?.status === CourseStatus.Draft) {
    return (
      <Container fluid>
        <Group className={classes.group}>
          <Card shadow={"sm"} className={classes.right}>
            <Box>{t("training_in_draft_mode")}</Box>
            <Button
              mt={20}
              component={Link}
              to={RoutePath.manageCourse.edit(id).route}
            >
              {t("continue_edit")}
            </Button>
          </Card>
          <Text>{t("or")}</Text>
          <Card shadow={"sm"} className={classes.left}>
            <Text>{t("done_editing_course")}</Text>
            <ol>
              <li>{t("correct_training_detail")}</li>
              <li>{t("lessons_sections_properly_added")}</li>
              <li>{t("lesson_description_added")}</li>
            </ol>
            <Button loading={courseStatus.isPending} onClick={onPublish}>
              {t("publish")}{" "}
            </Button>
          </Card>
        </Group>
      </Container>
    );
  }

  return <Box>{t("training_marked_completed")}</Box>;
};

export default Dashboard;
