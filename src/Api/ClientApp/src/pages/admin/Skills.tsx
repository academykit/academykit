import DeleteModal from '@components/Ui/DeleteModal';
import EmptyRow from '@components/Ui/EmptyRow';
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
  Grid,
  Group,
  Modal,
  Paper,
  ScrollArea,
  Switch,
  Table,
  Text,
  TextInput,
  Textarea,
  Title,
} from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { useDisclosure } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconPencil, IconTrash, IconUsers } from '@tabler/icons';
import errorType from '@utils/services/axiosError';
import {
  ISkill,
  useDeleteSkill,
  usePostDepartmentSetting,
  useSkills,
  useUpdateSkill,
} from '@utils/services/skillService';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import SkillUser from './Component/skills/SkillUser';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    skillName: Yup.string()
      .max(100, t('name_length_validation') as string)
      .required(t('skill_name_required') as string),
    description: Yup.string()
      .nullable()
      .max(200, t('description_length_validation') as string),
  });
};

const Skills = ({
  searchComponent,
  filterComponent,
  searchParams,
  pagination,
}: IWithSearchPagination) => {
  const { t } = useTranslation();
  const [opened, { open, close }] = useDisclosure(false);
  const [isEditing, setIsEditing] = useState(false);
  const skillData = useSkills(searchParams);
  const postSkill = usePostDepartmentSetting();
  const updateSkill = useUpdateSkill();
  const deleteSkill = useDeleteSkill();
  const [editItem, setEditItem] = useState<ISkill>();

  const form = useForm({
    initialValues: {
      skillName: '',
      isActive: true,
      description: '',
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  useEffect(() => {
    if (isEditing) {
      form.setValues({
        skillName: editItem?.skillName,
        isActive: editItem?.isActive,
        description: editItem?.description,
      });
    }
  }, [isEditing]);

  const handleSubmit = async (values: typeof form.values) => {
    try {
      await postSkill.mutateAsync(values);
      form.reset();
      close();
      showNotification({
        message: t('add_skill_success'),
      });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: t('error'),
        message: err,
        color: 'red',
      });
    }
  };
  const handleEditSubmit = async (values: typeof form.values) => {
    try {
      await updateSkill.mutateAsync({
        id: (editItem?.id as string) ?? '',
        data: values,
      });
      form.reset();
      showNotification({
        message: t('edit_skill_success'),
      });
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
  };

  const Rows = ({ item }: { item: ISkill }) => {
    const [opened, setOpened] = useState(false);
    const [openedUserModal, { open: openUserModal, close: closeUserModal }] =
      useDisclosure(false);

    const handleDelete = async () => {
      try {
        await deleteSkill.mutateAsync(item.id);
        showNotification({
          title: t('successful'),
          message: t('skill_deleted'),
        });
      } catch (error) {
        const err = errorType(error);
        showNotification({
          message: err,
          color: 'red',
        });
      }
      setOpened(false);
    };

    return (
      <>
        {opened && (
          <DeleteModal
            title={`${t('sure_to_delete')} "${item?.skillName}" ${t('skill')}?`}
            open={opened}
            onClose={() => setOpened(false)}
            onConfirm={handleDelete}
          />
        )}
        <Modal
          opened={openedUserModal}
          onClose={closeUserModal}
          title={t('users')}
        >
          <ScrollArea.Autosize mah={300} maw={400} mx="auto">
            {item.userModel.map((user, index) => (
              <SkillUser key={index} user={user} />
            ))}
          </ScrollArea.Autosize>
        </Modal>
        <Table.Tr>
          <Table.Td>{item.skillName}</Table.Td>
          <Table.Td maw={400} w={400}>
            <Text truncate="end">{item.description}</Text>
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
            <Flex justify={'center'}>
              <ActionIcon onClick={openUserModal} variant="subtle" c={'gray'}>
                <IconUsers size={18} />
              </ActionIcon>
            </Flex>
          </Table.Td>
          <Table.Td>
            <Group gap={0} justify="center">
              <ActionIcon
                onClick={() => {
                  opened ? close() : open();
                  setIsEditing(true);
                  setEditItem(item);
                }}
                color="gray"
                variant="subtle"
              >
                <IconPencil size={16} stroke={1.5} />
              </ActionIcon>
              <ActionIcon
                color="red"
                variant="subtle"
                onClick={() => {
                  setOpened(true);
                }}
              >
                <IconTrash size={16} stroke={1.5} />
              </ActionIcon>
            </Group>
          </Table.Td>
        </Table.Tr>
      </>
    );
  };

  return (
    <>
      <section>
        <Group
          style={{ justifyContent: 'space-between', alignItems: 'center' }}
          mb={15}
        >
          <Title>{t('skill')}</Title>
          <Button
            onClick={() => {
              open();
              form.reset();
            }}
          >
            {t('add_skill')}
          </Button>
        </Group>

        <Drawer
          opened={opened}
          onClose={() => {
            close();
            setIsEditing(false);
          }}
          overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
        >
          <Box>
            <form
              onSubmit={form.onSubmit(
                isEditing ? handleEditSubmit : handleSubmit
              )}
            >
              <Grid>
                <Grid.Col span={{ xs: 6, lg: 12 }}>
                  <TextInput
                    label={t('skill_name')}
                    withAsterisk
                    placeholder={t('skill_name_placeholder') as string}
                    {...form.getInputProps('skillName')}
                  />
                </Grid.Col>
                <Grid.Col span={{ xs: 12, lg: 12 }}>
                  <Textarea
                    label={t('description')}
                    placeholder={t('description_placeholder') as string}
                    minRows={2}
                    {...form.getInputProps('description')}
                  />
                </Grid.Col>
                {isEditing && (
                  <Grid.Col span={{ xs: 6, lg: 12 }}>
                    <Switch
                      style={{ input: { cursor: 'pointer' } }}
                      checked={form.values.isActive}
                      label={t('skill_enabled')}
                      labelPosition="left"
                      onChange={(e) => {
                        form.setFieldValue('isActive', e.currentTarget.checked);
                      }}
                    />
                  </Grid.Col>
                )}
              </Grid>
              <Group mt={20}>
                <Button type="submit">{t('submit')}</Button>
              </Group>
            </form>
          </Box>
        </Drawer>

        <Flex mb={10}>
          {searchComponent(t('search_skill') as string)}
          <Flex style={{ width: '210px' }}>
            {filterComponent(
              [
                { value: 'true', label: t('active') },
                { value: 'false', label: t('inactive') },
              ],
              t('skill_status'),
              'IsActive'
            )}
          </Flex>
        </Flex>

        <Paper>
          <ScrollArea>
            <Table
              striped
              highlightOnHover
              withTableBorder
              withColumnBorders
              style={{ marginTop: '10px' }}
            >
              <Table.Thead>
                <Table.Tr>
                  <Table.Th>
                    <Text>{t('skill_name')}</Text>
                  </Table.Th>
                  <Table.Th>
                    <Text>{t('description')}</Text>
                  </Table.Th>
                  <Table.Th>
                    <Text ta="center">{t('isActive')}</Text>
                  </Table.Th>
                  <Table.Th>
                    <Text ta="center">{t('users')}</Text>
                  </Table.Th>
                  <Table.Th style={{ minWidth: '90px' }}>
                    <Text ta="center">{t('actions')}</Text>
                  </Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
                {skillData.data && skillData.data?.totalCount > 0 ? (
                  skillData.data?.items.map((item: any) => (
                    <Rows item={item} key={item.id} />
                  ))
                ) : (
                  <EmptyRow colspan={5} message="no_skill" />
                )}
              </Table.Tbody>
            </Table>
          </ScrollArea>

          {skillData.data &&
            pagination(skillData.data?.totalPage, skillData.data?.items.length)}
        </Paper>
      </section>
    </>
  );
};

export default withSearchPagination(Skills);
