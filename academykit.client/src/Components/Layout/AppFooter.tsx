import { Anchor, Center, Container, Group, Text } from "@mantine/core";
import { useTranslation } from "react-i18next";
import { useLocation } from "react-router-dom";
import { ColorSchemeToggle } from "./ColorSchemeToggle";
import classes from "./styles/footer.module.css";

export function AppFooter({ name }: { name: string }) {
  const location = useLocation();
  const { t } = useTranslation();
  const appVersion = localStorage.getItem("version");
  if (location.pathname.split("/")[1] === "exam") return <></>;

  return (
    <footer className={classes.footer}>
      <Container fluid={true} className={classes.inner}>
        <Text c="dimmed" size="xs">
          {t("copyright")} © {new Date().getFullYear()} {name}.
        </Text>
        {/* <Group>
          <Anchor size={'xs'} component={Link} to="/privacy" c={'dimmed'}>
            {t('privacy')}
          </Anchor>
          <Divider orientation="vertical" />
          <Anchor size={'xs'} component={Link} to={'/terms'} c={'dimmed'}>
            {t('terms')}
          </Anchor>
          <Divider orientation="vertical" />
          <Anchor size={'xs'} component={Link} to="/about" c={'dimmed'}>
            {t('about_us')}
          </Anchor>
        </Group> */}
        <Group gap={0} justify="flex-end" wrap={"nowrap"}>
          {/* <LanguageSelector /> */}
          <ColorSchemeToggle size="lg" />
        </Group>
      </Container>
      <Center>
        <Text size={"xs"} c={"dimmed"} mr={"md"}>
          v{appVersion}
        </Text>
        <Text size={"xs"} c={"dimmed"} mr={3}>
          {t("powered_by")}
        </Text>
        <Anchor
          href={"https://www.academykit.co"}
          style={{ textDecoration: "none" }}
        >
          <Text size={"xs"} c={"dimmed"}>
            Academy Kit
          </Text>
        </Anchor>
      </Center>
    </footer>
  );
}
