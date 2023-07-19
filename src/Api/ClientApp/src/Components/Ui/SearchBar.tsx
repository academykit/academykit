import { Cross } from '@components/Icons';
import { ActionIcon, TextInput, Tooltip } from '@mantine/core';
import { useForm } from '@mantine/form';
import { IconSearch } from '@tabler/icons';
import React, { useRef } from 'react';
import { useTranslation } from 'react-i18next';

interface Props {
  placeholder?: string;
  setSearch: (query: string) => void;
  search?: string;
}

const SearchBar: React.FC<React.PropsWithChildren<Props>> = ({
  placeholder,
  setSearch,
  search,
}) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const form = useForm({
    initialValues: {
      search: search ?? '',
    },
  });
  // useEffect(() => {
  //   if (!form.values.search) {
  //     setSearch('');
  //   }
  // }, [form.values.search]);
  const clearField = () => {
    form.setFieldValue('search', '');
    inputRef.current?.focus();
    setSearch('');
  };
  const { t } = useTranslation();

  return (
    <div style={{ width: '100%' }}>
      <form onSubmit={form.onSubmit((values) => setSearch(values.search))}>
        <TextInput
          ref={inputRef}
          radius={'md'}
          size="sm"
          rightSection={
            form.values.search && (
              <Tooltip label={t('clear_search')}>
                <ActionIcon onClick={clearField}>
                  <Cross />
                </ActionIcon>
              </Tooltip>
            )
          }
          placeholder={placeholder}
          {...form.getInputProps('search')}
          icon={<IconSearch size={14} stroke={1.5} />}
        />
      </form>
    </div>
  );
};

export default SearchBar;
