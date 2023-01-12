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

const Rows = ({ item }: { item: IStudentStat }) => {
  const [openImage, setOpenImage] = useState(false);
  const { id } = useParams();
  const course_id = id as string;
  const [selected, setSelected] = useState<string[]>([]);

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
        {" "}
        <Checkbox
        // onChange={() => handelCheckboxChange(item.user.id)}
        // checked={selected.includes(item.user.id)}
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
            size={26}
            mr={8}
            src={
              "https://d2j66931zkyzgl.cloudfront.net/standalone/2eb8a5be-218a-43a6-b131-17531d669064.png"
            }
            radius={26}
          />

          {item?.fullName}
        </Anchor>
      </td>
      <td>
        <ProgressBar total={100} positive={item?.percentage} />
      </td>
      <td>
        <Flex direction={"column"}>
          {item?.hasCertificateIssued ? <Badge>Yes</Badge> : <Badge>No</Badge>}
          <div>{item?.certificateIssueDate}</div>
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
          // title={item?.user?.fullName}
          onClose={() => setOpenImage(false)}
        >
          <Image src={item?.certificateUrl}></Image>
        </Modal>
        <Anchor onClick={() => setOpenImage((v) => !v)}>
          <Image src={item?.certificateUrl} />
        </Anchor>
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

  return (
    <ScrollArea>
      <ConfirmationModal
        title="Are you sure want to issue certificate to everyone?"
        open={opened}
        onClose={() => setOpened(false)}
        onConfirm={handleIssueAll}
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
                    <div>isIssued</div>
                    <div></div>IssuedDate
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
      {getStudentStat.data && pagination(getStudentStat.data?.totalPage)}
    </ScrollArea>
  );
};

export default withSearchPagination(ManageStudents);
