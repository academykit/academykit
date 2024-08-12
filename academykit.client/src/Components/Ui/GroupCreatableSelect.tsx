import { IAuthContext } from "@context/AuthProvider";
import {
  Combobox,
  Group,
  InputBase,
  MantineSize,
  useCombobox,
} from "@mantine/core";
import { UseFormReturnType } from "@mantine/form";
import { UseMutationResult } from "@tanstack/react-query";
import { UserRole } from "@utils/enums";
import { IGroup, useGetGroupDetail } from "@utils/services/groupService";
import { AxiosResponse } from "axios";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

interface IProps {
  api: UseMutationResult<AxiosResponse<IGroup, any>, unknown, string, unknown>;
  form: UseFormReturnType<any, (values: any) => any>;
  data: IGroup[] | undefined;
  size?: MantineSize;
  auth: IAuthContext | null;
  readOnly?: boolean;
}

export default function GroupCreatableSelect({
  api,
  form,
  data,
  size = "sm",
  auth,
  readOnly = false,
}: IProps) {
  const { t } = useTranslation();
  const groupDetail = useGetGroupDetail(form.values.groups);
  const combobox = useCombobox({
    onDropdownClose: () => combobox.resetSelectedOption(),
  });
  const isAdmin =
    Number(auth?.auth?.role) == UserRole.SuperAdmin ||
    Number(auth?.auth?.role) == UserRole.Admin;
  const [value, setValue] = useState<string | null>(null); // current selected value
  const [search, setSearch] = useState("");

  const exactOptionMatch = data?.some((item) => item.name === search);
  const filteredOptions = exactOptionMatch
    ? data
    : data?.filter((item) =>
        item.name.toLowerCase().includes(search.toLowerCase().trim())
      );

  const options = filteredOptions?.map((item) => (
    <Combobox.Option value={item.id} key={item.id}>
      <Group>
        <span>{item.name}</span>
      </Group>
    </Combobox.Option>
  ));

  useEffect(() => {
    // when editing setting fetched value's name
    setSearch(groupDetail?.data?.data?.name ?? "");
  }, [groupDetail.isSuccess, groupDetail.isFetched, groupDetail.isRefetching]);

  return (
    <Combobox
      store={combobox}
      withinPortal={false}
      size={size}
      // creating new group
      onOptionSubmit={(val) => {
        if (val === "$create") {
          setValue(search);
          api
            .mutateAsync(search)
            .then((res: any) => form.setFieldValue("groups", res.data.id)); // setting value after fetch
        } else {
          form.setFieldValue("groups", val);
        }

        combobox.closeDropdown();
      }}
    >
      <Combobox.Target>
        <InputBase
          withAsterisk
          label={t("group")}
          rightSection={<Combobox.Chevron />}
          {...form.getInputProps("groups")}
          value={search}
          onChange={(event) => {
            if (!readOnly) {
              combobox.openDropdown();
              combobox.updateSelectedOptionIndex();
              setSearch(event.currentTarget.value);
            }
          }}
          onClick={() => {
            if (!readOnly) {
              combobox.openDropdown();
            }
          }}
          onFocus={() => {
            if (!readOnly) {
              combobox.openDropdown();
            }
          }}
          onBlur={() => {
            combobox.closeDropdown();
            setSearch(groupDetail?.data?.data?.name ?? (value || ""));
          }}
          placeholder={
            Number(auth?.auth?.role) == UserRole.Trainer
              ? (t("group_placeholder") as string)
              : (t("group_placeholder_admin") as string)
          }
          rightSectionPointerEvents="none"
          size={size}
          styles={{
            input: {
              cursor: readOnly ? "text" : "",
              border: readOnly ? "none" : "",
            },
          }}
        />
      </Combobox.Target>

      <Combobox.Dropdown>
        {/* creatable only for admins */}
        <Combobox.Options>
          {(options?.length as number) > 0
            ? options
            : !isAdmin && <Combobox.Empty>{t("no_data")}</Combobox.Empty>}

          {isAdmin && !exactOptionMatch && search.trim().length > 0 && (
            <Combobox.Option value="$create">+ Create {search}</Combobox.Option>
          )}
        </Combobox.Options>
      </Combobox.Dropdown>
    </Combobox>
  );
}
