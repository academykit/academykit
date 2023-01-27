import {
  Box,
  Button,
  Container,
  createStyles,
  Flex,
  Paper,
  Text,
  TextInput,
  Title,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import { useAddOnePool, useOnePool } from "@utils/services/poolService";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const useStyle = createStyles({});

const schema = Yup.object().shape({
  name: Yup.string().required("Group name is required!"),
});

const MCQDetails = () => {
  const { id } = useParams();
  const pool = useOnePool(id as string);
  const addPool = useAddOnePool(id as string);
  const [edit, setEdit] = useState(false);

  const { theme } = useStyle();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema),
  });
  useEffect(() => {
    if (pool.isSuccess) {
      form.setFieldValue("name", pool.data.name);
    }
  }, [pool.isSuccess]);
  const updatePool = async ({ name }: { name: string }) => {
    try {
      await addPool.mutateAsync({ name: name, poolId: id as string });
      showNotification({
        message: "Successfully updated pool",
      });
      setEdit(!edit);
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  return (
    <Container fluid>
      <Flex justify={"space-between"} w={"100%"}>
        <Title>Pool Details</Title>

        {!edit && (
          <Button onClick={() => setEdit(true)} variant="outline">
            Edit
          </Button>
        )}
      </Flex>

      <form onSubmit={form.onSubmit(updatePool)}>
        {!edit ? (
          <Paper withBorder p={10} mt={10}>
            <Flex direction="column">
              <Text size="lg" weight={"bold"}>
                Pool Name
              </Text>
              <Text>{pool?.data?.name}</Text>
            </Flex>
          </Paper>
        ) : (
          <Paper mt={20} p={20} withBorder>
            <Box>
              <TextInput
                sx={{ maxWidth: theme.breakpoints.xs }}
                name="name"
                label="Pool Name"
                placeholder="Please enter your pool name."
                {...form.getInputProps("name")}
              />
              <Button loading={addPool.isLoading} mt={20} type="submit">
                Save
              </Button>
              <Button variant="outline" onClick={() => setEdit(false)} ml={10}>
                Cancel
              </Button>
            </Box>
          </Paper>
        )}
      </form>
    </Container>
  );
};

export default MCQDetails;
