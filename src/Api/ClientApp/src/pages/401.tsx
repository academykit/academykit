import {
  createStyles,
  Title,
  Text,
  Button,
  Container,
  Group,
  Anchor,
} from "@mantine/core";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const useStyles = createStyles((theme) => ({
  root: {
    paddingTop: 80,
    paddingBottom: 80,
  },

  label: {
    textAlign: "center",
    fontWeight: 900,
    fontSize: 220,
    lineHeight: 1,
    marginBottom: theme.spacing.xl * 1.5,
    color:
      theme.colorScheme === "dark"
        ? theme.colors.dark[4]
        : theme.colors.gray[2],

    [theme.fn.smallerThan("sm")]: {
      fontSize: 120,
    },
  },

  title: {
    fontFamily: `Greycliff CF, ${theme.fontFamily}`,
    textAlign: "center",
    fontWeight: 900,
    fontSize: 38,

    [theme.fn.smallerThan("sm")]: {
      fontSize: 32,
    },
  },

  description: {
    maxWidth: 500,
    margin: "auto",
    marginTop: theme.spacing.xl,
    marginBottom: theme.spacing.xl * 1.5,
  },
}));

const UnAuthorize = () => {
  const { classes } = useStyles();
  const { t } = useTranslation();
  return (
    <Container className={classes.root}>
      <div className={classes.label}>{t("401")}</div>
      <Title className={classes.title}>{t("not_authorized")}</Title>
      <Text
        color="dimmed"
        size="lg"
        align="center"
        className={classes.description}
      >
        {/* Unfortunately, this is only a 404 page. You may have mistyped the
        address, or the page has been moved to another URL. */}
      </Text>
      <Group position="center">
        <Anchor href={"/"}>
          <Button variant="subtle" size="md">
            {t("back_to_home")}
          </Button>
        </Anchor>
      </Group>
    </Container>
  );
};

export default UnAuthorize;
