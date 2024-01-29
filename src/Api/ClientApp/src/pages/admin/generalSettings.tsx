import ThumbnailEditor from '@components/Ui/ThumbnailEditor';
import { BrandingContext } from '@context/BrandingThemeContext';
import useCustomForm from '@hooks/useCustomForm';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import {
  ActionIcon,
  Button,
  ColorInput,
  Container,
  Text,
  TextInput,
  Textarea,
} from '@mantine/core';
import { createFormContext, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { IconRefresh } from '@tabler/icons-react';
import { PHONE_VALIDATION } from '@utils/constants';
import {
  useGeneralSetting,
  useUpdateGeneralSetting,
} from '@utils/services/adminService';
import errorType from '@utils/services/axiosError';
import { useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    companyName: Yup.string().required(t('company_name_required') as string),
    companyAddress: Yup.string().required(
      t('company_address_required') as string
    ),
    companyContactNumber: Yup.string()
      .required(t('contact_number_required') as string)
      .matches(PHONE_VALIDATION, {
        message: t('enter_valid_phone'),
        excludeEmptyString: true,
      }),
    emailSignature: Yup.string().required(t('signature_required') as string),
    logoUrl: Yup.string().required(t('company_logo_required') as string),
  });
};

interface ICompanySettings {
  thumbnail?: string;
  companyName: string;
  companyAddress: string;
  companyContactNumber: string;
  emailSignature: string;
  logoUrl?: string | undefined;
}

const [FormProvider, useFormContext, useForm] =
  createFormContext<ICompanySettings>();

const GeneralSettings = () => {
  const cForm = useCustomForm();
  const generalSettings = useGeneralSetting();
  const updateGeneral = useUpdateGeneralSetting(generalSettings.data?.data.id);
  const data = generalSettings.data?.data;
  const { t } = useTranslation();
  const context = useContext(BrandingContext);
  const [color, setColor] = useState(context?.brandingTheme ?? '#0E99AC');

  const toggleBrandingTheme = context?.toggleBrandingTheme;

  const handleColorChange = (value: string) => {
    setColor(value);
  };

  useEffect(() => {
    form.setValues({
      logoUrl: data?.logoUrl || '',
      companyName: data?.companyName || '',
      companyAddress: data?.companyAddress || '',
      companyContactNumber: data?.companyContactNumber || '',
      emailSignature: data?.emailSignature || '',
    });
  }, [generalSettings.isSuccess]);

  const form = useForm({
    initialValues: {
      logoUrl: '',
      companyName: '',
      companyAddress: '',
      companyContactNumber: '',
      emailSignature: '',
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const handleSubmit = async (values: any) => {
    try {
      values = {
        ...values,
        customConfiguration: JSON.stringify({ accent: color }),
      };
      await updateGeneral.mutateAsync(values);
      showNotification({
        title: t('successful'),
        message: t('setting_updated'),
      });
      window.scrollTo(0, 0);
      toggleBrandingTheme && toggleBrandingTheme(color);
    } catch (error) {
      const err = errorType(error);

      showNotification({
        message: err,
        color: 'red',
      });
    }
  };

  return (
    <FormProvider form={form}>
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Container
          size={450}
          sx={{
            marginLeft: '0px',
          }}
        >
          {t('company_logo')} <sup style={{ color: 'red' }}>*</sup>
          <ThumbnailEditor
            formContext={useFormContext}
            label={t('image') as string}
            FormField="logoUrl"
            currentThumbnail={data?.logoUrl}
          />
          <Text c="dimmed" size="xs">
            {t('image_dimension')}
          </Text>
          <TextInput
            label={t('company_name')}
            withAsterisk
            mt={20}
            name="companyName"
            placeholder={t('enter_company_name') as string}
            {...form.getInputProps('companyName')}
          />
          <TextInput
            mt={10}
            label={t('company_address')}
            withAsterisk
            name="companyAddress"
            placeholder={t('enter_company_address') as string}
            {...form.getInputProps('companyAddress')}
          />
          <TextInput
            mt={10}
            label={t('company_contact')}
            withAsterisk
            type={'number'}
            name="ContactNumber"
            placeholder={t('enter_company_contact') as string}
            {...form.getInputProps('companyContactNumber')}
          />
          <ColorInput
            value={color}
            onChange={handleColorChange}
            placeholder="Pick color"
            label="Branding color"
            // defaultValue="#0E99AC"
            rightSection={
              <ActionIcon onClick={() => handleColorChange('#0E99AC')}>
                <IconRefresh size="1rem" />
              </ActionIcon>
            }
          />
          <Textarea
            mt={10}
            label={t('mail_signature')}
            withAsterisk
            name="signature"
            placeholder={t('enter_mail_signature') as string}
            {...form.getInputProps('emailSignature')}
          />
          <Button
            disabled={!cForm?.isReady}
            mt={10}
            type="submit"
            loading={updateGeneral.isLoading}
          >
            {t('submit')}
          </Button>
        </Container>
      </form>
    </FormProvider>
  );
};

export default GeneralSettings;
