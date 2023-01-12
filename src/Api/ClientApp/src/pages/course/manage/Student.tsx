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
import { useState } from "react";
import ConfirmationModal from "@components/Ui/ConfirmationModal";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import moment from "moment";
import { getInitials } from "@utils/getInitialName";

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

  const Rows = ({ item }: { item: IStudentStat }) => {
    const [openImage, setOpenImage] = useState(false);
    const { id } = useParams();
    const course_id = id as string;

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
        </td>
        <td>
          <Flex direction={"column"} justify={"center"} align={"center"}>
            {item?.hasCertificateIssued ? (
              <Badge maw={"60px"}>Yes</Badge>
            ) : (
              <Badge maw={"60px"}>No</Badge>
            )}
            <div style={{ marginTop: "10px" }}>
              {item?.certificateIssuedDate
                ? moment
                    .utc(item?.certificateIssuedDate)
                    .format("YYYY-MM-DD HH:mm:ss")
                : ""}
            </div>
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
        <td>
          <Modal
            opened={openImage}
            size="xl"
            title={item?.fullName}
            onClose={() => setOpenImage(false)}
          >
            <Image src={item?.certificateUrl}></Image>
          </Modal>
          {item?.certificateUrl ? (
            <Anchor onClick={() => setOpenImage((v) => !v)}>
              <Image
                width={150}
                height={100}
                fit="contain"
                src={item?.certificateUrl}
              />
            </Anchor>
          ) : (
            ""
          )}
        </td>
        <td>
          <UnstyledButton component={Link} to={item.userId}>
            <Badge color="green" variant="outline">
              View
            </Badge>
          </UnstyledButton>
        </td>
      </tr>
    );
  };

  const handleIssueAll = async () => {
    try {
      await postUserData.mutateAsync({
        data: [],
        issueAll: true,
        identity: course_id,
      });
      showNotification({ message: "Certificate sent successfully" });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: "Error!",
        color: "red",
        message: err,
      });
    }
  };

  const handleSubmit = async () => {
    try {
      await postUserData.mutateAsync({
        data: selected,
        issueAll: false,
        identity: course_id,
      });
      showNotification({ message: "Certificate sent successfully" });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: "Error!",
        color: "red",
        message: err,
      });
    }
  };

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
        <Title>Student Statistics</Title>
        {getStudentStat.data && getStudentStat.data?.items.length >= 1 && (
          <Button onClick={() => setOpened(true)}>Issue All</Button>
        )}
      </Group>

      <div style={{ display: "flex" }}>
        <Box mx={3} sx={{ width: "100%" }}>
          {searchComponent("Search for students")}
        </Box>
      </div>
      {getStudentStat.data && getStudentStat.data?.totalCount > 0 ? (
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
                    <div>(isIssued)</div>
                    <div>(IssuedDate)</div>
                  </Flex>
                </th>
                <th style={{ textAlign: "center" }}>Current Lesson</th>
                <th>Url</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {getStudentStat.data?.items?.map((item: any) => (
                <Rows item={item} key={item?.userId} />
              ))}
            </tbody>
          </Table>
        </Paper>
      ) : (
        <Box mt={5}>No Students Found</Box>
      )}
      {getStudentStat.data && getStudentStat.data?.items.length >= 1 && (
        <Group position="left">
          <Button
            mt={10}
            onClick={() => setSubmitModal(true)}
            disabled={selected.length === 0}
          >
            Issue Certificate
          </Button>
        </Group>
      )}

      {getStudentStat.data && pagination(getStudentStat.data?.totalPage)}
    </ScrollArea>
  );
};

export default withSearchPagination(ManageStudents);
