import withSearchPagination, {
  IWithSearchPagination,
} from '@hoc/useSearchPagination';
import {
  Box,
  Button,
  Container,
  Group,
  Title,
  Loader,
  SimpleGrid,
  Drawer,
  Space,
} from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { useDisclosure } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import errorType from '@utils/services/axiosError';
import { useAddPool, usePools } from '@utils/services/poolService';
import PoolCard from './Components/PoolCard';
import * as Yup from 'yup';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router';
import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t('pool_name_required') as string),
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
        sx={{ justifyContent: 'space-between', alignItems: 'center' }}
        mb={15}
      >
        <Title>{t('mcq_pools')}</Title>

        <Button onClick={open}>{t('create_pool')}</Button>
      </Group>
      <Drawer
        opened={opened}
        onClose={close}
        title={t('create_pool')}
        overlayProps={{ opacity: 0.5, blur: 4 }}
      >
        <Box>
          <form onSubmit={form.onSubmit(onSubmitForm)}>
            <CustomTextFieldWithAutoFocus
              label={t('pool_name')}
              placeholder={t('enter_pool_name') as string}
              name="name"
              withAsterisk
              {...form.getInputProps('name')}
            />
            <Space h="md" />
            <Group position="right">
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
            <SimpleGrid
              cols={1}
              spacing={10}
              breakpoints={[
                { minWidth: 'sx', cols: 1 },
                { minWidth: 'sm', cols: 2 },
                { minWidth: 'md', cols: 3 },
                { minWidth: 1280, cols: 3 },
                { minWidth: 1780, cols: 4 },
              ]}
            >
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
