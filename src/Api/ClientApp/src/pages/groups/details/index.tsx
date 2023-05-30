import useAuth from "@hooks/useAuth";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
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
import { UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  useGetGroupDetail,
  useUpdateGroup,
} from "@utils/services/groupService";
import i18next from "i18next";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const useStyle = createStyles({});

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t("group_name_required") as string),
  });
};
const GroupDetail = () => {
  const { id } = useParams();
  const { theme } = useStyle();
  const { t, i18n } = useTranslation();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema()),
  });
  const [edit, setEdit] = useState(false);
  const formErrors = useFormErrorHooks(form);

  const groupDetail = useGetGroupDetail(id as string);
  const updateGroups = useUpdateGroup(id as string);
  const auth = useAuth();

  if (groupDetail.error) {
    throw groupDetail.error;
  }

  useEffect(() => {
    if (groupDetail.isSuccess) {
      form.setFieldValue("name", groupDetail.data.data.name);
    }
  }, [groupDetail.isSuccess]);

  const updateGroup = async ({ name }: { name: string }) => {
    try {
      await updateGroups.mutateAsync({
        name,
        id: id as string,
        isActive: true,
      });
      showNotification({
        title: t("successful"),
        message: t("group_update_success"),
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    }
  };

  i18n.on("languageChanged", () => {
    console.log("first");
  });

  return (
    <Container fluid>
      <Flex justify={"space-between"} w={"100%"}>
        <Title>{t("group_details")}</Title>

        {!edit && auth?.auth && auth?.auth?.role < UserRole.Trainer && (
          <Button onClick={() => setEdit(true)} variant="outline">
            {t("edit")}
          </Button>
        )}
      </Flex>
      <form onSubmit={form.onSubmit(updateGroup)}>
        {!edit ? (
          <Paper withBorder p={10} mt={10}>
            <Flex direction="column">
              <Text size="lg" weight={"bold"}>
                {t("group_name")}
              </Text>
              <Text>{groupDetail?.data?.data?.name}</Text>
            </Flex>
          </Paper>
        ) : (
          <Paper withBorder p={20} mt={10}>
            <Box>
              <TextInput
                sx={{ maxWidth: theme.breakpoints.xs }}
                name="name"
                label={t("group_name")}
                withAsterisk
                placeholder={t("your_group_name") as string}
                {...form.getInputProps("name")}
              />
              <Button loading={updateGroups.isLoading} mt={20} type="submit">
                {t("save")}
              </Button>
              <Button variant="outline" onClick={() => setEdit(false)} ml={10}>
                {t("cancel")}
              </Button>
            </Box>
          </Paper>
        )}
      </form>
    </Container>
  );
};

export default GroupDetail;
