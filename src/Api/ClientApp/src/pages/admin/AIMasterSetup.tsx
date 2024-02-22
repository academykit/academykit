import { Button, Container, Switch, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { useAIMaster, useUpdateAISetup } from '@utils/services/aiService';
import errorType from '@utils/services/axiosError';
import { t } from 'i18next';
import { useEffect } from 'react';

const AIMasterSetup = () => {
  const formData = useAIMaster();
  const updateAISetup = useUpdateAISetup();

  const form = useForm({
    initialValues: {
      key: '',
      isActive: false,
    },
  });

  useEffect(() => {
    form.setValues({
      key: formData.data?.key ?? '',
      isActive: formData.data?.isActive ?? false,
    });
  }, [formData.isSuccess]);

  const handleSubmit = async (values: typeof form.values) => {
    try {
      await updateAISetup.mutateAsync({
        data: values,
      });
      showNotification({
        message: t('update_ai_setup_success'),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        color: 'red',
        message: error,
      });
    }
  };

  return (
    <>
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Container
          size={450}
          style={{
            marginLeft: '0px',
          }}
        >
          <TextInput
            autoFocus
            mb={10}
            label={t('ai_key')}
            placeholder={t('enter_ai_key') as string}
            {...form.getInputProps('key')}
          />
          <Switch
            mb={10}
            label={t('isActive')}
            {...form.getInputProps('isActive', { type: 'checkbox' })}
          />
          <Button loading={updateAISetup.isLoading} type="submit">
            {t('submit')}
          </Button>
        </Container>
      </form>
    </>
  );
};

export default AIMasterSetup;
