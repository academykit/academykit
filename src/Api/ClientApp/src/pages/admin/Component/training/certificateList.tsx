import useAuth from "@hooks/useAuth";
import {
  Card,
  Container,
  Text,
  Title,
  Box,
  Flex,
  Button,
  Group,
  Image,
  ActionIcon,
} from "@mantine/core";
import { IconDownload, IconEye } from "@tabler/icons";
import downloadImage from "@utils/downloadImage";
import { UserRole } from "@utils/enums";
import { useGetListCertificate } from "@utils/services/certificateService";
import moment from "moment";
import React from "react";

const CertificateList = () => {
  const listCertificate = useGetListCertificate("");
  const auth = useAuth();
  return (
    <Container fluid>
      <Title>List of External Trainings.</Title>
      {listCertificate.isSuccess &&
        listCertificate.data.data.items.map((x) => (
          <Card withBorder mt={10}>
            <Flex justify={"space-between"}>
              <Box>
                <Text weight={"bold"}>Name: {x.name} </Text>
                <Text weight={"bold"}>Duration: {x.duration}</Text>
                <Text weight={"bold"}>
                  Date: {moment(x.startDate).format("YYYY-MM-DD")} to{" "}
                  {moment(x.endDate).format("YYYY-MM-DD")}
                </Text>
                <Text weight={"bold"}>Location: {x.location}</Text>
                <Text weight={"bold"}>Institute: {x.institute}</Text>
              </Box>
              <Box
                style={{ width: 150, marginTop: "auto", marginBottom: "auto" }}
              >
                <div style={{ position: "relative" }}>
                  <Image
                    src={x.imageUrl || ""}
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
                    <ActionIcon onClick={() => window.open(x.imageUrl)} mr={10}>
                      <IconEye color="black" />
                    </ActionIcon>
                    <ActionIcon
                      onClick={() =>
                        downloadImage(x.imageUrl, x.userName ?? "")
                      }
                    >
                      <IconDownload color="black" />
                    </ActionIcon>
                  </div>
                </div>
              </Box>
            </Flex>
            {auth?.auth && auth.auth.role <= UserRole.Admin && (
              <Box mt={10}>
                <Button>Approve</Button>
                <Button ml={10} variant="outline" color={"red"}>
                  Reject
                </Button>
              </Box>
            )}
          </Card>
        ))}
    </Container>
  );
};

export default CertificateList;
