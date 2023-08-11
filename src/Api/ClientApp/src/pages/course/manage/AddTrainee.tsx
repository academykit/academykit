/* eslint-disable @typescript-eslint/no-unused-vars */
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import {
  Avatar,
  Box,
  Button,
  Group,
  Loader,
  MultiSelect,
  Text,
} from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import { useUsers } from '@utils/services/adminService';
import errorType from '@utils/services/axiosError';
import { useAddTrainee } from '@utils/services/courseService';
import { INotMember } from '@utils/services/groupService';
import { forwardRef, useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';
import * as Yup from 'yup';

interface IAddTrainee {
  email: string[];
}

interface ItemProps extends React.ComponentPropsWithoutRef<'div'> {
  fullName: string;
  imageUrl: string;
  email: string;
}

// eslint-disable-next-line react/display-name
const SelectUserItem = forwardRef<HTMLDivElement, ItemProps>(
  ({ fullName, imageUrl, email, ...others }: ItemProps, ref) => (
    <div ref={ref} {...others}>
      <Group noWrap>
        <Avatar src={imageUrl} />
        <div>
          <Text>{fullName}</Text>
          <Text size="xs" color="dimmed">
            {email}
          </Text>
        </div>
      </Group>
    </div>
  )
);

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    email: Yup.array().min(1, t('one_email_required') as string),
  });
};

const AddTrainee = ({
  onCancel,
  searchParams,
}: {
  onCancel: () => void;
  searchParams: string;
}) => {
  const { t } = useTranslation();
  const { courseId } = useParams();
  const [search, setSearch] = useState('');
  //   const [data, setData] = useState<INotMember[]>([]);

  const addTrainee = useAddTrainee(courseId as string);
  const nonTrainee = useUsers(`search=${search}`);
  const ref = useRef<HTMLInputElement>(null);

  console.log(nonTrainee?.data?.items);

  const form = useForm<IAddTrainee>({
    initialValues: {
      email: [],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  //   useEffect(() => {
  //     if (nonTrainee.isSuccess) {
  //       const t = nonTrainee.data?.items?.map((x) => {
  //         return {
  //           ...x,
  //           label: `${x.fullName}(${x.email})`,
  //           value: x.email,
  //         };
  //       });
  //       const mergedData = [...data, ...t];
  //       setData([
  //         ...new Map(mergedData.map((item) => [item['email'], item])).values(),
  //       ]);
  //     }
  //   }, [nonTrainee.isSuccess]);

  const onSubmitForm = async (email: string[]) => {
    try {
      const response: any = await addTrainee.mutateAsync({
        courseId: courseId as string,
        data: { emails: email },
      });
      if (response?.data?.httpStatusCode === 206) {
        return showNotification({
          message: response?.data?.message,
        });
      }
      showNotification({
        message: response.data?.message ?? t('add_trainee_success'),
      });
      onCancel();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  return (
    <Box>
      <form onSubmit={form.onSubmit(({ email }) => onSubmitForm(email))}>
        <MultiSelect
          tabIndex={0}
          autoComplete="off"
          placeholder={t('email_address') as string}
          ref={ref}
          searchable
          data={[]}
          mb={10}
          label={t('email_address')}
          itemComponent={SelectUserItem}
          withAsterisk
          name="email"
          size="md"
          nothingFound={
            nonTrainee.isLoading ? <Loader /> : <Box>{t('User Not found')}</Box>
          }
          getCreateLabel={(query) => `+ Create ${query}`}
          onSearchChange={(d) => {
            setSearch(d);
          }}
          {...form.getInputProps('email')}
        />

        <Group mt={'lg'} position="right">
          <Button mr={10} type="submit" size="md">
            {t('submit')}
          </Button>
        </Group>
      </form>
    </Box>
  );
};

export default AddTrainee;
