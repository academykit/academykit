import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  Box,
  Button,
  Container,
  createStyles,
  Group,
  Paper,
  TextInput,
  Title,
  Transition,
  Loader,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import { useAddPool, usePools } from "@utils/services/poolService";
import PoolCard from "./Components/PoolCard";
import * as Yup from "yup";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router";
import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
const useStyle = createStyles((theme) => ({
  paper: {
    [theme.fn.smallerThan("md")]: {
      width: "100%",
    },
    [theme.fn.smallerThan("lg")]: {
      width: "100%",
    },

    width: "50%",
    marginBottom: "20px",
  },
}));
const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t("pool_name_required") as string),
  });
};

const MCQPool = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const pools = usePools(searchParams);
  const addPool = useAddPool(searchParams);
  const [showAddForm, toggleAddForm] = useToggle();
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { classes } = useStyle();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const onSubmitForm = async ({ name }: { name: string }) => {
    try {
      const res = await addPool.mutateAsync(name);
      form.reset();
      toggleAddForm();
      navigate(res.data.slug + "/questions");
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: "red" });
    }
  };
  return (
    <Container fluid>
      <Group sx={{ justifyContent: "space-between", alignItems: "center" }}>
        <Title>{t("mcq_pools")}</Title>
        {!showAddForm && (
          <Button onClick={() => toggleAddForm()}>{t("create_pool")}</Button>
        )}
      </Group>
      <Transition
        mounted={showAddForm}
        transition={"slide-down"}
        duration={200}
        timingFunction="ease"
      >
        {(style) => (
          <Box mt={10}>
            <form onSubmit={form.onSubmit(onSubmitForm)}>
              <Group>
                <Paper
                  shadow={"sm"}
                  radius="md"
                  p="xl"
                  withBorder
                  className={classes.paper}
                >
                  <CustomTextFieldWithAutoFocus
                    label={t("pool_name")}
                    placeholder={t("enter_pool_name") as string}
                    name="name"
                    {...form.getInputProps("name")}
                  />
                  <Group mt={10}>
                    <Button type="submit" top={5}>
                      {t("create")}
                    </Button>
                    {showAddForm && (
                      <Button
                        top={5}
                        onClick={() => toggleAddForm()}
                        variant="outline"
                      >
                        {t("cancel")}
                      </Button>
                    )}
                  </Group>
                </Paper>
              </Group>
            </form>
          </Box>
        )}
      </Transition>
      {pools.isLoading && <Loader />}

      <Box mt={20}>
        {pools.isSuccess && (
          <>
            {searchComponent(t("search_pools") as string)}
            {pools.data.items.length >= 1 &&
              pools.data?.items.map((x) => (
                <PoolCard search={searchParams} pool={x} key={x.id} />
              ))}
            {pools.data?.items.length < 1 && <Box mt={10}>{t("no_pools")}</Box>}
          </>
        )}
        {pools.data &&
          pagination(pools.data.totalPage, pools.data.items.length)}
      </Box>
    </Container>
  );
};

export default withSearchPagination(MCQPool);
