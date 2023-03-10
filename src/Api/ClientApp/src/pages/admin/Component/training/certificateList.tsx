import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import { IAuthContext } from "@context/AuthProvider";
import useAuth from "@hooks/useAuth";
import {
  Card,
  Container,
  Text,
  Title,
  Box,
  Flex,
  Button,
  Badge,
  Image,
  ActionIcon,
  useMantineTheme,
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { IconDownload, IconEye } from "@tabler/icons";
import downloadImage from "@utils/downloadImage";
import { UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  CertificateStatus,
  ListCertificate,
  useGetListCertificate,
  useUpdateCertificateStatus,
} from "@utils/services/certificateService";
import moment from "moment";
import React from "react";
import UserShortProfile from "@components/UserShortProfile";

const CertificateCard = ({
  auth,
  item,
  search,
}: {
  auth: IAuthContext | null;
  item: ListCertificate;
  search: string;
}) => {
  const updateStatus = useUpdateCertificateStatus(item.id, search);
  const theme = useMantineTheme();

  const handleSubmit = async (approve: boolean) => {
    try {
      const status = approve
        ? CertificateStatus.Approved
        : CertificateStatus.Rejected;
      await updateStatus.mutateAsync({ id: item.id, status });
      showNotification({
        message: `Training ${approve ? "approved" : "rejected"} successfully.`,
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    }
  };

  return (
    <Card withBorder mt={10}>
      <Flex justify={"space-between"}>
        <Box>
          <Text weight={"bold"}>
            {item.name}
            <Badge ml={20}>{CertificateStatus[item.status]}</Badge>
          </Text>
          <Text mt={5}>
            From {moment(item.startDate).format(theme.dateFormat)} to{" "}
            {moment(item.endDate).format(theme.dateFormat)}, Completed in about{" "}
            {item.duration} hrs.
          </Text>
          <Text>
            {item.institute}
            {item.location && `, ${item.location}`}
          </Text>
          <UserShortProfile size={"sm"} user={item.user} />
        </Box>
        <Box style={{ width: 150, marginTop: "auto", marginBottom: "auto" }}>
          {item.imageUrl && (
            <div style={{ position: "relative" }}>
              <Image
                src={item.imageUrl || ""}
                radius="md"
                style={{
                  opacity: "0.5",
                }}
              />
              <div
                style={{
                  position: "absolute",
                  left: 0,
                  bottom: 0,
                  right: 0,
                  margin: "auto",
                  top: 0,
                  width: "45px",
                  height: "30px",
                  display: "flex",
                }}
              >
                <ActionIcon onClick={() => window.open(item.imageUrl)} mr={10}>
                  <IconEye color="black" />
                </ActionIcon>
                <ActionIcon
                  onClick={() =>
                    downloadImage(item.imageUrl, item.user.fullName ?? "")
                  }
                >
                  <IconDownload color="black" />
                </ActionIcon>
              </div>
            </div>
          )}
        </Box>
      </Flex>
      {auth?.auth &&
        auth.auth.role <= UserRole.Admin &&
        auth.auth.id !== item.user.id && (
          <Box mt={10}>
            <Button
              loading={updateStatus.isLoading}
              onClick={() => handleSubmit(true)}
            >
              Approve
            </Button>
            <Button
              ml={10}
              variant="outline"
              color={"red"}
              onClick={() => handleSubmit(false)}
              loading={updateStatus.isLoading}
            >
              Reject
            </Button>
          </Box>
        )}
    </Card>
  );
};

const CertificateList = ({
  searchParams,
  // searchComponent,
  pagination,
}: IWithSearchPagination) => {
  const listCertificate = useGetListCertificate(searchParams);
  const auth = useAuth();
  return (
    <Container fluid>
      <Title>List of External Trainings</Title>
      {/* {searchComponent("Search for trainings")} */}

      {listCertificate.isSuccess &&
        listCertificate.data.data.items.map((x) => (
          <CertificateCard auth={auth} item={x} search={searchParams} />
        ))}
      {listCertificate.isSuccess &&
        pagination(listCertificate.data.data.totalPage)}

      {listCertificate.isSuccess &&
        listCertificate.data?.data.totalCount < 1 && (
          <Box>No External trainings found!</Box>
        )}
    </Container>
  );
};

export default withSearchPagination(CertificateList);
