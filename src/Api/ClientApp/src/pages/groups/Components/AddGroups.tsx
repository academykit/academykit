import { Box, Button, createStyles, Paper, TextInput } from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import { useAddGroup } from "@utils/services/groupService";
import { useTranslation } from "react-i18next";
import * as Yup from "yup";
const AddGroups = ({ onCancel }: { onCancel: () => void }) => {
  const { t } = useTranslation();
  const schema = Yup.object().shape({
    name: Yup.string().required("Group Name is required."),
  });
  const { mutateAsync, isLoading } = useAddGroup();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema),
  });
  const useStyles = createStyles((theme) => ({
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
  const { classes } = useStyles();

  const onSubmitForm = async (name: string) => {
    try {
      await mutateAsync(name);
      showNotification({
        title: t("successful"),
        message: t("group_add_success"),
      });
      onCancel();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  return (
    <Paper
      shadow={"sm"}
      radius="md"
      p="xl"
      withBorder
      className={classes.paper}
    >
      <Box>
        <form onSubmit={form.onSubmit(({ name }) => onSubmitForm(name))}>
          <TextInput
            mb={10}
            label={t("group_name")}
            withAsterisk
            name="name"
            size="md"
            {...form.getInputProps("name")}
          />

          <Button loading={isLoading} mr={10} type="submit" size="md">
            {t("submit")}
          </Button>
          <Button
            variant="outline"
            disabled={isLoading}
            type="reset"
            onClick={(e: any) => onCancel()}
            size={"md"}
          >
            {t("cancel")}
          </Button>
        </form>
      </Box>
    </Paper>
  );
};

export default AddGroups;
