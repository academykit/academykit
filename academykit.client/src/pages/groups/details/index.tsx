import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
import useAuth from "@hooks/useAuth";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Box,
  Button,
  Container,
  Flex,
  Paper,
  Text,
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
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string()
      .trim()
      .required(t("group_name_required") as string)
      .max(250, t("group_character_limit") as string),
  });
};
const GroupDetail = () => {
  const { id } = useParams();
  const { t } = useTranslation();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema()),
  });
  const [edit, setEdit] = useState(false);
  useFormErrorHooks(form);

  const groupDetail = useGetGroupDetail(id as string);
  const updateGroups = useUpdateGroup(id as string);
  const auth = useAuth();

  useEffect(() => {
    if (groupDetail.data) {
      form.setFieldValue("name", groupDetail.data.data.name);
    }
  }, [groupDetail.data]);

  const updateGroup = async ({ name }: { name: string }) => {
    try {
      await updateGroups.mutateAsync({
        name: name.trim(),
        id: id as string,
        isActive: true,
      });
      setEdit(false);
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
    setEdit(false);
    form.reset();
  };
  if (groupDetail.error) {
    throw groupDetail.error;
  }

  return (
    <Container fluid>
      <Flex justify={"space-between"} w={"100%"}>
        <Title>{t("group_details")}</Title>

        {!edit && auth?.auth && Number(auth?.auth?.role) < UserRole.Trainer && (
          <Button
            onClick={() => {
              setEdit(true);
              form.setFieldValue(
                "name",
                groupDetail.data ? groupDetail.data.data.name : ""
              );
            }}
            variant="outline"
          >
            {t("edit")}
          </Button>
        )}
      </Flex>
      <form onSubmit={form.onSubmit(updateGroup)}>
        {!edit ? (
          <Paper withBorder p={10} mt={10}>
            <Flex direction="column">
              <Text size="lg" fw={"bold"}>
                {t("group_name")}
              </Text>
              <Text>{groupDetail?.data?.data?.name}</Text>
            </Flex>
          </Paper>
        ) : (
          <Paper withBorder p={20} mt={10}>
            <Box>
              <CustomTextFieldWithAutoFocus
                style={{ maxWidth: "36rem" }}
                name="name"
                label={t("group_name")}
                withAsterisk
                placeholder={t("your_group_name") as string}
                {...form.getInputProps("name")}
                mb={10}
              />
              <Button loading={updateGroups.isPending} type="submit">
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

export default GroupDetail;
