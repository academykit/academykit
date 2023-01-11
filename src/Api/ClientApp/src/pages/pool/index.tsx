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
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import { useAddPool, usePools } from "@utils/services/poolService";
import PoolCard from "./Components/PoolCard";
import * as Yup from "yup";
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
const schema = Yup.object().shape({
  name: Yup.string().required("Name is required"),
});

const MCQPool = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const pools = usePools(searchParams);
  const addPool = useAddPool(searchParams);
  const [showAddForm, toggleAddForm] = useToggle();

  const { classes } = useStyle();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema),
  });
  const onSubmitForm = async ({ name }: { name: string }) => {
    try {
      await addPool.mutateAsync(name);
      form.reset();
      toggleAddForm();
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: "red" });
    }
  };
  return (
    <Container fluid>
      <Group sx={{ justifyContent: "space-between", alignItems: "center" }}>
        <Title>MCQ Pools</Title>
        {!showAddForm && (
          <Button onClick={() => toggleAddForm()}>Create Pool</Button>
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
                  <TextInput
                    label={"Pool Name"}
                    placeholder="Enter the pool name"
                    name="name"
                    {...form.getInputProps("name")}
                  ></TextInput>
                  <Group mt={10}>
                    <Button type="submit" top={5}>
                      Create
                    </Button>
                    {showAddForm && (
                      <Button
                        top={5}
                        onClick={() => toggleAddForm()}
                        variant="outline"
                      >
                        Cancel
                      </Button>
                    )}
                  </Group>
                </Paper>
              </Group>
            </form>
          </Box>
        )}
      </Transition>
      <Box mt={20}>
        {searchComponent()}
        {pools.data && pools.data.items.length >= 1 ? (
          pools.data?.items.map((x) => (
            <PoolCard search={searchParams} pool={x} key={x.id} />
          ))
        ) : (
          <Box mt={10}>No Question Pools Found!</Box>
        )}
        {pools.data && pagination(pools.data.totalPage)}
      </Box>
    </Container>
  );
};

export default withSearchPagination(MCQPool);
