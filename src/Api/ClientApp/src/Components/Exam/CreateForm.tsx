import React, { FC, useEffect, useState } from "react";
import TextEditor from "@components/Ui/TextEditor";
import {
  Box,
  Checkbox,
  Group,
  Loader,
  MultiSelect,
  Select,
  Text,
  TextInput,
  UnstyledButton,
} from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import { IconPlus, IconTrash } from "@tabler/icons";
import { QuestionType, ReadableEnum } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import { IAddQuestionType } from "@utils/services/questionService";
import { useAddTag, useTags } from "@utils/services/tagService";

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

const fieldSize = "md";
const getQuestionType = () => {
  return Object.entries(QuestionType)
    .splice(0, Object.entries(QuestionType).length / 2)
    .map(([key, value]) => ({
      value: key,
      label:
        ReadableEnum[value as keyof typeof ReadableEnum] ?? value.toString(),
    }));
};

const CreateForm: FC<Props> = ({ form, onSubmit, useFormContext }) => {
  const [tagsList, setTagsList] = useState<{ value: string; label: string }[]>(
    []
  );
  const tags = useTags(
    queryStringGenerator({
      search: "",
      size: 10000,
    })
  );

  useEffect(() => {
    if (tags.isSuccess && tags.isFetched) {
      setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
    }
  }, [tags.isSuccess]);

  const { mutate } = useAddTag();
  return (
    <>
      <TextInput
        size={fieldSize}
        withAsterisk
        label="Title for question"
        placeholder="Enter Title of Question"
        {...form.getInputProps("name")}
      ></TextInput>
      <Box mt={20}>
        <Text size={"sm"}>Description</Text>
        <TextEditor formContext={useFormContext} />
      </Box>

      {tags.isSuccess ? (
        <MultiSelect
          searchable
          withAsterisk
          labelProps="name"
          creatable
          sx={{ maxWidth: "500px" }}
          data={tagsList}
          value={[]}
          {...form.getInputProps("tags")}
          getCreateLabel={(query) => `+ Create ${query}`}
          onCreate={(query) => {
            mutate(query);
          }}
          size={"lg"}
          label="Tags"
          placeholder="Please select Tags."
        />
      ) : (
        <Loader />
      )}

      <Box mt={20}>
        <Text size={"sm"}>Hint</Text>
        <TextEditor label="hints" formContext={useFormContext} />
      </Box>
      <Select
        mt={20}
        placeholder={"Please Question Type"}
        size={fieldSize}
        label="Question Type"
        {...form.getInputProps("type")}
        data={getQuestionType()}
      ></Select>
      {(form.values.type === QuestionType.MultipleChoice.toString() ||
        form.values.type === QuestionType.SingleChoice.toString()) && (
        <Box>
          <Text mt={20}>Options</Text>
          {form.values.answers.map((x, i) => (
            <Group key={i} mb={30}>
              <Checkbox
                {...form.getInputProps(`answers.${i}.isCorrect`)}
                name=""
              ></Checkbox>
              <TextEditor
                label={`answers.${i}.option`}
                formContext={useFormContext}
              ></TextEditor>
              <UnstyledButton
                onClick={() => {
                  form.insertListItem(
                    "answers",
                    {
                      option: "",
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
                    form.removeListItem("option", i);
                  }}
                >
                  <IconTrash color="red" />
                </UnstyledButton>
              )}
              {typeof form.errors[`answers.${i}.option`] === "string" && (
                <span style={{ color: "red" }}>
                  {form.errors[`answers.${i}.option`]}
                </span>
              )}
            </Group>
          ))}
          {typeof form.errors[`answers`] === "string" && (
            <span style={{ color: "red" }}>{form.errors[`answers`]}</span>
          )}
        </Box>
      )}
    </>
  );
};

export default CreateForm;
