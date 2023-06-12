import {
  createStyles,
  Title,
  Text,
  Button,
  Container,
  Group,
} from "@mantine/core";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useLocation } from "react-router-dom";

const useStyles = createStyles((theme) => ({
  root: {
    paddingTop: 80,
    paddingBottom: 120,
    backgroundColor: theme.fn.variant({
      variant: "filled",
      color: theme.primaryColor,
    }).background,
  },

  label: {
    textAlign: "center",
    fontWeight: 900,
    fontSize: 220,
    lineHeight: 1,
    marginBottom: theme.spacing.xl * 1.5,
    color: theme.colors[theme.primaryColor][0],

    [theme.fn.smallerThan("sm")]: {
      fontSize: 120,
    },
  },

  title: {
    fontFamily: `Greycliff CF, ${theme.fontFamily}`,
    textAlign: "center",
    fontWeight: 900,
    fontSize: 38,
    color: theme.white,

    [theme.fn.smallerThan("sm")]: {
      fontSize: 32,
    },
  },

  description: {
    maxWidth: 540,
    margin: "auto",
    marginTop: theme.spacing.xl,
    marginBottom: theme.spacing.xl * 1.5,
    color: theme.colors[theme.primaryColor][1],
  },
}));

const ServerError = () => {
  const { classes } = useStyles();
  const { t } = useTranslation();
  const [init, setInit] = useState(false);
  const location = useLocation();

  useEffect(() => {
    if (init) {
      window.location.reload();
    }
    setInit(true);
  }, [location.pathname]);

  return (
    <div className={classes.root}>
      <Container>
        <div className={classes.label}>{t("500")}</div>
        <Title className={classes.title}>{t("something_bad")}</Title>
        <Text size="lg" align="center" className={classes.description}>
          {t("server_error")}
        </Text>
        <Group position="center" sx={{ flexDirection: "column" }}>
          <Button
            variant="white"
            size="md"
            onClick={() => window.location.reload()}
          >
            {t("refresh_page")}
          </Button>
          <Button variant="white" size="md" component={Link} to={"/"}>
            {t("back_to_home")}
          </Button>
        </Group>
      </Container>
    </div>
  );
};

export default ServerError;
