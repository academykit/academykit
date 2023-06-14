import {
  Box,
  Card,
  Container,
  Flex,
  Image,
  Text,
  ActionIcon,
  Tooltip,
  useMantineTheme,
} from "@mantine/core";
import { IconDownload, IconEye } from "@tabler/icons";
import downloadImage from "@utils/downloadImage";
import { useGetInternalCertificate } from "@utils/services/certificateService";
import moment from "moment";
import React from "react";
import { useTranslation } from "react-i18next";
import { Route, Routes, useLocation, useNavigate } from "react-router";

const MyTrainingInternal = () => {
  const internal = useGetInternalCertificate();
  const theme = useMantineTheme();
  const { t } = useTranslation();
  return (
    <Container fluid>
      {internal.isSuccess && internal.data.data.length <= 0 && (
        <Box>{t("no_certificates")} </Box>
      )}
      {internal.isSuccess &&
        internal.data.data.map((x) => (
          <Card withBorder mt={10}>
            <Flex justify={"space-between"}>
              <Box>
                <Text weight={"bold"}>{x.courseName}</Text>
                <Text weight={"bold"}>
                  {t("certificate_issue_date")}{" "}
                  {moment(x.certificateIssuedDate).format(theme.dateFormat)}
                </Text>
                <Text>
                  {t("total")} {x.percentage}
                  {t("percent_completed")}
                </Text>
              </Box>
              <Box
                style={{ width: 150, marginTop: "auto", marginBottom: "auto" }}
              >
                {x.certificateUrl && (
                  <div style={{ position: "relative" }}>
                    <Image
                      src={x.certificateUrl || ""}
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
                      <Tooltip label={t("view_certificate")}>
                        <ActionIcon
                          onClick={() => window.open(x.certificateUrl)}
                          mr={10}
                        >
                          <IconEye color="black" />
                        </ActionIcon>
                      </Tooltip>
                      <Tooltip label={t("download_certificate")}>
                        <ActionIcon
                          onClick={() => {
                            downloadImage(
                              x?.certificateUrl ?? "",
                              x?.courseName ?? ""
                            );
                          }}
                        >
                          <IconDownload color="black" />
                        </ActionIcon>
                      </Tooltip>
                    </div>
                  </div>
                )}
              </Box>
            </Flex>
          </Card>
        ))}
    </Container>
  );
};

export default MyTrainingInternal;
