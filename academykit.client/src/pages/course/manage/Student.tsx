import ConfirmationModal from "@components/Ui/ConfirmationModal";
import ProgressBar from "@components/Ui/ProgressBar";
import withSearchPagination, {
  type IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  Anchor,
  Avatar,
  Badge,
  Box,
  Button,
  Checkbox,
  Drawer,
  Flex,
  Group,
  Loader,
  Paper,
  ScrollArea,
  Table,
  Text,
  Title,
  Tooltip,
  UnstyledButton,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconDownload, IconEye, IconPlus } from "@tabler/icons-react";
import { DATE_FORMAT } from "@utils/constants";
import downloadImage from "@utils/downloadImage";
import { getInitials } from "@utils/getInitialName";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import {
  type IStudentStat,
  useGetStudentStatistics,
  usePostStatisticsCertificate,
} from "@utils/services/manageCourseService";
import moment from "moment";
import { type Dispatch, type SetStateAction, useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useParams } from "react-router-dom";
import AddTrainee from "./AddTrainee";

const Rows = ({
  item,
  setSelected,
  selected,
  searchParams,
}: {
  item: IStudentStat;
  setSelected: Dispatch<SetStateAction<string[]>>;
  selected: string[];
  searchParams: string;
}) => {
  const { id } = useParams();
  const course_id = id as string;
  const postUserData = usePostStatisticsCertificate(course_id, searchParams);
  const { t } = useTranslation();

  const handleSubmit = async (dataUser: string[]) => {
    try {
      await postUserData.mutateAsync({
        data: dataUser,
        issueAll: false,
        identity: course_id,
      });
      showNotification({ message: t("certificate_issue_success") });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: t("error"),
        color: "red",
        message: err,
      });
    }
  };

  const handelCheckboxChange = (userId: string) => {
    if (selected.includes(userId)) {
      setSelected(selected.filter((x) => x !== userId));
    } else {
      setSelected([userId, ...selected]);
    }
  };

  return (
    <Table.Tr key={item.userId}>
      <Table.Td>
        <Checkbox
          onChange={() => handelCheckboxChange(item?.userId)}
          checked={selected.includes(item.userId)}
        />
      </Table.Td>
      <Table.Td>
        <Anchor
          component={Link}
          to={`${RoutePath.userProfile}/${item.userId}`}
          size="sm"
          style={{ display: "flex" }}
        >
          <Avatar
            style={{ cursor: "pointer" }}
            size={26}
            mr={8}
            src={item?.imageUrl}
            radius={26}
          >
            {!item?.imageUrl && getInitials(item?.fullName ?? "")}
          </Avatar>

          {item?.fullName}
        </Anchor>
      </Table.Td>
      <Table.Td>
        <ProgressBar total={100} positive={item?.percentage} />
        <UnstyledButton component={Link} to={item.userId}>
          <Badge color="green" variant="outline" mt={10}>
            {t("view")}
          </Badge>
        </UnstyledButton>
      </Table.Td>
      <Table.Td>
        <Flex direction={"column"} justify={"center"} align={"center"}>
          {item?.hasCertificateIssued ? (
            <div style={{ marginTop: "10px" }}>
              <Text>
                {t("issued_on")}{" "}
                {moment(`${item?.certificateIssuedDate}Z`).format(DATE_FORMAT)}
              </Text>
              <Flex justify={"center"} mt={8}>
                <Tooltip label={t("view_certificate")}>
                  <UnstyledButton
                    mr="sm"
                    onClick={() => {
                      window.open(item.certificateUrl);
                    }}
                  >
                    <IconEye size={23} color="green" />
                  </UnstyledButton>
                </Tooltip>
                <Tooltip label={t("download_certificate")}>
                  <UnstyledButton
                    onClick={() =>
                      downloadImage(item.certificateUrl, item.fullName)
                    }
                  >
                    <IconDownload size={23} color="green" />
                  </UnstyledButton>
                </Tooltip>
              </Flex>
            </div>
          ) : (
            <>
              <Badge>{t("not_issued")}</Badge>
              <Button
                size="xs"
                mt={10}
                loading={postUserData.isPending}
                onClick={() => handleSubmit([item.userId])}
              >
                {t("issue")}
              </Button>
            </>
          )}
        </Flex>
      </Table.Td>
      <Table.Td style={{ textAlign: "center" }}>
        <Anchor
          component={Link}
          to={`${RoutePath.classes}/${course_id}/${item.lessonSlug}`}
          size="sm"
        >
          {item?.lessonName}
        </Anchor>
      </Table.Td>
    </Table.Tr>
  );
};

