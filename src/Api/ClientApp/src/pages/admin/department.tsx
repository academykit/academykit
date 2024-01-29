import DeleteModal from '@components/Ui/DeleteModal';
import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import {
  ActionIcon,
  Badge,
  Box,
  Button,
  Drawer,
  Flex,
  Group,
  Paper,
  Switch,
  Table,
  Text,
  TextInput,
  Title,
} from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { useDisclosure } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconPencil, IconTrash } from '@tabler/icons';
import {
  useDeleteDepartmentSetting,
  useDepartmentSetting,
  usePostDepartmentSetting,
  useUpdateDepartmentSetting,
} from '@utils/services/adminService';
import errorType from '@utils/services/axiosError';
import { IUser } from '@utils/services/types';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

interface IDepartment<T> {
  id: string;
  name: string;
  isActive: boolean;
  user: T;
}
const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t('department_name_required') as string),
  });
};

const Department = ({
  searchParams,
  pagination,
  searchComponent,
  filterComponent,
}: IWithSearchPagination) => {
  const { t } = useTranslation();
  const [opened, { open, close }] = useDisclosure(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editItem, setEditItem] = useState<IDepartment<IUser>>();
  const getDepartment = useDepartmentSetting(searchParams);
  const postDepartment = usePostDepartmentSetting();
  const updateDepartment = useUpdateDepartmentSetting();
  const deleteDepartment = useDeleteDepartmentSetting();

  const form = useForm({
    initialValues: { name: '', isActive: true },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const Rows = ({ item }: { item: IDepartment<IUser> }) => {
    const [opened, setOpened] = useState(false);

    const handleDelete = async () => {
      try {
        await deleteDepartment.mutateAsync(item.id);
        showNotification({
          message: t('delete_department_success'),
        });
      } catch (error: any) {
        showNotification({
          message: error?.response?.data?.message,
          color: 'red',
        });
      }
    };
    return (
      <Table.Tr key={item.id}>
        {opened && (
          <DeleteModal
            title={`${t('sure_to_delete')} "${item?.name}" ${t('department?')}`}
            open={opened}
            onClose={() => setOpened(false)}
            onConfirm={handleDelete}
          />
        )}

        <Table.Td>
          <Group gap={'sm'}>
            <Text size="sm" fw={500}>
              {item?.name}
            </Text>
          </Group>
        </Table.Td>
        <Table.Td style={{ textAlign: 'center' }}>
          {item?.isActive ? (
            <Badge variant="light" color={'green'}>
              {t('active')}
            </Badge>
          ) : (
            <Badge variant="light" color={'red'}>
              {t('inactive')}
            </Badge>
          )}
        </Table.Td>
        <Table.Td>
          <Group gap={0} justify="center">
            <ActionIcon
              onClick={() => {
                opened ? close() : open();
                setIsEditing(true);
                setEditItem(item);
              }}
              variant="subtle"
            >
              <IconPencil size={16} stroke={1.5} />
            </ActionIcon>
            <ActionIcon
              color="red"
              onClick={() => {
                setOpened(true);
              }}
              variant="subtle"
            >
              <IconTrash size={16} stroke={1.5} />
            </ActionIcon>
          </Group>
        </Table.Td>
      </Table.Tr>
    );
  };

  useEffect(() => {
    if (isEditing) {
      form.setValues({
        name: editItem?.name,
        isActive: editItem?.isActive,
      });
    }
  }, [isEditing]);

  return (
    <>
      <Group
        style={{ justifyContent: 'space-between', alignItems: 'center' }}
        mb={15}
      >
        <Title>{t('departments')}</Title>
        {!opened && (
          <Button
            onClick={() => {
              open();
              form.reset();
            }}
          >
            {t('add_department')}
          </Button>
        )}
      </Group>

      <Drawer
        opened={opened}
        onClose={() => {
          close();
          setIsEditing(false);
          setEditItem(undefined);
        }}
        overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
      >
        <form
          onSubmit={form.onSubmit(async (values) => {
            try {
              if (isEditing) {
                await updateDepartment.mutateAsync({
                  id: editItem?.id as string,
                  ...values,
                });
                form.reset();
                showNotification({
                  message: t('update_department_success'),
                });
              } else {
                await postDepartment.mutateAsync(values);
                form.reset();
                showNotification({
                  message: t('add_department_success'),
                });
              }
            } catch (error) {
              const err = errorType(error);
              showNotification({
                title: t('error'),
                message: err,
                color: 'red',
              });
            } finally {
              close();
              setIsEditing(false);
            }
          })}
        >
          <TextInput
            label={t('department_name')}
            name="departmentName"
            withAsterisk
            placeholder={t('department_name_placeholder') as string}
            {...form.getInputProps('name')}
            mb={10}
          />

          {isEditing && (
            <Switch
              mt={'lg'}
              style={{ input: { cursor: 'pointer' } }}
              checked={form.values.isActive}
              label={t('department_enabled')}
              labelPosition="left"
              onChange={(e) => {
                form.setFieldValue('isActive', e.currentTarget.checked);
              }}
            />
          )}

          <Group mt={20}>
            <Button type="submit">{t('submit')}</Button>
          </Group>
        </form>
      </Drawer>

      <Flex mb={10}>
        {searchComponent(t('search_department') as string)}
        <Flex style={{ width: '210px' }}>
          {filterComponent(
            [
              { value: 'true', label: t('active') },
              { value: 'false', label: t('inactive') },
            ],
            t('department_status'),
            'IsActive'
          )}
        </Flex>
      </Flex>

      {getDepartment.data && getDepartment.data.totalCount > 0 ? (
        <Paper>
          <Table
            striped
            highlightOnHover
            withTableBorder
            withColumnBorders
            style={{ marginTop: '10px' }}
          >
            <Table.Thead>
              <Table.Tr>
                <Table.Th>{t('name')}</Table.Th>
                <Table.Th>
                  <Text ta="center">{t('department_status')}</Text>
                </Table.Th>
                <Table.Th>
                  <Text ta="center">{t('actions')}</Text>
                </Table.Th>
              </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
              {getDepartment.data?.items.map((item: any) => (
                <Rows item={item} key={item.id} />
              ))}
            </Table.Tbody>
          </Table>
        </Paper>
      ) : (
        <Box mt={10}>{t('no_department')}</Box>
      )}

      {getDepartment.data &&
        pagination(
          getDepartment.data?.totalPage,
          getDepartment.data?.items.length
        )}
    </>
  );
};

export default withSearchPagination(Department);
