import ThumbnailEditor from '@components/Ui/ThumbnailEditor';
import useAuth from '@hooks/useAuth';
import {
  ActionIcon,
  Box,
  Button,
  Card,
  Flex,
  Group,
  Badge,
  Image,
  Modal,
  Text,
  TextInput,
} from '@mantine/core';
import { DatePickerInput } from '@mantine/dates';
import { createFormContext, yupResolver } from '@mantine/form';
import { useToggle } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import { IconDownload, IconEdit, IconEye, IconTrash } from '@tabler/icons';
import downloadImage from '@utils/downloadImage';
import { UserRole } from '@utils/enums';
import errorType from '@utils/services/axiosError';
import {
  CertificateStatus,
  useAddCertificate,
  useDeleteCertificate,
  useGetExternalCertificate,
  useUpdateCertificate,
} from '@utils/services/certificateService';
import { useEffect, useState } from 'react';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';
import * as Yup from 'yup';
import useCustomForm from '@hooks/useCustomForm';
import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import moment from 'moment';
import DeleteModal from '@components/Ui/DeleteModal';

const [FormProvider, useFormContext, useForm] = createFormContext();

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t('certificate_name_required') as string),
    duration: Yup.number()
      .required(t('duration_in_hour') as string)
      .typeError(t('invalid_duration') as string),
    range: Yup.array()
      .min(2, t('start_end_date_required') as string)
      .of(Yup.string().required(t('start_end_date_required') as string))
      .test(
        // checking if array contains null values
        'not-null-values',
        t('start_end_date_required') as string,
        (arr) => {
          return arr !== undefined && arr.every((element) => element !== null);
        }
      ),
  });
};

