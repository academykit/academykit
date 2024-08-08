import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Box,
  Button,
  Center,
  Drawer,
  FocusTrap,
  Group,
  Loader,
  rem,
  ScrollArea,
  SegmentedControl,
  SimpleGrid,
  Space,
  TextInput,
  Title,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { useDisclosure } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconColumns, IconLayoutGrid } from "@tabler/icons-react";
import errorType from "@utils/services/axiosError";
import { useAddPool, usePools } from "@utils/services/poolService";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import * as Yup from "yup";
import PoolCard from "./Components/PoolCard";
import PoolTable from "./Components/PoolTable";

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string()
      .max(100, t("name_length_validation") as string)
      .required(t("pool_name_required") as string),
  });
};

const MCQPool = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const pools = usePools(searchParams);
  const [selectedView, setSelectedView] = useState("list");
  const { mutateAsync, isLoading } = useAddPool(searchParams);
  const [opened, { open, close }] = useDisclosure(false);
  const { t } = useTranslation();
  const navigate = useNavigate();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const onSubmitForm = async ({ name }: { name: string }) => {
    try {
      const res = await mutateAsync(name);
      form.reset();
      navigate(res.data.slug + "/questions");
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: "red" });
    }
  };
  return (
    <>
      <Group style={{ justifyContent: "space-between", alignItems: "center" }}>
        <Title>{t("mcq_pools")}</Title>

        <Button onClick={open}>{t("create_pool")}</Button>
      </Group>
      <Drawer
        opened={opened}
        onClose={close}
        title={t("create_pool")}
        overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
      >
        <Box>
          <form onSubmit={form.onSubmit(onSubmitForm)}>
            <FocusTrap active={true}>
              <TextInput
                data-autofocus
                label={t("pool_name")}
                placeholder={t("enter_pool_name") as string}
                withAsterisk
                {...form.getInputProps("name")}
              />
            </FocusTrap>
            <Space h="md" />
            <Group justify="flex-end">
              <Button type="submit" loading={isLoading}>
                {t("create")}
              </Button>
            </Group>
          </form>
        </Box>
      </Drawer>

      {pools.isLoading && <Loader />}

      <Group my={10}>
        <Box flex={1}>{searchComponent(t("search_pools") as string)}</Box>
        <SegmentedControl
          value={selectedView}
          onChange={setSelectedView}
          data={[
            {
              value: "list",
              label: (
                <Center style={{ gap: 10 }}>
                  <IconLayoutGrid style={{ width: rem(20), height: rem(20) }} />
                </Center>
              ),
            },
            {
              value: "table",
              label: (
                <Center style={{ gap: 10 }}>
                  <IconColumns style={{ width: rem(20), height: rem(20) }} />
                </Center>
              ),
            },
          ]}
        />
      </Group>

      {pools.isSuccess && (
        <ScrollArea>
          {pools?.data &&
            (pools.data.totalCount > 0 ? (
              selectedView === "table" ? (
                <PoolTable pool={pools.data?.items} search={searchParams} />
              ) : (
                <SimpleGrid spacing={10} cols={{ sx: 1, sm: 2, md: 3, lg: 4 }}>
                  {pools.data?.items.map((x) => (
                    <PoolCard search={searchParams} pool={x} key={x.id} />
                  ))}
                </SimpleGrid>
              )
            ) : (
              <Box mt={10}>{t("no_pools")}</Box>
            ))}
        </ScrollArea>
      )}
      {pools.data && pagination(pools.data.totalPage, pools.data.items.length)}
    </>
  );
};

export default withSearchPagination(MCQPool);
