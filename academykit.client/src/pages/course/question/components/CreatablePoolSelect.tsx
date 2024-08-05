import {
  CheckIcon,
  Combobox,
  Group,
  InputBase,
  MantineSize,
  useCombobox,
} from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import { UseMutationResult } from '@tanstack/react-query';
import { IPool } from '@utils/services/poolService';
import { IAddQuestionType } from '@utils/services/questionService';
import { AxiosResponse } from 'axios';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';

interface IProps {
  size?: MantineSize;
  data:
    | {
        value: string;
        label: string;
      }[]
    | undefined;
  form: UseFormReturnType<
    IAddQuestionType,
    (values: IAddQuestionType) => IAddQuestionType
  >;
  api: UseMutationResult<AxiosResponse<IPool, any>, unknown, string, unknown>;
}

export function CreatablePoolSelect({ data, form, api, size = 'sm' }: IProps) {
  const combobox = useCombobox({
    onDropdownClose: () => combobox.resetSelectedOption(),
  });
  const { t } = useTranslation();
  const [value, setValue] = useState<string | null>(null);
  const [search, setSearch] = useState('');

  const exactOptionMatch = data?.some((item) => item.label === search);
  const filteredOptions = exactOptionMatch
    ? data
    : data?.filter((item) =>
        item.label.toLowerCase().includes(search.toLowerCase().trim())
      );

  const options = filteredOptions?.map((item) => (
    <Combobox.Option value={item.value} key={item.value}>
      <Group gap="xs">
        {item.label === value && <CheckIcon size={12} />}
        <span>{item.label}</span>
      </Group>
    </Combobox.Option>
  ));

  return (
    <Combobox
      store={combobox}
      withinPortal={false}
      onOptionSubmit={(val, optionProps) => {
        if (val === '$create') {
          setValue(search);
          api
            .mutateAsync(search)
            .then((res: any) =>
              form.setFieldValue('questionPoolId', res.data.id)
            );
        } else {
          // TODO: find a way to access the label
          setValue(
            optionProps!.children!.props!.children[1].props.children as string
          );
          setSearch(
            optionProps!.children!.props!.children[1].props.children as string
          );
          form.setFieldValue('questionPoolId', val);
        }
        combobox.closeDropdown();
      }}
    >
      <Combobox.Target>
        <InputBase
          mt={20}
          withAsterisk
          rightSection={<Combobox.Chevron />}
          {...form.getInputProps('questionPoolId')}
          value={search}
          onChange={(event) => {
            combobox.openDropdown();
            combobox.updateSelectedOptionIndex();
            setSearch(event.currentTarget.value);
          }}
          onClick={() => combobox.openDropdown()}
          onFocus={() => combobox.openDropdown()}
          onBlur={() => {
            combobox.closeDropdown();
            setSearch(value || '');
          }}
          placeholder={t('select_pool') as string}
          label={t('question_pool')}
          rightSectionPointerEvents="none"
          size={size}
        />
      </Combobox.Target>

      <Combobox.Dropdown>
        <Combobox.Options>
          {options}
          {!exactOptionMatch && search.trim().length > 0 && (
            <Combobox.Option value="$create">+ Create {search}</Combobox.Option>
          )}
        </Combobox.Options>
      </Combobox.Dropdown>
    </Combobox>
  );
}
