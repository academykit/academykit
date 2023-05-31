import {
  createStyles,
  Title,
  Text,
  Button,
  Container,
  Group,
} from "@mantine/core";
import { useTranslation } from "react-i18next";

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
    color: theme.colors[theme.primaryColor][3],

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

  return (
    <div className={classes.root}>
      <Container>
        <div className={classes.label}>{t("500")}</div>
        <Title className={classes.title}>{t("something_bad")}</Title>
        <Text size="lg" align="center" className={classes.description}>
          {t("server_error")}
        </Text>
        <Group position="center">
          <Button
            variant="white"
            size="md"
            onClick={() => window.location.reload()}
          >
            {t("refresh_page")}
          </Button>
        </Group>
      </Container>
    </div>
  );
};

export default ServerError;
