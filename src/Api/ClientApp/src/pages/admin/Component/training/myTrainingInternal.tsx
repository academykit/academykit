import { Box, Card, Container, Flex, Image, Text } from "@mantine/core";
import React from "react";
import { Route, Routes, useLocation, useNavigate } from "react-router";

const MyTrainingInternal = () => {
  return (
    <Container fluid>
      <Card withBorder>
        <Flex justify={"space-between"}>
          <Box>
            <Text weight={"bold"}>Name:</Text>
            <Text weight={"bold"}>Duration:</Text>
          </Box>
          <Box style={{ width: 150 }}>
            <Image
              src={
                "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/Image_created_with_a_mobile_phone.png/330px-Image_created_with_a_mobile_phone.png"
              }
            />
          </Box>
        </Flex>
      </Card>
    </Container>
  );
};

export default MyTrainingInternal;
