import ConfirmationModal from "@components/Ui/ConfirmationModal";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  Anchor,
  Badge,
  Box,
  Button,
  Checkbox,
  Group,
  Image,
  Modal,
  Paper,
  ScrollArea,
  Table,
  Title,
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  ICertificateList,
  useGetStudentStatisticsCertificate,
  usePostStatisticsCertificate,
} from "@utils/services/manageCourseService";
import { useState } from "react";
import { useParams } from "react-router-dom";

const CertificateCourse = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const course_id = id as string;
  const [selected, setSelected] = useState<string[]>([]);
  const postUserData = usePostStatisticsCertificate(course_id, searchParams);

  const getStudentCertificate = useGetStudentStatisticsCertificate(
    course_id,
    searchParams
  );

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

  const handelCheckboxChange = (userId: string) => {
    if (selected.includes(userId)) {
      setSelected(selected.filter((x) => x !== userId));
    } else {
      setSelected([userId, ...selected]);
    }
  };

  const [opened, setOpened] = useState(false);
  const [openImage, setOpenImage] = useState(false);
  const [submitModal, setSubmitModal] = useState(false);
  const RowsCompleted = ({ item }: { item: ICertificateList }) => {
    return (
      <tr key={item?.user?.id}>
        <td>
          <Checkbox
            onChange={() => handelCheckboxChange(item.user.id)}
            checked={selected.includes(item.user.id)}
          />
        </td>
        <td>{item?.certificateIssuedDate}</td>
        <td>{item?.user?.fullName}</td>

        <td>{item?.percentage}%</td>
        <td>
          {item?.hasCertificateIssued ? <Badge>Yes</Badge> : <Badge>No</Badge>}
        </td>
        <td style={{ maxWidth: "0px" }}>
          <Modal
            opened={openImage}
            size="xl"
            title={item?.user?.fullName}
            onClose={() => setOpenImage(false)}
          >
            <Image src={item?.certificateUrl}></Image>
          </Modal>
          <Anchor onClick={() => setOpenImage((v) => !v)}>
            <Image src={item?.certificateUrl} />
          </Anchor>
        </td>
      </tr>
    );
  };

  return (
    <>
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

      <Group position="apart" mb={10}>
        <Title>Certificate</Title>

        {getStudentCertificate.data &&
          getStudentCertificate.data?.items.length >= 1 && (
            <Button onClick={() => setOpened(true)}>Issue All</Button>
          )}
      </Group>
      {searchComponent("Search for certificates")}

      {getStudentCertificate.data?.items?.length === 0 ? (
        <Box mt={10}>No Certificate Found</Box>
      ) : (
        <>
          <ScrollArea>
            <Paper mt={10}>
              <Table
                sx={{ minWidth: 800 }}
                verticalSpacing="sm"
                striped
                highlightOnHover
              >
                <thead>
                  <tr>
                    <th></th>
                    <th>Issued Date</th>
                    <th>Name</th>
                    <th>Completion</th>
                    <th>isIssued</th>
                    <th>URL</th>
                  </tr>
                </thead>
                <tbody>
                  {getStudentCertificate.data &&
                    getStudentCertificate.data?.items.length > 0 &&
                    getStudentCertificate.data?.items.map((x: any) => (
                      <RowsCompleted key={x.userId} item={x} />
                    ))}
                </tbody>
              </Table>
            </Paper>
          </ScrollArea>
          <Group position="left">
            <Button
              mt={10}
              onClick={() => setSubmitModal(true)}
              disabled={selected.length === 0}
            >
              Submit
            </Button>
          </Group>
        </>
      )}
      {getStudentCertificate.data &&
        getStudentCertificate.data?.totalPage > 1 &&
        pagination(getStudentCertificate.data?.totalPage)}
    </>
  );
};

export default withSearchPagination(CertificateCourse);