const ManageStudents = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const course_id = id as string;

  const getStudentStat = useGetStudentStatistics(course_id, searchParams);
  const [openModal, setOpenModal] = useState(false);
  const postUserData = usePostStatisticsCertificate(course_id, searchParams);
  const [selected, setSelected] = useState<string[]>([]);
  const [submitModal, setSubmitModal] = useState(false);
  const { t } = useTranslation();
  const [opened, { open, close }] = useDisclosure(false);

  const handleIssueAll = async () => {
    try {
      await postUserData.mutateAsync({
        data: [],
        issueAll: true,
        identity: course_id,
      });
      showNotification({ message: t("certificate_issue_success_all") });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: t("error"),
        color: "red",
        message: err,
      });
    }
  };

  const handleSubmit = async (dataUser?: string[]) => {
    try {
      await postUserData.mutateAsync({
        data: dataUser?.length === 1 ? dataUser : selected,
        issueAll: false,
        identity: course_id,
      });
      showNotification({ message: t("certificate_issue_success") });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: t("error"),
        color: "red",
        message: err,
      });
    }
  };

  if (getStudentStat.isLoading) return <Loader />;

  return (
    <ScrollArea>
      <ConfirmationModal
        title={t("sure_to_issue_certificate_everyone")}
        open={openModal}
        onClose={() => setOpenModal(false)}
        onConfirm={handleIssueAll}
      />
      <ConfirmationModal
        title={t("sure_to_issue_certificate")}
        open={submitModal}
        onClose={() => setSubmitModal(false)}
        onConfirm={handleSubmit}
      />
      <Group justify="space-between" mb={"lg"}>
        <Title>{t("trainee")}</Title>
        <Flex>
          <Button mr={20} leftSection={<IconPlus size={15} />} onClick={open}>
            {t("add_trainee")}
          </Button>
          <Drawer
            opened={opened}
            onClose={close}
            title={t("add_trainee")}
            overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
          >
            <AddTrainee onCancel={close} searchParams={searchParams} />
          </Drawer>

          {getStudentStat.data && getStudentStat.data?.items.length >= 1 && (
            <Button
              onClick={() => setSubmitModal(true)}
              disabled={selected.length === 0}
              mr={20}
              loading={selected.length !== 0 && postUserData.isPending}
            >
              {t("issue_certificate")}
            </Button>
          )}

          {getStudentStat.data && getStudentStat.data?.items.length >= 1 && (
            <Button
              loading={selected.length === 0 && postUserData.isPending}
              disabled={selected.length > 0}
              onClick={() => setOpenModal(true)}
            >
              {t("issue_certificates_to_all")}
            </Button>
          )}
        </Flex>
      </Group>

      <div style={{ display: "flex" }}>
        <Box mx={3} style={{ width: "100%" }}>
          {searchComponent(t("search_trainees") as string)}
        </Box>
      </div>
      {getStudentStat.data && getStudentStat.data?.totalCount > 0 ? (
        <Paper mt={10}>
          <Table
            style={{ minWidth: 800 }}
            verticalSpacing="xs"
            striped
            highlightOnHover
            withTableBorder
            withColumnBorders
          >
            <Table.Thead>
              <Table.Tr>
                <Table.Th />
                <Table.Th>{t("name")}</Table.Th>
                <Table.Th>{t("progress")}</Table.Th>
                <Table.Th>
                  <Flex align={"center"} direction={"column"}>
                    {t("internal_certificate")}
                  </Flex>
                </Table.Th>
                <Table.Th style={{ textAlign: "center" }}>
                  {t("current_lesson")}
                </Table.Th>
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {getStudentStat.data?.items?.map((item) => (
                <Rows
                  item={item}
                  key={item?.userId}
                  selected={selected}
                  setSelected={setSelected}
                  searchParams={searchParams}
                />
              ))}
            </Table.Tbody>
          </Table>
        </Paper>
      ) : (
        <Box>{t("no_trainees")}</Box>
      )}

      {getStudentStat.data &&
        pagination(
          getStudentStat.data?.totalPage,
          getStudentStat.data.items.length
        )}
    </ScrollArea>
  );
};

export default withSearchPagination(ManageStudents);
