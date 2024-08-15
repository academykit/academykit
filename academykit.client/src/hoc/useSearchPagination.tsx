import SearchBar from "@components/Ui/SearchBar";
import { MultiSelect, Pagination, Select, UnstyledButton } from "@mantine/core";
import { DatePickerInput } from "@mantine/dates";
import {
  IconArrowsSort,
  IconChevronDown,
  IconChevronUp,
} from "@tabler/icons-react";
import queryStringGenerator from "@utils/queryStringGenerator";
import moment from "moment";
import React, { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";

export interface IWithSearchPagination {
  searchParams: string;

  pagination: (totalPage: number, length: number) => JSX.Element;
  searchComponent: (placeholder?: string) => JSX.Element;
  sortComponent: (props: { title: string; sortKey: string }) => JSX.Element;
  filterComponent: (
    data: {
      value: string;
      label: string;
    }[],
    placeholder: string,
    key: string
  ) => JSX.Element;
  setInitialSearch: React.Dispatch<
    React.SetStateAction<
      {
        key: string;
        value: string;
      }[]
    >
  >;
  multiFilterComponent: (
    data: {
      value: string;
      label: string;
    }[],
    placeholder: string,
    key: string
  ) => JSX.Element;
  startDateFilterComponent: (
    placeholder: string,
    key: string,
    label?: string
  ) => JSX.Element;
  endDateFilterComponent: (
    placeholder: string,
    key: string,
    label?: string
  ) => JSX.Element;
}

const withSearchPagination = <P extends object>(
  Component: React.FC<P & IWithSearchPagination>,
  updateUrl = true
) => {
  const withSearchPagination = (props: P) => {
    const [params, setParams] = useSearchParams();
    const [sort, setSort] = useState(params.get("so") ?? "");
    const [filterKey, setFilterKey] = useState<string>("");
    const [initialSearch, setInitialSearch] = useState<
      {
        key: string;
        value: string;
      }[]
    >([
      {
        key: "",
        value: "",
      },
    ]);

    const search = params.get("s") ?? null;
    const pageSize = 12;
    const [itemLength, setItemLength] = useState<number>();
    const [filterValue, setFilterValue] = useState<string>("");

    const [multiFilteredValue, setMultiFilteredValue] = useState<string[]>([]);
    const [multiFilteredKey, setMultiFilteredKey] = useState<string>("");

    const [currentPage, setCurrentPage] = useState(
      parseInt(params.get("p") ?? "1")
    );
    const [startDate, setStartDate] = useState<string>("");
    const [startDateKey, setStartDateKey] = useState<string>("");

    const [endDate, setEndDate] = useState<string>("");
    const [endDateKey, setEndDateKey] = useState<string>("");

    useEffect(() => {
      if (currentPage !== 1 && itemLength == 0) {
        setPage(currentPage - 1);
      }
    }, [itemLength]);
    const qs = useMemo(() => {
      const [by, type] = sort.split(":");
      const initSearchObject: Record<string, string> = {};
      initialSearch.forEach((x) => {
        initSearchObject[x.key] = x.value;
      });

      const qs = queryStringGenerator({
        ...initSearchObject,
        search,
        page: currentPage,
        size: pageSize,
        sortBy: by,
        sortType: type,
        [filterKey]: filterValue,
        [multiFilteredKey]: multiFilteredValue,
        [startDateKey]: startDate,
        [endDateKey]: endDate,
      });

      !!search && search != "" && params.set("s", search);
      sort && params.set("so", sort);

      pageSize && pageSize != 12 && params.set("si", pageSize);

      currentPage &&
        currentPage != 1 &&
        params.set("p", currentPage.toString());
      startDate && params.set("sd", startDate.toString());
      endDate && params.set("ed", endDate.toString());

      if (updateUrl) {
        setParams(params, { replace: true });
      }
      return qs;
    }, [
      currentPage,
      search,
      pageSize,
      sort,
      filterValue,
      multiFilteredValue,
      initialSearch,
      startDate,
      endDate,
    ]);

    const setSearch = (search: string) => {
      for (const value of params.entries()) {
        if (value[0] !== "s") params.delete(value[0]);
      }
      if (updateUrl) {
        params.set("s", search);
        setParams(params);
      }
    };

    const sortComponent = (props: { title: string; sortKey: string }) => {
      const sortKey = sort && sort.split(":").length > 0 && sort.split(":")[0];
      const sortValue =
        sort && sort.split(":").length > 0 && sort.split(":")[1];
      const isAscending = sortKey === props.sortKey && sortValue === "1";

      return (
        <UnstyledButton
          style={{
            display: "flex",
            alignItems: "center",
            fontWeight: "bold",
            fontSize: "inherit",
          }}
          onClick={() => {
            setSort(() => props.sortKey + `:${!isAscending ? "1" : "0"}`);
          }}
        >
          {props.title}

          {props.sortKey === sortKey ? (
            isAscending ? (
              <IconChevronUp style={{ marginLeft: "10px" }} size={20} />
            ) : (
              <IconChevronDown style={{ marginLeft: "10px" }} size={20} />
            )
          ) : (
            <IconArrowsSort style={{ marginLeft: "10px" }} size={20} />
          )}
        </UnstyledButton>
      );
    };

    const filterComponent = (
      data: { value: string; label: string }[],
      placeholder: string,
      key: string
    ) => {
      return (
        <Select
          placeholder={placeholder}
          ml={5}
          clearable
          maw={"184px"}
          value={filterValue}
          data={data}
          onChange={(value) => {
            setFilterValue(() => value ?? "");
            setFilterKey(() => key);
          }}
        />
      );
    };

    const multiFilterComponent = (
      data: { value: string; label: string }[],
      placeholder: string,
      key: string
    ) => {
      setMultiFilteredKey(key);
      return (
        <MultiSelect
          placeholder={placeholder}
          ml={5}
          clearable
          styles={{ input: { height: "20px", overflow: "hidden" } }}
          maw={"184px"}
          value={multiFilteredValue}
          data={data}
          onChange={(value) => {
            setMultiFilteredValue(() => value ?? []);
          }}
        />
      );
    };

    const setPage = (pageNumber: number) => {
      setCurrentPage(pageNumber);
    };

    const pagination = (totalPage: number, length: number) => {
      setItemLength(length);
      return totalPage > 1 ? (
        <Pagination
          my={20}
          total={totalPage}
          value={currentPage}
          onChange={setPage}
        />
      ) : (
        <></>
      );
    };
    const searchComponent = (placeholder?: string) => (
      <SearchBar
        search={search ?? ""}
        setSearch={setSearch}
        placeholder={placeholder}
      />
    );

    const startDateFilterComponent = (
      placeholder: string,
      key: string,
      label?: string
    ) => {
      return (
        <DatePickerInput
          clearable
          valueFormat="YYYY MMM DD"
          label={label}
          placeholder={placeholder}
          onChange={(val: any) => {
            setStartDate(moment(val).toISOString());
            setStartDateKey(key);
          }}
          miw={125}
          mr={5}
        />
      );
    };

    const endDateFilterComponent = (
      placeholder: string,
      key: string,
      label?: string
    ) => {
      return (
        <DatePickerInput
          clearable
          valueFormat="YYYY MMM DD"
          label={label}
          placeholder={placeholder}
          onChange={(val: any) => {
            setEndDate(moment(val).toISOString());
            setEndDateKey(key);
          }}
          miw={125}
          mr={5}
        />
      );
    };

    return (
      <Component
        {...(props as P)}
        filterComponent={filterComponent}
        multiFilterComponent={multiFilterComponent}
        searchParams={qs}
        pagination={pagination}
        searchComponent={searchComponent}
        sortComponent={sortComponent}
        setInitialSearch={setInitialSearch}
        startDateFilterComponent={startDateFilterComponent}
        endDateFilterComponent={endDateFilterComponent}
      />
    );
  };
  return withSearchPagination;
};

export default withSearchPagination;
