import {
  ActionIcon,
  Anchor,
  Badge,
  CopyButton,
  Flex,
  Image,
  Box,
  Paper,
  ScrollArea,
  Table,
  Title,
  Tooltip,
  useMantineTheme,
  Text,
} from "@mantine/core";
import { IconCheck, IconCopy, IconDownload, IconEye } from "@tabler/icons";
import {
  GetExternalCertificate,
  useGetUserCertificate,
} from "@utils/services/certificateService";
import moment from "moment";
import { useState } from "react";
import { useParams } from "react-router-dom";
import downloadImage from "@utils/downloadImage";
import { TFunction } from "i18next";
import { useTranslation } from "react-i18next";
import { DATE_FORMAT } from "@utils/constants";

const RowsExternal = ({
  item,
  t,
}: {
  item: GetExternalCertificate;
  t: TFunction;
}) => {
  const theme = useMantineTheme();
  const [opened, setOpened] = useState(false);
  const handleDownload = () => {
    window.open(item?.imageUrl);
  };
  return (
    <tr key={item?.user?.id}>
      <td>{item?.name}</td>

      <td>
        {item?.status === 2 ? (
          <Badge>{t("yes")}</Badge>
        ) : (
          <Badge>{t("No")}</Badge>
        )}
      </td>
      <td>{moment(item?.startDate).format(DATE_FORMAT)}</td>
      <td>{moment(item?.endDate).format(DATE_FORMAT)}</td>
      <td>{item?.duration} Hour(s)</td>
      <td>{item?.institute}</td>
      <td style={{ wordBreak: "break-all" }}>{item.location}</td>
      <td>
        <Box style={{ width: 150, marginTop: "auto", marginBottom: "auto" }}>
          {item?.imageUrl ? (
            <div style={{ position: "relative" }}>
              <Image
                width={150}
                height={100}
                fit="contain"
                sx={{ opacity: "0.5" }}
                src={item?.imageUrl}
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
                <Tooltip label={t("view_certificate")}>
                  <ActionIcon
                    onClick={() => window.open(item?.imageUrl)}
                    mr={10}
                  >
                    <IconEye color="black" />
                  </ActionIcon>
                </Tooltip>
                <Tooltip label={t("download_certificate")}>
                  <ActionIcon
                    onClick={() =>
                      downloadImage(item?.imageUrl, item?.user?.fullName ?? "")
                    }
                  >
                    <IconDownload color="black" />
                  </ActionIcon>
                </Tooltip>
              </div>
            </div>
          ) : (
            <Text>{t("no_certificate")}</Text>
          )}
        </Box>
      </td>
    </tr>
  );
};
const ExternalCertificate = () => {
  const { id } = useParams();
  const externalCertificate = useGetUserCertificate(id as string);
  const { t } = useTranslation();

  return (
    <>
      {externalCertificate.data && externalCertificate.data.length > 0 && (
        <>
          <Title mt={"xl"}>{t("external_certificate")}</Title>
          <ScrollArea>
            <Paper mt={10}>
              <Table
                sx={{ minWidth: 800 }}
                verticalSpacing="sm"
                striped
                highlightOnHover
                withBorder
              >
                <thead>
                  <tr>
                    <th>{t("training_name")}</th>
                    <th>{t("verified")}</th>
                    <th>{t("start_date")}</th>
                    <th>{t("end_date")}</th>
                    <th>{t("duration")}</th>
                    <th>{t("issued_by")}</th>
                    <th>{t("issuer_location")}</th>
                    <th>{t("external_certificate")}</th>
                  </tr>
                </thead>
                <tbody>
                  {externalCertificate?.data.map((x: any) => (
                    <RowsExternal key={x.id} item={x} t={t} />
                  ))}
                </tbody>
              </Table>
            </Paper>
          </ScrollArea>
        </>
      )}
    </>
  );
};

export default ExternalCertificate;
