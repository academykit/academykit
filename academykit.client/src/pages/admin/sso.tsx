import { Google, Microsoft } from "@components/Icons";
import {
  Button,
  Checkbox,
  Group,
  ScrollArea,
  Select,
  Text,
  Title,
} from "@mantine/core";
import { IconMail } from "@tabler/icons-react";
import { t } from "i18next";

const Sso = () => {
  return (
    <ScrollArea>
      <Group
        style={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>{t("security")}</Title>
        <Button onClick={() => {}}>{t("add_license")}</Button>
      </Group>
      <Text>{t("security_description")}</Text>
      <Title order={2} mt="xl" mb="md">
        {t("sign_in")}
      </Title>

      <Group grow w="50%" mb="md">
        <Group
          justify="space-between"
          style={{
            border: "1px solid #ddd",
            padding: "10px",
            borderRadius: "4px",
          }}
        >
          <Group>
            <Google height={28} width={28} />
            <Group
              style={{
                flexDirection: "column",
                gap: 0,
                alignItems: "flex-start",
              }}
            >
              <Text fw={600}>{t("google")}</Text>
              <Text size="xs" c="dimmed">
                {t("google_allow_description")}
              </Text>
            </Group>
          </Group>
          <Checkbox />
        </Group>
      </Group>

      <Group grow w="50%" mb="md">
        <Group
          justify="space-between"
          style={{
            border: "1px solid #ddd",
            padding: "10px",
            borderRadius: "4px",
          }}
        >
          <Group>
            <Microsoft height={28} width={28} />
            <Group
              style={{
                flexDirection: "column",
                gap: 0,
                alignItems: "flex-start",
              }}
            >
              <Text fw={600}>{t("microsoft")}</Text>
              <Text size="xs" c="dimmed">
                {t("microsoft_allow_description")}
              </Text>
            </Group>
          </Group>
          <Checkbox />
        </Group>
      </Group>

      <Group w="50%" grow mb="md">
        <Group
          justify="space-between"
          style={{
            border: "1px solid #ddd",
            padding: "10px",
            borderRadius: "4px",
          }}
        >
          <Group>
            <IconMail size={24} />
            <Group
              style={{
                flexDirection: "column",
                gap: 0,
                alignItems: "flex-start",
              }}
            >
              <Text fw={600}>{t("email")}</Text>
              <Text size="xs" c="dimmed">
                {t("email_allow_description")}
              </Text>
            </Group>
          </Group>
          <Checkbox />
        </Group>
      </Group>

      <Title order={2} mt="xl" mb="md">
        {t("access")}
      </Title>

      <Group w="50%" grow justify="space-between" align="flex-start" mb="md">
        <div>
          <Text fw={600} mb={4}>
            Allowed domains
          </Text>
          <Text size="sm" c="dimmed">
            Only allow users with email addresses from specific domains to
            access your organization.
          </Text>
        </div>
        <Button w={10} variant="outline">
          Add a domain
        </Button>
      </Group>

      <Group w="50%" grow justify="space-between" align="flex-start" mb="md">
        <div>
          <Text fw={600} mb={4}>
            Allowed domains
          </Text>
          <Text size="sm" c="dimmed">
            Only allow users with email addresses from specific domains to
            access your organization.
          </Text>
        </div>
        <Select
          w={200}
          placeholder="Select role"
          data={[
            { value: "editor", label: "Editor" },
            { value: "admin", label: "Admin" },
            { value: "superAdmin", label: "Super Admin" },
          ]}
        />
      </Group>
    </ScrollArea>
  );
};

export default Sso;
