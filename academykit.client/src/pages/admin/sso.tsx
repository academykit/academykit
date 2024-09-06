import { Google, Microsoft } from "@components/Icons";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  ActionIcon,
  Button,
  Checkbox,
  Drawer,
  Flex,
  Group,
  ScrollArea,
  Select,
  Text,
  TextInput,
  Title,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { useDisclosure } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconMail, IconPlus, IconTrash } from "@tabler/icons-react";
import { UserRole } from "@utils/enums";
import {
  useGetAllowedDomains,
  useGetDefaultRole,
  useGetSignInOptions,
  useSetAllowedDomains,
  useSetDefaultRole,
  useSetSignInOptions,
} from "@utils/services/adminService";
import errorType from "@utils/services/axiosError";
import { t } from "i18next";
import { useEffect, useState } from "react";
import * as Yup from "yup";

const domainRegex = /^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.[a-zA-Z]{2,}$/;

const Sso = () => {
  const [opened, { open, close }] = useDisclosure(false);
  const [defaultRoleValue, setDefaultRoleValue] = useState("");
  const [signInOptionsState, setSignInOptionsState] = useState({
    google: false,
    microsoft: false,
    email: false,
  });

  const allowedDomains = useSetAllowedDomains();
  const { data: domainList } = useGetAllowedDomains();

  const { data: defaultRole } = useGetDefaultRole();
  const setDefaultRole = useSetDefaultRole();

  const { data: signInOptions } = useGetSignInOptions();
  const setSignInOptions = useSetSignInOptions();

  const schema = () =>
    Yup.object().shape({
      domain: Yup.array()
        .of(
          Yup.string().test("domain-validation", "", function (value) {
            if (!value || value.trim() === "") {
              return this.createError({
                message: t("domain_required") as string,
              });
            }
            if (!domainRegex.test(value.trim())) {
              return this.createError({
                message: t("invalid_domain_format") as string,
              });
            }
            return true;
          })
        )
        .min(1, t("at_least_one_domain_required") as string),
    });

  const form = useForm({
    initialValues: {
      domain: [""],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  useEffect(() => {
    if (domainList?.data) {
      form.setFieldValue("domain", domainList.data);
    }
  }, [domainList?.data]);

  useEffect(() => {
    if (defaultRole?.data) {
      setDefaultRoleValue(defaultRole.data.toString());
    }
  }, [defaultRole?.data]);

  useEffect(() => {
    if (signInOptions?.data) {
      const newState = {
        google: false,
        microsoft: false,
        email: false,
      };
      for (const option of signInOptions.data) {
        if (option.signIn === 1) newState.google = option.isAllowed;
        if (option.signIn === 2) newState.microsoft = option.isAllowed;
        if (option.signIn === 3) newState.email = option.isAllowed;
      }
      setSignInOptionsState(newState);
    }
  }, [signInOptions?.data]);

  const handleSubmit = async (values: { domain: string[] }) => {
    try {
      await allowedDomains.mutateAsync(values.domain);
      showNotification({
        title: t("successful"),
        message: t("allowed_domains_saved_successfully"),
      });
      form.reset();
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    } finally {
      close();
    }
  };

  const handleDefaultRoleSubmit = async (value: string) => {
    try {
      await setDefaultRole.mutateAsync(Number(value));
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    }
  };

  const handleSignInOptionsSubmit = async (
    updatedOption: keyof typeof signInOptionsState
  ) => {
    try {
      const updatedState = {
        ...signInOptionsState,
        [updatedOption]: !signInOptionsState[updatedOption],
      };

      const options = [
        { signIn: 1, isAllowed: updatedState.google },
        { signIn: 2, isAllowed: updatedState.microsoft },
        { signIn: 3, isAllowed: updatedState.email },
      ].filter((option) => option.isAllowed);

      await setSignInOptions.mutateAsync(options);
      showNotification({
        title: t("successful"),
        message: t("sign_in_options_saved_successfully"),
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    }
  };

  console.log(signInOptions?.data);

  return (
    <ScrollArea>
      <Drawer
        opened={opened}
        onClose={() => {
          close();
        }}
        overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
      >
        <form onSubmit={form.onSubmit(handleSubmit)}>
          {form.values.domain.map((_domain, index) => (
            <Flex mb={10} gap={10} align={"flex-end"} key={index} wrap={"wrap"}>
              <TextInput
                placeholder={t("License_email") as string}
                name={`domain.${index}`}
                label={index > 0 ? t("additional_domain") : t("domain")}
                withAsterisk
                {...form.getInputProps(`domain.${index}`)}
              />
              <ActionIcon
                variant="subtle"
                onClick={() => {
                  form.insertListItem("domain", "");
                }}
              >
                <IconPlus />
              </ActionIcon>
              <ActionIcon
                variant="subtle"
                c={"red"}
                disabled={form.values.domain.length === 1}
                onClick={() => {
                  form.removeListItem("domain", index);
                }}
              >
                <IconTrash />
              </ActionIcon>
            </Flex>
          ))}
          <Group mt={20}>
            <Button type="submit">{t("save")}</Button>
          </Group>
        </form>
      </Drawer>
      <Group
        style={{ justifyContent: "space-between", alignItems: "center" }}
        mb={15}
      >
        <Title>{t("security")}</Title>
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
          <Checkbox
            checked={signInOptionsState.google}
            onChange={() => {
              setSignInOptionsState({
                ...signInOptionsState,
                google: !signInOptionsState.google,
              });
              handleSignInOptionsSubmit("google");
            }}
          />
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
          <Checkbox
            checked={signInOptionsState.microsoft}
            onChange={() => {
              setSignInOptionsState({
                ...signInOptionsState,
                microsoft: !signInOptionsState.microsoft,
              });
              handleSignInOptionsSubmit("microsoft");
            }}
          />
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
          <Checkbox
            checked={signInOptionsState.email}
            onChange={() => {
              setSignInOptionsState({
                ...signInOptionsState,
                email: !signInOptionsState.email,
              });
              handleSignInOptionsSubmit("email");
            }}
          />
        </Group>
      </Group>

      <Title order={2} mt="xl" mb="md">
        {t("access")}
      </Title>

      <Group w="50%" grow justify="space-between" align="flex-start" mb="md">
        <div>
          <Text fw={600} mb={4}>
            {t("allowed_domains")}
          </Text>
          <Text size="sm" c="dimmed">
            {t("allowed_domains_description")}
          </Text>
        </div>

        <Button
          w={10}
          type="button"
          variant="outline"
          onClick={() => {
            open();
          }}
        >
          {t("add_domain")}
        </Button>
      </Group>

      <Group w="50%" grow justify="space-between" align="flex-start" mb="md">
        <div>
          <Text fw={600} mb={4}>
            {t("default_role")}
          </Text>
          <Text size="sm" c="dimmed">
            {t("default_role_description")}
          </Text>
        </div>
        <Select
          w={200}
          value={defaultRoleValue}
          placeholder={t("select_role")}
          data={Object.entries(UserRole)
            .filter(([key]) => !Number.isNaN(Number(key)))
            .map(([key, value]) => ({
              value: key,
              label: t(`${value}`),
            }))}
          disabled={setDefaultRole.isPending}
          onChange={(value) => {
            setDefaultRoleValue(value as string);
            handleDefaultRoleSubmit(value as string);
          }}
        />
      </Group>
    </ScrollArea>
  );
};

export default Sso;
