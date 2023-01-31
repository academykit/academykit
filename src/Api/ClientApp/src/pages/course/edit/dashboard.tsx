import {
  Box,
  Button,
  Card,
  Container,
  createStyles,
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
import { useState } from "react";
import { Link, useParams } from "react-router-dom";

const useStyles = createStyles((theme) => ({
  group: {
    justifyContent: "center",
  },
  right: {
    width: "40%",
    [theme.fn.smallerThan(theme.breakpoints.md)]: {
      width: "100%",
    },
  },
  left: {
    width: "50%",
    [theme.fn.smallerThan(theme.breakpoints.md)]: {
      width: "100%",
    },
  },
  center: {
    width: "50%",
    margin: "auto",
  },
}));
const Dashboard = () => {
  const { classes } = useStyles();
  const { id } = useParams();
  const course = useCourseDescription(id as string);
  const courseStatus = useCourseStatus(id as string, "");
  const courseUpdateStatus = useCourseUpdateStatus(id as string);
  const [courseButton, setCourseButton] = useState(
    course.data?.status === CourseStatus.Published
  );
  const onPublish = async () => {
    try {
      await courseStatus.mutateAsync({
        identity: id as string,
        status: CourseStatus.Review,
      });
      showNotification({
        message: "Training has been sent to be reviewed!",
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  const onUpdatePublish = async () => {
    try {
      await courseUpdateStatus.mutateAsync({
        id: id as string,
      });
      showNotification({
        message: "Training has been sent to be draft!",
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
  const [opened, setOpened] = useState(false);
  const handleCompleted = async () => {
    try {
      await courseStatus.mutateAsync({
        identity: id as string,
        status: CourseStatus.Completed,
      });
      showNotification({
        message: "Training has been sent to completed!",
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  if (course.data?.status === CourseStatus.Published) {
    return (
      <Container fluid>
        <Modal
          opened={opened}
          onClose={() => setOpened(false)}
          title="Want to change status of this Training to Completed?"
        >
          <Button mr={5} onClick={handleCompleted}>
            Yes
          </Button>
          <Button variant="outline" onClick={() => setOpened(false)}>
            No
          </Button>
        </Modal>
        <Group className={classes.group}>
          <Card shadow={"sm"} className={classes.right}>
            <Box>Your Training is in published Mode.</Box>
            <Button
              mt={20}
              component={Link}
              onClick={onUpdatePublish}
              to={RoutePath.manageCourse.edit(id).route}
            >
              Update Training
            </Button>
          </Card>
          <Text>Or</Text>
          <Card shadow={"sm"} className={classes.left}>
            <Text>
              If you are done editing, you may publish your training. Before
              publishing, make sure that the following are done.
            </Text>
            <ol>
              <li>Training detail is correct.</li>
              <li>All lessons and sections are properly added.</li>

              <li>The lesson description is added.</li>
            </ol>
            <Button
              loading={courseStatus.isLoading}
              onClick={onPublish}
              disabled={courseButton}
            >
              Update Publish
            </Button>
          </Card>
        </Group>
        <Divider my={20} />
        <Card
          shadow={"sm"}
          className={classes.center}
          sx={{
            display: "flex",
            alignItems: "center",
            flexDirection: "column",
          }}
        >
          <Text mb={10}>Do you wish to complete your Training?</Text>
          <Button onClick={() => setOpened(true)}>Complete Training</Button>
        </Card>
      </Container>
    );
  }

  if (course.data?.status === CourseStatus.Review) {
    return <>Training is under Review</>;
  }

  if (course.data?.status === CourseStatus.Rejected) {
    return (
      <>
        <Card
          shadow={"sm"}
          className={classes.center}
          sx={{
            display: "flex",
            alignItems: "center",
            flexDirection: "column",
          }}
        >
          <Text mb={10}>
            Your training has been rejected. If you wish to edit your training,
            Please update training to Draft.
          </Text>
          <Button onClick={() => onUpdatePublish()}>Update to Draft</Button>
        </Card>
      </>
    );
  }

  if (course.data?.status === CourseStatus.Draft) {
    return (
      <Container fluid>
        <Group className={classes.group}>
          <Card shadow={"sm"} className={classes.right}>
            <Box>Your Training is in draft mode.</Box>
            <Button
              mt={20}
              component={Link}
              to={RoutePath.manageCourse.edit(id).route}
            >
              Continue Edit
            </Button>
          </Card>
          <Text>Or</Text>
          <Card shadow={"sm"} className={classes.left}>
            <Text>
              If you are done editing, you may publish your course. Before
              publishing, make sure that the following are done.
            </Text>
            <ol>
              <li>Training detail is correct.</li>
              <li>All lessons and sections are properly added.</li>
              <li>The lesson description is added.</li>
            </ol>
            <Button loading={courseStatus.isLoading} onClick={onPublish}>
              Publish
            </Button>
          </Card>
        </Group>
      </Container>
    );
  }

  return (
    <Box>
      This Training has been marked as Completed. No Further changes can be made
      in this Training.
    </Box>
  );
};

export default Dashboard;
