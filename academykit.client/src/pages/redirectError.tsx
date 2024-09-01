import {
  Anchor,
  Button,
  Center,
  Container,
  Group,
  Text,
  Title,
} from "@mantine/core";
import { t } from "i18next";
import { useState } from "react";
import classes from "./styles/error.module.css";

function RedirectError() {
  const [seeMore, setSeeMore] = useState(false);

  const queryParams = new URLSearchParams(location.search);
  const error = decodeURIComponent(queryParams.get("error") ?? "");
  const details = decodeURIComponent(queryParams.get("details") ?? "");

  return (
    <div>
      <Container className={classes.root}>
        <div style={{ fontSize: "150px" }} className={classes.label}>
          {t("error")}
        </div>
        <Title className={classes.title}>{error}</Title>
        {seeMore && (
          <Text
            c="dimmed"
            size="lg"
            ta="center"
            className={classes.description}
          >
            <p> {details}</p>
          </Text>
        )}
        {!seeMore && (
          <Center mt={20}>
            <Button
              variant="gradient"
              onClick={() => setSeeMore(true)}
              size="md"
            >
              {t("see_more")}
            </Button>
          </Center>
        )}

        <Group mt={50} justify="center">
          <Anchor href="/">
            <Button variant="subtle" size="md">
              {t("back_to_home")}
            </Button>
          </Anchor>
        </Group>
      </Container>
    </div>
  );
}

export default RedirectError;
