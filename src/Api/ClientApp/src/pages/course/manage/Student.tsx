import {
  Table,
  Anchor,
  ScrollArea,
  Badge,
  UnstyledButton,
  Box,
  Paper,
  Avatar,
  Modal,
  Image,
  Flex,
  Button,
  Group,
  Title,
  Checkbox,
  Text,
  Tooltip,
} from "@mantine/core";
import { Link, useParams } from "react-router-dom";
import ProgressBar from "@components/Ui/ProgressBar";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  IStudentStat,
  useGetStudentStatistics,
  usePostStatisticsCertificate,
} from "@utils/services/manageCourseService";
import RoutePath from "@utils/routeConstants";
import { Dispatch, SetStateAction, useState } from "react";
import ConfirmationModal from "@components/Ui/ConfirmationModal";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import moment from "moment";
import { getInitials } from "@utils/getInitialName";
import { IconDownload, IconEye } from "@tabler/icons";
import downloadImage from "@utils/downloadImage";

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

  const handleSubmit = async (dataUser: string[]) => {
    try {
      await postUserData.mutateAsync({
        data: dataUser,
        issueAll: false,
        identity: course_id,
      });
      showNotification({ message: "Certificate issued successfully." });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: "Error!",
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
    <tr key={item.userId}>
      <td>
        <Checkbox
          onChange={() => handelCheckboxChange(item?.userId)}
          checked={selected.includes(item.userId)}
        />
      </td>
      <td>
        <Anchor
          component={Link}
          to={`${RoutePath.userProfile}/${item.userId}`}
          size="sm"
          sx={{ display: "flex" }}
        >
          <Avatar
            sx={{ cursor: "pointer" }}
            size={26}
            mr={8}
            src={item?.imageUrl}
            radius={26}
          >
            {!item?.imageUrl && getInitials(item?.fullName ?? "")}
          </Avatar>

          {item?.fullName}
        </Anchor>
      </td>
      <td>
        <ProgressBar total={100} positive={item?.percentage} />
        <UnstyledButton component={Link} to={item.userId}>
          <Badge color="green" variant="outline" mt={10}>
            View
          </Badge>
        </UnstyledButton>
      </td>
      <td>
        <Flex direction={"column"} justify={"center"} align={"center"}>
          {item?.hasCertificateIssued ? (
            <div style={{ marginTop: "10px" }}>
              <Text>
                Issued on{" "}
                {moment(item?.certificateIssuedDate + "Z").format("YYYY-MM-DD")}
              </Text>
              <Flex justify={"center"} mt={8}>
                <Tooltip label="View Certificate">
                  <UnstyledButton
                    mr="sm"
                    onClick={() => {
                      window.open(item.certificateUrl);
                    }}
                  >
                    <IconEye size={23} color="green" />
                  </UnstyledButton>
                </Tooltip>
                <Tooltip label="Download Certificate">
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
              <Badge>Not Issued</Badge>
              <Button
                size="xs"
                mt={10}
                loading={postUserData.isLoading}
                onClick={() => handleSubmit([item.userId])}
              >
                Issue
              </Button>
            </>
          )}
        </Flex>
      </td>
      <td style={{ textAlign: "center" }}>
        <Anchor
          component={Link}
          to={`${RoutePath.classes}/${course_id}/${item.lessonSlug}`}
          size="sm"
        >
          {item?.lessonName}
        </Anchor>
      </td>
    </tr>
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
  const [opened, setOpened] = useState(false);
  const postUserData = usePostStatisticsCertificate(course_id, searchParams);
  const [selected, setSelected] = useState<string[]>([]);
  const [submitModal, setSubmitModal] = useState(false);

  const handleIssueAll = async () => {
    try {
      await postUserData.mutateAsync({
        data: [],
        issueAll: true,
        identity: course_id,
      });
      showNotification({ message: "Certificate issued to all successfully." });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: "Error!",
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
      showNotification({ message: "Certificate issued successfully." });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: "Error!",
        color: "red",
        message: err,
      });
    }
  };

  if (getStudentStat.data?.totalCount === 0)
    return <Box>No students found.</Box>;

  return (
    <ScrollArea>
      <ConfirmationModal
        title="Are you sure want to issue certificate to everyone?"
        open={opened}
        onClose={() => setOpened(false)}
        onConfirm={handleIssueAll}
      />
      <ConfirmationModal
        title="Are you sure want to issue certificate?"
        open={submitModal}
        onClose={() => setSubmitModal(false)}
        onConfirm={handleSubmit}
      />
      <Group position="apart" mb={"lg"}>
        <Title>Students</Title>
        <Flex>
          {getStudentStat.data && getStudentStat.data?.items.length >= 1 && (
            <Button
              onClick={() => setSubmitModal(true)}
              disabled={selected.length === 0}
              mr={20}
              loading={selected.length !== 0 && postUserData.isLoading}
            >
              Issue Certificate
            </Button>
          )}

          {getStudentStat.data && getStudentStat.data?.items.length >= 1 && (
            <Button
              loading={selected.length === 0 && postUserData.isLoading}
              disabled={selected.length > 0}
              onClick={() => setOpened(true)}
            >
              Issue Certificates to All
            </Button>
          )}
        </Flex>
      </Group>

      <div style={{ display: "flex" }}>
        <Box mx={3} sx={{ width: "100%" }}>
          {searchComponent("Search for students")}
        </Box>
      </div>
      {getStudentStat.data && getStudentStat.data?.totalCount > 0 && (
        <Paper mt={10}>
          <Table
            sx={{ minWidth: 800 }}
            verticalSpacing="xs"
            striped
            highlightOnHover
          >
            <thead>
              <tr>
                <th></th>
                <th>Name</th>
                <th>Progress</th>
                <th>
                  <Flex align={"center"} direction={"column"}>
                    Certificate
                  </Flex>
                </th>
                <th style={{ textAlign: "center" }}>Current Lesson</th>
              </tr>
            </thead>
            <tbody>
              {getStudentStat.data?.items?.map((item) => (
                <Rows
                  item={item}
                  key={item?.userId}
                  selected={selected}
                  setSelected={setSelected}
                  searchParams={searchParams}
                />
              ))}
            </tbody>
          </Table>
        </Paper>
      )}

      {getStudentStat.data && pagination(getStudentStat.data?.totalPage)}
    </ScrollArea>
  );
};

export default withSearchPagination(ManageStudents);
