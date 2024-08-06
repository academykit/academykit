/* eslint-disable @typescript-eslint/no-unused-vars */
import {
  Combobox,
  InputBase,
  ScrollArea,
  Text,
  useCombobox,
} from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import { IUserProfile } from "@utils/services/types";
import { Dispatch, SetStateAction, useEffect, useRef, useState } from "react";

interface FormValues {
  thumbnail: string;
  title: string;
  level: any;
  groups: string;
  description: string;
  tags: string[];
  language: string;
  startDate: Date;
  endDate: Date;
  isCostApplicable: boolean;
  trainingId: string;
  trainingCode: string;
  trainingType: string;
  trainerId: string;
}
interface IInfiniteScrollSelect {
  placeholder: string;
  form: UseFormReturnType<FormValues, (values: FormValues) => FormValues>;
  setPage: Dispatch<SetStateAction<number>>;
  data: IUserProfile[] | undefined;
  totalData: number;
}

const InfiniteScrollSelect = ({
  placeholder,
  setPage,
  form,
  data = [],
  totalData,
}: IInfiniteScrollSelect) => {
  const combobox = useCombobox({
    onDropdownClose: () => combobox.resetSelectedOption(),
  });
  const [scrollPosition, onScrollPositionChange] = useState({ x: 0, y: 0 });
  const viewport = useRef<HTMLDivElement>(null);

  const [value, setValue] = useState<string | null>(null);
  const [search, setSearch] = useState("");

  const shouldFilterOptions = data.every((item) => item.fullName !== search);
  const filteredOptions = shouldFilterOptions
    ? data.filter((item) =>
        item.fullName.toLowerCase().includes(search.toLowerCase().trim())
      )
    : data;

  const options = filteredOptions.map((item) => (
    <Combobox.Option value={item.id} key={item.id}>
      {item.fullName}
    </Combobox.Option>
  ));

  useEffect(() => {
    if (viewport.current) {
      // if the user scrolled near the bottom of the list
      if (
        viewport.current?.scrollHeight - viewport.current?.clientHeight ===
        scrollPosition.y
      ) {
        if (viewport.current.scrollHeight !== 0 && totalData > data.length) {
          // increase page number
          setPage((prev) => prev + 1);
        }
      }
    }
  }, [scrollPosition.y]);

  return (
    <>
      <Text>{scrollPosition.y}</Text>
      <Combobox
        store={combobox}
        withinPortal={false}
        onOptionSubmit={(val, optionProps) => {
          setValue(optionProps.children as string);
          setSearch(optionProps.children as string);
          form.setFieldValue("trainerId", val);
          combobox.closeDropdown();
        }}
      >
        <Combobox.Target>
          <InputBase
            rightSection={<Combobox.Chevron />}
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
              setSearch(value || "");
            }}
            placeholder={placeholder}
            rightSectionPointerEvents="none"
          />
        </Combobox.Target>

        <Combobox.Dropdown>
          <Combobox.Options>
            <ScrollArea
              viewportRef={viewport}
              onScrollPositionChange={onScrollPositionChange}
              h={140}
            >
              {options.length > 0 ? (
                options
              ) : (
                <Combobox.Empty>Nothing found</Combobox.Empty>
              )}
            </ScrollArea>
          </Combobox.Options>
        </Combobox.Dropdown>
      </Combobox>
    </>
  );
};

export default InfiniteScrollSelect;
