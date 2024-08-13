import UserShortProfile from "@components/UserShortProfile";
import { IAuthContext } from "@context/AuthProvider";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  ActionIcon,
  Badge,
  Box,
  Button,
  Card,
  Container,
  Flex,
  Image,
  Text,
  Title,
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { IconDownload, IconEye } from "@tabler/icons-react";
import { DATE_FORMAT } from "@utils/constants";
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
import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation();
  const handleSubmit = async (approve: boolean) => {
    try {
      const status = approve
        ? CertificateStatus.Approved
        : CertificateStatus.Rejected;
      await updateStatus.mutateAsync({ id: item.id, status });
      showNotification({
        message: `${t("training")} ${approve ? t("approved") : t("rejected")
          } ${t("successfully")}`,
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
          <Text fw={"bold"}>
            {item.name}
            <Badge ml={20} c="cyan" variant="light">
              {CertificateStatus[item.status]}
            </Badge>
          </Text>
          <Text mt={5}>
            {t("from")} {moment(item.startDate).format(DATE_FORMAT)} {t("to")}{" "}
            {moment(item.endDate).format(DATE_FORMAT)} {t("completed_in_about")}{" "}
            {item.duration} {t("hrs")}
          </Text>
          <Text>
            {item.institute}
            {item.location && `, ${item.location}`}
          </Text>
          <Text>
            {item.optionalCost !== 0 && `Cost: Rs.${item.optionalCost ?? 0}`}
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
                <ActionIcon
                  variant="subtle"
                  onClick={() => window.open(item.imageUrl)}
                  mr={10}
                >
                  <IconEye color="black" />
                </ActionIcon>
                <ActionIcon
                  variant="subtle"
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
        Number(auth.auth.role) <= UserRole.Admin &&
        auth.auth.id !== item.user.id && (
          <Box mt={10}>
            <Button
              loading={updateStatus.isPending}
              onClick={() => handleSubmit(true)}
            >
              {t("approve")}
            </Button>
            <Button
              ml={10}
              variant="outline"
              color={"red"}
              onClick={() => handleSubmit(false)}
              loading={updateStatus.isPending}
            >
              {t("reject")}
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
  const { t } = useTranslation();
  return (
    <Container fluid>
      <Title>{t("external_trainings_list")}</Title>
      {/* {searchComponent("Search for trainings")} */}

      {listCertificate.isSuccess &&
        listCertificate.data.data.items.map((x) => (
          <CertificateCard
            key={x.id}
            auth={auth}
            item={x}
            search={searchParams}
          />
        ))}
      {listCertificate.isSuccess &&
        pagination(
          listCertificate.data.data.totalPage,
          listCertificate.data.data.items.length
        )}

      {listCertificate.isSuccess &&
        listCertificate.data?.data.totalCount < 1 && (
          <Box>{t("no_external_trainings")}</Box>
        )}
    </Container>
  );
};

export default withSearchPagination(CertificateList);