const MyTrainingExternal = () => {
  const cForm = useCustomForm();
  const [showConfirmation, setShowConfirmation] = useToggle();
  const { id } = useParams();
  const addCertificate = useAddCertificate();
  const certificateList = useGetExternalCertificate(id ? false : true);
  const update = useUpdateCertificate();
  const certificateDelete = useDeleteCertificate();
  const [idd, setIdd] = useState<any>();
  const [updates, setUpdates] = useState(false);
  const [deleteCertificate, setDeleteCertificate] = useState(false);
  const [deleteCertificateId, setDeleteCertificateId] = useState('');
  const { t } = useTranslation();

  const form = useForm({
    initialValues: {
      name: '',
      duration: 0,
      location: '',
      institute: '',
      imageUrl: '',
      range: [],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  useEffect(() => {
    if (idd) {
      setShowConfirmation();
      const range = [idd?.startDate, idd?.endDate];
      form.setValues({
        name: idd?.name,
        duration: idd?.duration,
        location: idd?.location,
        institute: idd?.institute,
        imageUrl: idd?.imageUrl,
        range: [new Date(range[0]), new Date(range[1])],
      });
    }
  }, [idd]);

  const handleSubmit = async (data: any) => {
    data = {
      ...data,
      startDate: moment(data.range[0] + 'Z')
        .local()
        .toDate(),
      endDate: moment(data.range[1] + 'Z')
        .local()
        .toDate(),
    };
    try {
      if (updates) {
        await update.mutateAsync({ data, id: idd?.id });
      } else {
        await addCertificate.mutateAsync(data);
      }
      showNotification({
        message: updates
          ? t('training_certificate_edited')
          : t('training_certificate_added'),
      });
      form.reset();
    } catch (error) {
      form.reset();
      const err = errorType(error);
      showNotification({
        color: 'red',
        message: err,
      });
    }
    setShowConfirmation();
    setIdd(() => null);
    setUpdates(() => false);
  };

  const handleDelete = async () => {
    try {
      await certificateDelete.mutateAsync({ id: deleteCertificateId });
      showNotification({
        message: updates
          ? t('training_certificate_edited')
          : t('training_certificate_added'),
      });
      setDeleteCertificate(false);
      setDeleteCertificateId('');
    } catch (error) {
      form.reset();
      const err = errorType(error);
      showNotification({
        color: 'red',
        message: err,
      });
      setDeleteCertificate(false);
      setDeleteCertificateId('');
    }
  };

  const auth = useAuth();

  return (
    <div>
      <DeleteModal
        title={`Delete this certificate?`}
        open={deleteCertificate}
        onClose={() => setDeleteCertificate(false)}
        onConfirm={handleDelete}
      />
      <Modal
        title={t('add_certificate')}
        opened={showConfirmation}
        onClose={() => {
          setShowConfirmation();
          setIdd(null);
          setUpdates(false);
          form.reset();
        }}
        styles={{
          title: {
            fontWeight: 'bold',
          },
        }}
      >
        <FormProvider form={form}>
          {showConfirmation && (
            <form onSubmit={form.onSubmit(handleSubmit)}>
              <CustomTextFieldWithAutoFocus
                label={t('name')}
                name="name"
                placeholder={t('Name of Training') as string}
                withAsterisk
                {...form.getInputProps('name')}
              />
              <TextInput
                withAsterisk
                label={t('duration_hour')}
                placeholder={t('Duration of Training') as string}
                name="duration"
                {...form.getInputProps('duration')}
              />
              <DatePickerInput
                allowSingleDateInRange
                maxDate={new Date()}
                required
                valueFormat="MMM DD, YYYY"
                label={t('start_end_date')}
                placeholder={t('date_range') as string}
                type="range"
                {...form.getInputProps('range')}
              />
              <TextInput
                label={t('location')}
                placeholder={t('Location of Training') as string}
                name="location"
                {...form.getInputProps('location')}
              />
              <TextInput
                label={t('institute')}
                placeholder={t('Name of institute') as string}
                name="institute"
                {...form.getInputProps('institute')}
              />
              <Text>{t('certificate_image')}</Text>
              <ThumbnailEditor
                formContext={useFormContext}
                label={t('certificate_image') as string}
                FormField="imageUrl"
                currentThumbnail={idd?.imageUrl}
              />
              <Button
                disabled={!cForm?.isReady}
                type="submit"
                loading={addCertificate.isLoading}
              >
                {t('submit')}
              </Button>
            </form>
          )}
        </FormProvider>
      </Modal>

      <Group position="right">
        <Button onClick={() => setShowConfirmation()}>
          {t('add_certificate')}
        </Button>
      </Group>
      {certificateList.isSuccess && certificateList.data?.length <= 0 && (
        <Box>{t('no_external_training')}</Box>
      )}
      {certificateList.isSuccess &&
        certificateList.data.map((x) => (
          <Card key={x.id} withBorder mt={10}>
            <Flex justify={'space-between'}>
              <Box>
                <Flex>
                  <Text weight={'bold'}>
                    {x.name}
                    <Badge
                      color={
                        CertificateStatus[x.status] == 'Rejected'
                          ? 'red'
                          : 'cyan'
                      }
                      ml={20}
                    >
                      {t(`${CertificateStatus[x.status]}`)}
                    </Badge>
                  </Text>
                  {x.status !== CertificateStatus.Approved && (
                    <>
                      <ActionIcon
                        ml={5}
                        onClick={() => {
                          setIdd(x);
                          setUpdates(true);
                        }}
                      >
                        <IconEdit />
                      </ActionIcon>
                      <ActionIcon
                        ml={5}
                        onClick={() => {
                          setDeleteCertificateId(x.id);
                          setDeleteCertificate(true);
                        }}
                      >
                        <IconTrash color="red" />
                      </ActionIcon>
                    </>
                  )}
                </Flex>
                {/* <Text mt={5}>
                  {x?.startDate &&
                    `${t("from")} ${moment(x.startDate).format(
                      "MMM DD, YYYY"
                    )} ${t("to")} ${moment(x.endDate).format(
                      "MMM DD, YYYY"
                    )}, `}
                  {t("completed_in_about")} {x.duration} {t("hrs")}
                </Text> */}
                <Text>
                  {x.institute}
                  {x.location && `, ${x.location}`}
                </Text>
              </Box>
              <Box
                style={{ width: 150, marginTop: 'auto', marginBottom: 'auto' }}
              >
                {x.imageUrl && (
                  <div style={{ position: 'relative' }}>
                    <Image
                      src={x.imageUrl || ''}
                      radius="md"
                      style={{
                        opacity: '0.5',
                      }}
                    />
                    <div
                      style={{
                        position: 'absolute',
                        left: 0,
                        bottom: 0,
                        right: 0,
                        margin: 'auto',
                        top: 0,
                        width: '45px',
                        height: '30px',
                        display: 'flex',
                      }}
                    >
                      <ActionIcon
                        onClick={() => window.open(x.imageUrl)}
                        mr={10}
                      >
                        <IconEye color="black" />
                      </ActionIcon>
                      <ActionIcon
                        onClick={() =>
                          downloadImage(x.imageUrl, x.user.fullName ?? '')
                        }
                      >
                        <IconDownload color="black" />
                      </ActionIcon>
                    </div>
                  </div>
                )}
              </Box>
            </Flex>
            {auth?.auth &&
              auth.auth.role <= UserRole.Admin &&
              auth.auth.id !== x.user.id && (
                <Box mt={10}>
                  <Button>{t('approve')}</Button>
                  <Button ml={10} variant="outline" color={'red'}>
                    {t('reject')}
                  </Button>
                </Box>
              )}
          </Card>
        ))}
    </div>
  );
};

export default MyTrainingExternal;
