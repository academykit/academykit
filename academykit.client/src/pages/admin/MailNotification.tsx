import DeleteModal from '@components/Ui/DeleteModal';
import EmptyRow from '@components/Ui/EmptyRow';
import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import TextViewer from '@components/Ui/RichTextViewer';
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
  Select,
  Switch,
  Table,
  Text,
  TextInput,
  Title,
  Tooltip,
} from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { useDisclosure, useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import {
  IconAsteriskSimple,
  IconFileSearch,
  IconInfoCircle,
  IconPencil,
  IconSend,
  IconTrash,
} from '@tabler/icons-react';
import { MailType } from '@utils/enums';
import {
  IMailNotification,
  useDeleteMailNotification,
  useMailNotification,
  useMailPreview,
  usePostMailNotification,
  useTestEmail,
  useUpdateMailNotification,
} from '@utils/services/adminService';
import errorType from '@utils/services/axiosError';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import VariableHelpTable from './Component/VariableHelpTable';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    mailName: Yup.string().required(t('mail_name_required') as string),
    mailSubject: Yup.string().required(t('mail_subject_required') as string),
    mailType: Yup.string().required(t('mail_type_required') as string),
    mailMessage: Yup.string()
      .required(t('mail_message_required') as string)
      .test(
        'check-for-empty-p-tags',
        t('mail_message_required') as string,
        (value) => {
          return value !== '<p></p>';
        }
      ),
  });
};
const emailSchema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    emailAddress: Yup.string()
      .email()
      .required(t('email_validation') as string),
  });
};

