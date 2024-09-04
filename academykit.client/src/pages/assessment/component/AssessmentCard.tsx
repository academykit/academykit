import DeleteModal from "@components/Ui/DeleteModal";
import UserShortProfile from "@components/UserShortProfile";
import {
  Anchor,
  Badge,
  Button,
  Card,
  Flex,
  Group,
  List,
  Menu,
  Text,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import {
  IconCheck,
  IconChevronRight,
  IconDotsVertical,
  IconEdit,
  IconTrash,
  IconX,
} from "@tabler/icons-react";
import { AssessmentStatus, UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import {
  IAssessmentResponse,
  useDeleteAssessment,
} from "@utils/services/assessmentService";
import { t } from "i18next";
import { Link, useNavigate } from "react-router-dom";
import { getAssessmentText } from "../AssessmentDescription";

export const getAssessmentStatus = (assessmentStatus: AssessmentStatus) => {
  if (assessmentStatus === AssessmentStatus.Draft) {
    return (
      <Badge variant="light" color="grape">
        {t("Draft")}
      </Badge>
    );
  } else if (assessmentStatus === AssessmentStatus.Review) {
    return (
      <Badge variant="light" color="orange">
        {t("review")}
      </Badge>
    );
  } else if (assessmentStatus === AssessmentStatus.Published) {
    return (
      <Badge variant="light" color="green">
        {t("Published")}
      </Badge>
    );
  } else if (assessmentStatus === AssessmentStatus.Rejected) {
    return (
      <Badge variant="light" color="red">
        {t("rejected")}
      </Badge>
    );
  }
};

const AssessmentCard = ({
  data,
  userRole,
  currentUser,
}: {
  data: IAssessmentResponse;
  userRole: UserRole;
  currentUser: string;
}) => {
  const navigate = useNavigate();
  const deleteAssessment = useDeleteAssessment();
  const [deleteConfirmation, setDeleteConfirmation] = useToggle();

  // always visible to admin and super-admin
  // but, if the user is trainer, the assessment made by them is only editable
  const getMenuPermission = () => {
    if (userRole <= UserRole.Admin) {
      return true;
    } else if (userRole === UserRole.Trainer && data.user.id === currentUser) {
      return true;
    } else {
      false;
    }
  };

  const getEligibilityStatus = () => {
    if (data?.isEligible === true) {
      return true;
    }
    return false;
  };

  // do not show eligibility status to admin and super-admin
  // and the creator of the assessment (i.e., trainers and admins)
  const displayEligibilityStatus = () => {
    if (userRole > UserRole.Admin && data.user.id !== currentUser) {
      return true;
    } else {
      return false;
    }
  };

  const handleDelete = async () => {
    try {
      await deleteAssessment.mutateAsync(data.id);
      showNotification({
        message: t("delete_assessment_success"),
      });
    } catch (error: any) {
      showNotification({
        message: error?.response?.data?.message,
        color: "red",
      });
    } finally {
      setDeleteConfirmation();
    }
  };

  return (
    <>
      <DeleteModal
        title={t("delete_assessment_confirmation")}
        open={deleteConfirmation}
        onClose={setDeleteConfirmation}
        onConfirm={handleDelete}
      />

      <Card p="md" withBorder radius={"md"}>
        <Group justify="space-between">
          <Anchor
            component={Link}
            to={RoutePath.assessment.description(data.slug).route}
            size={"md"}
            lineClamp={1}
            maw={"80%"}
          >
            <Text truncate>{data.title}</Text>
          </Anchor>

          {getMenuPermission() && (
            <Menu
              shadow="md"
              width={150}
              trigger="hover"
              withArrow
              position="left"
            >
              <Menu.Target>
                <Button style={{ zIndex: 50 }} variant="subtle" px={4}>
                  <IconDotsVertical />
                </Button>
              </Menu.Target>
              <Menu.Dropdown>
                <Menu.Item
                  leftSection={<IconEdit size={14} />}
                  component={Link}
                  to={RoutePath.manageAssessment.edit(data.slug).route}
                  rightSection={<IconChevronRight size={12} stroke={1.5} />}
                >
                  {t("manage")}
                </Menu.Item>
                <Menu.Divider />
                <Menu.Item
                  c="red"
                  leftSection={<IconTrash size={14} />}
                  onClick={(e) => {
                    e.preventDefault();
                    setDeleteConfirmation();
                  }}
                >
                  {t("delete")}
                </Menu.Item>
              </Menu.Dropdown>
            </Menu>
          )}
        </Group>

        <Group mt={"sm"}>
          <UserShortProfile user={data.user} size={"sm"} page="assessment" />
        </Group>
        <Flex mt={"sm"} gap={10}>
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
          {getAssessmentStatus(data.assessmentStatus)}
        </Flex>
        <Card.Section
          py={"xs"}
          px="lg"
          mt={"sm"}
          style={{
            borderTop: "1px solid var(--mantine-color-pool-border)",
          }}
        >
          <Text size="xs" c="dimmed" mb={10}>
            {t("eligibility")}
          </Text>

          <List>
            {data.eligibilityCreationRequestModels.length >= 1 ? (
              data.eligibilityCreationRequestModels
                .slice(0, 4)
                .map((eligibility, index) => (
                  <List.Item
                    key={index}
                    icon={
                      // show eligibility status icon only if the user is not admin or super-admin
                      // and it not the owner of the assessment
                      displayEligibilityStatus() && (
                        <>
                          {eligibility.isEligible ? (
                            <IconCheck size={18} />
                          ) : (
                            <IconX size={18} />
                          )}
                        </>
                      )
                    }
                  >
                    <Text lineClamp={1}>{`Must${getAssessmentText(
                      eligibility
                    )}`}</Text>
                  </List.Item>
                ))
            ) : (
              <Text>{t("no_eligibility_criteria")}</Text>
            )}
          </List>
          {data.eligibilityCreationRequestModels.length > 4 && (
            <Anchor
              component={Link}
              to={RoutePath.assessment.description(data.slug).route}
              size={"md"}
              lineClamp={1}
            >
              <Text truncate>{t("see_more")}</Text>
            </Anchor>
          )}
        </Card.Section>

        <Button
          variant="light"
          mt={"auto"}
          onClick={() =>
            navigate(RoutePath.assessment.description(data.slug).route)
          }
        >
          {t("view_assessment")}
        </Button>
      </Card>
    </>
  );
};

export default AssessmentCard;
