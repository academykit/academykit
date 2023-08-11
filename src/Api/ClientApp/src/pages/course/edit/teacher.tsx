import DeleteModal from '@components/Ui/DeleteModal';
import UserShortProfile from '@components/UserShortProfile';
import useAuth from '@hooks/useAuth';
import {
  Box,
  Button,
  Container,
  Group,
  Title,
  Transition,
  Card,
  Text,
  Select,
  Flex,
  Loader,
  ScrollArea,
} from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconTrash } from '@tabler/icons';
import errorType from '@utils/services/axiosError';
import {
  ICreateCourseTeacher,
  useCourseTeacher,
  useCreateTeacherCourse,
  useDeleteCourseTeacher,
} from '@utils/services/courseService';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';
import * as Yup from 'yup';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import { useGetTrainers } from '@utils/services/adminService';
import { TrainingTypeEnum } from '@utils/enums';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import queryStringGenerator from '@utils/queryStringGenerator';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    email: Yup.string()
      .email(t('invalid_email') as string)
      .required(t('email_required') as string),
  });
};

const TeacherCards = ({
  teacher: { id, user, courseCreatedBy },
}: {
  teacher: ICreateCourseTeacher;
}) => {
  const deleteTeacher = useDeleteCourseTeacher();
  const { t } = useTranslation();
  const handleDelete = async () => {
    try {
      await deleteTeacher.mutateAsync(id);
      showNotification({ message: t('trainer_deleted') });
      setDeletePopUP(false);
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: 'red' });
    }
  };
  const [deletePopup, setDeletePopUP] = useState(false);
  const auth = useAuth();

  return (
    <>
      <DeleteModal
        title={`${t('delete_trainer?')}`}
        open={deletePopup}
        onClose={() => setDeletePopUP(false)}
        onConfirm={handleDelete}
      />

      <Card radius={'lg'} mb={10}>
        <Group py={5} position="apart">
          {user && (
            <UserShortProfile user={user} size={'md'} page="Trainings" />
          )}
          <Group>
            <Text color={'dimmed'} size={'sm'}></Text>
            {auth?.auth &&
              auth?.auth.id !== user?.id &&
              user?.id !== courseCreatedBy && (
                <IconTrash
                  color="red"
                  style={{ cursor: 'pointer' }}
                  onClick={() => setDeletePopUP(true)}
                />
              )}
          </Group>
        </Group>
      </Card>
    </>
  );
};

const Teacher = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { t } = useTranslation();

  const form = useForm({
    initialValues: {
      email: '',
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const slug = useParams();
  const {
    data,
    isLoading: loading,
    isError: error,
  } = useCourseTeacher(slug.id as string, searchParams);
  const createTeacher = useCreateTeacherCourse();

  const [showAddForm, toggleAddForm] = useToggle();

  const onSubmitForm = async ({ email }: { email: string }) => {
    try {
      await createTeacher.mutateAsync({
        courseIdentity: slug.id as string,
        email: email,
      });
      showNotification({
        message: t('add_trainer_success'),
      });
      form.reset();

      toggleAddForm();
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: 'red' });
    }
  };
  const [search, setSearch] = useState('');
  const lessonType = TrainingTypeEnum.Course;
  const { data: trainers, isLoading } = useGetTrainers(
    queryStringGenerator({ search }),
    lessonType,
    slug.id
  );
  return (
    <Container fluid>
      <Group sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
        <Title>{t('trainers')}</Title>
        <Button onClick={() => toggleAddForm()}>
          {!showAddForm ? t('add_trainer') : t('cancel')}
        </Button>
      </Group>
      <Transition
        mounted={showAddForm}
        transition={'slide-down'}
        duration={200}
        timingFunction="ease"
      >
        {() => (
          <Box mt={10}>
            <form onSubmit={form.onSubmit(onSubmitForm)}>
              <Group sx={{ alignItems: 'start' }}>
                <Select
                  clearable
                  placeholder={t('enter_email_trainer') as string}
                  searchable
                  nothingFound={isLoading ? 'Loading...' : 'No Trainers Found!'}
                  data={trainers?.map((e) => e.email) ?? []}
                  onSearchChange={setSearch}
                  searchValue={search}
                  {...form.getInputProps('email')}
                />

                <Button loading={createTeacher.isLoading} type="submit">
                  {t('add')}
                </Button>
              </Group>
            </form>
          </Box>
        )}
      </Transition>
      <Flex my={'lg'} hidden>
        {searchComponent(t('search_users') as string)}
      </Flex>
      {loading && <Loader />}
      {error && <Box>{errorType(error)}</Box>}

      <ScrollArea>
        {data &&
          data?.items &&
          (data.items.length < 1 ? (
            <Box>{t('no_users')}</Box>
          ) : (
            data.items.map((item) => (
              <TeacherCards teacher={item} key={item.id} />
            ))
          ))}
      </ScrollArea>
      {data && pagination(data.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(Teacher);