const MailNotification = ({
  searchComponent,
  filterComponent,
  searchParams,
  pagination,
}: IWithSearchPagination) => {
  const { t } = useTranslation();
  const [isEditing, setIsEditing] = useState(false);
  const [helpModal, toggleHelpModal] = useToggle();
  const [opened, { open, close }] = useDisclosure(false);
  const getMailNotification = useMailNotification(searchParams);
  const updateMailNotification = useUpdateMailNotification();
  const postMailNotification = usePostMailNotification();
  const deleteMailNotification = useDeleteMailNotification();
  const testEmail = useTestEmail();
  const [editItem, setEditItem] = useState<IMailNotification>();

  const getMailType = () => {
    return Object.entries(MailType)
      .splice(0, Object.entries(MailType).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
  };

  const form = useForm({
    initialValues: {
      mailName: '',
      mailSubject: '',
      mailMessage: '',
      mailType: '',
      isActive: true,
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  useEffect(() => {
    if (isEditing) {
      form.setValues({
        mailName: editItem?.mailName,
        mailSubject: editItem?.mailSubject,
        isActive: editItem?.isActive,
        mailMessage: editItem?.mailMessage,
        mailType: editItem?.mailType.toString(),
      });
    }
  }, [isEditing]);

  const handleEditSubmit = async (values: typeof form.values) => {
    try {
      await updateMailNotification.mutateAsync({
        id: editItem?.id as string,
        data: { ...values, mailType: Number(values.mailType) },
      });
      form.reset();
      showNotification({
        message: t('edit_mail_success'),
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

  const handleSubmit = async (values: typeof form.values) => {
    try {
      await postMailNotification.mutateAsync({
        ...values,
        mailType: Number(values.mailType),
      });
      form.reset();
      showNotification({
        message: t('post_mail_success'),
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
    }
  };

  const Rows = ({ item }: { item: IMailNotification }) => {
    const mailReview = useMailPreview(item.id);
    const [testEmailOpened, { open: openEmailModal, close: closeEmailModal }] =
      useDisclosure(false);
    const [previewModal, setPreviewModal] = useState(false);
    const [opened, setOpened] = useState(false);

    const emailForm = useForm({
      initialValues: {
        emailAddress: '',
      },
      validate: yupResolver(emailSchema()),
    });
    useFormErrorHooks(emailForm);

    const handleEmailSubmit = async (values: typeof emailForm.values) => {
      try {
        await testEmail.mutateAsync({
          id: item.id,
          data: values,
        });
        showNotification({
          message: t('Email sent successfully!'),
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

    const handleDelete = async () => {
      try {
        await deleteMailNotification.mutateAsync(item.id);
        showNotification({
          title: t('successful'),
          message: t('mail_deleted'),
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
            title={`${t('sure_to_delete')} "${item?.mailName}" ${t('mail')}?`}
            open={opened}
            onClose={() => setOpened(false)}
            onConfirm={handleDelete}
          />
        )}

        <Modal
          opened={testEmailOpened}
          onClose={() => {
            closeEmailModal();
            emailForm.reset();
          }}
          title={t('test_email')}
        >
          <form onSubmit={emailForm.onSubmit(handleEmailSubmit)}>
            <TextInput
              withAsterisk
              label={t('email')}
              placeholder={t('email_placeholder') as string}
              {...emailForm.getInputProps('emailAddress')}
            />
            <Button type="submit" mt={15}>
              {t('send')}
            </Button>
          </form>
        </Modal>

        <Modal
          size={'xl'}
          opened={previewModal}
          onClose={() => {
            setPreviewModal(false);
          }}
          title={t('mail_preview')}
        >
          <TextViewer content={mailReview.data ?? ''} />
        </Modal>

        <Table.Tr>
          <Table.Td style={{ maxWidth: '190px' }}>
            <Text truncate>{item.mailName}</Text>
            <Text truncate>{item.mailSubject}</Text>
          </Table.Td>
          <Table.Td>
            <Text truncate>{t(`${MailType[item.mailType]}`)}</Text>
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
            <Group gap={'sm'} justify="center">
              <Tooltip label={t('edit')}>
                <ActionIcon
                  onClick={() => {
                    open();
                    setEditItem(item);
                    setIsEditing(true);
                  }}
                  variant="subtle"
                  color="gray"
                >
                  <IconPencil size={16} stroke={1.5} />
                </ActionIcon>
              </Tooltip>

              <Tooltip label={t('preview')}>
                <ActionIcon
                  onClick={async () => {
                    await mailReview.refetch();
                    setPreviewModal(true);
                  }}
                  variant="subtle"
                  color="gray"
                >
                  <IconFileSearch size={16} stroke={1.5} />
                </ActionIcon>
              </Tooltip>

              <Tooltip label={t('test_email')}>
                <ActionIcon
                  onClick={() => {
                    openEmailModal();
                  }}
                  variant="subtle"
                  color="gray"
                >
                  <IconSend size={16} stroke={1.5} />
                </ActionIcon>
              </Tooltip>

              <ActionIcon
                onClick={() => {
                  setOpened(true);
                }}
                variant="subtle"
              >
                <IconTrash color="red" size={16} stroke={1.5} />
              </ActionIcon>
            </Group>
          </Table.Td>
        </Table.Tr>
      </>
    );
  };

  return (
    <>
      <Modal
        title={t('modal_variables')}
        opened={helpModal}
        onClose={toggleHelpModal}
        size={'xl'}
      >
        <ScrollArea h={690} scrollHideDelay={0}>
          <VariableHelpTable />
        </ScrollArea>
      </Modal>
      <section>
        <Group
          style={{ justifyContent: 'space-between', alignItems: 'center' }}
          mb={15}
        >
          <Group>
            <Title>{t('mail-notification')}</Title>
            <IconInfoCircle
              style={{ cursor: 'pointer' }}
              onClick={() => {
                toggleHelpModal();
              }}
            />
          </Group>
          <Button
            onClick={() => {
              open();
              form.reset();
            }}
          >
            {t('add_mail')}
          </Button>
        </Group>

        <Drawer
          opened={opened}
          onClose={() => {
            close();
            setIsEditing(false);
          }}
          overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
          size="xl"
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
                    label={t('name')}
                    withAsterisk
                    placeholder={t('name_placeholder') as string}
                    {...form.getInputProps('mailName')}
                  />
                </Grid.Col>
                <Grid.Col span={{ xs: 6, lg: 12 }}>
                  <TextInput
                    label={t('subject')}
                    withAsterisk
                    placeholder={t('subject_placeholder') as string}
                    {...form.getInputProps('mailSubject')}
                  />
                </Grid.Col>
                <Grid.Col span={{ xs: 6, lg: 12 }}>
                  <Select
                    withAsterisk
                    allowDeselect={false}
                    label={t('mail_type')}
                    placeholder={t('mail_type_placeholder') as string}
                    data={getMailType() ?? []}
                    {...form.getInputProps('mailType')}
                  />
                </Grid.Col>
                <Grid.Col span={{ xs: 12, lg: 12 }}>
                  <Text size="sm">
                    {t('message')} <IconAsteriskSimple color="red" size={10} />
                  </Text>
                  <RichTextEditor
                    error={form.errors?.mailMessage}
                    placeholder={t('message_placeholder') as string}
                    {...form.getInputProps('mailMessage')}
                  />
                </Grid.Col>
                {isEditing && (
                  <Grid.Col span={{ xs: 6, lg: 12 }} mt={10}>
                    <Switch
                      style={{ input: { cursor: 'pointer' } }}
                      checked={form.values.isActive}
                      label={t('mail_enabled')}
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
          {searchComponent(t('search_mail_notification') as string)}
          <Flex style={{ width: '210px' }}>
            {filterComponent(
              [
                { value: 'true', label: t('active') },
                { value: 'false', label: t('inactive') },
              ],
              t('mail_status'),
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
                    <Text>{t('name_subject')}</Text>
                  </Table.Th>
                  <Table.Th>
                    <Text>{t('mail_type')}</Text>
                  </Table.Th>
                  <Table.Th>
                    <Text ta="center">{t('isActive')}</Text>
                  </Table.Th>
                  <Table.Th style={{ minWidth: '180px' }}>
                    <Text ta="center">{t('actions')}</Text>
                  </Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
                {getMailNotification.data &&
                getMailNotification.data.totalCount > 0 ? (
                  getMailNotification.data?.items.map((item) => (
                    <Rows key={item.id} item={item} />
                  ))
                ) : (
                  <EmptyRow colspan={4} message="no_email_notifications" />
                )}
              </Table.Tbody>
            </Table>
          </ScrollArea>

          {getMailNotification.data &&
            pagination(
              getMailNotification.data?.totalPage,
              getMailNotification.data?.items.length
            )}
        </Paper>
      </section>
    </>
  );
};

export default withSearchPagination(MailNotification);
