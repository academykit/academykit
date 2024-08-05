import DeleteModal from "@components/Ui/DeleteModal";
import { Anchor, Button, List, Menu, Paper, Table, Text } from "@mantine/core";
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
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import {
  IAssessmentResponse,
  useDeleteAssessment,
} from "@utils/services/assessmentService";
import { t } from "i18next";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { getAssessmentText } from "./AssessmentDescription";
import { getAssessmentStatus } from "./component/AssessmentCard";

const AssessmentRow = ({
  data,
  userRole,
  currentUser,
}: {
  data: IAssessmentResponse;
  userRole: UserRole;
  currentUser: string;
}) => {
  const deleteAssessment = useDeleteAssessment();
  const [deleteConfirmation, setDeleteConfirmation] = useToggle();

  const getMenuPermission = () => {
    if (userRole <= UserRole.Admin) {
      return true;
    } else if (userRole === UserRole.Trainer && data.user.id === currentUser) {
      return true;
    } else {
      false;
    }
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
      <Table.Tr key={data?.id}>
        <Table.Td
          style={{
            minWidth: "122px",
            overflow: "hidden",
            textOverflow: "ellipsis",
          }}
        >
          {
            <Anchor
              component={Link}
              to={RoutePath.assessment.description(data.slug).route}
              size={"md"}
              lineClamp={1}
              maw={"80%"}
            >
              <Text truncate>{data.title}</Text>
            </Anchor>
          }
        </Table.Td>

        <Table.Td>{getAssessmentStatus(data.assessmentStatus)}</Table.Td>
        <Table.Td>{data?.user?.fullName ?? ""}</Table.Td>
        <Table.Td
          style={{
            maxWidth: "200px",
            overflow: "hidden",
            textOverflow: "ellipsis",
          }}
        >
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
        </Table.Td>
        <Table.Td style={{ display: "flex", justifyContent: "center" }}>
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
        </Table.Td>
      </Table.Tr>
      <DeleteModal
        title={t("delete_assessment_confirmation")}
        open={deleteConfirmation}
        onClose={setDeleteConfirmation}
        onConfirm={handleDelete}
      />
    </>
  );
};

const AssessmentTable = ({
  assessment,
  userRole,
  currentUser,
}: {
  assessment: IAssessmentResponse[];
  userRole: UserRole;
  currentUser: string;
}) => {
  const { t } = useTranslation();

  const Rows = () =>
    assessment.map((data: any) => {
      return (
        <AssessmentRow
          data={data}
          userRole={userRole}
          currentUser={currentUser}
          key={data.id}
        />
      );
    });

  return (
    <>
      <Paper>
        <Table
          style={{ minWidth: 800 }}
          verticalSpacing="sm"
          striped
          highlightOnHover
          withTableBorder
          withColumnBorders
        >
          <Table.Thead>
            <Table.Tr>
              <Table.Th>{t("title")}</Table.Th>
              <Table.Th>{t("status")}</Table.Th>
              <Table.Th>{t("Creator")}</Table.Th>
              <Table.Th>{t("eligibility")}</Table.Th>
              <Table.Th></Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>{Rows()}</Table.Tbody>
        </Table>
      </Paper>
    </>
  );
};

export default AssessmentTable;
