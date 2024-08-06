import { Anchor, Button, Container, Group, Text, Title } from "@mantine/core";
import { useTranslation } from "react-i18next";
import classes from "./styles/error.module.css";

const NotFound = () => {
  const { t } = useTranslation();
  return (
    <Container className={classes.root}>
      <div className={classes.label}>{t("404")}</div>
      <Title className={classes.title}>{t("found_a_secret_place")}</Title>
      <Text c="dimmed" size="lg" ta="center" className={classes.description}>
        {t("mistyped_or_moved")}{" "}
      </Text>
      <Group justify="center">
        <Anchor href="/">
          <Button variant="subtle" size="md">
            {t("back_to_home")}
          </Button>
        </Anchor>
      </Group>
    </Container>
  );
};

export default NotFound;
