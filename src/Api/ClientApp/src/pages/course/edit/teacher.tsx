import DeleteModal from "@components/Ui/DeleteModal";
import UserShortProfile from "@components/UserShortProfile";
import useAuth from "@hooks/useAuth";
import {
  Anchor,
  Box,
  Button,
  Container,
  Group,
  TextInput,
  Title,
  Transition,
  Card,
  Text,
  Modal,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconTrash } from "@tabler/icons";
import { UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  ICreateCourseTeacher,
  useCourseTeacher,
  useCreateTeacherCourse,
  useDeleteCourseTeacher,
} from "@utils/services/courseService";
import { useState } from "react";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const TeacherCards = ({
  teacher: { id, user },
}: {
  teacher: ICreateCourseTeacher;
}) => {
  const deleteTeacher = useDeleteCourseTeacher();
  const handleDelete = async () => {
    try {
      await deleteTeacher.mutateAsync(id);
      showNotification({ message: "Course Teacher deleted successfully." });
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: "red" });
    }
  };
  const [deletePopup, setDeletePopUP] = useState(false);
  const auth = useAuth();

  return (
    <>
      <DeleteModal
        title={`Do you want to delete trainer?`}
        open={deletePopup}
        onClose={setDeletePopUP}
        onConfirm={handleDelete}
      />

      <Card radius={"lg"} mb={10}>
        <Group py={5} position="apart">
          {user && <UserShortProfile user={user} size={"md"} />}
          <Group>
            <Text color={"dimmed"} size={"sm"}></Text>
            {auth?.auth &&
              auth?.auth?.role === UserRole.SuperAdmin &&
              auth?.auth.id !== user?.id && (
                <IconTrash
                  color="red"
                  style={{ cursor: "pointer" }}
                  onClick={() => setDeletePopUP(true)}
                />
              )}
          </Group>
        </Group>
      </Card>
    </>
  );
};

const Teacher = () => {
  const schema = Yup.object().shape({
    email: Yup.string().email("Invalid email").required("Email is required."),
  });
  const form = useForm({
    initialValues: {
      email: "",
    },
    validate: yupResolver(schema),
  });
  const slug = useParams();
  const getTeacher = useCourseTeacher(slug.id as string);
  const createTeacher = useCreateTeacherCourse();

  const [showAddForm, toggleAddForm] = useToggle();

  const onSubmitForm = async ({ email }: { email: string }) => {
    try {
      await createTeacher.mutateAsync({
        courseIdentity: slug.id as string,
        email: email,
      });
      showNotification({
        message: "Teacher added successfully!",
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
                <Button type="submit">Add</Button>
              </Group>
            </form>
          </Box>
        )}
      </Transition>
      <Box mt={20}>
        {getTeacher.data?.items.map((item) => (
          <TeacherCards teacher={item} key={item.id} />
        ))}
      </Box>
    </Container>
  );
};

export default Teacher;
