import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import {
  Box,
  Button,
  Container,
  Drawer,
  FocusTrap,
  Group,
  Loader,
  SimpleGrid,
  Space,
  TextInput,
  Title,
} from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { useDisclosure } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import errorType from '@utils/services/axiosError';
import { useAddPool, usePools } from '@utils/services/poolService';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import * as Yup from 'yup';
import PoolCard from './Components/PoolCard';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string()
      .max(100, t('name_length_validation') as string)
      .required(t('pool_name_required') as string),
  });
};

const MCQPool = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const pools = usePools(searchParams);
  const { mutateAsync, isLoading } = useAddPool(searchParams);
  const [opened, { open, close }] = useDisclosure(false);
  const { t } = useTranslation();
  const navigate = useNavigate();
  const form = useForm({
    initialValues: {
      name: '',
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const onSubmitForm = async ({ name }: { name: string }) => {
    try {
      const res = await mutateAsync(name);
      form.reset();
      navigate(res.data.slug + '/questions');
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: 'red' });
    }
  };
  return (
    <Container fluid>
      <Group
        style={{ justifyContent: 'space-between', alignItems: 'center' }}
        mb={15}
      >
        <Title>{t('mcq_pools')}</Title>

        <Button onClick={open}>{t('create_pool')}</Button>
      </Group>
      <Drawer
        opened={opened}
        onClose={close}
        title={t('create_pool')}
        overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
      >
        <Box>
          <form onSubmit={form.onSubmit(onSubmitForm)}>
            <FocusTrap active={true}>
              <TextInput
                data-autofocus
                label={t('pool_name')}
                placeholder={t('enter_pool_name') as string}
                withAsterisk
                {...form.getInputProps('name')}
              />
            </FocusTrap>
            <Space h="md" />
            <Group justify="flex-end">
              <Button type="submit" loading={isLoading}>
                {t('create')}
              </Button>
            </Group>
          </form>
        </Box>
      </Drawer>
      <Box>{searchComponent(t('search_pools') as string)}</Box>
      {pools.isLoading && <Loader />}

      <Box mt={20}>
        {pools.isSuccess && (
          <>
            <SimpleGrid spacing={10} cols={{ sx: 1, sm: 2, md: 3, lg: 4 }}>
              {pools.data.items.length >= 1 &&
                pools.data?.items.map((x) => (
                  <PoolCard search={searchParams} pool={x} key={x.id} />
                ))}
              {pools.data?.items.length < 1 && (
                <Box mt={10}>{t('no_pools')}</Box>
              )}
            </SimpleGrid>
          </>
        )}
        {pools.data &&
          pagination(pools.data.totalPage, pools.data.items.length)}
      </Box>
    </Container>
  );
};

export default withSearchPagination(MCQPool);
