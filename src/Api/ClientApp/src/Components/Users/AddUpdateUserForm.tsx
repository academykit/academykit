import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import { Button, Grid, Group, Select, Switch, TextInput } from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { IconInfoCircle } from '@tabler/icons';
import { PHONE_VALIDATION } from '@utils/constants';
import { UserRole, UserStatus } from '@utils/enums';
import queryStringGenerator from '@utils/queryStringGenerator';
import { useDepartmentSetting } from '@utils/services/adminService';
import errorType from '@utils/services/axiosError';
import { IUserProfile } from '@utils/services/types';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import classes from './styles/userForm.module.css';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    email: Yup.string()
      .trim()
      .email(t('invalid_email') as string)
      .required(t('email_required') as string),
    firstName: Yup.string()
      .trim()
      .max(100, t('first_name_character_required') as string)
      .required(t('first_name_required') as string),
    lastName: Yup.string()
      .trim()
      .max(100, t('last_name_character_required') as string)
      .required(t('last_name_required') as string),
    middleName: Yup.string()
      .max(100, t('middle_name_character_required') as string)
      .trim()
      .nullable()
      .notRequired(),
    role: Yup.string()
      .oneOf(['1', '2', '3', '4'], t('role_required') as string)
      .required(t('role_required') as string),
    mobileNumber: Yup.string()
      .trim()
      .nullable()
      .matches(PHONE_VALIDATION, {
        message: t('enter_valid_phone'),
        excludeEmptyString: true,
      }),
  });
};

const AddUpdateUserForm = ({
  setOpened,
  opened,
  isEditing,
  apiHooks,
  item, // currentTab,
}: {
  setOpened: (b: boolean) => void;
  opened: boolean;
  isEditing: boolean;
  apiHooks: any;
  item?: IUserProfile;
}) => {
  const { t } = useTranslation();
  const form = useForm<IUserProfile>({
    initialValues: item,
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const { data: department } = useDepartmentSetting(
    queryStringGenerator({
      search: '',
      size: 200,
      IsActive: true,
    })
  );

  useEffect(() => {
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    form.setFieldValue('role', item?.role.toString() ?? '4');
    item?.departmentId &&
      form.setFieldValue('departmentId', item?.departmentId.toString() ?? '');
    form.setFieldValue(
      'isActive',
      item?.status === UserStatus.Active || item?.status === UserStatus.Pending
        ? true
        : false
    );
  }, [isEditing]);

  const onSubmitForm = async (data: typeof form.values) => {
    try {
      if (!isEditing) {
        await apiHooks.mutateAsync({
          ...data,
          email: data?.email?.toLowerCase(),
          role: Number(data?.role),
        });
      } else {
        const userData = { ...data, email: data?.email?.toLowerCase() };
        const status =
          item?.status === UserStatus.Pending
            ? UserStatus.Pending
            : data.isActive
            ? UserStatus.Active
            : UserStatus.InActive;
        data = { ...userData, role: Number(data?.role), status };
        await apiHooks.mutateAsync({ id: item?.id as string, data });
      }
      showNotification({
        message: isEditing ? t('user_edited_success') : t('user_added_success'),
        title: t('successful'),
      });
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        title: t('error'),
        color: 'red',
      });
    }
    setOpened(!opened);
  };

  return (
    <form onSubmit={form.onSubmit(onSubmitForm)}>
      <Grid align={'center'}>
        <Grid.Col span={{ xs: 6, lg: 4 }} mt={7}>
          <CustomTextFieldWithAutoFocus
            label={t('ID')}
            placeholder={t('user_id') as string}
            {...form.getInputProps('memberId')}
            name="memberId"
          />
        </Grid.Col>
        <Grid.Col span={{ xs: 6, lg: 4 }}>
          <TextInput
            styles={{ error: { position: 'absolute' } }}
            withAsterisk
            label={t('firstname')}
            placeholder={t('user_firstname') as string}
            name="firstName"
            {...form.getInputProps('firstName')}
          />
        </Grid.Col>
        <Grid.Col span={{ xs: 6, lg: 4 }} mt={5}>
          <TextInput
            label={t('middlename')}
            placeholder={t('user_middlename') as string}
            {...form.getInputProps('middleName')}
          />
        </Grid.Col>
        <Grid.Col span={{ xs: 6, lg: 4 }}>
          <TextInput
            styles={{ error: { position: 'absolute' } }}
            withAsterisk
            label={t('lastname')}
            placeholder={t('user_lastname') as string}
            {...form.getInputProps('lastName')}
          />
        </Grid.Col>
        <Grid.Col span={{ xs: 6, lg: 4 }}>
          <TextInput
            styles={{ error: { position: 'absolute' } }}
            withAsterisk
            label={t('email')}
            type="email"
            placeholder={t('user_email') as string}
            {...form.getInputProps('email')}
          />
        </Grid.Col>
        <Grid.Col span={{ xs: 6, lg: 4 }} mt={5}>
          <TextInput
            styles={{ error: { position: 'absolute' } }}
            label={t('mobilenumber')}
            placeholder={t('user_phone_number') as string}
            {...form.getInputProps('mobileNumber')}
          />
        </Grid.Col>
        <Grid.Col span={{ xs: 6, lg: 4 }} mt={5}>
          <TextInput
            label={t('profession')}
            placeholder={t('user_profession') as string}
            {...form.getInputProps('profession')}
          />
        </Grid.Col>

        <Grid.Col span={{ xs: 6, lg: 4 }}>
          <Select
            withAsterisk
            error={t('user_role_pick')}
            label={t('user_role')}
            placeholder={t('user_role_pick') as string}
            data={[
              { value: UserRole.Admin.toString(), label: t('Admin') as string },
              {
                value: UserRole.Trainer.toString(),
                label: t('Trainer') as string,
              },
              {
                value: UserRole.Trainee.toString(),
                label: t('Trainee') as string,
              },
            ]}
            {...form.getInputProps('role')}
          />
        </Grid.Col>
        <Grid.Col span={{ xs: 6, lg: 4 }} mt={5}>
          <Select
            label={t('department')}
            placeholder={t('pick_department') as string}
            searchable
            nothingFoundMessage={t('no_department')}
            data={
              department
                ? department.items.map((x) => ({
                    label: x.name,
                    value: x.id,
                  }))
                : ['']
            }
            {...form.getInputProps('departmentId')}
          />
          {department && department?.items.length < 1 && (
            <span className={classes.departmentInfo}>
              <IconInfoCircle size={12} />
              {t('no_active_department')}
            </span>
          )}
        </Grid.Col>

        {isEditing && item?.status !== UserStatus.Pending && (
          <Grid.Col span={{ xs: 6, lg: 4 }}>
            <Switch
              label={t('user_status')}
              {...form.getInputProps('isActive', { type: 'checkbox' })}
            />
          </Grid.Col>
        )}
      </Grid>

      <Group justify="flex-end" mt="md">
        <Button type="submit" loading={apiHooks.isLoading}>
          {t('submit')}
        </Button>
      </Group>
    </form>
  );
};
export default AddUpdateUserForm;
