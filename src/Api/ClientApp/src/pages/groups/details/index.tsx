import useAuth from "@hooks/useAuth";
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
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const useStyle = createStyles({});

const schema = Yup.object().shape({
  name: Yup.string().required("Group name is required!"),
});

const GroupDetail = () => {
  const { id } = useParams();
  const { theme } = useStyle();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema),
  });
  const [edit, setEdit] = useState(false);

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
      showNotification({ message: "Group updated successfully." });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        message: err,
        color: "red",
      });
    }
  };

  return (
    <Container fluid>
      <Flex justify={"space-between"} w={"100%"}>
        <Title>Group Details</Title>

        {!edit && auth?.auth && auth?.auth?.role < UserRole.Trainer && (
          <Button onClick={() => setEdit(true)} variant="outline">
            Edit
          </Button>
        )}
      </Flex>
      <form onSubmit={form.onSubmit(updateGroup)}>
        {!edit ? (
          <Paper withBorder p={10} mt={10}>
            <Flex direction="column">
              <Text size="lg" weight={"bold"}>
                Group Name
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
                label="Group Name"
                withAsterisk
                placeholder="Your group name."
                {...form.getInputProps("name")}
              />
              <Button loading={updateGroups.isLoading} mt={20} type="submit">
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

export default GroupDetail;
