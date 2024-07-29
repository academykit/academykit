import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import {
  Box,
  Checkbox,
  Flex,
  Loader,
  MultiSelect,
  Select,
  Text,
  TextInput,
  UnstyledButton,
} from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { QuestionType, ReadableEnum } from '@utils/enums';
import queryStringGenerator from '@utils/queryStringGenerator';
import { IAddQuestionType } from '@utils/services/questionService';
import { useTags } from '@utils/services/tagService';
import React, { FC, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

type Props = {
  form: UseFormReturnType<
    IAddQuestionType,
    (values: IAddQuestionType) => IAddQuestionType
  >;
  onSubmit: (
    values: IAddQuestionType,
    event: React.FormEvent<HTMLFormElement>
  ) => void;
  useFormContext: () => UseFormReturnType<
    IAddQuestionType,
    (values: IAddQuestionType) => IAddQuestionType
  >;
};

const fieldSize = 'md';
const getQuestionType = () => {
  return Object.entries(QuestionType)
    .splice(0, Object.entries(QuestionType).length / 2)
    .map(([key, value]) => ({
      value: key,
      label:
        ReadableEnum[value as keyof typeof ReadableEnum] ?? value.toString(),
    }));
};

const CreateForm: FC<Props> = ({ form, useFormContext }) => {
  const [tagsList, setTagsList] = useState<{ value: string; label: string }[]>(
    []
  );
  const tags = useTags(
    queryStringGenerator({
      search: '',
      size: 10000,
    })
  );

  useEffect(() => {
    if (tags.isSuccess && tags.isFetched) {
      setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
    }
  }, [tags.isSuccess]);

  // const { mutate } = useAddTag();
  const { t } = useTranslation();
  return (
    <>
      <TextInput
        size={fieldSize}
        withAsterisk
        label={t('title_for_question')}
        placeholder={t('title_for_question_placeholder') as string}
        {...form.getInputProps('name')}
      ></TextInput>
      <Box mt={20}>
        <Text size={'sm'}>{t('description')}</Text>
        <RichTextEditor formContext={useFormContext} />
      </Box>

      {tags.isSuccess ? (
        <MultiSelect
          searchable
          withAsterisk
          style={{ maxWidth: '500px' }}
          data={tagsList}
          {...form.getInputProps('tags')}
          size={'lg'}
          label={t('tags')}
          placeholder={t('select_tags') as string}
        />
      ) : (
        <Loader />
      )}

      <Box mt={20}>
        <RichTextEditor label="hints" formContext={useFormContext} />
      </Box>
      <Select
        mt={20}
        placeholder={t('select_question_type') as string}
        size={fieldSize}
        label={t('select_question')}
        {...form.getInputProps('type')}
        data={getQuestionType()}
      ></Select>
      {(form.values.type === QuestionType.MultipleChoice.toString() ||
        form.values.type === QuestionType.SingleChoice.toString()) && (
        <Box>
          <Text mt={20}>{t('options')}</Text>
          {form.values.answers.map((x, i) => (
            <Flex
              key={i}
              mb={30}
              style={{
                width: '100%',
                flexWrap: 'nowrap',
              }}
            >
              <Checkbox
                {...form.getInputProps(`answers.${i}.isCorrect`)}
                name=""
              ></Checkbox>

              <RichTextEditor
                placeholder={t('option_placeholder') as string}
                label={`answers.${i}.option`}
                formContext={useFormContext}
              ></RichTextEditor>
              <UnstyledButton
                onClick={() => {
                  form.insertListItem(
                    'answers',
                    {
                      option: '',
                      isCorrect: false,
                    },
                    i + 1
                  );
                }}
              >
                <IconPlus color="green" />
              </UnstyledButton>
              {form.values.answers.length > 1 && (
                <UnstyledButton
                  onClick={() => {
                    form.removeListItem('option', i);
                  }}
                >
                  <IconTrash color="red" />
                </UnstyledButton>
              )}
              {typeof form.errors[`answers.${i}.option`] === 'string' && (
                <span style={{ color: 'red' }}>
                  {form.errors[`answers.${i}.option`]}
                </span>
              )}
            </Flex>
          ))}
          {typeof form.errors[`answers`] === 'string' && (
            <span style={{ color: 'red' }}>{form.errors[`answers`]}</span>
          )}
        </Box>
      )}
    </>
  );
};

export default CreateForm;
