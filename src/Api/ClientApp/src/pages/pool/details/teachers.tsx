import {
  Box,
  Button,
  Container,
  Group,
  TextInput,
  Title,
  Transition,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  useCreateTeacherPool,
  usePoolsTeacher,
} from "@utils/services/poolService";

import { useParams } from "react-router-dom";
import TeacherCard from "./Component/TeacherCard";

const MCQTeacher = () => {
  const form = useForm({
    initialValues: {
      email: "",
    },
  });
  const slug = useParams();
  const getPoolsTeacher = usePoolsTeacher(slug.id as string);
  const createPoolTeacher = useCreateTeacherPool(slug.id as string);

  const [showAddForm, toggleAddForm] = useToggle();

  const onSubmitForm = async ({ email }: { email: string }) => {
    try {
      await createPoolTeacher.mutateAsync({
        questionPoolIdentity: slug.id as string,
        email: email,
      });
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
        <Title>Trainers</Title>
        <Button onClick={() => toggleAddForm()}>
          {!showAddForm ? "Add Trainer" : "Cancel"}
        </Button>
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
                <TextInput
                  placeholder="Enter the email"
                  name="email"
                  type={"email"}
                  {...form.getInputProps("email")}
                ></TextInput>
                <Button type="submit" loading={createPoolTeacher.isLoading}>
                  Add
                </Button>
              </Group>
            </form>
          </Box>
        )}
      </Transition>
      <Box mt={20}>
        {getPoolsTeacher.data?.items.map((item) => (
          <TeacherCard teacher={item} key={item.id} slug={slug.id as string} />
        ))}
      </Box>
    </Container>
  );
};

export default MCQTeacher;
