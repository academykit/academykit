import { Cross } from "@components/Icons";
import { ActionIcon, TextInput, Tooltip } from "@mantine/core";
import { useForm } from "@mantine/form";
import { IconClearAll, IconSearch } from "@tabler/icons";
import React, { useEffect } from "react";

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
    form.reset();
    setSearch("");
  };

  return (
    <div style={{ width: "100%" }}>
      <form onSubmit={form.onSubmit((values) => setSearch(values.search))}>
        <TextInput
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
