import DeleteModal from '@components/Ui/DeleteModal';
import UserShortProfile from '@components/UserShortProfile';
import useAuth from '@hooks/useAuth';
import { Card, Group } from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconTrash } from '@tabler/icons';
import { PoolRole } from '@utils/enums';
import errorType from '@utils/services/axiosError';
import {
  IPoolTeacher,
  useDeletePoolTeacher,
} from '@utils/services/poolService';
import { useTranslation } from 'react-i18next';

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
  const { t } = useTranslation();
  const handleDelete = async () => {
    try {
      setConfirmation();
      await deleteTeacher.mutateAsync(teacher.id as string);
      showNotification({ message: t('delete_teacher_success') });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  return (
    <Card radius={'lg'} my={10}>
      <DeleteModal
        title={t('want_to_delete_trainer')}
        open={confirmation}
        onClose={setConfirmation}
        onConfirm={handleDelete}
      />

      <Group py={5} position="apart">
        <UserShortProfile
          size={'md'}
          user={{ ...teacher.user, role: teacher?.role }}
        />

        {auth?.auth &&
          auth?.auth.id !== teacher.user.id &&
          teacher?.role !== PoolRole.Creator && (
            <Group>
              <IconTrash
                color="red"
                style={{ cursor: 'pointer' }}
                onClick={() => setConfirmation()}
              />
            </Group>
          )}
      </Group>
    </Card>
  );
};

export default TeacherCard;
