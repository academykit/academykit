import { Anchor, Button, Container, Group, Text, Title } from "@mantine/core";
import { t } from "i18next";
import classes from "../styles/error.module.css";

function OAuthErrorPage() {
  const queryParams = new URLSearchParams(location.search);
  const provider = queryParams.get("provider");
  const error = decodeURIComponent(queryParams.get("error") ?? "");
  const details = decodeURIComponent(queryParams.get("details") ?? "");

  return (
    <div>
      <Container className={classes.root}>
        <div className={classes.label}>{provider}</div>
        <Title className={classes.title}>{error}</Title>
        <Text c="dimmed" size="lg" ta="center" className={classes.description}>
          <p> {details}</p>
        </Text>
        <Group justify="center">
          <Anchor href="/">
            <Button variant="subtle" size="md">
              {t("go_back_login")}
            </Button>
          </Anchor>
        </Group>
      </Container>
    </div>
  );
}

export default OAuthErrorPage;
