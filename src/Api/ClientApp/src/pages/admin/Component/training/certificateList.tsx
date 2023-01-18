import { Card, Container, Text, Title } from "@mantine/core";
import { useGetListCertificate } from "@utils/services/certificateService";
import React from "react";

const CertificateList = () => {
  const listCertificate = useGetListCertificate("");
  return (
    <Container fluid>
      <Title>List of External Trainings.</Title>
      {listCertificate.isSuccess &&
        listCertificate.data.data.items.map((x) => (
          <Card key={x.id}>
            <Text>{x.cerificateName}</Text>
          </Card>
        ))}
    </Container>
  );
};

export default CertificateList;
