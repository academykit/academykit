import {
  Box,
  Card,
  Container,
  Flex,
  Image,
  Text,
  ActionIcon,
} from "@mantine/core";
import { IconDownload, IconEye } from "@tabler/icons";
import downloadImage from "@utils/downloadImage";
import { useGetInternalCertificate } from "@utils/services/certificateService";
import moment from "moment";
import React from "react";
import { Route, Routes, useLocation, useNavigate } from "react-router";

const MyTrainingInternal = () => {
  const internal = useGetInternalCertificate();

  return (
    <Container fluid>
      {internal.isSuccess &&
        internal.data.data.map((x) => (
          <Card withBorder mt={10}>
            <Flex justify={"space-between"}>
              <Box>
                <Text weight={"bold"}>{x.courseName}</Text>
                <Text weight={"bold"}>
                  Certificate Issued Date:{" "}
                  {moment(x.certificateIssuedDate).format("YYYY-MM-DD")}
                </Text>
                <Text>Total {x.percentage}% completed</Text>
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
                      <ActionIcon
                        onClick={() => window.open(x.certificateUrl)}
                        mr={10}
                      >
                        <IconEye color="black" />
                      </ActionIcon>
                      <ActionIcon
                        onClick={() =>
                          downloadImage(x.certificateUrl, x.user.fullName ?? "")
                        }
                      >
                        <IconDownload color="black" />
                      </ActionIcon>
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
