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
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import * as Yup from "yup";
import useFormErrorHooks from "@hooks/useFormErrorHooks";

const useStyle = createStyles({});

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t("pool_name_required") as string),
  });
};

const MCQDetails = () => {
  const { id } = useParams();
  const pool = useOnePool(id as string);
  const addPool = useAddOnePool(id as string);
  const [edit, setEdit] = useState(false);
  const { t } = useTranslation();

  const { theme } = useStyle();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  useEffect(() => {
    if (pool.isSuccess) {
      form.setFieldValue("name", pool.data.name);
    }
  }, [pool.isSuccess]);
  const updatePool = async ({ name }: { name: string }) => {
    try {
      await addPool.mutateAsync({ name: name, poolId: id as string });
      showNotification({
        message: t("update_pool_success"),
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
        <Title>{t("pool_details")}</Title>

        {!edit && (
          <Button
            onClick={() => {
              setEdit(true);
              form.setFieldValue("name", pool.data ? pool.data?.name : "");
            }}
            variant="outline"
          >
            {t("edit")}
          </Button>
        )}
      </Flex>

      <form onSubmit={form.onSubmit(updatePool)}>
        {!edit ? (
          <Paper withBorder p={10} mt={10}>
            <Flex direction="column">
              <Text size="lg" weight={"bold"}>
                {t("pool_name")}{" "}
              </Text>
              <Text>{pool?.data?.name}</Text>
            </Flex>
          </Paper>
        ) : (
          <Paper mt={20} p={20} withBorder>
            <Box>
              <TextInput
                autoFocus
                sx={{ maxWidth: theme.breakpoints.xs }}
                name="name"
                label={t("pool_name")}
                placeholder={t("enter_pool_name") as string}
                {...form.getInputProps("name")}
              />
              <Button loading={addPool.isLoading} mt={20} type="submit">
                {t("save")}
              </Button>
              <Button
                variant="outline"
                onClick={() => {
                  setEdit(false);
                  form.reset();
                }}
                ml={10}
              >
                {t("cancel")}
              </Button>
            </Box>
          </Paper>
        )}
      </form>
    </Container>
  );
};

export default MCQDetails;
