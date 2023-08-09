import {
  Avatar,
  Box,
  Button,
  Group,
  Loader,
  MultiSelect,
  Select,
  Text,
} from '@mantine/core';
import { useForm, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import errorType from '@utils/services/axiosError';
import {
  INotMember,
  useAddGroupMember,
  useGroupNotMember,
} from '@utils/services/groupService';
import { useEffect, useState, useRef, forwardRef } from 'react';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';
import * as Yup from 'yup';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import { useDepartmentSetting } from '@utils/services/adminService';

interface IAddMember {
  email: string[];
}

interface ItemProps extends React.ComponentPropsWithoutRef<'div'> {
  fullName: string;
  imageUrl: string;
  email: string;
  departmentName: string;
  departementId: string;
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
const AddMember = ({
  onCancel,
  searchParams,
}: {
  onCancel: () => void;
  searchParams: string;
}) => {
  const [search, setSearch] = useState('');
  const { t } = useTranslation();
  const [departments, setDepartments] = useState<
    { label: string; value: string }[]
  >([]);
  const [depSearch, setDepSearch] = useState('');
  const [departmentId, setDepartmentId] = useState<string | undefined>('');
  const getDepartment = useDepartmentSetting(depSearch);

  const { id } = useParams();
  const { mutateAsync, isLoading } = useAddGroupMember(
    id as string,
    searchParams
  );

  const form = useForm<IAddMember>({
    initialValues: {
      email: [],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const ref = useRef<HTMLInputElement>(null);
  const getNotMemberList = useGroupNotMember(
    id as string,
    `search=${search}`,
    departmentId
  );

  useEffect(() => {
    if (getNotMemberList.isSuccess) {
      const t = getNotMemberList.data?.items?.map((x) => {
        return {
          ...x,
          label: `${x.fullName}(${x.email})`,
          value: x.email,
        };
      });
      const mergedData = [...data, ...t];
      setData([
        ...new Map(mergedData.map((item) => [item['email'], item])).values(),
      ]);
    }
  }, [getNotMemberList.isSuccess, departmentId]);

  useEffect(() => {
    if (getDepartment.isSuccess) {
      const t = getDepartment.data?.items?.map((x) => {
        return {
          label: x.name,
          value: x.id,
        };
      });
      setDepartments(t ?? []);
    }
  }, [getDepartment.isSuccess]);

  const [data, setData] = useState<INotMember[]>([]);
  const onSubmitForm = async (email: string[]) => {
    try {
      const response: any = await mutateAsync({
        id: id as string,
        data: { emails: email },
      });
      if (response?.data?.httpStatusCode === 206) {
        return showNotification({
          message: response?.data?.message,
        });
      }
      showNotification({
        message: response.data?.message ?? t('add_group_member_success'),
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
    <>
      <Box>
        <form onSubmit={form.onSubmit(({ email }) => onSubmitForm(email))}>
          <MultiSelect
            tabIndex={0}
            autoComplete="off"
            placeholder={t('email_address') as string}
            ref={ref}
            searchable
            data={data}
            mb={10}
            label={t('email_address')}
            itemComponent={SelectUserItem}
            withAsterisk
            name="email"
            size="md"
            nothingFound={
              getNotMemberList.isLoading ? (
                <Loader />
              ) : (
                <Box>{t('User Not found')}</Box>
              )
            }
            getCreateLabel={(query) => `+ Create ${query}`}
            onSearchChange={(d) => {
              setSearch(d);
            }}
            {...form.getInputProps('email')}
          />
          <Select
            data={departments}
            withAsterisk
            label="Choose Department"
            clearable
            searchable
            onSearchChange={(d) => {
              setDepSearch(d);
            }}
            searchValue={depSearch}
            placeholder="Choose Department"
            onChange={(d) => {
              if (d) {
                setData([]);
                form.reset();
                setDepartmentId(d);
              }
            }}
          ></Select>
          <Group mt={'lg'} position="right">
            <Button loading={isLoading} mr={10} type="submit" size="md">
              {t('submit')}
            </Button>
          </Group>
        </form>
      </Box>
    </>
  );
};

export default AddMember;
