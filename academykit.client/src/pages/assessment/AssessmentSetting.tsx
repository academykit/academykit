import {
  Box,
  Button,
  Card,
  Container,
  Group,
  Loader,
  Text,
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { AssessmentStatus } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import {
  useGetSingleAssessment,
  useUpdateAssessmentStatus,
} from "@utils/services/assessmentService";
import errorType from "@utils/services/axiosError";
import { t } from "i18next";
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import classes from "./styles/setting.module.css";

const AssessmentSetting = () => {
  const { id } = useParams();
  const assessmentStatus = useUpdateAssessmentStatus(id as string);
  const assessmentDetail = useGetSingleAssessment(id as string);
  const [assessmentButton, setAssessmentButton] = useState(
    assessmentDetail.data?.assessmentStatus === AssessmentStatus.Published
  );

  const onPublish = async () => {
    try {
      const res = await assessmentStatus.mutateAsync({
        identity: id as string,
        status: AssessmentStatus.Review,
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

  const onUpdatePublish = async () => {
    try {
      // api call
      await assessmentStatus.mutateAsync({
        identity: id as string,
        status: AssessmentStatus.Draft,
      });
      showNotification({
        message: t("assessment_sent_to_draft"),
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
    setAssessmentButton(true);
  }, [assessmentStatus.isSuccess]);

  if (assessmentDetail.isLoading) {
    return <Loader />;
  }

  if (assessmentDetail.data?.assessmentStatus == AssessmentStatus.Published) {
    return (
      <Container fluid>
        <Group className={classes.group}>
          <Card shadow={"sm"} className={classes.right}>
            <Box>{t("assessment_in_published_mode")}</Box>
            <Button
              mt={20}
              component={Link}
              onClick={onUpdatePublish}
              to={RoutePath.manageAssessment.edit(id).route}
            >
              {t("update_assessment")}
            </Button>
          </Card>
          <Text>{t("or")}</Text>
          <Card shadow={"sm"} className={classes.left}>
            <Text>{t("done_editing_assessment")}</Text>
            <ol>
              <li>{t("correct_assessment_detail")}</li>
              <li>{t("assessment_questions_properly_added")}</li>

              <li>{t("assessment_description_added")}</li>
            </ol>
            <Button
              loading={assessmentStatus.isPending}
              onClick={onPublish}
              disabled={assessmentButton}
            >
              {t("update_publish")}
            </Button>
          </Card>
        </Group>
      </Container>
    );
  }

  if (assessmentDetail.data?.assessmentStatus == AssessmentStatus.Review) {
    return <>{t("assessment_under_review")}</>;
  }

  if (assessmentDetail.data?.assessmentStatus == AssessmentStatus.Rejected) {
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
          <Text mb={10}>{t("assessment_rejected")}</Text>
          <Button onClick={() => onUpdatePublish()}>
            {t("update_to_draft")}
          </Button>
        </Card>
      </>
    );
  }

  if (assessmentDetail.data?.assessmentStatus === AssessmentStatus.Draft) {
    return (
      <Container fluid>
        <Group className={classes.group}>
          <Card shadow={"sm"} className={classes.right}>
            <Box>{t("assessment_in_draft_mode")}</Box>
            <Button
              mt={20}
              component={Link}
              to={RoutePath.manageAssessment.edit(id).route}
            >
              {t("continue_edit")}
            </Button>
          </Card>
          <Text>{t("or")}</Text>
          <Card shadow={"sm"} className={classes.left}>
            <Text>{t("done_editing_assessment")}</Text>
            <ol>
              <li>{t("correct_assessment_detail")}</li>
              <li>{t("assessment_questions_properly_added")}</li>
              <li>{t("assessment_description_added")}</li>
            </ol>
            <Button loading={false} onClick={onPublish}>
              {t("publish")}{" "}
            </Button>
          </Card>
        </Group>
      </Container>
    );
  }

  return <Box>{t("assessment_marked_completed")}</Box>;
};

export default AssessmentSetting;
