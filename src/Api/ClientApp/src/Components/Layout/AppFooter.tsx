import {
  createStyles,
  Text,
  Container,
  Group,
  Anchor,
  Divider,
  Center,
} from "@mantine/core";

import { Link, useLocation } from "react-router-dom";
import { ColorSchemeToggle } from "./ColorSchemeToggle";
import LanguageSelector from "@components/Ui/LanguageSelector";
import { useTranslation } from "react-i18next";

const useStyles = createStyles((theme) => ({
  footer: {
    borderTop: `1px solid ${
      theme.colorScheme === "dark" ? theme.colors.dark[5] : theme.colors.gray[2]
    }`,
    paddingLeft: 310,
    [theme.fn.smallerThan("sm")]: {
      paddingLeft: 10,
    },
    paddingRight: 10,
    zIndex: 1000,
  },

  inner: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    paddingTop: theme.spacing.sm,
    paddingBottom: theme.spacing.sm,

    [theme.fn.smallerThan("xs")]: {
      flexDirection: "column",
    },
  },

  links: {
    [theme.fn.smallerThan("xs")]: {
      marginTop: theme.spacing.md,
    },
  },
}));

export function AppFooter({ name }: { name: string }) {
  const { classes } = useStyles();
  const location = useLocation();
  const { t } = useTranslation();

  if (location.pathname.split("/")[1] === "exam") return <></>;

  return (
    <footer className={classes.footer}>
      <Container fluid={true} className={classes.inner}>
        <Text color="dimmed" size="xs">
          {t("copyright")} © {new Date().getFullYear()} {name}.
        </Text>
        <Group>
          <Anchor size={"xs"} component={Link} to="/privacy" color={"dimmed"}>
            {t("privacy")}
          </Anchor>
          <Divider orientation="vertical" />
          <Anchor size={"xs"} component={Link} to={"/terms"} color={"dimmed"}>
            {t("terms")}
          </Anchor>
          <Divider orientation="vertical" />
          <Anchor size={"xs"} component={Link} to="/about" color={"dimmed"}>
            {t("about_us")}
          </Anchor>
        </Group>
        <Group spacing={0} position="right" noWrap>
          <LanguageSelector />
          <ColorSchemeToggle size="lg" />
        </Group>
      </Container>
      <Center>
        <Text size={"xs"} color={"dimmed"} mr={3}>
          {t("powered_by")}
        </Text>
        <Anchor href={"https://vurilo.com"} style={{ textDecoration: "none" }}>
          <Text size={"xs"} color={"dimmed"}>
            Vurilo
          </Text>
        </Anchor>
      </Center>
    </footer>
  );
}
