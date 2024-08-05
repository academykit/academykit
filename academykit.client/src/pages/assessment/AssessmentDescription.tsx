import TextViewer from "@components/Ui/RichTextViewer";
import UserShortProfile from "@components/UserShortProfile";
import useAuth from "@hooks/useAuth";
import {
  Badge,
  Box,
  Button,
  Container,
  Flex,
  Grid,
  Group,
  List,
  Modal,
  Paper,
  ScrollArea,
  Text,
  Textarea,
  Title,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import {
  IconArrowForward,
  IconCalendar,
  IconCheck,
  IconClockHour10,
  IconHelp,
  IconMilitaryAward,
  IconRepeat,
  IconX,
} from "@tabler/icons-react";
import { DATE_FORMAT } from "@utils/constants";
import { AssessmentStatus, SkillAssessmentRule, UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import {
  IResponseEligibilityCreation,
  useGetSingleAssessment,
  useUpdateAssessmentStatus,
} from "@utils/services/assessmentService";
import errorType from "@utils/services/axiosError";
import { t } from "i18next";
import moment from "moment";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getAssessmentStatus } from "./component/AssessmentCard";
import ResultTable from "./component/ResultTable";

const AssessmentStat = ({
  icon,
  label,
  value,
}: {
  icon: JSX.Element;
  label: string;
  value: string;
}) => {
  return (
    <Flex gap={10} mb={10}>
      <Group gap="sm">
        {icon}
        <Text>{t(`${label}`)}:</Text>
      </Group>

      <Text>{value}</Text>
    </Flex>
  );
};

export const getAssessmentText = (criteria: IResponseEligibilityCreation) => {
  let assessmentText = "";

  if (criteria.skillName !== null) {
    assessmentText += `, have skill "${criteria.skillName}"`;
  }

  if (criteria.role !== 0) {
    assessmentText += `, have role "${UserRole[criteria.role]}"`;
  }

  if (criteria.departmentName !== null) {
    assessmentText += `, be in department "${criteria.departmentName}"`;
  }

  if (criteria.groupName !== null) {
    assessmentText += `, be in group "${criteria.groupName}"`;
  }

  if (criteria.trainingName !== null) {
    assessmentText += `, complete training "${criteria.trainingName}"`;
  }

  if (criteria.assessmentName !== null) {
    assessmentText += `, complete assessment "${criteria.assessmentName}"`;
  }

  return assessmentText;
};

const AssessmentDescription = () => {
  const params = useParams();
  const navigate = useNavigate();
  const [resultModal, toggleResultModal] = useToggle();
  const assessmentStatus = useUpdateAssessmentStatus(params.id as string);
  const [isRejected, toggleRejected] = useToggle();
  const [confirmPublish, togglePublish] = useToggle();
  const assessmentDetail = useGetSingleAssessment(params.id as string);
  const auth = useAuth();
  const userRole = auth?.auth?.role as UserRole;

  // to validate reject message
  const form = useForm({
    initialValues: {
      message: "",
    },
    validate: {
      message: (value) =>
        value.length === 0 ? "Rejection message is required!" : null,
    },
  });

  // always visible to admin and super-admin
  // but, if the user is trainer, the assessment made by them is only editable
  const getEditAndPreviewPermission = () => {
    if (userRole <= UserRole.Admin) {
      return true;
    } else if (
      userRole === UserRole.Trainer &&
      assessmentDetail.data?.user.id === auth?.auth?.id
    ) {
      return true;
    } else {
      false;
    }
  };

  // only visible to trainees
  // and trainers who are not the creator of the assessment
  // and the assessment is not completed
  const getTakeAssessmentPermission = () => {
    if (userRole === UserRole.Trainee && !assessmentDetail.data?.hasCompleted) {
      return true;
    } else if (
      userRole === UserRole.Trainer &&
      assessmentDetail.data?.user.id !== auth?.auth?.id &&
      !assessmentDetail.data?.hasCompleted
    ) {
      return true;
    } else {
      false;
    }
  };

  const getEligibilityStatus = () => {
    if (assessmentDetail.data?.isEligible === true) {
      return true;
    }
    return false;
  };

  const checkAssessmentAvailability = () => {
    const startDate = moment(assessmentDetail.data?.startDate);
    const endDate = moment(assessmentDetail.data?.endDate);
    const currentDate = moment(new Date());

    if (
      currentDate.isAfter(startDate) &&
      currentDate.isBefore(endDate) &&
      getEligibilityStatus()
    ) {
      return true;
    }
    return false;
  };

  // do not show eligibility status to admin and super-admin
  // and the creator of the assessment (i.e., trainers and admins)
  const displayEligibilityStatus = () => {
    if (
      userRole > UserRole.Admin &&
      assessmentDetail.data?.user.id !== auth?.auth?.id
    ) {
      return true;
    } else {
      return false;
    }
  };

  const togglePublished = () => {
    togglePublish();
    toggleRejected(false);
    form.reset();
  };

  const onPublish = async (message?: string) => {
    try {
      await assessmentStatus.mutateAsync({
        identity: params.id as string,
        status: message
          ? AssessmentStatus.Rejected
          : AssessmentStatus.Published,
        message: message ?? "",
      });
      showNotification({
        message: message
          ? t("assessment_rejected_success")
          : t("assessment_published_success"),
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
    <>
      <Modal
        opened={confirmPublish}
        onClose={togglePublished}
        title={
          isRejected
            ? t("leave_message_reject")
            : `${t("publish_confirmation")} ${assessmentDetail.data?.title} ${t(
                "?"
              )}`
        }
      >
        {!isRejected ? (
          <Group mt={10}>
            <Button onClick={() => onPublish()}>{t("publish")}</Button>
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
              <Textarea
                autoFocus
                w={"100%"}
                {...form.getInputProps("message")}
              />
              <Button type="submit">{t("submit")}</Button>
              <Button variant="outline" onClick={() => toggleRejected()}>
                {t("cancel")}
              </Button>
            </Group>
          </form>
        )}
      </Modal>

      <Modal
        opened={resultModal}
        onClose={() => toggleResultModal()}
        size={"lg"}
      >
        <ResultTable
          assessmentId={params.id as string}
          userId={auth?.auth?.id as string}
        />
      </Modal>

      <Container fluid>
        <Flex wrap={"wrap"} align={"baseline"}>
          <Box maw={{ base: "100%", md: 300, lg: 500 }}>
            <Title
              style={{
                whiteSpace: "nowrap",
                overflow: "hidden",
                textOverflow: "ellipsis",
              }}
            >
              {assessmentDetail.data?.title}
            </Title>
          </Box>
        </Flex>
        <Flex my={"sm"} gap={10} wrap={"wrap"}>
          {displayEligibilityStatus() && (
            <>
              {getEligibilityStatus() ? (
                <Badge variant="light" color="green">
                  {t("eligible")}
                </Badge>
              ) : (
                <Badge variant="light" color="red">
                  {t("not_eligible")}
                </Badge>
              )}
            </>
          )}
          {getAssessmentStatus(
            assessmentDetail.data?.assessmentStatus as AssessmentStatus
          )}
        </Flex>

        {assessmentDetail.data && (
          <Group my={4}>
            <UserShortProfile user={assessmentDetail.data.user} size={"md"} />
          </Group>
        )}

        <Flex gap={10} justify={"flex-end"} mb={15}>
          {Number(auth?.auth?.role) <= UserRole.Admin &&
            assessmentDetail.data?.assessmentStatus ===
              AssessmentStatus.Review && (
              <Button onClick={() => togglePublished()} radius="xl">
                {t("publish")}
              </Button>
            )}

          {getEditAndPreviewPermission() && (
            <Button
              radius={"xl"}
              component={Link}
              to={RoutePath.manageAssessment.edit(params.id).route}
            >
              {t("edit")}
            </Button>
          )}

          {getTakeAssessmentPermission() && (
            <Button
              radius={"xl"}
              onClick={() =>
                navigate(RoutePath.assessmentExam.details(params.id).route)
              }
              disabled={!checkAssessmentAvailability()}
            >
              {t("take")}
            </Button>
          )}

          {getEditAndPreviewPermission() && (
            <Button
              radius={"xl"}
              component={Link}
              to={RoutePath.assessmentExam.details(params.id).route}
            >
              {t("preview")}
            </Button>
          )}

          {assessmentDetail.data?.hasCompleted && (
            <Button radius={"xl"} onClick={() => toggleResultModal()}>
              {t("view_result")}
            </Button>
          )}
        </Flex>

        <Grid>
          <Grid.Col
            span={{ base: 12, sm: 12, lg: 6 }}
            order={{ base: 2, sm: 2, lg: 1 }}
          >
            {assessmentDetail.isFetched && (
              <TextViewer content={assessmentDetail.data?.description ?? ""} />
            )}
          </Grid.Col>
          <Grid.Col
            span={{ base: 12, sm: 12, lg: 6 }}
            order={{ base: 1, sm: 1, lg: 2 }}
          >
            <Paper p="md">
              {assessmentDetail.data?.duration && (
                <AssessmentStat
                  icon={<IconClockHour10 />}
                  label="time_duration"
                  value={(assessmentDetail.data.duration / 60).toString() ?? ""} // show in minutes
                />
              )}
              <AssessmentStat
                icon={<IconHelp />}
                label="no_of_question"
                value={assessmentDetail.data?.noOfQuestion.toString() ?? ""}
              />
              <AssessmentStat
                icon={<IconRepeat />}
                label="total_retake_assessment"
                value={assessmentDetail.data?.retakes.toString() ?? ""}
              />
              {!getEditAndPreviewPermission() && (
                <AssessmentStat
                  icon={<IconArrowForward />}
                  label="remaining_retakes"
                  value={
                    assessmentDetail.data?.remainingAttempt.toString() ?? ""
                  }
                />
              )}
              <AssessmentStat
                icon={<IconCalendar />}
                label="start_date"
                value={moment(assessmentDetail.data?.startDate).format(
                  DATE_FORMAT
                )}
              />
              <AssessmentStat
                icon={<IconCalendar />}
                label="end_date"
                value={moment(assessmentDetail.data?.endDate).format(
                  DATE_FORMAT
                )}
              />
            </Paper>

            <Paper p="md" mt={15}>
              <Text mb={10}>{t("eligibility_criteria")}</Text>

              <ScrollArea.Autosize mah={300} mx="auto" scrollHideDelay={0}>
                <List>
                  {assessmentDetail.data?.eligibilityCreationRequestModels.map(
                    (criteria, index) => (
                      <List.Item
                        key={index}
                        icon={
                          // show eligibility status icon only if the user is not admin or super-admin
                          // and it not the owner of the assessment
                          displayEligibilityStatus() && (
                            <>
                              {criteria.isEligible ? (
                                <IconCheck size={18} />
                              ) : (
                                <IconX size={18} />
                              )}
                            </>
                          )
                        }
                      >{`Must${getAssessmentText(criteria)}`}</List.Item>
                    )
                  )}
                  {assessmentDetail.data &&
                    assessmentDetail.data?.eligibilityCreationRequestModels
                      .length < 1 && (
                      <Text>{t("no_eligibility_criteria")}</Text>
                    )}
                </List>
              </ScrollArea.Autosize>
            </Paper>

            <Paper p="md" mt={15}>
              <Text>{t("skill_schema")}</Text>

              <ScrollArea.Autosize mah={300} mx="auto">
                <List icon={<IconMilitaryAward size={18} />}>
                  {assessmentDetail.data?.skillsCriteriaRequestModels.map(
                    (criteria, index) => (
                      <List.Item key={index}>{`${
                        criteria.skillTypeName
                      } skill is earned if obtained percentage is ${
                        criteria.skillAssessmentRule ==
                        SkillAssessmentRule.IsGreaterThan
                          ? "greater than"
                          : "less than"
                      } ${criteria.percentage}%`}</List.Item>
                    )
                  )}
                </List>
              </ScrollArea.Autosize>
            </Paper>
          </Grid.Col>
        </Grid>
      </Container>
    </>
  );
};

export default AssessmentDescription;
