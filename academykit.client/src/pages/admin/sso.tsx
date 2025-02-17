import SignInOption from "@components/Admin/SSO/SignInOption";
import { Google, Microsoft } from "@components/Icons";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  ActionIcon,
  Button,
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
import { SignInType, UserRole } from "@utils/enums";
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

const Sso = () => {
  const [opened, { open, close }] = useDisclosure(false);
  const [defaultRoleValue, setDefaultRoleValue] = useState("");
  const [signInOptionsState, setSignInOptionsState] = useState({
    [SignInType.Email]: false,
    [SignInType.Google]: false,
    [SignInType.Microsoft]: false,
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
          Yup.object().shape({
            domain: Yup.string().required(t("domain_required") as string),
          })
        )
        .min(1, t("at_least_one_domain_required") as string),
    });

  const form = useForm({
    initialValues: {
      domain: [{ id: 0, domain: "" }],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  useEffect(() => {
    if (domainList?.data?.length) {
      form.setFieldValue(
        "domain",
        domainList.data.map((domain, index) => ({ id: index, domain: domain }))
      );
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
        [SignInType.Email]: false,
        [SignInType.Google]: false,
        [SignInType.Microsoft]: false,
      };
      for (const option of signInOptions.data) {
        newState[option.signIn as SignInType] = option.isAllowed;
      }
      setSignInOptionsState(newState);
    }
  }, [signInOptions?.data]);

  const handleSubmit = async (values: {
    domain: { id: number; domain: string }[];
  }) => {
    try {
      await allowedDomains.mutateAsync(
        values.domain.map((domain) => domain.domain)
      );
      showNotification({
        title: t("successful"),
        message: t("allowed_domains_saved_successfully"),
      });
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

  const handleSignInOptionsSubmit = async (updatedOption: SignInType) => {
    try {
      const currentOption = signInOptions?.data.find(
        (option) => option.signIn === updatedOption
      );
      const newIsAllowed = !currentOption?.isAllowed;

      await setSignInOptions.mutateAsync({
        signIn: updatedOption,
        isAllowed: newIsAllowed,
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    }
  };

  const handleSignInOptionToggle = (signInType: SignInType) => {
    setSignInOptionsState((prevState) => ({
      ...prevState,
      [signInType]: !prevState[signInType],
    }));
    handleSignInOptionsSubmit(signInType);
  };

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
            <Flex
              mb={10}
              gap={10}
              align={"flex-end"}
              key={_domain.id}
              wrap={"wrap"}
            >
              <TextInput
                placeholder={t("domain") as string}
                name={`domain.${index}.domain`}
                label={index === 0 && t("domain")}
                withAsterisk
                {...form.getInputProps(`domain.${index}.domain`)}
              />
              <ActionIcon
                variant="subtle"
                onClick={() => {
                  form.insertListItem("domain", {
                    id: form.values.domain.length,
                    domain: "",
                  });
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

      <SignInOption
        icon={<Google height={28} width={28} />}
        title={t("google")}
        description={t("google_allow_description")}
        signInType={SignInType.Google}
        checked={signInOptionsState[SignInType.Google]}
        onToggle={handleSignInOptionToggle}
      />

      <SignInOption
        icon={<Microsoft height={28} width={28} />}
        title={t("microsoft")}
        description={t("microsoft_allow_description")}
        signInType={SignInType.Microsoft}
        checked={signInOptionsState[SignInType.Microsoft]}
        onToggle={handleSignInOptionToggle}
      />

      <SignInOption
        icon={<IconMail size={24} />}
        title={t("email")}
        description={t("email_allow_description")}
        signInType={SignInType.Email}
        checked={signInOptionsState[SignInType.Email]}
        onToggle={handleSignInOptionToggle}
      />

      <Title order={2} mt="xl" mb="md">
        {t("access")}
      </Title>

      <Group w="50%" grow justify="space-between" align="flex-start" mb="md">
        <div>
          <Text fw={600} mb={4}>
            {t("allowed_domains")}
          </Text>
          <Text size="sm" c="dimmed">
            {t("allow_domains_description")}
          </Text>
        </div>

        <Button
          w={10}
          type="button"
          variant="outline"
          maw={200}
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
          maw={200}
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
