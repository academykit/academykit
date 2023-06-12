import { Cross } from "@components/Icons";
import { ActionIcon, TextInput, Tooltip } from "@mantine/core";
import { useForm } from "@mantine/form";
import { IconClearAll, IconSearch } from "@tabler/icons";
import React, { useEffect, useRef } from "react";

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
      search: search ?? "",
    },
  });
  useEffect(() => {
    if (!form.values.search) {
      setSearch("");
    }
  }, [form.values.search]);
  const clearField = () => {
    setSearch("");
    form.setFieldValue("search", "");
    inputRef.current?.focus();
  };

  return (
    <div style={{ width: "100%" }}>
      <form onSubmit={form.onSubmit((values) => setSearch(values.search))}>
        <TextInput
          ref={inputRef}
          radius={"md"}
          size="sm"
          rightSection={
            form.values.search && (
              <Tooltip label="Clear search">
                <ActionIcon onClick={clearField}>
                  <Cross />
                </ActionIcon>
              </Tooltip>
            )
          }
          placeholder={placeholder}
          {...form.getInputProps("search")}
          icon={<IconSearch size={14} stroke={1.5} />}
        />
      </form>
    </div>
  );
};

export default SearchBar;
