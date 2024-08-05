import {
  CheckIcon,
  Combobox,
  Group,
  MantineSize,
  Pill,
  PillsInput,
  useCombobox,
} from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import { UseMutateAsyncFunction } from "@tanstack/react-query";
import { IFullCourse } from "@utils/services/courseService";
import { ITag } from "@utils/services/tagService";
import { AxiosResponse } from "axios";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

interface IProps {
  mutateAsync: UseMutateAsyncFunction<
    AxiosResponse<ITag, any>,
    unknown,
    string,
    unknown
  >;
  form: UseFormReturnType<any, (values: any) => any>;
  size?: MantineSize;
  data: ITag[] | undefined;
  existingTags?: IFullCourse | undefined;
  readOnly?: boolean;
}
export default function TagMultiSelectCreatable({
  mutateAsync,
  form,
  size = "sm",
  data, // dropdown options
  existingTags,
  readOnly = false,
}: IProps) {
  const combobox = useCombobox({
    onDropdownClose: () => combobox.resetSelectedOption(),
    onDropdownOpen: () => combobox.updateSelectedOptionIndex("active"),
  });
  const { t } = useTranslation();
  const [search, setSearch] = useState("");
  const [value, setValue] = useState<ITag[]>([]); // current selected values

  const exactOptionMatch = data?.some((item) => item.name === search);

  const handleValueSelect = (val: string) => {
    if (val === "$create") {
      // create new tag and add tag-name to state list,
      // add IDs to the form
      mutateAsync(search).then((res) => {
        setValue((current) => [...current, res.data]);
        form.setFieldValue("tags", [...form.values.tags, res.data.id]);
      });
      setSearch("");
    } else {
      setValue((current) =>
        current.some((item) => item.id === val)
          ? current.filter((v) => v.id !== val)
          : [...current, data!.find((item) => item.id === val)!]
      );
      form.setFieldValue("tags", [...form.values.tags, val]);
    }
  };

  const handleValueRemove = (pillId: string) => {
    setValue((current) => current.filter((v) => v.id !== pillId));
    form.setFieldValue(
      "tags",
      form.values.tags.filter((v: any) => v !== pillId)
    );
  };

  const values = value.map((item) => (
    <Pill
      key={item.id}
      withRemoveButton={!readOnly}
      onRemove={() => handleValueRemove(item.id)}
    >
      {item.name}
    </Pill>
  ));

  const options = data
    ?.filter((item) =>
      item.name.toLowerCase().includes(search.trim().toLowerCase())
    )
    .map((item) => (
      <Combobox.Option
        value={item.id}
        key={item.id}
        active={value.includes(item)}
      >
        <Group gap="sm">
          {value.includes(item) ? <CheckIcon size={12} /> : null}
          <span>{item.name}</span>
        </Group>
      </Combobox.Option>
    ));

  useEffect(() => {
    // setting existing tags
    if (existingTags) {
      const tags = existingTags?.tags?.map((tag) => {
        return {
          id: tag.tagId,
          name: tag.tagName,
          slug: tag.tagName,
          isActive: true,
        };
      });
      setValue(tags ?? []);
    }
  }, [existingTags]);

  return (
    <Combobox
      store={combobox}
      onOptionSubmit={handleValueSelect}
      withinPortal={false}
    >
      <Combobox.DropdownTarget>
        <PillsInput
          label={t("tags")}
          onClick={() => {
            if (!readOnly) {
              combobox.openDropdown();
            }
          }}
          size={size}
          styles={{
            input: {
              cursor: readOnly ? "text" : "",
              border: readOnly ? "none" : "",
            },
          }}
        >
          <Pill.Group>
            {values}

            <Combobox.EventsTarget>
              <PillsInput.Field
                {...form.getInputProps("tags")}
                onFocus={() => {
                  if (!readOnly) {
                    combobox.openDropdown();
                  }
                }}
                onBlur={() => combobox.closeDropdown()}
                value={search}
                placeholder={t("tags_placeholder") as string}
                onChange={(event) => {
                  if (!readOnly) {
                    combobox.updateSelectedOptionIndex();
                    setSearch(event.currentTarget.value);
                  }
                }}
                onKeyDown={(event) => {
                  if (!readOnly) {
                    if (event.key === "Backspace" && search.length === 0) {
                      event.preventDefault();
                      handleValueRemove(value[value.length - 1].id);
                    }
                  }
                }}
              />
            </Combobox.EventsTarget>
          </Pill.Group>
        </PillsInput>
      </Combobox.DropdownTarget>

      <Combobox.Dropdown>
        <Combobox.Options>
          {options}

          {!exactOptionMatch && search.trim().length > 0 && (
            <Combobox.Option value="$create">+ Create {search}</Combobox.Option>
          )}

          {exactOptionMatch &&
            search.trim().length > 0 &&
            options?.length === 0 && (
              <Combobox.Empty>{t("no_data")}</Combobox.Empty>
            )}
        </Combobox.Options>
      </Combobox.Dropdown>
    </Combobox>
  );
}
