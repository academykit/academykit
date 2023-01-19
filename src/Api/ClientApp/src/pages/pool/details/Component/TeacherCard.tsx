import DeleteModal from "@components/Ui/DeleteModal";
import UserShortProfile from "@components/UserShortProfile";
import useAuth from "@hooks/useAuth";
import { Button, Card, Group, Modal } from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconTrash } from "@tabler/icons";
import { PoolRole, UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  IPoolTeacher,
  useDeletePoolTeacher,
} from "@utils/services/poolService";

const TeacherCard = ({
  teacher,
  slug,
}: {
  teacher: IPoolTeacher;
  slug: string;
}) => {
  const deleteTeacher = useDeletePoolTeacher(slug);
  const [confirmation, setConfirmation] = useToggle();
  const auth = useAuth();

  const handleDelete = async () => {
    try {
      setConfirmation();
      await deleteTeacher.mutateAsync(teacher.id as string);
      showNotification({ message: "Teacher deleted successfully" });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <Card radius={"lg"} my={10}>
      <DeleteModal
        title={`Do you want to delete this teacher?`}
        open={confirmation}
        onClose={setConfirmation}
        onConfirm={handleDelete}
      />

      <Group py={5} position="apart">
        <UserShortProfile
          size={"md"}
          user={{ ...teacher.user, role: teacher?.role }}
        />

        {auth?.auth &&
          auth?.auth.id !== teacher.user.id &&
          teacher?.role !== PoolRole.Creator && (
            <Group>
              <IconTrash
                color="red"
                style={{ cursor: "pointer" }}
                onClick={() => setConfirmation()}
              />
            </Group>
          )}
      </Group>
    </Card>
  );
};

export default TeacherCard;
