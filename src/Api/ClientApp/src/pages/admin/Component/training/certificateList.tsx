import {
  Card,
  Container,
  Text,
  Title,
  Box,
  Flex,
  Button,
  Group,
} from "@mantine/core";
import { useGetListCertificate } from "@utils/services/certificateService";
import moment from "moment";
import React from "react";

const CertificateList = () => {
  const listCertificate = useGetListCertificate("");
  return (
    <Container fluid>
      <Title>List of External Trainings.</Title>
      {listCertificate.isSuccess &&
        listCertificate.data.data.items.map((x) => (
          <Card withBorder mt={10} key={x.id}>
            <Group position="apart">
              <Flex justify={"space-between"}>
                <Box>
                  <Text weight={"bold"}>
                    Training Name: {x.cerificateName}{" "}
                  </Text>
                  <Text weight={"bold"}>
                    Date: {moment(x.startDate).format("YYYY-MM-DD")} to{" "}
                    {moment(x.endDate).format("YYYY-MM-DD")}
                  </Text>
                  <Text weight={"bold"}>User: {x.userName}</Text>
                </Box>
              </Flex>
              <Flex direction="column">
                <Button mb={10}>View Training Details</Button>
                {/* <Button>View User's Trainings</Button> */}
              </Flex>
            </Group>
            {/* {auth?.auth && auth.auth.role <= UserRole.Admin && (
              <Box mt={10}>
                <Button>Approve</Button>
                <Button ml={10} variant="outline" color={"red"}>
                  Reject
                </Button>
              </Box>
            )} */}
          </Card>
        ))}
    </Container>
  );
};

export default CertificateList;
